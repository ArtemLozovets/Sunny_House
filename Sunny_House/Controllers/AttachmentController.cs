using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Sunny_House.Models;
using System.IO;

namespace Sunny_House.Controllers
{
    public class AttachmentController : Controller
    {
        private SunnyModel db = new SunnyModel();


        [HttpPost]
        public JsonResult Upload()
        {
            try
            {
                var upload = Request.Files[0];
                Guid _relGUID = new Guid(Request.Form["GUID"]);
                if (upload != null && upload.ContentLength > 0)
                {
                    // получаем имя файла
                    string fileName = System.IO.Path.GetFileName(upload.FileName);
                    Guid _fileGuid = Guid.NewGuid();

                    //upload.SaveAs(Server.MapPath("~/App_Data/Upload/" + _fileGuid));
                    string AttachmentPath = System.Configuration.ConfigurationManager.AppSettings["AttachmentPath"];
                    upload.SaveAs(AttachmentPath + _fileGuid);

                    Attachment _att = new Attachment
                    {
                        FileName = fileName,
                        ServerFileName = _fileGuid,
                        RelGuid = _relGUID
                    };

                    db.Attachments.Add(_att);
                    db.SaveChanges();

                    var _result = new { fileName = fileName, attId = _att.Id };

                    return Json(_result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var _result = new { message = "Ошибка загрузки файла. Проверьте настройки пути сохранения файлов на сервере.\nОтвет сервера: \n\"" + ex.Message + "\"", JsonRequestBehavior.AllowGet };
                return Json(_result);
            }

            return Json("NaN", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AttDelete()
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    int _attId = Convert.ToInt16(Request.Form["attId"]);
                    var _attachment = db.Attachments.FirstOrDefault(x => x.Id == _attId);

                    string AttachmentPath = System.Configuration.ConfigurationManager.AppSettings["AttachmentPath"];
                    string fullPath = AttachmentPath + _attachment.ServerFileName.ToString();

                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }

                    db.Attachments.Remove(_attachment);
                    db.SaveChanges();
                    dbContextTransaction.Commit();

                    var _result = new { deleted = true, JsonRequestBehavior.AllowGet };
                    return Json(_result);
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    var _result = new { deleted = false, message = "Ошибка удаления файла. \nОтвет сервера: \n\"" + ex.Message + "\"", JsonRequestBehavior.AllowGet };
                    return Json(_result);
                }
            }
        }

        public ActionResult GetFile(int? id)
        {
            try
            {
                var _file = db.Attachments.FirstOrDefault(f => f.Id == id);
                string _fName = _file.FileName.ToString();
                string _fGuid = _file.ServerFileName.ToString();
                string AttachmentPath = System.Configuration.ConfigurationManager.AppSettings["AttachmentPath"];

                // Путь к файлу
                //string file_path = Server.MapPath("~/App_Data/Upload/" +_fGuid);
                string file_path = (AttachmentPath + _fGuid);
                // Тип файла - content-type
                string file_type = "multipart/mixed";
                // Имя файла - необязательно
                string file_name = _fName;

                if (System.IO.File.Exists(file_path)) return File(file_path, file_type, file_name);
                else 
                {
                    ViewBag.ErMes = "Файл " + file_name + " не найден.";
                    return View("Error");      
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErMes = ex.Message;
                ViewBag.ErStack = ex.StackTrace;
                return View("Error");                
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
