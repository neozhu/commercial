using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using WebApp.Models;

namespace WebApp.Controllers
{
  [Authorize]
  public class RoleManageController : Controller
  {

    private ApplicationDbContext dbContext => HttpContext.GetOwinContext().Get<ApplicationDbContext>();
    private ApplicationUserManager _userManager
    {
      get => this.HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

    }
    private ApplicationRoleManager _roleManager
    {
      get =>  this.HttpContext.GetOwinContext().Get<ApplicationRoleManager>();

    }


    public async Task<ActionResult> Index()
    {
      var roles = await _roleManager.Roles.OrderBy(x=>x.Name).ToListAsync();
      var users = await _userManager.Users.OrderBy(x => x.UserName).ToListAsync();
      var userinroles = new List<UserInRolesViewModel>();
      foreach (var user in users) {
        var inroles = await _userManager.GetRolesAsync(user.Id);
        userinroles.Add(new UserInRolesViewModel()
        {
           GivenName=user.FullName,
            Roles=inroles.ToArray(),
             UserId=user.Id,
              UserName=user.UserName
        });
      }
      ViewBag.roles = roles;
      ViewBag.userinroles = userinroles;
      
      return View();
    }
    //新增角色
    public async Task<JsonResult> CreateRole(string name) {
      var exist = await _roleManager.RoleExistsAsync(name);
      if (!exist) {
        await _roleManager.CreateAsync(new ApplicationRole() { Name=name });
       }
      return Json("", JsonRequestBehavior.AllowGet);
    }
    //删除角色
    public async Task<JsonResult> RemoveRole(string name)
    {
      var exist = await _roleManager.FindByNameAsync(name);
      if (exist!=null)
      {
        await _roleManager.DeleteAsync(exist);
      }
      return Json("", JsonRequestBehavior.AllowGet);
    }
    //分配角色
    public async Task<JsonResult> AddToRoles(string userName, string[] roles) {
      var user = await _userManager.FindByNameAsync(userName);
      var myroles = await _userManager.GetRolesAsync(user.Id);
      var result1=await _userManager.RemoveFromRolesAsync(user.Id,myroles.ToArray());
      var result2= await _userManager.AddToRolesAsync(user.Id, roles);
      return Json("", JsonRequestBehavior.AllowGet);
    }
    //移除角色
    public async Task<JsonResult> RemoveFromRole(string userName, string role) {
      var user = await _userManager.FindByNameAsync(userName);
      var result1 = await _userManager.RemoveFromRoleAsync(user.Id, role);
      return Json("", JsonRequestBehavior.AllowGet);
    }
  }
}
