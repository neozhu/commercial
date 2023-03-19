using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models.ViewModel
{
  public class logtimetotal
  {
    public DateTime time { get; set; }
    public int total { get; set; }
    public string level { get; set; }
  }
  public class logleveltotal {
    public string level { get; set; }
    public string total { get; set; }
   }
}