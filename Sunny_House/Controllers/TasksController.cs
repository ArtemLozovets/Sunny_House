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
        public ActionResult TaskCreate(string Subject)
        {
            ViewData["Subject"] = Subject;
            return View();
        }

        // POST: Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TaskCreate([Bind(Include = "STaskId,Date,DateOfCreation,Subject,TaskComplete,Note")] STask sTask)
        {
            try
            {
                sTask.TaskComplete = false;
                sTask.DateOfCreation = DateTime.Today;
                if (ModelState.IsValid)
                {
                    db.STask.Add(sTask);
                    await db.SaveChangesAsync();
                    TempData["MessageOk"] = "Задача добавлена";
                    return RedirectToAction("TasksShow");
                }

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
        public async Task<ActionResult> Edit([Bind(Include = "STaskId,Date,DateOfCreation,Subject,TaskComplete,Note")] STask sTask)
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
