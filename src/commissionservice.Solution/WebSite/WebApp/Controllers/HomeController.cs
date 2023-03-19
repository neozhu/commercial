using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using LazyCache;
using WebApp.App_Helpers.third_party.api;
using WebApp.Services;

namespace WebApp.Controllers
{
  [Authorize]
  [RoutePrefix("Home")]
  public class HomeController : Controller
  {
    private readonly IAppCache cache;
    private readonly IMapper mapper;
    private readonly NLog.ILogger logger;
    private readonly SqlSugar.ISqlSugarClient db;
    private readonly ICompanyService companyService;

    public HomeController(
      ICompanyService companyService,
      NLog.ILogger logger,
      SqlSugar.ISqlSugarClient db,
      IAppCache cache, IMapper mapper) {
      this.db = db;
      this.cache = cache;
      this.mapper = mapper;
      this.logger = logger;
      this.companyService = companyService;
    }

    public async Task<ActionResult> Index()
    {
      this.logger.Debug("访问首页");
       
 
      return this.View();
    }
 




  }
}