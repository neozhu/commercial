using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Repository.Pattern.UnitOfWork;
using Repository.Pattern.Infrastructure;
using Z.EntityFramework.Plus;
using TrackableEntities;
using WebApp.Models;
using WebApp.Services;
using WebApp.Repositories;
using AutoMapper;

namespace WebApp.Controllers
{
/// <summary>
/// File: CompaniesController.cs
/// Purpose:组织管理/公司信息
/// Created Date: 2020/8/7 9:08:04
/// Author: neo.zhu
/// Tools: SmartCode MVC5 Scaffolder for Visual Studio 2017
/// TODO: Registers the type mappings with the Unity container(Mvc.UnityConfig.cs)
/// <![CDATA[
///    container.RegisterType<IRepositoryAsync<Company>, Repository<Company>>();
///    container.RegisterType<ICompanyService, CompanyService>();
/// ]]>
/// Copyright (c) 2012-2018 All Rights Reserved
/// </summary>
    [Authorize]
    [RoutePrefix("Companies")]
	public class CompaniesController : Controller
	{
		private readonly ICompanyService  companyService;
		private readonly IUnitOfWorkAsync unitOfWork;
        private readonly NLog.ILogger logger;
    private readonly IMapper mapper;
		public CompaniesController (
       IMapper mapper,
          ICompanyService  companyService, 
          IUnitOfWorkAsync unitOfWork,
          NLog.ILogger logger
          )
		{
      this.mapper = mapper;
      this.companyService  = companyService;
			this.unitOfWork = unitOfWork;
            this.logger = logger;
		}
        		//GET: Companies/Index
        //[OutputCache(Duration = 60, VaryByParam = "none")]
        [Route("Index", Name = "公司信息", Order = 1)]
		public ActionResult Index() => this.View();

    //Get :Companies/GetData
    //For Index View datagrid datasource url
    [HttpGet]
    public async Task<JsonResult> GetTreeData()
    {
      var list = await getCompanyTreeData();
      return Json(list, JsonRequestBehavior.AllowGet);
    }
    private async Task<IEnumerable<CompanyTreeItem>> getCompanyTreeData()
    {
      var list = new List<CompanyTreeItem>();
      var result = await this.companyService.Queryable().Where(x => x.ParentId == null).ToListAsync();
      var root = this.mapper.Map< IEnumerable<Company>, IEnumerable<CompanyTreeItem>>(
       result
        );
       
      foreach (var top in root)
      {
        await recursioncompanytreedata(top, top.Id);
      }
      return root;

    }
    private async Task recursioncompanytreedata(CompanyTreeItem item, int? parentid)
    {
      var result = await companyService.Queryable().Where(x => x.ParentId == parentid
           ).ToListAsync();
      var children= this.mapper.Map<IEnumerable<Company>, IEnumerable<CompanyTreeItem>>(
       result
        );
      foreach (var child in children)
      {
        await recursioncompanytreedata(child, child.Id);
      }
      item.children = children.ToArray();
    }



