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
  public  class CompanyTreeItem 
  {


    public int Id { get; set; }
  
    public string Name { get; set; }
    
    public string TradeCode { get; set; }
   
    public string MasterCustom { get; set; }
   
    public string CreditCode { get; set; }
   
    public string Code { get; set; }
    
    public string Ctype { get; set; }
    
    public string Scope { get; set; }
    
    public string Address { get; set; }
   
    public string LegalPerson  { get; set; }
    
    public string Contect { get; set; }
   
    public string PhoneNumber { get; set; }
    
    public DateTime RegisterDate { get; set; }
    
    public DateTime? ExpirationDate { get; set; }

    public string iconCls { get; set; } = "";
    public string state { get; set; } = "open";
    public IEnumerable<CompanyTreeItem> children { get; set; }
  }

  public class CompanyComboTreeItem
  {
    public int id { get; set; }
    public string text { get; set; }
    public IEnumerable<CompanyComboTreeItem> children { get; set; }
  }

}