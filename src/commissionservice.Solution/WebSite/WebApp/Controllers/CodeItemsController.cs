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
using WebApp.Models;
using WebApp.Services;
using WebApp.Repositories;

using System.IO;
using TrackableEntities;
using System.Diagnostics;

namespace WebApp.Controllers
{
  [RoutePrefix("CodeItems")]
  public class CodeItemsController : Controller
  {

    private readonly ICodeItemService _codeItemService;
    private readonly IUnitOfWorkAsync _unitOfWork;
    private readonly NLog.ILogger logger;

    public CodeItemsController(
      NLog.ILogger logger,
      ICodeItemService codeItemService, IUnitOfWorkAsync unitOfWork)
    {
      this.logger = logger;
      _codeItemService = codeItemService;
      _unitOfWork = unitOfWork;
    }

    // GET: CodeItems/Index
    [Route("Index", Name = "键值对维护", Order = 1)]
    public ActionResult Index() => View();

    public async Task<JsonResult> DeleteChecked(int[] id)
    {
      foreach (var key in id)
      {
        await this._codeItemService.DeleteAsync(key);
      }
      await this._unitOfWork.SaveChangesAsync();
      return Json(new { success = true }, JsonRequestBehavior.AllowGet);
    }
    // Get :CodeItems/PageList
    // For Index View Boostrap-Table load  data 
    [HttpGet]
    public async Task<ActionResult> GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
    {
      var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
      var totalCount = 0;
      //int pagenum = offset / limit +1;
      var codeitems = await _codeItemService
 .Query(new CodeItemQuery().Withfilter(filters))
 .OrderBy(n => n.OrderBy(sort, order))
 .SelectPageAsync(page, rows, out totalCount);

      var datarows = codeitems.Select(n => new
      {
        Multiple = n.Multiple,
        CodeType = n.CodeType,
        Id = n.Id,
        Code = n.Code,
        Text = n.Text,
        Description = n.Description,
        IsDisabled = n.IsDisabled
      }).ToList();
      var pagelist = new { total = totalCount, rows = datarows };
      return Json(pagelist, JsonRequestBehavior.AllowGet);
    }

    [HttpPost]
    public async Task<ActionResult> UpdateJavascript()
    {
      var jsfilename = Path.Combine(Server.MapPath("~/Scripts/"), "jquery.extend.formatter.js");
      await this._codeItemService.UpdateJavascriptAsync(jsfilename);
      return Json(new { success = true }, JsonRequestBehavior.AllowGet);
    }



    [HttpPost]
    public async Task<ActionResult> SaveData(CodeItem[] codeitems)
    {
      if (ModelState.IsValid)
      {
        try
        {
          foreach (var item in codeitems)
          {
            this._codeItemService.ApplyChanges(item);
          }
          await this._unitOfWork.SaveChangesAsync();
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
      else
      {
        var modelStateErrors = string.Join(",", ModelState.Keys.SelectMany(key => ModelState[key].Errors.Select(n => n.ErrorMessage)));
        return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
      }
    }





    // GET: CodeItems/Details/5
    public async Task<ActionResult> Details(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }

      var codeItem = await _codeItemService.FindAsync(id);

      if (codeItem == null)
      {
        return HttpNotFound();
      }
      return View(codeItem);
    }


    // GET: CodeItems/Create
    public ActionResult Create()
    {
      var codeItem = new CodeItem();
      //set default value


      return View(codeItem);
    }

    // POST: CodeItems/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    //[ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind(Include = "BaseCode,Id,Code,Text,Description,IsDisabled,BaseCodeId,CreatedDate,CreatedBy,LastModifiedDate,LastModifiedBy")] CodeItem codeItem)
    {
      if (ModelState.IsValid)
      {
        _codeItemService.Insert(codeItem);
        await _unitOfWork.SaveChangesAsync();
        if (Request.IsAjaxRequest())
        {
          return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        //DisplaySuccessMessage("Has append a CodeItem record");
        return RedirectToAction("Index");
      }
      else
      {
        var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
        if (Request.IsAjaxRequest())
        {
          return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
        }
      }



      return View(codeItem);
    }

    // GET: CodeItems/Edit/5
    public async Task<ActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      var codeItem = await _codeItemService.FindAsync(id);
      if (codeItem == null)
      {
        return HttpNotFound();
      }


      return View(codeItem);
    }

    // POST: CodeItems/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    //[ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "BaseCode,Id,Code,Text,Description,IsDisabled,BaseCodeId,CreatedDate,CreatedBy,LastModifiedDate,LastModifiedBy")] CodeItem codeItem)
    {
      if (ModelState.IsValid)
      {
        codeItem.TrackingState = TrackingState.Modified;
        _codeItemService.Update(codeItem);

        await _unitOfWork.SaveChangesAsync();
        if (Request.IsAjaxRequest())
        {
          return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        return RedirectToAction("Index");
      }
      else
      {
        var modelStateErrors = String.Join("", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n => n.ErrorMessage)));
        if (Request.IsAjaxRequest())
        {
          return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
        }
      }

      return View(codeItem);
    }

