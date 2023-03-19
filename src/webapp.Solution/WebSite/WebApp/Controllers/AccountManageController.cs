using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
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

namespace WebApp.Controllers
{
  [Authorize]
  [RoutePrefix("AccountManage")]
  public class AccountManageController : Controller
  {
    private readonly NLog.ILogger logger;
    private ApplicationUserManager _userManager;

    private  ApplicationDbContext dbContext => HttpContext.GetOwinContext().Get<ApplicationDbContext>();
    public AccountManageController(
                                NLog.ILogger logger
                               ) {
      this.logger = logger;
    }
    public ApplicationUserManager UserManager
    {
      get => this._userManager ?? this.HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
      private set => this._userManager = value;
    }
    private ApplicationRoleManager roleManager
    {
      get => this.HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
    }
    private ApplicationSignInManager _signInManager;
     
    public ApplicationSignInManager SignInManager
    {
      get => this._signInManager ?? this.HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
      private set => this._signInManager = value;
    }
    [Route("Index", Name = "系统账号管理", Order = 1)]
    public async Task<ActionResult> Index() {
      var data = await this.dbContext.Tenants
        .Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() })
        .ToListAsync();
      ViewBag.TenantId = data;
      return View();
      }


    //获取租户数据
    public async Task<JsonResult> GetTenantData()
    {
      var data = await this.dbContext.Tenants.ToListAsync();
      return Json(data, JsonRequestBehavior.AllowGet);
    }

    //解锁，加锁账号
    public async Task<JsonResult> SetLockout(string[] userid)
    {
      foreach (var id in userid)
      {
        await this.UserManager.SetLockoutEndDateAsync(id, new DateTimeOffset(DateTime.Now.AddYears(1)));
      }
      return Json(new { success = true }, JsonRequestBehavior.AllowGet);
    }
    //注册新用户
    public async Task<JsonResult> ReregisterUser(AccountRegistrationModel model) {
      if (this.ModelState.IsValid)
      {
        var tenant =await this.dbContext.Tenants.FindAsync(model.TenantId);
        var user = new ApplicationUser
        {
          UserName = model.Username,
          FullName = model.FullName,
          TenantDb = tenant.ConnectionStrings,
          TenantName = tenant.Name,
          TenantId = model.TenantId,
          Gender = 0,
          Email = model.Email,
          PhoneNumber = model.PhoneNumber,
          AccountType = 0,
          Avatars = "ng.jpg",
          AvatarsX120 = "ng.jpg",

        };
        var result = await this.UserManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
          this.logger.Info($"注册成功【{user.UserName}】");
          await this.UserManager.AddClaimAsync(user.Id, new System.Security.Claims.Claim("http://schemas.microsoft.com/identity/claims/tenantid", user.TenantId.ToString()));
          await this.UserManager.AddClaimAsync(user.Id, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.UserName));
          await this.UserManager.AddClaimAsync(user.Id, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.GivenName,  user.FullName??""));
          await this.UserManager.AddClaimAsync(user.Id, new System.Security.Claims.Claim("http://schemas.microsoft.com/identity/claims/tenantname",   user.TenantName??""));
          await this.UserManager.AddClaimAsync(user.Id, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email));
          await this.UserManager.AddClaimAsync(user.Id, new System.Security.Claims.Claim("http://schemas.microsoft.com/identity/claims/avatars",  user.Avatars ?? ""));
          await this.UserManager.AddClaimAsync(user.Id, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.MobilePhone,   user.PhoneNumber??""));
          await this.UserManager.AddClaimAsync(user.Id, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Country, "zh-cn"));
          var role = "users";
          var any = await this.roleManager.FindByNameAsync(role);
          if (any != null)
          {
            await this.UserManager.AddToRoleAsync(user.Id, role);
          }
          return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        else
        {
          return Json(new { success = false, err = string.Join(",", result.Errors) }, JsonRequestBehavior.AllowGet);
        }

      }
      else
      {
        var modelStateErrors = string.Join(",", ModelState.Keys.SelectMany(key => ModelState[key].Errors.Select(n => n.ErrorMessage)));
        return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
      }
    }
    public async Task<JsonResult> SetUnLockout(string[] userid)
    {
      foreach (var id in userid)
      {
        await this.UserManager.SetLockoutEndDateAsync(id, DateTime.Now);
      }
      return Json(new { success = true }, JsonRequestBehavior.AllowGet);
    }
    /// <summary>
    /// 重置密码
    /// </summary>
    /// <param name="id"></param>
    /// <param name="newPassword"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<JsonResult> ResetPassword(string id, string newPassword)
    {
      var code =await this.UserManager.GeneratePasswordResetTokenAsync(id);
      var result =await this.UserManager.ResetPasswordAsync(id, code, newPassword);
      if (result.Succeeded)
      {
        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
      }
      else
      {
        return Json(new { success = false, err = string.Join(",", result.Errors) }, JsonRequestBehavior.AllowGet);
      }

    }
    
    [HttpGet]
    public  JsonResult GetData(int page = 1, int rows = 10, string sort = "Id", string order = "desc", string filterRules = "")
    {
      var filters = PredicateBuilder.From<ApplicationUser>(filterRules);
      var totalCount = 0;

      var users = this.UserManager.Users.Where(filters).OrderByName(sort, order);
     
      totalCount = users.Count();
      var datalist = users.Skip(( page - 1 ) * rows).Take(rows);
      var datarows = datalist.Select(n => new
      {
        Id = n.Id,
        UserName = n.UserName,
        FullName = n.FullName,
        Gender = n.Gender,
        TenantDb = n.TenantName,
        TenantName = n.TenantName,
        AccountType = n.AccountType,
        Email = n.Email,
        TenantId = n.TenantId,
        PhoneNumber = n.PhoneNumber,
        AvatarsX50 = n.Avatars,
        AvatarsX120 = n.AvatarsX120,
        AccessFailedCount = n.AccessFailedCount,
        LockoutEnabled = n.LockoutEnabled,
        LockoutEndDateUtc = n.LockoutEndDateUtc,
        IsOnline = n.IsOnline,
        EnabledChat = n.EnabledChat
      }).ToList();
      var pagelist = new { total = totalCount, rows = datarows };
      return this.Json(pagelist, JsonRequestBehavior.AllowGet);
    }

   
    [HttpGet]
    public JsonResult GetAvatarsX50()
    {
      var list = new List<dynamic>();
      for (var i = 1; i <= 8; i++)
      {
        list.Add(new { name = "femal" + i.ToString() });
        list.Add(new { name = "male" + i.ToString() });
      }
      return this.Json(list.ToArray(), JsonRequestBehavior.AllowGet);
    }

    [HttpPost]
    public async Task<JsonResult> DeleteCheckedAsync(string[] id)
    {
      foreach (var key in id)
      {
        var user = await this.UserManager.FindByIdAsync(key);
        await this.UserManager.DeleteAsync(user);
      }
      return this.Json(new { success = true }, JsonRequestBehavior.AllowGet);
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
        await this.importUser(data);
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
    private async Task importUser(DataTable datatable)
    {
      foreach (DataRow dr in datatable.Rows)
      {
        var userName = dr["账号"].ToString();
        var email = dr["电子邮件"].ToString();
        var password = dr["密码"].ToString();
        var fullName = dr["姓名"].ToString();
        var role = dr["角色"].ToString();
        var user = new ApplicationUser
        {
          UserName = userName,
          Email = email,
          FullName = fullName,
          Gender = 1,
          TenantId = ViewBag.TenantId,
          TenantDb = ViewBag.TenantDb,
          TenantName = ViewBag.TenantName,
          PhoneNumber = null,
          AccountType = 0,
          Avatars = "ng.png",
          LockoutEnabled = true,
          AvatarsX120 = "ng.png"
        };
        var result = await this._userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
          await this._userManager.AddClaimAsync(user.Id, new System.Security.Claims.Claim("http://schemas.microsoft.com/identity/claims/tenantid", user.TenantId.ToString()));
          await this._userManager.AddClaimAsync(user.Id, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, string.IsNullOrEmpty(user.FullName) ? "" : user.FullName));
          await this._userManager.AddClaimAsync(user.Id, new System.Security.Claims.Claim("http://schemas.microsoft.com/identity/claims/tenantname", string.IsNullOrEmpty(user.TenantName) ? "" : user.TenantName));
          await this._userManager.AddClaimAsync(user.Id, new System.Security.Claims.Claim("http://schemas.microsoft.com/identity/claims/tenantdb", string.IsNullOrEmpty(user.TenantDb) ? "" : user.TenantDb));
          await this._userManager.AddClaimAsync(user.Id, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email));
          await this._userManager.AddClaimAsync(user.Id, new System.Security.Claims.Claim("http://schemas.microsoft.com/identity/claims/avatars", user.Avatars));
          await this._userManager.AddClaimAsync(user.Id, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.MobilePhone, string.IsNullOrEmpty(user.PhoneNumber) ? "" : user.PhoneNumber));
          await this._userManager.AddClaimAsync(user.Id, new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Country, "zh-cn"));

          if (!string.IsNullOrEmpty(role))
          {
            var rolearray = role.Split(new char[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var r in rolearray)
            {
              var any = await this.roleManager.FindByNameAsync(r);
              if (any != null)
              {
                await this._userManager.AddToRoleAsync(user.Id, r);
              }
              else
              {
                await this.roleManager.CreateAsync(new ApplicationRole() { Name = r });
                await this._userManager.AddToRoleAsync(user.Id, role);
              }
            }
          }

        }
      }
    }

  }
}