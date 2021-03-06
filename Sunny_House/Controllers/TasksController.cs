﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Sunny_House.Models;
using System.Web.Routing;
using PagedList;
using PagedList.Mvc;

namespace Sunny_House.Controllers
{
    [Authorize(Roles = "Administrator, User, Presenter")]
    public class TasksController : Controller
    {
        private SunnyModel db = new SunnyModel();
        private ApplicationDbContext aspdb = new ApplicationDbContext();

        // GET: Tasks
        public ActionResult TasksShow()
        {
            return View();
        }

        // GET: Tasks
        public ActionResult TasksShowPartial(DateTime? SearchDateOfCreation, DateTime? SearchDate, string SearchString, bool? TaskComplete, int? page, string SortBy)
        {
            ViewBag.SortDateOfCreation = SortBy == "DateOfCreation" ? "DateOfCreation desc" : "DateOfCreation";
            ViewBag.SortDate = SortBy == "Date" ? "Date desc" : "Date";
            ViewBag.SortSubject = SortBy == "Subject" ? "Subject desc" : "Subject";
            ViewBag.SortComplete = SortBy == "Complete" ? "Complete desc" : "Complete";
            ViewBag.SortNote = SortBy == "Note" ? "Note desc" : "Note";
            ViewBag.SortResponsible = SortBy == "Responsible" ? "Responsible desc" : "Responsible";

            var _tasks = (from task in db.STask
                          join resp in db.Persons on task.ResponsibleId equals resp.PersonId into resptmp
                          from resp in resptmp.DefaultIfEmpty()
                          where (task.DateOfCreation == SearchDateOfCreation || SearchDateOfCreation == null)
                          && (task.Date == SearchDate || SearchDate == null)
                          && (task.Subject.Contains(SearchString)
                                || task.Note.Contains(SearchString)
                                || resp.FirstName.Contains(SearchString)
                                || resp.LastName.Contains(SearchString)
                                || string.IsNullOrEmpty(SearchString))
                          && (task.TaskComplete == TaskComplete || TaskComplete == null)
                          select new
                          {
                              STaskId = task.STaskId,
                              DateOfCreation = task.DateOfCreation,
                              Date = task.Date,
                              Subject = task.Subject,
                              TaskComplete = task.TaskComplete,
                              Note = task.Note,
                              ResponsibleId = task.ResponsibleId,
                              PersonFIO = resp.FirstName + " " + resp.LastName + " " + resp.MiddleName
                          }).AsEnumerable().Select(x => new STask
                          {
                              STaskId = x.STaskId,
                              DateOfCreation = x.DateOfCreation,
                              Date = x.Date,
                              Subject = x.Subject,
                              TaskComplete = x.TaskComplete,
                              Note = x.Note,
                              ResponsibleId = x.ResponsibleId,
                              PersonFIO = x.PersonFIO
                          });

            switch (SortBy)
            {
                case "DateOfCreation desc":
                    _tasks = _tasks.OrderByDescending(x => x.DateOfCreation);
                    ViewData["SortColumn"] = "DateOfCreation";
                    break;
                case "DateOfCreation":
                    _tasks = _tasks.OrderBy(x => x.DateOfCreation);
                    ViewData["SortColumn"] = "DateOfCreation";
                    break;
                case "Date desc":
                    _tasks = _tasks.OrderByDescending(x => x.Date);
                    ViewData["SortColumn"] = "Date";
                    break;
                case "Date":
                    _tasks = _tasks.OrderBy(x => x.Date);
                    ViewData["SortColumn"] = "Date";
                    break;
                case "Subject desc":
                    _tasks = _tasks.OrderByDescending(x => x.Subject);
                    ViewData["SortColumn"] = "Subject";
                    break;
                case "Subject":
                    _tasks = _tasks.OrderBy(x => x.Subject);
                    ViewData["SortColumn"] = "Subject";
                    break;
                case "Complete desc":
                    _tasks = _tasks.OrderByDescending(x => x.TaskComplete);
                    ViewData["SortColumn"] = "Complete";
                    break;
                case "Complete":
                    _tasks = _tasks.OrderBy(x => x.TaskComplete);
                    ViewData["SortColumn"] = "Complete";
                    break;
                case "Note desc":
                    _tasks = _tasks.OrderByDescending(x => x.Note);
                    ViewData["SortColumn"] = "Note";
                    break;
                case "Note":
                    _tasks = _tasks.OrderBy(x => x.Note);
                    ViewData["SortColumn"] = "Note";
                    break;
                case "Responsible desc":
                    _tasks = _tasks.OrderByDescending(x => x.PersonFIO);
                    ViewData["SortColumn"] = "Responsible";
                    break;
                case "Responsible":
                    _tasks = _tasks.OrderBy(x => x.PersonFIO);
                    ViewData["SortColumn"] = "Responsible";
                    break;
                default:
                    break;
            }

            int pageSize = 50;
            int pageNumber = (page ?? 1);

            return PartialView(_tasks.ToList().ToPagedList(pageNumber, pageSize));
        }

