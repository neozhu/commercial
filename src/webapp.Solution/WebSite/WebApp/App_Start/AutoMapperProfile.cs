using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using WebApp.Models;

namespace WebApp
{
  public class AutoMapperProfile : Profile
  {
    public AutoMapperProfile()
    {
      CreateMap<Company, CompanyTreeItem>().ReverseMap();
      //var map = CreateMap<Order, Order>();
      //CreateMap<Order, OrderDetail>()
      //  .ForMember(x => x.OrderId, opt => opt.MapFrom(x => x.Id));
       

    }
  }
   
}