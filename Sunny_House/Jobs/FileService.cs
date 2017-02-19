using System;
using System.IO;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using Quartz;
using Sunny_House.Models;

namespace Sunny_House.Jobs
{
    public class FileService : IJob
    {

        public void Execute(IJobExecutionContext context)
        {
            using (SunnyModel db = new SunnyModel())
            {
                string AttachmentPath = System.Configuration.ConfigurationManager.AppSettings["AttachmentPath"];
                try
                {
                    using (StreamWriter streamWriter = new StreamWriter(AttachmentPath + @"IDGLog.txt", true))
                    {
                        string _res = String.Format("Старт задачи: {0}", DateTime.Now.ToString());
                        streamWriter.WriteLine(_res);
                    }

                    List<Guid> _comm = db.Comments.Select(x => x.RelGuid).ToList();
                    DateTime CurrenDT = DateTime.Now.AddDays(-1);

                    var _lostAtt = (from att in db.Attachments
                                    where (!_comm.Contains(att.RelGuid)) && (att.CreateDT < CurrenDT)
                                    select new { Id = att.Id }).ToList();

                    foreach (var item in _lostAtt)
                    {

                        var _attachment = db.Attachments.FirstOrDefault(x => x.Id == item.Id);
                        String _attFileName = _attachment.ServerFileName.ToString();
                        string fullPath = AttachmentPath + _attachment.ServerFileName.ToString();

                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }

                        db.Attachments.Remove(_attachment);
                        db.SaveChanges();

                        using (StreamWriter streamWriter = new StreamWriter(AttachmentPath + @"IDGLog.txt", true))
                        {
                            string _res = String.Format("{0}: Файл {1} удален \n\r----------------------------------", DateTime.Now.ToString(), _attFileName);
                            streamWriter.WriteLine(_res);
                            System.Diagnostics.Debug.WriteLine(_res);
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (StreamWriter streamWriter = new StreamWriter(AttachmentPath + @"IDGLog.txt", true))
                    {
                        string _res = String.Format("{0}: Ошибка удаления \n{1}", DateTime.Now.ToString(),ex.Message);
                        streamWriter.WriteLine(_res);
                        System.Diagnostics.Debug.WriteLine(_res);
                    }
                }

            }


        }

    }
}

