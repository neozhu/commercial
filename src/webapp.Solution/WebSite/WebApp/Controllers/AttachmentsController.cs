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
using Microsoft.Ajax.Utilities;

namespace WebApp.Controllers
{
/// <summary>
/// File: AttachmentsController.cs
/// Purpose:主数据管理/附件管理
/// Created Date: 2020/5/20 20:49:55
/// Author: neo.zhu
/// Tools: SmartCode MVC5 Scaffolder for Visual Studio 2017
/// TODO: Registers the type mappings with the Unity container(Mvc.UnityConfig.cs)
/// <![CDATA[
///    container.RegisterType<IRepositoryAsync<Attachment>, Repository<Attachment>>();
///    container.RegisterType<IAttachmentService, AttachmentService>();
/// ]]>
/// Copyright (c) 2012-2018 All Rights Reserved
/// </summary>
    [Authorize]
    [RoutePrefix("Attachments")]
	public class AttachmentsController : Controller
	{
		private readonly IAttachmentService  attachmentService;
		private readonly IUnitOfWorkAsync unitOfWork;
        private readonly NLog.ILogger logger;
		public AttachmentsController (
          IAttachmentService  attachmentService, 
          IUnitOfWorkAsync unitOfWork,
          NLog.ILogger logger
          )
		{
			this.attachmentService  = attachmentService;
			this.unitOfWork = unitOfWork;
            this.logger = logger;
		}
        		//GET: Attachments/Index
        //[OutputCache(Duration = 60, VaryByParam = "none")]
        [Route("Index", Name = "附件管理", Order = 1)]
		public ActionResult Index() => this.View();
    //接收上传文件
    public async Task<ActionResult> Upload() {
      var file = this.Request.Files[0];
      var user = ViewBag.GivenName;
      try
      {
        var tags = this.Request.Form["tags"];
        var name = this.Request.Form["name"];
        var folder = this.Server.MapPath("~/UploadFiles");
        var relpath = "~/UploadFiles";
        await this.attachmentService.SaveFile(file, tags, folder, relpath, name, user);
        await this.unitOfWork.SaveChangesAsync();
        return Content($"{file.FileName}:上传成功", "text/plain");
      }
      catch (Exception e) {
        throw e;
      }
    }
    //重命名
    public async Task<JsonResult> Rename(string fileid, string newfilename) {
      try
      {
        if (!string.IsNullOrEmpty(newfilename))
        {
          await this.attachmentService.Rename(fileid, newfilename);
          await this.unitOfWork.SaveChangesAsync();
        }
        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
      }
      catch (Exception e)
      {
        return Json(new { success = false,err=e.GetBaseException().Message }, JsonRequestBehavior.AllowGet);
      }
    }
		//Get :Attachments/GetData
		//For Index View datagrid datasource url
        
