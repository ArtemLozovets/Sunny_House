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
using PagedList;
using PagedList.Mvc;

namespace Sunny_House.Controllers
{
    [Authorize(Roles = "Administrator, User, Presenter")]
    public class ExercisesController : Controller
    {
        private SunnyModel db = new SunnyModel();


        // GET: Exercises
        [HttpGet]
        public ActionResult ExShow(int? ExerciseId, string ReturnUrl, string FilterMode, string SearchString, string SearchStartDate, string SearchEndDate, int? page, string SortBy)
        {
            db.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s)); //Debug Ingormation --------------------------

            ViewBag.SortEventName = SortBy == "EventName" ? "EventName desc" : "EventName";
            ViewBag.SortSubject = SortBy == "Subject" ? "Subject desc" : "Subject";
            ViewBag.SortStartTime = SortBy == "StartTime" ? "StartTime desc" : "StartTime";
            ViewBag.SortEndTime = SortBy == "EndTime" ? "EndTime desc" : "EndTime";

            if (!String.IsNullOrEmpty(ReturnUrl))
            {
                ViewData["ReturnUrl"] = ReturnUrl;
            }

            int pageSize = 50;
            int pageNumber = (page ?? 1);

            if (ExerciseId != null)
            {
                var _exercises = db.Exercises.Include(e => e.Event).Where(e => e.ExerciseId == ExerciseId);
                return View(_exercises.ToList().ToPagedList(pageNumber, pageSize));
            }

