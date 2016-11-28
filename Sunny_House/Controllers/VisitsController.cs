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
    public class VisitsController : Controller
    {
        private SunnyModel db = new SunnyModel();

        // GET: Visits
        public async Task<ActionResult> VisShow()
        {
            var visits = db.Visits.Include(v => v.Exercise).Include(v => v.Person).Include(v => v.PersonRole);
            return View(await visits.ToListAsync());
        }


        // GET: Visits/Create
        public ActionResult VisCreate()
        {
            ViewBag.ExerciseId = new SelectList(db.Exercises, "ExerciseId", "Subject");
            ViewBag.VisitorId = new SelectList(db.Persons, "PersonId", "FirstName");
            ViewBag.RoleId = new SelectList(db.PersonRoles, "RoleId", "RoleName");
            return View();
        }

        // POST: Visits/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VisCreate([Bind(Include = "VisitorId,ExerciseId,RoleId")] Visit visit)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Visits.Add(visit);
                    await db.SaveChangesAsync();

                    TempData["MessageOk"] = "Информация о посещении успешно добавлена";

                    return RedirectToAction("VisShow");

                }
                catch (Exception ex)
                {
                    ViewBag.ErMes = ex.Message;
                    ViewBag.ErStack = ex.StackTrace;
                    return View("Error");
                }
            }
            TempData["MessageError"] = "Ошибка валидации модели";

            ViewBag.ExerciseId = new SelectList(db.Exercises, "ExerciseId", "Subject", visit.ExerciseId);
            ViewBag.VisitorId = new SelectList(db.Persons, "PersonId", "FirstName", visit.VisitorId);
            ViewBag.RoleId = new SelectList(db.PersonRoles, "RoleId", "RoleName", visit.RoleId);
            return View(visit);
        }

        // GET: Visits/Edit/5
        public async Task<ActionResult> VisEdit(int? VisitId)
        {
            if (VisitId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Visit visit = await db.Visits.FindAsync(VisitId);
            if (visit == null)
            {
                return HttpNotFound();
            }
            ViewBag.ExerciseId = new SelectList(db.Exercises, "ExerciseId", "Subject", visit.ExerciseId);
            ViewBag.VisitorId = new SelectList(db.Persons, "PersonId", "FirstName", visit.VisitorId);
            ViewBag.RoleId = new SelectList(db.PersonRoles, "RoleId", "RoleName", visit.RoleId);
            return View(visit);
        }

        // POST: Visits/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VisEdit([Bind(Include = "VisitorId,ExerciseId,VisitId,RoleId")] Visit visit)
        {
            // db.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s)); //Debug Information====================
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(visit).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    TempData["MessageOk"] = "Информация о посещении успешно изменена";

                    return RedirectToAction("VisShow");
                }
                catch (Exception ex)
                {
                    ViewBag.ErMes = ex.Message;
                    ViewBag.ErStack = ex.StackTrace;
                    return View("Error");
                }

            }

            TempData["MessageError"] = "Ошибка валидации модели";

            ViewBag.ExerciseId = new SelectList(db.Exercises, "ExerciseId", "Subject", visit.ExerciseId);
            ViewBag.VisitorId = new SelectList(db.Persons, "PersonId", "FirstName", visit.VisitorId);
            ViewBag.RoleId = new SelectList(db.PersonRoles, "RoleId", "RoleName", visit.RoleId);
            return View(visit);
        }

        // GET: Visits/Delete/5
        public async Task<ActionResult> VisDelete(int? VisitId)
        {
            if (VisitId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Visit visit = await db.Visits.FindAsync(VisitId);
            if (visit == null)
            {
                return HttpNotFound();
            }
            return View(visit);
        }

        // POST: Visits/Delete/5
        [HttpPost, ActionName("VisDelete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int VisitId)
        {
            try
            {
                Visit visit = await db.Visits.FindAsync(VisitId);
                db.Visits.Remove(visit);
                await db.SaveChangesAsync();

                TempData["MessageOk"] = "Информация о посещении успешно удалена";

                return RedirectToAction("VisShow");
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
