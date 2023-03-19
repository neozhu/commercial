using System;
using System.Data;
using System.Reflection;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using Repository.Pattern.Repositories;
using Repository.Pattern.Infrastructure;
using Service.Pattern;
using WebApp.Models;
using WebApp.Repositories;
using System.Web;

namespace WebApp.Services
{
  /// <summary>
  /// File: AttachmentService.cs
  /// Purpose: Within the service layer, you define and implement 
  /// the service interface and the data contracts (or message types).
  /// One of the more important concepts to keep in mind is that a service
  /// should never expose details of the internal processes or 
  /// the business entities used within the application. 
  /// Created Date: 2020/5/20 20:49:53
  /// Author: neo.zhu
  /// Tools: SmartCode MVC5 Scaffolder for Visual Studio 2017
  /// Copyright (c) 2012-2018 All Rights Reserved
  /// </summary>
  public class AttachmentService : Service<Attachment>, IAttachmentService
  {
    private readonly IRepositoryAsync<Attachment> repository;
    private readonly IDataTableImportMappingService mappingservice;
    private readonly NLog.ILogger logger;
    public AttachmentService(
      IRepositoryAsync<Attachment> repository,
      IDataTableImportMappingService mappingservice,
      NLog.ILogger logger
      )
        : base(repository)
    {
      this.repository = repository;
      this.mappingservice = mappingservice;
      this.logger = logger;
    }



    public async Task ImportDataTableAsync(DataTable datatable, string username)
    {
      var mapping = await this.mappingservice.Queryable()
                        .Where(x => x.EntitySetName == "Attachment" &&
                           ( x.IsEnabled == true || ( x.IsEnabled == false && x.DefaultValue != null ) )
                           ).ToListAsync();
      if (mapping.Count == 0)
      {
        throw new KeyNotFoundException("没有找到Attachment对象的Excel导入配置信息，请执行[系统管理/Excel导入配置]");
      }
      foreach (DataRow row in datatable.Rows)
      {

        var requiredfield = mapping.Where(x => x.IsRequired == true && x.IsEnabled == true && x.DefaultValue == null).FirstOrDefault()?.SourceFieldName;
        if (requiredfield != null || !row.IsNull(requiredfield))
        {
          var item = new Attachment();
          foreach (var field in mapping)
          {
            var defval = field.DefaultValue;
            var contain = datatable.Columns.Contains(field.SourceFieldName ?? "");
            if (contain && !row.IsNull(field.SourceFieldName))
            {
              var attachmenttype = item.GetType();
              var propertyInfo = attachmenttype.GetProperty(field.FieldName);
              var safetype = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
              var safeValue = ( row[field.SourceFieldName] == null ) ? null : Convert.ChangeType(row[field.SourceFieldName], safetype);
              propertyInfo.SetValue(item, safeValue, null);
            }
            else if (!string.IsNullOrEmpty(defval))
            {
              var attachmenttype = item.GetType();
              var propertyInfo = attachmenttype.GetProperty(field.FieldName);
              if (string.Equals(defval, "now", StringComparison.OrdinalIgnoreCase) && ( propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(Nullable<DateTime>) ))
              {
                var safetype = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                var safeValue = Convert.ChangeType(DateTime.Now, safetype);
                propertyInfo.SetValue(item, safeValue, null);
              }
              else if (string.Equals(defval, "guid", StringComparison.OrdinalIgnoreCase))
              {
                propertyInfo.SetValue(item, Guid.NewGuid().ToString(), null);
              }
              else if (string.Equals(defval, "user", StringComparison.OrdinalIgnoreCase))
              {
                propertyInfo.SetValue(item, username, null);
              }
              else
              {
                var safetype = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                var safeValue = Convert.ChangeType(defval, safetype);
                propertyInfo.SetValue(item, safeValue, null);
              }
            }
          }
          this.Insert(item);
        }
      }
    }
    public async Task<Stream> ExportExcelAsync(string filterRules = "", string sort = "Id", string order = "asc")
    {
      var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
      var expcolopts = await this.mappingservice.Queryable()
             .Where(x => x.EntitySetName == "Attachment")
             .Select(x => new ExpColumnOpts()
             {
               EntitySetName = x.EntitySetName,
               FieldName = x.FieldName,
               IgnoredColumn = x.IgnoredColumn,
               SourceFieldName = x.SourceFieldName
             }).ToArrayAsync();

      var attachments = this.Query(new AttachmentQuery().Withfilter(filters)).OrderBy(n => n.OrderBy(sort, order)).Select().ToList();
      var datarows = attachments.Select(n => new
      {

        Id = n.Id,
        FileName = n.FileName,
        FileId = n.FileId,
        Ext = n.Ext,
        FilePath = n.FilePath,
        RefKey = n.RefKey,
        Owner = n.Owner,
        Upload = n.Upload.ToString("yyyy-MM-dd HH:mm:ss")
      }).ToList();
      return await NPOIHelper.ExportExcelAsync("附件管理", datarows, expcolopts);
    }
    public async Task Delete(int[] id)
    {
      var items = await this.Queryable().Where(x => id.Contains(x.Id)).ToListAsync();
      foreach (var item in items)
      {
        if (File.Exists(item.FilePath))
        {
          File.Delete(item.FilePath);
        }
        this.Delete(item);
      }

    }

    public async Task SaveFile(HttpPostedFileBase file, string tags,string folder,string relpath, string name, dynamic user)
    {
       var path= Path.Combine(folder, user);
      var relativepath = $"{relpath}/{user}/{name}";
      var ext = Path.GetExtension(name);
      var size = file.ContentLength;
      if (!Directory.Exists(path))
      {
        Directory.CreateDirectory(path);
      }
      var filepath= Path.Combine(path,name);
      if (!File.Exists(filepath))
      {
        file.SaveAs(filepath);
      }
      var item = new Attachment()
      {
        Ext = ext,
        FileName = name,
        FilePath = filepath,
        RelativePath= relativepath,
        Size=size,
        Tags=tags,
        FileId = Guid.NewGuid().ToString(),
        Owner = user,
        Upload = DateTime.Now
      };
      this.Insert(item);
        
    }

    public async Task Rename(string fileid, string newfilename)
    {
      var item =await this.Queryable().Where(x => x.FileId == fileid).FirstOrDefaultAsync();
      if (item != null)
      {
        var path = item.FilePath;
        var fileinfo = new FileInfo(path);
        if (!Path.HasExtension(newfilename))
        {
          newfilename = newfilename + item.Ext;
        }
        var newpath = fileinfo.Directory.FullName + "\\" + newfilename;
        fileinfo.MoveTo(newpath);
        
        item.RelativePath = item.RelativePath.Replace(item.FileName, newfilename);
        item.FileName = newfilename;
        item.FilePath = newpath;
        this.Update(item);
      }
    }
  }
}