            else
            {
                DateTime? _startDate = (String.IsNullOrEmpty(SearchStartDate)) ? Convert.ToDateTime("1900-01-01") : Convert.ToDateTime(SearchStartDate);
                DateTime? _endDate = (String.IsNullOrEmpty(SearchEndDate)) ? Convert.ToDateTime("2900-01-01") : Convert.ToDateTime(SearchEndDate);

                var _exercises = (from ex in db.Exercises

                                  where ((_endDate >= ex.StartTime) && (_startDate <= ex.EndTime)) &&
                                        (ex.Subject.ToUpper().Contains(SearchString.ToUpper()) || String.IsNullOrEmpty(SearchString))
                                  select ex);

                switch (FilterMode)
                {
                    case "All":
                        break;

                    case "Archive":
                        _exercises = _exercises.Where(e => e.EndTime < DateTime.Now);
                        break;

                    default:
                        _exercises = _exercises.Where(e => e.EndTime >= DateTime.Now);
                        break;
                }


                switch (SortBy)
                {
                    case "EventName desc":
                        _exercises = _exercises.OrderByDescending(x => x.Event.EventName);
                        ViewData["SortColumn"] = "EventName";
                        break;
                    case "EventName":
                        _exercises = _exercises.OrderBy(x => x.Event.EventName);
                        ViewData["SortColumn"] = "EventName";
                        break;

                    case "Subject desc":
                        _exercises = _exercises.OrderByDescending(x => x.Subject);
                        ViewData["SortColumn"] = "Subject";
                        break;
                    case "Subject":
                        _exercises = _exercises.OrderBy(x => x.Subject);
                        ViewData["SortColumn"] = "Subject";
                        break;

                    case "StartTime desc":
                        _exercises = _exercises.OrderByDescending(x => x.StartTime);
                        ViewData["SortColumn"] = "StartTime";
                        break;
                    case "StartTime":
                        _exercises = _exercises.OrderBy(x => x.StartTime);
                        ViewData["SortColumn"] = "StartTime";
                        break;

                    case "EndTime desc":
                        _exercises = _exercises.OrderByDescending(x => x.EndTime);
                        ViewData["SortColumn"] = "EndTime";
                        break;
                    case "EndTime":
                        _exercises = _exercises.OrderBy(x => x.EndTime);
                        ViewData["SortColumn"] = "EndTime";
                        break;
                    default:
                        _exercises = _exercises.OrderByDescending(x => x.ExerciseId);
                        break;
                }


                return View(_exercises.ToList().ToPagedList(pageNumber, pageSize));

            }

        }

        // GET: Exercises/Details/5
        public async Task<ActionResult> ExDetails(int? ObjectId, string ReturnParam)
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

            ViewData["Roles"] = new SelectList(db.PersonRoles, "RoleId", "RoleName");

            if (!string.IsNullOrEmpty(ReturnParam))
            {
                ViewData["ReturnParam"] = ReturnParam;
            }
            else
            {
                ViewData["ReturnParam"] = "/Exercises/ExShow";
            }

            return View(exercise);
        }

        // GET: Exercises/Create
         [Authorize(Roles = "Administrator, User")]
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
        [Authorize(Roles = "Administrator, User")]
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

                    if ((exercise.EndTime - exercise.StartTime).TotalMinutes >= 30)
                    {
                        db.Exercises.Add(exercise);
                        await db.SaveChangesAsync();

                        string _message = "Информация о занятии добавлена в базу данных";
                        TempData["MessageOk"] = _message;

                        return Redirect(_returnurl);


                    }
                    else
                    {
                        TempData["MessageError"] = "Продолжительность занятия должна быть не менее 30 минут";
                        return Redirect(_returnurl);
                    }


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
        [Authorize(Roles = "Administrator, User")]
        public async Task<ActionResult> ExEdit(int? ObjectId, int? AddressId, string ReturnParam)
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

            if (!string.IsNullOrEmpty(ReturnParam))
            {
                ViewData["ReturnParam"] = ReturnParam;
            }
            else
            {
                ViewData["ReturnParam"] = "/Exercises/ExShow";
            }

            return View(_exercise);
        }

        // POST: Exercises/Edit/5
        [HttpPost]
        [Authorize(Roles = "Administrator, User")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExEdit([Bind(Include = "ExerciseId,Subject,StartTime,EndTime,AddressId,Note,EventId")] Exercise exercise, string ReturnParam)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if ((exercise.EndTime - exercise.StartTime).TotalMinutes >= 30)
                    {
                        db.Entry(exercise).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        string _message = "Информация о занятии успешно обновлена";
                        TempData["MessageOk"] = _message;

                        if (!string.IsNullOrEmpty(ReturnParam))
                        {
                            return Redirect(ReturnParam);
                        }
                        else return RedirectToAction("ExShow");
                    }
                    else
                    {
                        TempData["MessageError"] = "Продолжительность занятия должна быть не менее 30 минут";
                        return Redirect("ExShow");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErMes = ex.Message;
                    ViewBag.ErStack = ex.StackTrace;
                    return View("Error");
                }
            }

            if (!string.IsNullOrEmpty(ReturnParam))
            {
                ViewData["ReturnParam"] = ReturnParam;
            }
            else
            {
                ViewData["ReturnParam"] = "/Exercises/ExShow";
            }

            return View(exercise);
        }

        // GET: Exercises/Delete/5
        [Authorize(Roles = "Administrator, User")]
        public async Task<ActionResult> ExDelete(int? ObjectId, string ReturnParam, string SuccessReturn)
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

            if (!string.IsNullOrEmpty(SuccessReturn))
            {
                ViewData["SuccessReturn"] = SuccessReturn;
            }

            if (!string.IsNullOrEmpty(ReturnParam))
            {
                ViewData["ReturnParam"] = ReturnParam;
            }
            else
            {
                ViewData["ReturnParam"] = "/Exercises/ExShow";
            }

            return View(exercise);
        }

        // POST: Exercises/Delete/5
        [HttpPost, ActionName("ExDelete")]
        [Authorize(Roles = "Administrator, User")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExDeleteConfirmed(int ObjectId, string ReturnParam, string SuccessReturn)
        {
            try
            {
                if (db.Exercises.First(e => e.ExerciseId == ObjectId).Visit.Any() || db.Exercises.First(e => e.ExerciseId == ObjectId).Comment.Any())
                {

                    TempData["MessageError"] = "Ошибка выполнения операции! В таблице посещений или отзывов имеются связанные данные.";

                    if (!string.IsNullOrEmpty(ReturnParam))
                    {
                        return Redirect(ReturnParam);
                    }
                    else return RedirectToAction("ExShow");
                }

                Exercise exercise = await db.Exercises.FindAsync(ObjectId);
                db.Exercises.Remove(exercise);
                await db.SaveChangesAsync();

                string _message = "Информация о занятии успешно удалена";
                TempData["MessageOk"] = _message;

                if (string.IsNullOrEmpty(SuccessReturn))
                {
                    if (!string.IsNullOrEmpty(ReturnParam))
                    {
                        return Redirect(ReturnParam);
                    }
                    else return RedirectToAction("ExShow");
                }
                else
                {
                    return Redirect(SuccessReturn);
                }

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
        [Authorize(Roles = "Administrator, User")]
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
        [Authorize(Roles = "Administrator, User")]
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
