using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using TrackableEntities;

namespace WebApp.Models
{
  // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
  public class ApplicationUser : IdentityUser
  {
    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
    {
      // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
      var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
      //var result = await manager.AddClaimAsync(userIdentity.GetUserId(), new Claim("http://schemas.microsoft.com/identity/claims/tenantid", this.TenantId.ToString()));
      // Add custom user claims here
      //userIdentity.AddClaim(new Claim("http://schemas.microsoft.com/identity/claims/tenantid", this.TenantId.ToString()));
      //userIdentity.AddClaim(new Claim("CompanyName", this.CompanyName));
      //userIdentity.AddClaim(new Claim("EnabledChat", this.EnabledChat.ToString()));
      //userIdentity.AddClaim(new Claim("FullName", this.FullName));
      //userIdentity.AddClaim(new Claim("AvatarsX50", this.AvatarsX50));
      //userIdentity.AddClaim(new Claim("AvatarsX120", this.AvatarsX120));
      
      return userIdentity;
    }
    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
    {
      // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
      var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
      // Add custom user claims here
      //userIdentity.AddClaim(new Claim("http://schemas.microsoft.com/identity/claims/tenantid", this.TenantId.ToString()));
      //userIdentity.AddClaim(new Claim("CompanyName", this.CompanyName));
      //userIdentity.AddClaim(new Claim("EnabledChat", this.EnabledChat.ToString()));
      //userIdentity.AddClaim(new Claim("FullName", this.FullName));
      //userIdentity.AddClaim(new Claim("AvatarsX50", this.AvatarsX50));
      //userIdentity.AddClaim(new Claim("AvatarsX120", this.AvatarsX120));
      return userIdentity;
    }

    [Display(Name = "全名")]
    public string FullName { get; set; }
    [Display(Name = "性别")]
    public int Gender { get; set; }
    public int AccountType { get; set; }
    [Display(Name = "租户数据库")]
    public string TenantDb { get; set; }
    [Display(Name = "租户名称")]
    public string TenantName { get; set; }
    [Display(Name = "是否在线")]
    public bool IsOnline { get; set; }
    [Display(Name = "是否开启聊天功能")]
    public bool EnabledChat { get; set; }
    [Display(Name = "头像")]
    public string Avatars { get; set; }
    [Display(Name = "大头像")]
    public string AvatarsX120 { get; set; }
    [Display(Name = "租户ID")]
    public int TenantId { get; set; }
  }
  [Table("AspNetTenants")]
  public partial class Tenant 
  {
    [Key]
    public int Id { get; set; }
    [Display(Name = "租户名称", Description = "租户名称")]
    [MaxLength(50)]
    [Required]
    public string Name { get; set; }
    [Display(Name = "数据库连接", Description = "数据库连接")]
    [MaxLength(500)]
    public string ConnectionStrings { get; set; }
    [Display(Name = "是否禁用", Description = "是否禁用")]
    public bool Disabled { get; set; }
    [NotMapped]
    public TrackingState TrackingState { get; set; }
  }
  [Table("AspNetLogs")]
  public partial class Log {
    [Key] 
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [MaxLength(128)]
    public string Id { get; set; }
    [Display(Name = "主机名", Description = "主机名")]
    [MaxLength(128)]
    public string MachineName { get; set; }
    [Display(Name = "时间", Description = "时间")]
    public DateTime? Logged { get; set; }
    [Display(Name = "级别", Description = "级别")]
    [MaxLength(5)]
    public string Level { get; set; }
    [Display(Name = "信息", Description = "信息")]
    public string Message { get; set; }
    [Display(Name = "异常信息", Description = "异常信息")]
    public string Exception { get; set; }
    [Display(Name = "客户端IP", Description = "客户端IP")]
    [MaxLength(128)]
    public string ClientIP { get; set; }
    [Display(Name = "事件属性", Description = "事件属性")]
    public string Properties { get; set; }
    [Display(Name = "表单值", Description = "表单值")]
    public string RequestForm { get; set; }
    [Display(Name = "使用账号", Description = "使用账号")]
    [MaxLength(56)]
    public string User { get; set; }
    [Display(Name = "日志", Description = "日志")]
    [MaxLength(256)]
    public string Logger { get; set; }
    [Display(Name = "调用站点", Description = "调用站点")]
    [MaxLength(256)]
    public string Callsite { get; set; }
    [Display(Name = "已处理", Description = "已处理")]
    public bool Resolved { get; set; }
  }
  public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
  {
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Log> Logs { get; set; }
    public ApplicationDbContext()
        : base("DefaultConnection", throwIfV1Schema: false) => Database.SetInitializer<ApplicationDbContext>(null);

    public static ApplicationDbContext Create() => new ApplicationDbContext();


  }


}