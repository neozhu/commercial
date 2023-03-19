using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace WebApp
{
  public sealed class JsonNetResult : JsonResult
  {
    public JsonNetResult()
    {
      JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      ContentType = "application/json";
      ContentEncoding = Encoding.UTF8;
      Settings = new JsonSerializerSettings
      {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        NullValueHandling= NullValueHandling.Ignore
      };
    }

    public JsonSerializerSettings Settings { get; private set; }

    public override void ExecuteResult(ControllerContext context)
    {
      if (context == null)
      {
        throw new ArgumentNullException("context");
      }

      if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet && string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
      {
        throw new InvalidOperationException("JSON GET is not allowed");
      }

      var response = context.HttpContext.Response;
      response.ContentType = string.IsNullOrEmpty(this.ContentType) ? "application/json" : this.ContentType;

      if (this.ContentEncoding != null)
      {
        response.ContentEncoding = this.ContentEncoding;
      }

      if (this.Data == null)
      {
        return;
      }

      var scriptSerializer = JsonSerializer.Create(this.Settings);

      using (var sw = new StringWriter())
      {
        scriptSerializer.Serialize(sw, this.Data);
        response.Write(sw.ToString());
      }
    }
  }
}