using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.UnitOfWork;
using SqlSugar;
using TrackableEntities;
using WebApp.Models;
using WebApp.Models.ViewModel;
using WebApp.Repositories;
using WebApp.Services;
using Z.EntityFramework.Plus;
using Microsoft.AspNet.Identity.Owin;
namespace WebApp.Controllers
{
  /// <summary>
  /// File: LogsController.cs
  /// Purpose:系统管理/系统日志
  /// Created Date: 9/19/2019 8:51:53 AM
  /// Author: neo.zhu
  /// Tools: SmartCode MVC5 Scaffolder for Visual Studio 2017
  /// TODO: Registers the type mappings with the Unity container(Mvc.UnityConfig.cs)
  /// <![CDATA[
  ///    container.RegisterType<IRepositoryAsync<Log>, Repository<Log>>();
  ///    container.RegisterType<ILogService, LogService>();
  /// ]]>
  /// Copyright (c) 2012-2018 All Rights Reserved
  /// </summary>
  [Authorize]
  [RoutePrefix("Logs")]
  public class LogsController : Controller
  {
    private ApplicationDbContext dbContext => HttpContext.GetOwinContext().Get<ApplicationDbContext>();
    private readonly NLog.ILogger logger;
    private readonly ISqlSugarClient db;
    public LogsController(

      NLog.ILogger logger,
      ISqlSugarClient db
      )
    {

      this.logger = logger;
      this.db = db;
    }
    //GET: Logs/Index
    //[OutputCache(Duration = 60, VaryByParam = "none")]
    [Route("Index", Name = "系统日志", Order = 1)]
    public async Task<ActionResult> Index()
    {
      return View();
    }

    public async Task<JsonResult> GetChartData()
    {
      var levels = new string[] { "Info", "Trace", "Debug", "Warn", "Error", "Fatal" };
      var sql = @"SELECT [level], CONVERT(Datetime,format(min(Logged),'yyyy-MM-dd HH:00:00')) AS [time],
       COUNT(*) AS total
FROM AspNetLogs
where DATEDIFF(D, GETDATE(), Logged)> -3
GROUP BY [level], CAST(Logged as date),
       DATEPART(hour, Logged)
order by [level], CAST(Logged as date),
       DATEPART(hour, Logged)";
      var data = await this.db.Ado.SqlQueryAsync<logtimetotal>(sql);
      var date = DateTime.Now.AddDays(-2).Date;
      var today = DateTime.Now.AddDays(1).Date;
      var list = new List<dynamic>();
      while (( date = date.AddHours(1) ) < today)
      {
        foreach (var level in levels)
        {
          var item = data.Where(x => x.time == date && x.level == level).FirstOrDefault();
          if (item != null)
          {
            list.Add(new { time = date.ToString("yyyy-MM-dd HH:mm"), level = level, total = item.total });
          }
          else
          {
            list.Add(new { time = date.ToString("yyyy-MM-dd HH:mm"), level = level, total = 0 });

          }
        }

      }
      var sql1 = @"select Level [level],count(*) total
FROM AspNetLogs
where DATEDIFF(D, GETDATE(), Logged)> -3
group by Level";
      var array = await this.db.Ado.SqlQueryAsync<logleveltotal>(sql1);

      var group = new List<dynamic>();
      foreach (var level in levels)
      {
        var item = array.Where(x => x.level == level).FirstOrDefault();
        if (item != null)
        {
          group.Add(new { level, item.total });
        }
        else
        {
          group.Add(new { level, total = 0 });

        }
      }

      return Json(new { list = list, group = group }, JsonRequestBehavior.AllowGet);
    }
    //Get :Logs/GetData
    //For Index View datagrid datasource url
    //更新日志状态
    [HttpGet]
    public async Task<JsonResult> SetLogState(int id)
    {
      var log = await this.dbContext.Logs.FindAsync(id);
      log.Resolved = true;
      await this.dbContext.SaveChangesAsync();
      return Json(new { success = true }, JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    [OutputCache(Duration = 10, VaryByParam = "*")]
    public async Task<JsonResult> GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
    {
      var filters = PredicateBuilder.From<Log>(filterRules);
      var total = await this.dbContext.Logs
                        .Where(filters)
                        .AsNoTracking()
                        .CountAsync();
      var pagerows = await this.dbContext
                                 .Logs
                                 .Where(filters)
                                 .OrderBy(sort, order)
                                 .Skip(page - 1).Take(rows)
                                 .AsNoTracking()
                                 .ToListAsync();

      var pagelist = new { total = total, rows = pagerows };
      return this.Json(pagelist,JsonRequestBehavior.AllowGet);
    }
    //easyui datagrid post acceptChanges 
    
    //导出Excel
    //[HttpPost]
    //public async Task<ActionResult> ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
    //{
    //  var fileName = "logs_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
    //  var stream = await this.logService.ExportExcelAsync(filterRules, sort, order);
    //  return this.File(stream, "application/vnd.ms-excel", fileName);
    //}
     

  }
}
