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
        public async Task<ActionResult> Index()
        {
            return View(await db.STask.ToListAsync());
        }

        // GET: Tasks/Details/5
        public async Task<ActionResult> Details(int? id)
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
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "STaskId,Date,DateOfCreation,Subject,TaskComplete,Note")] STask sTask)
        {
            if (ModelState.IsValid)
            {
                db.STask.Add(sTask);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(sTask);
        }

        // GET: Tasks/Edit/5
        public async Task<ActionResult> Edit(int? id)
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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "STaskId,Date,DateOfCreation,Subject,TaskComplete,Note")] STask sTask)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sTask).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(sTask);
        }

        // GET: Tasks/Delete/5
        public async Task<ActionResult> Delete(int? id)
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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            STask sTask = await db.STask.FindAsync(id);
            db.STask.Remove(sTask);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
