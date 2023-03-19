using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity.SqlServer;
using Repository.Pattern.Repositories;
using Repository.Pattern.Ef6;
using System.Web.WebPages;
using WebApp.Models;

namespace WebApp.Repositories
{
/// <summary>
/// File: CompanyQuery.cs
/// Purpose: easyui datagrid filter query 
/// Created Date: 2020/8/7 9:08:03
/// Author: neo.zhu
/// Tools: SmartCode MVC5 Scaffolder for Visual Studio 2017
/// Copyright (c) 2012-2018 All Rights Reserved
/// </summary>
   public class CompanyQuery:QueryObject<Company>
   {
		public CompanyQuery Withfilter(IEnumerable<filterRule> filters)
        {
           if (filters != null)
           {
               foreach (var rule in filters)
               {
						if (rule.field == "Id" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
						{
							var val = Convert.ToInt32(rule.value);
							switch (rule.op) {
                            case "equal":
                                And(x => x.Id == val);
                                break;
                            case "notequal":
                                And(x => x.Id != val);
                                break;
                            case "less":
                                And(x => x.Id < val);
                                break;
                            case "lessorequal":
                                And(x => x.Id <= val);
                                break;
                            case "greater":
                                And(x => x.Id > val);
                                break;
                            case "greaterorequal" :
                                And(x => x.Id >= val);
                                break;
                            default:
                                And(x => x.Id == val);
                                break;
                        }
						}
						if (rule.field == "Name"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.Name.Contains(rule.value));
						}
						if (rule.field == "TradeCode"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.TradeCode.Contains(rule.value));
						}
						if (rule.field == "MasterCustom"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.MasterCustom.Contains(rule.value));
						}
						if (rule.field == "CreditCode"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.CreditCode.Contains(rule.value));
						}
						if (rule.field == "Code"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.Code.Contains(rule.value));
						}
						if (rule.field == "Ctype"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.Ctype.Contains(rule.value));
						}
						if (rule.field == "Scope"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.Scope.Contains(rule.value));
						}
						if (rule.field == "Address"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.Address.Contains(rule.value));
						}
						if (rule.field == "LegalPerson"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.LegalPerson.Contains(rule.value));
						}
						if (rule.field == "Contect"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.Contect.Contains(rule.value));
						}
						if (rule.field == "PhoneNumber"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.PhoneNumber.Contains(rule.value));
						}
						if (rule.field == "RegisterDate" && !string.IsNullOrEmpty(rule.value) )
						{	
							if (rule.op == "between")
                            {
                                var datearray = rule.value.Split(new char[] { '-' });
                                var start = Convert.ToDateTime(datearray[0]);
                                var end = Convert.ToDateTime(datearray[1]);
 
							    And(x => SqlFunctions.DateDiff("d", start, x.RegisterDate) >= 0);
                                And(x => SqlFunctions.DateDiff("d", end, x.RegisterDate) <= 0);
						    }
						}
						if (rule.field == "ExpirationDate" && !string.IsNullOrEmpty(rule.value) )
						{	
							if (rule.op == "between")
                            {
                                var datearray = rule.value.Split(new char[] { '-' });
                                var start = Convert.ToDateTime(datearray[0]);
                                var end = Convert.ToDateTime(datearray[1]);
 
							    And(x => SqlFunctions.DateDiff("d", start, x.ExpirationDate) >= 0);
                                And(x => SqlFunctions.DateDiff("d", end, x.ExpirationDate) <= 0);
						    }
						}
						if (rule.field == "ParentId" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
						{
							var val = Convert.ToInt32(rule.value);
							switch (rule.op) {
                            case "equal":
                                And(x => x.ParentId == val);
                                break;
                            case "notequal":
                                And(x => x.ParentId != val);
                                break;
                            case "less":
                                And(x => x.ParentId < val);
                                break;
                            case "lessorequal":
                                And(x => x.ParentId <= val);
                                break;
                            case "greater":
                                And(x => x.ParentId > val);
                                break;
                            case "greaterorequal" :
                                And(x => x.ParentId >= val);
                                break;
                            default:
                                And(x => x.ParentId == val);
                                break;
                        }
						}
						if (rule.field == "CreatedDate" && !string.IsNullOrEmpty(rule.value) )
						{	
							if (rule.op == "between")
                            {
                                var datearray = rule.value.Split(new char[] { '-' });
                                var start = Convert.ToDateTime(datearray[0]);
                                var end = Convert.ToDateTime(datearray[1]);
 
							    And(x => SqlFunctions.DateDiff("d", start, x.CreatedDate) >= 0);
                                And(x => SqlFunctions.DateDiff("d", end, x.CreatedDate) <= 0);
						    }
						}
						if (rule.field == "CreatedBy"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.CreatedBy.Contains(rule.value));
						}
						if (rule.field == "LastModifiedDate" && !string.IsNullOrEmpty(rule.value) )
						{	
							if (rule.op == "between")
                            {
                                var datearray = rule.value.Split(new char[] { '-' });
                                var start = Convert.ToDateTime(datearray[0]);
                                var end = Convert.ToDateTime(datearray[1]);
 
							    And(x => SqlFunctions.DateDiff("d", start, x.LastModifiedDate) >= 0);
                                And(x => SqlFunctions.DateDiff("d", end, x.LastModifiedDate) <= 0);
						    }
						}
						if (rule.field == "LastModifiedBy"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.LastModifiedBy.Contains(rule.value));
						}
						if (rule.field == "TenantId" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
						{
							var val = Convert.ToInt32(rule.value);
							switch (rule.op) {
                            case "equal":
                                And(x => x.TenantId == val);
                                break;
                            case "notequal":
                                And(x => x.TenantId != val);
                                break;
                            case "less":
                                And(x => x.TenantId < val);
                                break;
                            case "lessorequal":
                                And(x => x.TenantId <= val);
                                break;
                            case "greater":
                                And(x => x.TenantId > val);
                                break;
                            case "greaterorequal" :
                                And(x => x.TenantId >= val);
                                break;
                            default:
                                And(x => x.TenantId == val);
                                break;
                        }
						}
     
               }
           }
            return this;
        }
         public  CompanyQuery ByParentIdWithfilter(int parentid, IEnumerable<filterRule> filters)
         {
            And(x => x.ParentId == parentid);
            if (filters != null)
           {
               foreach (var rule in filters)
               {     
						if (rule.field == "Id" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
						{
							var val = Convert.ToInt32(rule.value);
							switch (rule.op) {
                            case "equal":
                                And(x => x.Id == val);
                                break;
                            case "notequal":
                                And(x => x.Id != val);
                                break;
                            case "less":
                                And(x => x.Id < val);
                                break;
                            case "lessorequal":
                                And(x => x.Id <= val);
                                break;
                            case "greater":
                                And(x => x.Id > val);
                                break;
                            case "greaterorequal" :
                                And(x => x.Id >= val);
                                break;
                            default:
                                And(x => x.Id == val);
                                break;
                        }
						}
						if (rule.field == "Name"  && !string.IsNullOrEmpty(rule.value))
						{
                           if (rule.op == "equal")
                           {
                             And(x => x.Name == rule.value);
                           } 
                           else
                           {
							And(x => x.Name.Contains(rule.value));
						    }
                        }
						if (rule.field == "TradeCode"  && !string.IsNullOrEmpty(rule.value))
						{
                           if (rule.op == "equal")
                           {
                             And(x => x.TradeCode == rule.value);
                           } 
                           else
                           {
							And(x => x.TradeCode.Contains(rule.value));
						    }
                        }
						if (rule.field == "MasterCustom"  && !string.IsNullOrEmpty(rule.value))
						{
                           if (rule.op == "equal")
                           {
                             And(x => x.MasterCustom == rule.value);
                           } 
                           else
                           {
							And(x => x.MasterCustom.Contains(rule.value));
						    }
                        }
						if (rule.field == "CreditCode"  && !string.IsNullOrEmpty(rule.value))
						{
                           if (rule.op == "equal")
                           {
                             And(x => x.CreditCode == rule.value);
                           } 
                           else
                           {
							And(x => x.CreditCode.Contains(rule.value));
						    }
                        }
						if (rule.field == "Code"  && !string.IsNullOrEmpty(rule.value))
						{
                           if (rule.op == "equal")
                           {
                             And(x => x.Code == rule.value);
                           } 
                           else
                           {
							And(x => x.Code.Contains(rule.value));
						    }
                        }
						if (rule.field == "Ctype"  && !string.IsNullOrEmpty(rule.value))
						{
                           if (rule.op == "equal")
                           {
                             And(x => x.Ctype == rule.value);
                           } 
                           else
                           {
							And(x => x.Ctype.Contains(rule.value));
						    }
                        }
						if (rule.field == "Scope"  && !string.IsNullOrEmpty(rule.value))
						{
                           if (rule.op == "equal")
                           {
                             And(x => x.Scope == rule.value);
                           } 
                           else
                           {
							And(x => x.Scope.Contains(rule.value));
						    }
                        }
						if (rule.field == "Address"  && !string.IsNullOrEmpty(rule.value))
						{
                           if (rule.op == "equal")
                           {
                             And(x => x.Address == rule.value);
                           } 
                           else
                           {
							And(x => x.Address.Contains(rule.value));
						    }
                        }
						if (rule.field == "LegalPerson"  && !string.IsNullOrEmpty(rule.value))
						{
                           if (rule.op == "equal")
                           {
                             And(x => x.LegalPerson == rule.value);
                           } 
                           else
                           {
							And(x => x.LegalPerson.Contains(rule.value));
						    }
                        }
						if (rule.field == "Contect"  && !string.IsNullOrEmpty(rule.value))
						{
                           if (rule.op == "equal")
                           {
                             And(x => x.Contect == rule.value);
                           } 
                           else
                           {
							And(x => x.Contect.Contains(rule.value));
						    }
                        }
						if (rule.field == "PhoneNumber"  && !string.IsNullOrEmpty(rule.value))
						{
                           if (rule.op == "equal")
                           {
                             And(x => x.PhoneNumber == rule.value);
                           } 
                           else
                           {
							And(x => x.PhoneNumber.Contains(rule.value));
						    }
                        }
						if (rule.field == "RegisterDate" && !string.IsNullOrEmpty(rule.value) )
						{	
                            if (rule.op == "between")
                            {
                                var datearray = rule.value.Split(new char[] { '-' });
                                var start = Convert.ToDateTime(datearray[0]);
                                var end = Convert.ToDateTime(datearray[1]);
 
							    And(x => SqlFunctions.DateDiff("d", start, x.RegisterDate) >= 0);
                                And(x => SqlFunctions.DateDiff("d", end, x.RegisterDate) <= 0);
						    }
                        }
						if (rule.field == "ExpirationDate" && !string.IsNullOrEmpty(rule.value) )
						{	
                            if (rule.op == "between")
                            {
                                var datearray = rule.value.Split(new char[] { '-' });
                                var start = Convert.ToDateTime(datearray[0]);
                                var end = Convert.ToDateTime(datearray[1]);
 
							    And(x => SqlFunctions.DateDiff("d", start, x.ExpirationDate) >= 0);
                                And(x => SqlFunctions.DateDiff("d", end, x.ExpirationDate) <= 0);
						    }
                        }
						if (rule.field == "ParentId" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
						{
							var val = Convert.ToInt32(rule.value);
							switch (rule.op) {
                            case "equal":
                                And(x => x.ParentId == val);
                                break;
                            case "notequal":
                                And(x => x.ParentId != val);
                                break;
                            case "less":
                                And(x => x.ParentId < val);
                                break;
                            case "lessorequal":
                                And(x => x.ParentId <= val);
                                break;
                            case "greater":
                                And(x => x.ParentId > val);
                                break;
                            case "greaterorequal" :
                                And(x => x.ParentId >= val);
                                break;
                            default:
                                And(x => x.ParentId == val);
                                break;
                        }
						}
               }
            }
            return this;
         }    
    }
}
