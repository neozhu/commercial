using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Repository.Pattern.UnitOfWork;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers
{
  [Authorize]
  public class FileManagerController : Controller
  {

    private readonly NLog.ILogger logger;
    
    public FileManagerController(
       NLog.ILogger logger
)
    {

      this.logger = logger;
    }
    //Excel上传导入接口
    [HttpPost]
    public JsonResult Upload()
    {
      
      
        return this.Json(new { success = true }, JsonRequestBehavior.AllowGet);
      
    }


    [FileDownload]
    public async  Task<FileContentResult> Download(string file = "")
    {
      if (string.IsNullOrEmpty(file))
      {
        throw new ArgumentNullException($"文件名不能为空!");
      }
      byte[] fileContent = null;
      var fileName = "";
      var mimeType = "";
      this.HttpContext.Response.AppendCookie(new HttpCookie("fileDownload", "true") { Path = "/" });

      var downloadFile = new FileInfo(this.Server.MapPath(file));
      if (downloadFile.Exists)
      {
        fileName = downloadFile.Name;
        mimeType = this.GetMimeType(downloadFile.Extension);
        fileContent = new byte[Convert.ToInt32(downloadFile.Length)];
        using (var fs = downloadFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
        {
          await fs.ReadAsync(fileContent, 0, Convert.ToInt32(downloadFile.Length));
        }
        return this.File(fileContent, mimeType, fileName);
      }
      else
      {
        throw new FileNotFoundException($"文件 {file} 不存在!");
      }
    }
    [HttpDelete]
    public async Task<JsonResult> Revert() {
      var req = Request.InputStream;
      var filename =await new StreamReader(req).ReadToEndAsync();
      if (!string.IsNullOrEmpty(filename))
      {
        var folder = this.Server.MapPath("~/UploadFiles");
        var path = Path.Combine(folder, filename);
        if (System.IO.File.Exists(path))
        {
          System.IO.File.Delete(path);
        }
      }
      return this.Json(new { success = true }, JsonRequestBehavior.AllowGet);
    }
    [HttpPost]
    public JsonResult Remove(string filename = "")
    {
      if (!string.IsNullOrEmpty(filename))
      {
        var folder = this.Server.MapPath("~/UploadFiles");
        var path = Path.Combine(folder, filename);
        if (System.IO.File.Exists(path))
        {
          System.IO.File.Delete(path);
        }
      }
      return this.Json(new { success = true }, JsonRequestBehavior.AllowGet);
    }

    private string GetMimeType(string fileExtensionStr)
    {
      var ContentTypeStr = "application/octet-stream";
      var fileExtension = fileExtensionStr.ToLower();
      switch (fileExtension)
      {
        case ".mp3":
          ContentTypeStr = "audio/mpeg3";
          break;
        case ".mpeg":
          ContentTypeStr = "video/mpeg";
          break;
        case ".jpg":
          ContentTypeStr = "image/jpeg";
          break;
        case ".bmp":
          ContentTypeStr = "image/bmp";
          break;
        case ".gif":
          ContentTypeStr = "image/gif";
          break;
        case ".doc":
          ContentTypeStr = "application/msword";
          break;
        case ".css":
          ContentTypeStr = "text/css";
          break;
        case ".html":
          ContentTypeStr = "text/html";
          break;
        case ".htm":
          ContentTypeStr = "text/html";
          break;
        case ".swf":
          ContentTypeStr = "application/x-shockwave-flash";
          break;
        case ".exe":
          ContentTypeStr = "application/octet-stream";
          break;
        case ".inf":
          ContentTypeStr = "application/x-texinfo";
          break;
        case ".xls":
        case ".xlsx":
          ContentTypeStr = "application/vnd.ms-excel";
          break;
        default:
          ContentTypeStr = "application/octet-stream";
          break;
      }
      return ContentTypeStr;
    }

    
  }
}
