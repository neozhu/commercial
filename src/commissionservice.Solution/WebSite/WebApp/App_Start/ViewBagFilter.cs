using System.Security.Claims;
using System.Web.Mvc;

namespace WebApp
{
  public class ViewBagFilter : IActionFilter
  {
    private static readonly string Enabled = "Enabled";
    private static readonly string Disabled = string.Empty;

    public void OnActionExecuting(ActionExecutingContext context)
    {
      if (context.Controller is Controller controller)
      {
        
        var claimsidentity = (ClaimsIdentity)controller.User.Identity;
        var username = claimsidentity.FindFirst(ClaimTypes.Name)?.Value;
        var givenname = claimsidentity.FindFirst(ClaimTypes.GivenName)?.Value;
        var email = claimsidentity.FindFirst(ClaimTypes.Email)?.Value;
        var mobilephone = claimsidentity.FindFirst(ClaimTypes.MobilePhone)?.Value;
        var avatars = claimsidentity.FindFirst("http://schemas.microsoft.com/identity/claims/avatars")?.Value;
        var tenantid = claimsidentity.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value;
        var tenantname = claimsidentity.FindFirst("http://schemas.microsoft.com/identity/claims/tenantname")?.Value;
        var tenantdb = claimsidentity.FindFirst("http://schemas.microsoft.com/identity/claims/tenantdb")?.Value;
        var culture = claimsidentity.FindFirst(ClaimTypes.Country)?.Value;
        var role = claimsidentity.FindFirst(ClaimTypes.Role)?.Value;
        // SmartAdmin Toggle Features
        controller.ViewBag.AppSidebar = Enabled;
        controller.ViewBag.AppHeader = Enabled;
        controller.ViewBag.AppLayoutShortcut = Enabled;
        controller.ViewBag.AppFooter = Enabled;
        controller.ViewBag.ShortcutMenu = Enabled;
        controller.ViewBag.ChatInterface = Enabled;
        controller.ViewBag.LayoutSettings = Enabled;

        // SmartAdmin Default Settings
        controller.ViewBag.App = Settings.App;
        controller.ViewBag.AppName = Settings.AppName;
        controller.ViewBag.Company = Settings.Company;
        controller.ViewBag.ICP = Settings.ICP;
        controller.ViewBag.AppFlavor = Settings.AppFlavor;
        controller.ViewBag.AppFlavorSubscript = Settings.AppFlavorSubscript;
        controller.ViewBag.User = username;
        controller.ViewBag.Role = role;
        controller.ViewBag.Email = email;
        controller.ViewBag.Twitter = email;
        controller.ViewBag.Avatar = avatars;
        controller.ViewBag.MobilePhone = mobilephone;
        controller.ViewBag.GivenName = givenname;
        controller.ViewBag.TenantId = tenantid;
        controller.ViewBag.TenantName = tenantname;
        controller.ViewBag.TenantDb = tenantdb;
        controller.ViewBag.Culture = culture;
        controller.ViewBag.Version = Settings.Version;
        controller.ViewBag.ThemeVersion = Settings.ThemeVersion;
        controller.ViewBag.Bs4v = "4.5.0";
        controller.ViewBag.IconPrefix = Settings.IconPrefix;
        controller.ViewBag.Logo = "logo.png";
        controller.ViewBag.LogoM = "logo.png";
        controller.ViewBag.Copyright = $"2019 © { Settings.AppName} &nbsp; { Settings.Company} <a href='http://beian.miit.gov.cn/' class='text-primary fw-500' title='粤ICP备{Settings.ICP}号' target='_blank'>工业和信息化部备案管理系统网站 粤ICP备{Settings.ICP}号 </a>";
        controller.ViewBag.CopyrightInverse = $"2019 © { Settings.AppName} &nbsp;{ Settings.Company} <a href='http://beian.miit.gov.cn/' class='text-primary fw-500' title='粤ICP备{Settings.ICP}号' target='_blank'>工业和信息化部备案管理系统网站 粤ICP备{Settings.ICP}号 </a>";
      }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
  }
}
