using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
  public partial class Company : Entity
  {


    [Display(Name = "企业名称", Description = "企业名称")]
    [MaxLength(50)]
    [Required]
    [Index(IsUnique = true)]
    public string Name { get; set; }
    [Display(Name = "企业十位编码", Description = "企业十位编码")]
    [MaxLength(10)]
    public string TradeCode { get; set; }
    [Display(Name = "主管海关代码", Description = "主管海关代码")]
    [MaxLength(10)]
    public string MasterCustom { get; set; }
    [Display(Name = "统一社会信用代码", Description = "统一社会信用代码")]
    [MaxLength(18)]
    [Index(IsUnique = true)]
    //[Required]
    public string CreditCode { get; set; }
    [Display(Name = "备案号", Description = "备案号")]
    [MaxLength(10)]
    public string Code { get; set; }
    [Display(Name = "企业类型", Description = "企业类型")]
    [MaxLength(128)]
    public string Ctype { get; set; }
    [Display(Name = "经营范围", Description = "经营范围")]
    [MaxLength(512)]
    public string Scope { get; set; }
    [Display(Name = "地址", Description = "地址")]
    [MaxLength(128)]
    [DefaultValue("-")]
    public string Address { get; set; }
    [Display(Name = "法人", Description = "法人")]
    [MaxLength(56)]
    public string LegalPerson  { get; set; }
    [Display(Name = "联系人", Description = "联系人")]
    [MaxLength(56)]
    public string Contect { get; set; }
    [Display(Name = "联系电话", Description = "联系电话")]
    [MaxLength(56)]
    public string PhoneNumber { get; set; }
    [Display(Name = "注册日期", Description = "注册日期")]
    [DefaultValue("now")]
    public DateTime RegisterDate { get; set; }
    [Display(Name = "有效期", Description = "有效期")]
    [DefaultValue(null)]
    public DateTime? ExpirationDate { get; set; }
    [Display(Name = "母公司", Description = "母公司")]
    public int? ParentId { get; set; }
    [ForeignKey("ParentId")]
    [Display(Name = "母公司", Description = "母公司")]
    public Company Parent { get; set; }
  }



}