        // GET: Tasks/Details/5
        public async Task<ActionResult> TaskDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            STask sTask = await db.STask.FindAsync(id);
            if (sTask == null)
            {
                return HttpNotFound();
            }
            return View(sTask);
        }

        // GET: Tasks/Create
        public ActionResult TaskCreate(string Subject, string SubjectLock, string ActionName, string ControllerName, string ParameterName, string ParameterValue)
        {

            //Инициализируем имена метода и контроллера для возврата
            ActionName = (!string.IsNullOrEmpty(ActionName)) ? ActionName : "TasksShow";
            ControllerName = (!string.IsNullOrEmpty(ControllerName)) ? ControllerName : "Tasks";
            ViewData["Subject"] = Subject;
            ViewData["SubjectLock"] = SubjectLock;
            ViewData["ActionName"] = ActionName;
            ViewData["ControllerName"] = ControllerName;
            ViewData["ParameterName"] = ParameterName;
            ViewData["ParameterValue"] = ParameterValue;

            return View();
        }

        // POST: Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TaskCreate([Bind(Include = "STaskId,Date,DateOfCreation,Subject,TaskComplete,Note,ResponsibleId")] STask sTask, string Subject, string SubjectLock, string ActionName, string ControllerName, string ParameterName, string ParameterValue, string CreatorName)
        {
            try
            {
                //Получаем идентификатор текущего пользователя
                if (!String.IsNullOrEmpty(CreatorName))
                {
                    sTask.CreatorId = Guid.Parse(aspdb.Users.FirstOrDefault(x => x.UserName == CreatorName).Id.ToString());
                }

                sTask.TaskComplete = false;
                sTask.DateOfCreation = DateTime.Today;
                //Инициализируем имена метода и контроллера для возврата
                ActionName = (!string.IsNullOrEmpty(ActionName)) ? ActionName : "TasksShow";
                ControllerName = (!string.IsNullOrEmpty(ControllerName)) ? ControllerName : "Tasks";

                if (ModelState.IsValid)
                {
                    db.STask.Add(sTask);
                    await db.SaveChangesAsync();
                    TempData["MessageOK"] = "Задача добавлена";

                    if (!string.IsNullOrEmpty(ParameterName) && !string.IsNullOrEmpty(ParameterValue))
                    {
                        var obj = new RouteValueDictionary();
                        obj[ParameterName] = ParameterValue;
                        return RedirectToAction(ActionName, ControllerName, obj);
                    }

                    return RedirectToAction(ActionName, ControllerName);
                }

                ViewData["Subject"] = Subject;
                ViewData["SubjectLock"] = SubjectLock;
                ViewData["ActionName"] = ActionName;
                ViewData["ControllerName"] = ControllerName;
                ViewData["ParameterName"] = ParameterName;
                ViewData["ParameterValue"] = ParameterValue;

                return View(sTask);
            }
            catch (Exception ex)
            {
                ViewBag.ErMes = ex.Message;
                ViewBag.ErStack = ex.StackTrace;
                return View("Error");
            }

        }

        // GET: Tasks/Edit/5
        public async Task<ActionResult> TaskEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            STask sTask = await db.STask.FindAsync(id);
            if (sTask == null)
            {
                return HttpNotFound();
            }

            string _fio = db.Persons.Where(x => x.PersonId == sTask.ResponsibleId).Select(z => z.FirstName + " " + z.LastName + " " + z.MiddleName).FirstOrDefault();
            ViewData["ResponsibleFIO"] = _fio;

