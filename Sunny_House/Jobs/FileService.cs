using Quartz;
using System.Net;
using System.Net.Mail;
using System;
using System.IO;

namespace Sunny_House.Jobs
{
    public class FileService : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            string AttPath = System.Configuration.ConfigurationManager.AppSettings["AttachmentPath"];
            using (StreamWriter streamWriter = new StreamWriter(AttPath+@"IDGLog.txt", true))
            {
                streamWriter.WriteLine(DateTime.Now.ToString());
            } 

            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString());
        }

    }
}