		[HttpGet]
        //[OutputCache(Duration = 10, VaryByParam = "*")]
		 public async Task<JsonResult> GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
		{
			var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
			var pagerows  = (await this.attachmentService
						               .Query(new AttachmentQuery().Withfilter(filters))
							           .OrderBy(n=>n.OrderBy(sort,order))
							           .SelectPageAsync(page, rows, out var totalCount))
                                       .Select(  n => new { 

    Id = n.Id,
    FileName = n.FileName,
    FileId = n.FileId,
    Ext = n.Ext,
    FilePath = n.FilePath,
    n.RelativePath,
    n.Size,
    n.Tags,
    RefKey = n.RefKey,
    Owner = n.Owner,
    Upload = n.Upload.ToString("yyyy-MM-dd HH:mm:ss")
}).ToList();
			var pagelist = new { total = totalCount, rows = pagerows };
			return Json(pagelist, JsonRequestBehavior.AllowGet);
		}
        //easyui datagrid post acceptChanges 
		[HttpPost]
		public async Task<JsonResult> SaveData(Attachment[] attachments)
		{
            if (attachments == null)
            {
                throw new ArgumentNullException(nameof(attachments));
            }
            if (ModelState.IsValid)
			{
            try{
               foreach (var item in attachments)
               {
                 this.attachmentService.ApplyChanges(item);
               }
			   var result = await this.unitOfWork.SaveChangesAsync();
			   return Json(new {success=true,result}, JsonRequestBehavior.AllowGet);
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                var errormessage = string.Join(",", e.EntityValidationErrors.Select(x => x.ValidationErrors.FirstOrDefault()?.PropertyName + ":" + x.ValidationErrors.FirstOrDefault()?.ErrorMessage));
                 return Json(new { success = false, err = errormessage }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
                {
                    return Json(new { success = false, err = e.GetBaseException().Message }, JsonRequestBehavior.AllowGet);
                }
		    }
            else
            {
                var modelStateErrors = string.Join(",", ModelState.Keys.SelectMany(key => ModelState[key].Errors.Select(n => n.ErrorMessage)));
                return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
            }
        
        }
						//GET: Attachments/Details/:id
		public ActionResult Details(int id)
		{
			
			var attachment = this.attachmentService.Find(id);
			if (attachment == null)
			{
				return HttpNotFound();
			}
			return View(attachment);
		}
        //GET: Attachments/GetItem/:id
        [HttpGet]
        public async Task<JsonResult> GetItem(int id) {
            var  attachment = await this.attachmentService.FindAsync(id);
            return Json(attachment,JsonRequestBehavior.AllowGet);
        }
		//GET: Attachments/Create
        		public ActionResult Create()
				{
			var attachment = new Attachment();
			//set default value
			return View(attachment);
		}
		//POST: Attachments/Create
		//To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create(Attachment attachment)
		{
			if (attachment == null)
            {
                throw new ArgumentNullException(nameof(attachment));
            } 
            if (ModelState.IsValid)
			{
                try{ 
				this.attachmentService.Insert(attachment);
				var result = await this.unitOfWork.SaveChangesAsync();
                return Json(new { success = true,result }, JsonRequestBehavior.AllowGet);
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException e)
                {
                   var errormessage = string.Join(",", e.EntityValidationErrors.Select(x => x.ValidationErrors.FirstOrDefault()?.PropertyName + ":" + x.ValidationErrors.FirstOrDefault()?.ErrorMessage));
                   return Json(new { success = false, err = errormessage }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    return Json(new { success = false, err = e.GetBaseException().Message }, JsonRequestBehavior.AllowGet);
                }
			    //DisplaySuccessMessage("Has update a attachment record");
			}
			else {
			   var modelStateErrors =string.Join(",", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
			   return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
			   //DisplayErrorMessage(modelStateErrors);
			}
			//return View(attachment);
		}

        //新增对象初始化
        [HttpGet]
        public async Task<JsonResult> NewItem() {
            var attachment = await Task.Run(() => {
                return new Attachment();
                });
            return Json(attachment, JsonRequestBehavior.AllowGet);
        }

         
		//GET: Attachments/Edit/:id
		public ActionResult Edit(int id)
		{
			var attachment = this.attachmentService.Find(id);
			if (attachment == null)
			{
				return HttpNotFound();
			}
			return View(attachment);
		}
		//POST: Attachments/Edit/:id
		//To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Edit(Attachment attachment)
		{
            if (attachment == null)
            {
                throw new ArgumentNullException(nameof(attachment));
            }
			if (ModelState.IsValid)
			{
				attachment.TrackingState = TrackingState.Modified;
				                try{
				this.attachmentService.Update(attachment);
				                
				var result = await this.unitOfWork.SaveChangesAsync();
                return Json(new { success = true,result = result }, JsonRequestBehavior.AllowGet);
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException e)
                {
                    var errormessage = string.Join(",", e.EntityValidationErrors.Select(x => x.ValidationErrors.FirstOrDefault()?.PropertyName + ":" + x.ValidationErrors.FirstOrDefault()?.ErrorMessage));
                    return Json(new { success = false, err = errormessage }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    return Json(new { success = false, err = e.GetBaseException().Message }, JsonRequestBehavior.AllowGet);
                }
				
				//DisplaySuccessMessage("Has update a Attachment record");
				//return RedirectToAction("Index");
			}
			else {
			var modelStateErrors =string.Join(",", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
			return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
			//DisplayErrorMessage(modelStateErrors);
			}
						//return View(attachment);
		}
        //删除当前记录
		//GET: Attachments/Delete/:id
        [HttpGet]
		public async Task<ActionResult> Delete(int id)
		{
          try{
               await this.attachmentService.Queryable().Where(x => x.Id == id).DeleteAsync();
               return Json(new { success = true }, JsonRequestBehavior.AllowGet);
           }
           catch (System.Data.Entity.Validation.DbEntityValidationException e)
           {
                var errormessage = string.Join(",", e.EntityValidationErrors.Select(x => x.ValidationErrors.FirstOrDefault()?.PropertyName + ":" + x.ValidationErrors.FirstOrDefault()?.ErrorMessage));
                return Json(new { success = false, err = errormessage }, JsonRequestBehavior.AllowGet);
           }
           catch (Exception e)
           {
                return Json(new { success = false, err = e.GetBaseException().Message }, JsonRequestBehavior.AllowGet);
           }
		}
		 
       
 

        //删除选中的记录
        [HttpPost]
        public async Task<JsonResult> DeleteChecked(int[] id) {
           if (id == null)
           {
                throw new ArgumentNullException(nameof(id));
           }
           try{
               await this.attachmentService.Delete(id);
               await this.unitOfWork.SaveChangesAsync();
               return Json(new { success = true }, JsonRequestBehavior.AllowGet);
           }
           catch (System.Data.Entity.Validation.DbEntityValidationException e)
           {
                    var errormessage = string.Join(",", e.EntityValidationErrors.Select(x => x.ValidationErrors.FirstOrDefault()?.PropertyName + ":" + x.ValidationErrors.FirstOrDefault()?.ErrorMessage));
                    return Json(new { success = false, err = errormessage }, JsonRequestBehavior.AllowGet);
           }
           catch (Exception e)
           {
                    return Json(new { success = false, err = e.GetBaseException().Message }, JsonRequestBehavior.AllowGet);
           }
        }
		//导出Excel
		[HttpPost]
		public async Task<ActionResult> ExportExcel( string filterRules = "",string sort = "Id", string order = "asc")
		{
			var fileName = "attachments_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
			var stream = await this.attachmentService.ExportExcelAsync(filterRules,sort, order );
			return File(stream, "application/vnd.ms-excel", fileName);
		}
		private void DisplaySuccessMessage(string msgText) => TempData["SuccessMessage"] = msgText;
        private void DisplayErrorMessage(string msgText) => TempData["ErrorMessage"] = msgText;
		 
	}
}
