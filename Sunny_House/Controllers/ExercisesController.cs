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
using System.Diagnostics;

namespace Sunny_House.Controllers
{
    [Authorize(Roles = "Administrator, User")]
    public class ExercisesController : Controller
    {
        private SunnyModel db = new SunnyModel();

        // GET: Exercises
        public async Task<ActionResult> ExShow(int? ExerciseId, string ReturnUrl)
        {
            if (!String.IsNullOrEmpty(ReturnUrl))
            {
                ViewData["ReturnUrl"] = ReturnUrl;
            }

            var exercises = db.Exercises.Include(e => e.Event).Where(e => e.ExerciseId == ExerciseId || ExerciseId == null);
            return View(await exercises.ToListAsync());
        }

        // GET: Exercises/Details/5
        public async Task<ActionResult> ExDetails(int? ObjectId)
        {
            if (ObjectId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exercise exercise = await db.Exercises.FindAsync(ObjectId);
            if (exercise == null)
            {
                return HttpNotFound();
            }
            return View(exercise);
        }

        // GET: Exercises/Create
        public ActionResult ExCreate(int? AddressId, string ReturnUrl)
        {
            string _returnurl = (!String.IsNullOrEmpty(ReturnUrl)) ? ReturnUrl : "/Exercises/ExShow";
            ViewData["ReturnUrl"] = _returnurl;

            if (AddressId != null)
            {
                Address _address = db.Addresses.FirstOrDefault(a => a.AddressId == AddressId);
                string _addressString = string.Format("{0} {1}", _address.City, _address.AddressValue);
                ViewData["AddressText"] = _addressString;
                Exercise _exercise = new Exercise() { AddressId = _address.AddressId };
                return View(_exercise);
            }
            else
            {
                return View();
            }

        }

        // POST: Exercises/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExCreate([Bind(Include = "ExerciseId,Subject,StartTime,EndTime,AddressId,Note,EventId")] Exercise exercise, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s)); //Debug Information====================

                    string _returnurl = (!String.IsNullOrEmpty(ReturnUrl)) ? ReturnUrl : "/Exercises/ExShow";
                    ViewData["ReturnUrl"] = _returnurl;

                    db.Exercises.Add(exercise);
                    await db.SaveChangesAsync();

                    string _message = "Информация о занятии добавлена в базу данных";
                    TempData["MessageOk"] = _message;

                    return Redirect(_returnurl);
                }
                catch (Exception ex)
                {
                    ViewBag.ErMes = ex.Message;
                    ViewBag.ErStack = ex.StackTrace;
                    return View("Error");
                }
            }

            return View(exercise);
        }

        // GET: Exercises/Edit/5
        public async Task<ActionResult> ExEdit(int? ObjectId, int? AddressId)
        {
            if (ObjectId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Exercise _exercise = await db.Exercises.FindAsync(ObjectId);
            if (_exercise == null)
            {
                return HttpNotFound();
            }
            ViewData["EventName"] = db.Exercises.First(s => s.ExerciseId == ObjectId).Event.EventName;

            if (AddressId != null)
            {
                Address _address = db.Addresses.FirstOrDefault(a => a.AddressId == AddressId);
                string _addressString = string.Format("{0} {1}", _address.City, _address.AddressValue);
                ViewData["AddressText"] = _addressString;
            }
            else
            {
                string _addressString = string.Format("{0} {1}", _exercise.Address.City, _exercise.Address.AddressValue);
                ViewData["AddressText"] = _addressString;
            }

            return View(_exercise);
        }

        // POST: Exercises/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExEdit([Bind(Include = "ExerciseId,Subject,StartTime,EndTime,AddressId,Note,EventId")] Exercise exercise)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(exercise).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    string _message = "Информация о занятии успешно обновлена";
                    TempData["MessageOk"] = _message;

                    return RedirectToAction("ExShow");
                }
                catch (Exception ex)
                {
                    ViewBag.ErMes = ex.Message;
                    ViewBag.ErStack = ex.StackTrace;
                    return View("Error");
                }
            }

            return View(exercise);
        }

        // GET: Exercises/Delete/5
        public async Task<ActionResult> ExDelete(int? ObjectId)
        {
            if (ObjectId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exercise exercise = await db.Exercises.FindAsync(ObjectId);
            if (exercise == null)
            {
                return HttpNotFound();
            }
            return View(exercise);
        }

        // POST: Exercises/Delete/5
        [HttpPost, ActionName("ExDelete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExDeleteConfirmed(int ObjectId)
        {
            try
            {
                if (db.Exercises.First(e => e.ExerciseId == ObjectId).Visit.Any() || db.Exercises.First(e => e.ExerciseId == ObjectId).Comment.Any())
                {

                    TempData["MessageError"] = "Ошибка выполнения операции! В таблице посещений или отзывов имеются связанные данные.";
                    return RedirectToAction("ExShow");
                }

                Exercise exercise = await db.Exercises.FindAsync(ObjectId);
                db.Exercises.Remove(exercise);
                await db.SaveChangesAsync();

                string _message = "Информация о занятии успешно удалена";
                TempData["MessageOk"] = _message;

                return RedirectToAction("ExShow");
            }
            catch (Exception ex)
            {
                ViewBag.ErMes = ex.Message;
                ViewBag.ErStack = ex.StackTrace;
                return View("Error");
            }
        }

        public ActionResult CalendarShow()
        {
            return View();
        }


        [HttpGet]
        public ActionResult ShowExercises(DateTime start, DateTime end)
        {

            var _exercises = (from _ex in db.Exercises
                              select new
                              {
                                  id = _ex.ExerciseId,
                                  title = _ex.Subject,
                                  start = _ex.StartTime,
                                  end = _ex.EndTime
                              }).AsEnumerable().Where(ex => ex.start >= start && ex.end < end).Select(e => new
                              {
                                  id = e.id,
                                  title = e.title,
                                  start = e.start.ToLocalTime(),
                                  end = e.end.ToLocalTime()
                              });

            var rows = _exercises.ToArray();

            foreach (var item in rows)
            {
                Debug.WriteLine(item);
            }

            return Json(rows, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult FCEventChange(DateTime Start_Time, DateTime End_Time, int? Ex_Id)
        {
            if (Ex_Id == null || Start_Time == null || End_Time == null)
            {
                return Json(new { Result = false, Message = "Неверные входные параметры" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                Exercise _exercise = db.Exercises.Find(Ex_Id);
                _exercise.StartTime = Start_Time;
                _exercise.EndTime = End_Time;

                db.Entry(_exercise).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { Result = true, Message = "Успешно выполнено" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = "Ошибка выполнения! \n" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost, ActionName("FCEventDelete")]
        public ActionResult FCDelete(int? Ex_Id)
        {
            if (Ex_Id == null)
            {
                return Json(new { Result = false, Message = "Неверные входные параметры" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                Exercise _exercise = db.Exercises.FirstOrDefault(e => e.ExerciseId == Ex_Id);
                if (db.Exercises.First(e => e.ExerciseId == Ex_Id).Visit.Any() || db.Exercises.First(e => e.ExerciseId == Ex_Id).Comment.Any())
                {
                    return Json(new { Result = false, Message = "Удаление невозможно! \nВ таблице посещений или отзывов имеются связанные данные." }, JsonRequestBehavior.AllowGet);
                }

                db.Exercises.Remove(_exercise);
                db.SaveChanges();
                return Json(new { Result = true, Message = "Успешно выполнено" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = "Ошибка выполнения! \n" + ex.Message }, JsonRequestBehavior.AllowGet);
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
