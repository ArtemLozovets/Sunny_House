using Quartz;
using System.Net;
using System.Net.Mail;
using System;
using System.IO;
using System.Data;
using System.Data.Entity;
using System.Linq;
using Sunny_House.Models;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Sunny_House.Jobs
{
    public class FileService : IJob
    {

        public void Execute(IJobExecutionContext context)
        {
            string AttPath = System.Configuration.ConfigurationManager.AppSettings["AttachmentPath"];
            using (StreamWriter streamWriter = new StreamWriter(AttPath + @"IDGLog.txt", true))
            {
                streamWriter.WriteLine(DateTime.Now.ToString());
            }

            using (SunnyModel db = new SunnyModel())
            {
                var commentList = (from cm in db.Comments
                                   select new
                                   {
                                       RelGuid = cm.RelGuid
                                   }).ToList();

                //bool a = commentList.Contains();
                
                //var missingList = (from at in db.Attachments
                //                   where !commentList.Contains(at.RelGuid)
                //                   select new
                //                   {
                //                       Id = at.Id
                //                   });

                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString());
            }

        }
    }
}