            return View(sTask);
        }

        // POST: Tasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TaskEdit([Bind(Include = "STaskId,Date,DateOfCreation,Subject,TaskComplete,Note,CreatorId, ResponsibleId")] STask sTask, string CurrentUser)
        {
            if (ModelState.IsValid)
            {

                Guid? _currentUserId = null;
                if (!String.IsNullOrEmpty(CurrentUser))
                {
                    _currentUserId = Guid.Parse(aspdb.Users.FirstOrDefault(x => x.UserName == CurrentUser).Id.ToString());
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (sTask.CreatorId == _currentUserId || User.IsInRole("Administrator") || User.IsInRole("User"))
                {
                    try
                    {
                        db.Entry(sTask).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        TempData["MessageOk"] = "Изменение параметров задачи выполнено успешно";
                        return RedirectToAction("TasksShow");
                    }
                    catch (Exception ex)
                    {
                        ViewBag.ErMes = ex.Message;
                        ViewBag.ErStack = ex.StackTrace;
                        return View("Error");
                    }
                }
                else
                {
                    TempData["MessageError"] = "Вы не являетесь создателем данной задачи. Редактирование запрещено.";
                    return RedirectToAction("TasksShow");
                }

            }
            return View(sTask);
        }

        // GET: Tasks/Delete/5
        public async Task<ActionResult> TaskDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            STask sTask = await db.STask.FindAsync(id);
            if (sTask == null)
            {
                return HttpNotFound();
            }
            return View(sTask);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("TaskDelete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id, string CurrentUser)
        {
            Guid? _currentUserId = null;
            if (!String.IsNullOrEmpty(CurrentUser))
            {
                _currentUserId = Guid.Parse(aspdb.Users.FirstOrDefault(x => x.UserName == CurrentUser).Id.ToString());
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            STask sTask = db.STask.FirstOrDefault(x=>x.STaskId == id);
            if (sTask != null)
            {
                if (sTask.CreatorId == _currentUserId || User.IsInRole("Administrator") || User.IsInRole("User"))
                {
                    try
                    {
                        db.STask.Remove(sTask);
                        await db.SaveChangesAsync();
                        TempData["MessageOk"] = "Задача успешно удалена";
                        return RedirectToAction("TasksShow");
                    }

                    catch (Exception ex)
                    {
                        ViewBag.ErMes = ex.Message;
                        ViewBag.ErStack = ex.StackTrace;
                        return View("Error");
                    }
                }
                else
                {
                    TempData["MessageError"] = "Вы не являетесь создателем данной задачи. Удаление запрещено.";
                    return RedirectToAction("TasksShow");
                }
            }
            else
            {
                string _message = string.Format("Удаление невозможно. Указанный объект отсутствует в базе данных.");
                TempData["MessageError"] = _message;
                return RedirectToAction("TasksShow");
            }

        }

        public ActionResult TasksGroupShow(string mode)
        {

            var _result = (from task in db.STask
                           join resp in db.Persons on task.ResponsibleId equals resp.PersonId into resptmp
                           from resp in resptmp.DefaultIfEmpty()
                           select new
                           {
                               STaskId = task.STaskId,
                               DateOfCreation = task.DateOfCreation,
                               Date = task.Date,
                               Subject = task.Subject,
                               TaskComplete = task.TaskComplete,
                               Note = task.Note,
                               ResponsibleId = task.ResponsibleId,
                               PersonFIO = resp.FirstName + " " + resp.LastName + " " + resp.MiddleName
                           }).AsEnumerable().Select(x => new STask
                          {
                              STaskId = x.STaskId,
                              DateOfCreation = x.DateOfCreation,
                              Date = x.Date,
                              Subject = x.Subject,
                              TaskComplete = x.TaskComplete,
                              Note = x.Note,
                              ResponsibleId = x.ResponsibleId,
                              PersonFIO = x.PersonFIO
                          });


            switch (mode)
            {
                case "overdue":
                    _result = _result.Where(t => (t.TaskComplete == false) && (t.Date < DateTime.Today)).ToList();
                    break;
                case "today":
                    _result = _result.Where(t => (t.TaskComplete == false) && (t.Date == DateTime.Today)).ToList();
                    break;
                case "tomorrow":
                    _result = _result.Where(t => (t.TaskComplete == false) && (t.Date == DateTime.Today.AddDays(1))).ToList();
                    break;
                case "nextseven":
                    _result = _result.Where(t => (t.TaskComplete == false) && (t.Date > DateTime.Today.AddDays(1)) && t.Date <= DateTime.Today.AddDays(8)).ToList();
                    break;
                case "later":
                    _result = _result.Where(t => (t.TaskComplete == false) && (t.Date > DateTime.Today.AddDays(8))).ToList();
                    break;
                case "withoutdate":
                    _result = _result.Where(t => (t.TaskComplete == false) && (t.Date == null)).ToList();
                    break;
                case "completed":
                    _result = _result.Where(t => (t.TaskComplete == true)).ToList();
                    break;
                default:
                    break;
            }

            return PartialView(_result);
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
