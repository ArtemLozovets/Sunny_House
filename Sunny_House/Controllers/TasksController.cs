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

namespace Sunny_House.Controllers
{
    public class TasksController : Controller
    {
        private SunnyModel db = new SunnyModel();

        // GET: Tasks
        public async Task<ActionResult> TasksShow()
        {
            return View(await db.STask.ToListAsync());
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
            ViewData["Subject"] = Subject;
            ViewData["SubjectLock"] = SubjectLock;
            ViewData["ActionName"] = (!string.IsNullOrEmpty(ActionName))? ActionName:"TasksShow";
            ViewData["ControllerName"] = (!string.IsNullOrEmpty(ControllerName)) ? ControllerName : "Tasks";
            ViewData["ParameterName"] = ParameterName;
            ViewData["ParameterValue"] = ParameterValue;

            return View();
        }

        // POST: Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TaskCreate([Bind(Include = "STaskId,Date,DateOfCreation,Subject,TaskComplete,Note")] STask sTask, string Subject, string SubjectLock, string ActionName, string ControllerName, string ParameterName, string ParameterValue)
        {
            try
            {
                sTask.TaskComplete = false;
                sTask.DateOfCreation = DateTime.Today;
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
                ViewData["ActionName"] = (!string.IsNullOrEmpty(ActionName)) ? ActionName : "TasksShow";
                ViewData["ControllerName"] = (!string.IsNullOrEmpty(ControllerName)) ? ControllerName : "Tasks";
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
        public async Task<ActionResult> TaskEdit([Bind(Include = "STaskId,Date,DateOfCreation,Subject,TaskComplete,Note")] STask sTask)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sTask).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("TasksShow");
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
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            STask sTask = await db.STask.FindAsync(id);
            db.STask.Remove(sTask);
            await db.SaveChangesAsync();
            return RedirectToAction("TasksShow");
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
