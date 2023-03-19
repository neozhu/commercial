using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using Repository.Pattern.Infrastructure;
using WebApp.Models;
using WebApp.Services;
using Z.EntityFramework.Plus;
using EntityFramework;
namespace WebApp.Controllers
{
  [Authorize]
  [RoutePrefix("Tenants")]
  public class TenantsController : Controller
  {
    private readonly NLog.ILogger logger;

    private  ApplicationDbContext dbContext => HttpContext.GetOwinContext().Get<ApplicationDbContext>();
    public TenantsController(
                                NLog.ILogger logger
                               ) {
      this.logger = logger;
    }
     
    //租户管理
    [Route("Index", Name = "租户管理", Order = 2)]
    public ActionResult Index() => View();

    //保存租户信息
    public async Task<JsonResult> SaveData(Tenant[] tenant) {
      if (ModelState.IsValid)
      {
        try
        {
          foreach (var item in tenant)
          {
            if(item.TrackingState== TrackableEntities.TrackingState.Added)
            {
              this.dbContext.Tenants.Add(item);
            }
            if (item.TrackingState == TrackableEntities.TrackingState.Modified)
            {
              var update = await this.dbContext.Tenants.Where(x => x.Id == item.Id).FirstAsync();
              update.Name = item.Name;
              update.ConnectionStrings = item.ConnectionStrings;
              update.Disabled = item.Disabled;
              this.dbContext.Entry(update).State = EntityState.Modified;
              //this.dbContext.Tenants.Attach(update);
  
            }
            if (item.TrackingState == TrackableEntities.TrackingState.Deleted)
            {
              var delete = await this.dbContext.Tenants.Where(x => x.Id == item.Id).FirstAsync();
              this.dbContext.Tenants.Remove(delete);

            }
           
          }
          await this.dbContext.SaveChangesAsync();
          return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception e)
        {
          return Json(new { success = false, err = e.GetMessage() }, JsonRequestBehavior.AllowGet);
        }
      }
      else
      {
        var modelStateErrors = string.Join(",", ModelState.Keys.SelectMany(key => ModelState[key].Errors.Select(n => n.ErrorMessage)));
        return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
      }

    }
    //删除租户信息
    public async Task<JsonResult> DeleteChecked(int[] id) {
      var items = this.dbContext.Tenants.Where(x => id.Contains(x.Id));
      foreach (var item in items)
      {
        this.dbContext.Tenants.Remove(item);
      }
      await this.dbContext.SaveChangesAsync();
      return Json(new { success = true }, JsonRequestBehavior.AllowGet);
    }
   
    [HttpGet]
    public JsonResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "desc", string filterRules = "")
    {
      var filters = PredicateBuilder.From<Tenant>(filterRules);
      var totalCount = 0;
      var tenants = this.dbContext.Tenants.Where(filters).OrderByName(sort, order);
      totalCount = tenants.Count();
      var datalist = tenants.Skip(( page - 1 ) * rows).Take(rows);
      var datarows = datalist.Select(n => new
      {
        Id = n.Id,
        Name = n.Name,
        ConnectionStrings = n.ConnectionStrings
      }).ToList();
      var pagelist = new { total = totalCount, rows = datarows };
      return this.Json(pagelist, JsonRequestBehavior.AllowGet);
    }
     
  }
}