using System;
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
            ViewBag.SortTaskComplete = SortBy == "TaskComplete" ? "TaskComplete desc" : "TaskComplete";
            ViewBag.SortNote = SortBy == "Note" ? "Note desc" : "Note";


            var _tasks = (from task in db.STask
                          where (task.DateOfCreation == SearchDateOfCreation || SearchDateOfCreation == null)
                          && (task.Date == SearchDate || SearchDate == null)
                          && ((task.Subject.Contains(SearchString) || string.IsNullOrEmpty(SearchString)) || (task.Note.Contains(SearchString) || string.IsNullOrEmpty(SearchString)))
                          && (task.TaskComplete == TaskComplete || TaskComplete == null)
                          select task);

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
                case "TaskComplete desc":
                    _tasks = _tasks.OrderByDescending(x => x.TaskComplete);
                    ViewData["SortColumn"] = "TaskComplete";
                    break;
                case "TaskComplete":
                    _tasks = _tasks.OrderBy(x => x.TaskComplete);
                    ViewData["SortColumn"] = "TaskComplete";
                    break;
                case "Note desc":
                    _tasks = _tasks.OrderByDescending(x => x.Note);
                    ViewData["SortColumn"] = "Note";
                    break;
                case "Note":
                    _tasks = _tasks.OrderBy(x => x.Note);
                    ViewData["SortColumn"] = "Note";
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
        public async Task<ActionResult> TaskCreate([Bind(Include = "STaskId,Date,DateOfCreation,Subject,TaskComplete,Note")] STask sTask, string Subject, string SubjectLock, string ActionName, string ControllerName, string ParameterName, string ParameterValue, string CreatorName)
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
            return View(sTask);
        }

        // POST: Tasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TaskEdit([Bind(Include = "STaskId,Date,DateOfCreation,Subject,TaskComplete,Note,CreatorId")] STask sTask, string CurrentUser)
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

            STask sTask = await db.STask.FindAsync(id);
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

        public ActionResult TasksGroupShow(string mode)
        {
            var _result = db.STask.ToList();

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