    [HttpGet]
        //[OutputCache(Duration = 10, VaryByParam = "*")]
		 public async Task<JsonResult> GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
		{
			var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
			var pagerows  = (await this.companyService
						               .Query(new CompanyQuery().Withfilter(filters)).Include(c => c.Parent)
							           .OrderBy(n=>n.OrderBy(sort,order))
							           .SelectPageAsync(page, rows, out var totalCount))
                                       .Select(  n => new { 

    ParentName = n.Parent?.Name,
    Id = n.Id,
    Name = n.Name,
    TradeCode = n.TradeCode,
    MasterCustom = n.MasterCustom,
    CreditCode = n.CreditCode,
    Code = n.Code,
    Ctype = n.Ctype,
    Scope = n.Scope,
    Address = n.Address,
    LegalPerson = n.LegalPerson,
    Contect = n.Contect,
    PhoneNumber = n.PhoneNumber,
    RegisterDate = n.RegisterDate.ToString("yyyy-MM-dd HH:mm:ss"),
    ExpirationDate = n.ExpirationDate?.ToString("yyyy-MM-dd HH:mm:ss"),
    ParentId = n.ParentId
}).ToList();
			var pagelist = new { total = totalCount, rows = pagerows };
			return Json(pagelist, JsonRequestBehavior.AllowGet);
		}
        [HttpGet]
        //[OutputCache(Duration = 10, VaryByParam = "*")]
        public async Task<JsonResult> GetDataByParentId (int  parentid ,int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
        {    
            var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
			    var pagerows = (await this.companyService
						               .Query(new CompanyQuery().ByParentIdWithfilter(parentid,filters)).Include(c => c.Parent)
							           .OrderBy(n=>n.OrderBy(sort,order))
							           .SelectPageAsync(page, rows, out var totalCount))
                                       .Select(  n => new { 

    ParentName = n.Parent?.Name,
    Id = n.Id,
    Name = n.Name,
    TradeCode = n.TradeCode,
    MasterCustom = n.MasterCustom,
    CreditCode = n.CreditCode,
    Code = n.Code,
    Ctype = n.Ctype,
    Scope = n.Scope,
    Address = n.Address,
    LegalPerson = n.LegalPerson,
    Contect = n.Contect,
    PhoneNumber = n.PhoneNumber,
    RegisterDate = n.RegisterDate.ToString("yyyy-MM-dd HH:mm:ss"),
    ExpirationDate = n.ExpirationDate?.ToString("yyyy-MM-dd HH:mm:ss"),
    ParentId = n.ParentId
}).ToList();
			var pagelist = new { total = totalCount, rows = pagerows };
            return Json(pagelist, JsonRequestBehavior.AllowGet);
        }
        //easyui datagrid post acceptChanges 
		[HttpPost]
		public async Task<JsonResult> AcceptChanges(Company[] companies)
		{
            if (ModelState.IsValid)
			{
            try{
               foreach (var item in companies)
               {
                 this.companyService.ApplyChanges(item);
               }
			   var result = await this.unitOfWork.SaveChangesAsync();
			   return Json(new {success=true,result}, JsonRequestBehavior.AllowGet);
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
				//[OutputCache(Duration = 10, VaryByParam = "q")]
		public async Task<JsonResult> GetCompanies(string q="")
		{
			var companyRepository = this.unitOfWork.RepositoryAsync<Company>();
			var rows = await companyRepository
                            .Queryable()
                            .Where(n=>n.Name.Contains(q))
                            .OrderBy(n=>n.Name)
                            .Select(n => new { Id = n.Id, Name = n.Name })
                            .ToListAsync();
			return Json(rows, JsonRequestBehavior.AllowGet);
		}
		 
				
		//GET: Companies/Details/:id
		public ActionResult Details(int id)
		{
			
			var company = this.companyService.Find(id);
			if (company == null)
			{
				return HttpNotFound();
			}
			return View(company);
		}
        //GET: Companies/GetItem/:id
        [HttpGet]
        public async Task<JsonResult> GetItem(int id) {
            var  company = await this.companyService.FindAsync(id);
            return Json(company,JsonRequestBehavior.AllowGet);
        }
		//GET: Companies/Create
        		public ActionResult Create()
				{
			var company = new Company();
			//set default value
			var companyRepository = this.unitOfWork.RepositoryAsync<Company>();
		   			ViewBag.ParentId = new SelectList(companyRepository.Queryable().OrderBy(n=>n.Name), "Id", "Name");
		   			return View(company);
		}
		//POST: Companies/Create
		//To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create(Company company)
		{
            if (ModelState.IsValid)
			{
                try{ 
				this.companyService.Insert(company);
				var result = await this.unitOfWork.SaveChangesAsync();
                return Json(new { success = true,result }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    return Json(new { success = false, err = e.GetMessage() }, JsonRequestBehavior.AllowGet);
                }
			    //DisplaySuccessMessage("Has update a company record");
			}
			else {
			   var modelStateErrors =string.Join(",", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
			   return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
			   //DisplayErrorMessage(modelStateErrors);
			}
			//var companyRepository = this.unitOfWork.RepositoryAsync<Company>();
			//ViewBag.ParentId = new SelectList(await companyRepository.Queryable().OrderBy(n=>n.Name).ToListAsync(), "Id", "Name", company.ParentId);
			//return View(company);
		}

        //新增对象初始化
        [HttpGet]
        public async Task<JsonResult> NewItem() {
            var company = await Task.Run(() => {
                return new Company();
                });
            return Json(company, JsonRequestBehavior.AllowGet);
        }

         
		//GET: Companies/Edit/:id
		public ActionResult Edit(int id)
		{
			var company = this.companyService.Find(id);
			if (company == null)
			{
				return HttpNotFound();
			}
			var companyRepository = this.unitOfWork.RepositoryAsync<Company>();
			ViewBag.ParentId = new SelectList(companyRepository.Queryable().OrderBy(n=>n.Name), "Id", "Name", company.ParentId);
			return View(company);
		}
		//POST: Companies/Edit/:id
		//To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Edit(Company company)
		{
			if (ModelState.IsValid)
			{
				company.TrackingState = TrackingState.Modified;
				                try{
				this.companyService.Update(company);
				                
				var result = await this.unitOfWork.SaveChangesAsync();
                return Json(new { success = true,result = result }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    return Json(new { success = false, err = e.GetMessage() }, JsonRequestBehavior.AllowGet);
                }
				
				//DisplaySuccessMessage("Has update a Company record");
				//return RedirectToAction("Index");
			}
			else {
			var modelStateErrors =string.Join(",", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
			return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
			//DisplayErrorMessage(modelStateErrors);
			}
						//var companyRepository = this.unitOfWork.RepositoryAsync<Company>();
												//return View(company);
		}
        //删除当前记录
		//GET: Companies/Delete/:id
        [HttpGet]
		public async Task<ActionResult> Delete(int id)
		{
          try{
               await this.companyService.Queryable().Where(x => x.Id == id).DeleteAsync();
               return Json(new { success = true }, JsonRequestBehavior.AllowGet);
           }
           catch (Exception e)
           {
                return Json(new { success = false, err = e.GetMessage() }, JsonRequestBehavior.AllowGet);
           }
		}
		 
       
 

        //删除选中的记录
        [HttpPost]
        public async Task<JsonResult> DeleteChecked(int[] id) {
           try{
               await this.companyService.Delete(id);
               await this.unitOfWork.SaveChangesAsync();
               return Json(new { success = true }, JsonRequestBehavior.AllowGet);
           }
           catch (Exception e)
           {
                    return Json(new { success = false, err = e.GetMessage() }, JsonRequestBehavior.AllowGet);
           }
        }
		//导出Excel
		[HttpPost]
		public async Task<ActionResult> ExportExcel( string filterRules = "",string sort = "Id", string order = "asc")
		{
			var fileName = "companies_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
			var stream = await this.companyService.ExportExcelAsync(filterRules,sort, order );
			return File(stream, "application/vnd.ms-excel", fileName);
		}
    
  }
}