    // GET: CodeItems/Delete/5
    public async Task<ActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      var codeItem = await _codeItemService.FindAsync(id);
      if (codeItem == null)
      {
        return HttpNotFound();
      }
      return View(codeItem);
    }

    // POST: CodeItems/Delete/5
    [HttpPost, ActionName("Delete")]
    //[ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(int id)
    {
      var codeItem = await _codeItemService.FindAsync(id);
      _codeItemService.Delete(codeItem);
      await _unitOfWork.SaveChangesAsync();
      if (Request.IsAjaxRequest())
      {
        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
      }
      return RedirectToAction("Index");
    }






    //导出Excel
    [HttpPost]
    public async Task<ActionResult> ExportExcel(string filterRules = "", string sort = "Id", string order = "asc")
    {
      var fileName = "codeitems_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
      var stream = await _codeItemService.ExportExcelAsync(filterRules, sort, order);
      return File(stream, "application/vnd.ms-excel", fileName);

    }


    //导入数据
    [HttpPost]
    public async Task<JsonResult> ImportData()
    {
      var watch = new Stopwatch();
      watch.Start();
      var uploadfile = this.Request.Files[0];
      var uploadfilename = uploadfile.FileName;
      var model = this.Request.Form["model"] ?? "model";
      var autosave = Convert.ToBoolean(this.Request.Form["autosave"] ?? "false");
      try
      {

        var ext = Path.GetExtension(uploadfilename);
        var newfileName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}_{uploadfile.FileName.Replace(ext, "")}{ext}";//重组成新的文件名
        var stream = new MemoryStream();
        await uploadfile.InputStream.CopyToAsync(stream);
        stream.Seek(0, SeekOrigin.Begin);
        uploadfile.InputStream.Seek(0, SeekOrigin.Begin);
        var data = await NPOIHelper.GetDataTableFromExcelAsync(stream, ext);
        await this._codeItemService.ImportDataTableAsync(data);
        await this._unitOfWork.SaveChangesAsync();
        if (autosave)
        {
          var folder = this.Server.MapPath($"/UploadFiles/{model}");
          if (!Directory.Exists(folder))
          {
            Directory.CreateDirectory(folder);
          }
          var savepath = Path.Combine(folder, newfileName);
          uploadfile.SaveAs(savepath);
        }
        watch.Stop();
        //获取当前实例测量得出的总运行时间（以毫秒为单位）
        var elapsedTime = watch.ElapsedMilliseconds.ToString();
        return Json(new { success = true, filename = newfileName, elapsedTime = elapsedTime }, JsonRequestBehavior.AllowGet);
      }
      catch (Exception e)
      {
        var message = e.GetMessage();
        this.logger.Error(e, $"导入失败,文件名:{uploadfilename}");
        return this.Json(new { success = false, filename = uploadfilename, message = message }, JsonRequestBehavior.AllowGet);
      }
    }


  }
}
