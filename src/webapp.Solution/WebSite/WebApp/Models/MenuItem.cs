using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
  //Please Register DbSet in DbContext.cs
  //public DbSet<MenuItem> MenuItems { get; set; }
  //public Entity.DbSet<MenuItem> MenuItems { get; set; }
  public partial class MenuItem : Entity
  {
    public MenuItem()
    {
      SubMenus = new HashSet<MenuItem>();
    }
    [Display(Name = "菜单名", Description = "菜单名")]
    [MaxLength(50)]
    [Required]
    public string Title { get; set; }
    [Display(Name = "描述", Description = "描述")]
    [MaxLength(100)]
    public string Description { get; set; }
    [Display(Name = "序号", Description = "序号")]
    [MaxLength(20)]
    [Required]
    public string Code { get; set; }
    [Display(Name = "Url", Description = "Url")]
    [MaxLength(100)]
    [Required]
    public string Url { get; set; }
    [Display(Name = "Controller", Description = "Controller")]
    [MaxLength(100)]
    public string Controller { get; set; }
    [Display(Name = "Action", Description = "Action")]
    [MaxLength(100)]
    public string Action { get; set; }
    [Display(Name = "Icon", Description = "Icon")]
    [StringLength(50)]
    public string IconCls { get; set; }
    [Display(Name = "是否启用", Description = "是否启用")]
    public bool IsEnabled { get; set; }
    [Display(Name = "子菜单", Description = "子菜单")]
    public ICollection<MenuItem> SubMenus { get; set; }
    [Display(Name = "父菜单", Description = "父菜单")]
    public int? ParentId { get; set; }
    [Display(Name = "父菜单", Description = "父菜单")]
    [ForeignKey("ParentId")]
    public MenuItem Parent { get; set; }
  }
}