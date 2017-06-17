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
using PagedList;
using PagedList.Mvc;
using Sunny_House.Methods;
using System.Globalization;

namespace Sunny_House.Controllers
{
    [Authorize(Roles = "Administrator, User, Presenter")]


    public class VisitsController : Controller
    {
        private SunnyModel db = new SunnyModel();

        private class JSONVisitorModel
        {
            public int PersonId { get; set; }
            public int RoleId { get; set; }
        }

        // GET: Visits
        public ActionResult VisShow(int? EventId, string EventName)
        {
            ViewData["Roles"] = new SelectList(db.PersonRoles, "RoleId", "RoleName");
            if (EventId != null)
            {
                ViewData["EventId"] = EventId;
                ViewData["EventName"] = EventName;
            }
            return View();
        }


        // GET: Visits/Create
        [Authorize(Roles = "Administrator, User")]
        public ActionResult VisCreate()
        {
            ViewBag.RoleId = new SelectList(db.PersonRoles, "RoleId", "RoleName");
            return View();
        }

        // POST: Visits/Create
        [HttpPost]
        [Authorize(Roles = "Administrator, User")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VisCreate([Bind(Include = "VisitorId,ExerciseId,RoleId,Note,FactVisit")] Visit visit)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (db.Visits.Where(v => v.Person.PersonId == visit.VisitorId && v.ExerciseId == visit.ExerciseId).Count() > 0)
                    {
                        TempData["MessageError"] = "Данная персона уже присутствует в списке посещений";
                        return RedirectToAction("VisShow");
                    }

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

            if (visit.ExerciseId != 0)
            {
                ViewBag.ExerciseName = db.Exercises.FirstOrDefault(x => x.ExerciseId == visit.ExerciseId).Subject.ToString();
            }

            if (visit.VisitorId != 0)
            {
                var _fio = (from person in db.Persons
                            where person.PersonId == visit.VisitorId
                            select new
                            {
                                FirstName = person.FirstName,
                                LastName = person.LastName,
                                MiddleName = person.MiddleName
                            }).FirstOrDefault();
                ViewBag.VisitorFIO = String.Format("{0} {1} {2}", _fio.FirstName, _fio.LastName, _fio.MiddleName);
            }

            ViewBag.RoleId = new SelectList(db.PersonRoles, "RoleId", "RoleName", visit.RoleId);
            return View(visit);
        }

        // GET: Visits/Edit/5
        [Authorize(Roles = "Administrator, User")]
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


            var _fio = (from person in db.Persons
                        where person.PersonId == visit.VisitorId
                        select new
                        {
                            FirstName = person.FirstName,
                            LastName = person.LastName,
                            MiddleName = person.MiddleName
                        }).FirstOrDefault();

            ViewBag.VisitorFIO = String.Format("{0} {1} {2}", _fio.FirstName, _fio.LastName, _fio.MiddleName);
            ViewBag.ExerciseName = db.Exercises.FirstOrDefault(x => x.ExerciseId == visit.ExerciseId).Subject.ToString();
            ViewBag.RoleId = new SelectList(db.PersonRoles, "RoleId", "RoleName", visit.RoleId);
            return View(visit);
        }

        // POST: Visits/Edit/5
        [HttpPost]
        [Authorize(Roles = "Administrator, User")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VisEdit([Bind(Include = "VisitorId,ExerciseId,VisitId,RoleId,Note,FactVisit")] Visit visit)
        {
            db.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s)); //Debug Information====================
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

            var _fio = (from person in db.Persons
                        where person.PersonId == visit.VisitorId
                        select new
                        {
                            FirstName = person.FirstName,
                            LastName = person.LastName,
                            MiddleName = person.MiddleName
                        }).FirstOrDefault();

            ViewBag.VisitorFIO = String.Format("{0} {1} {2}", _fio.FirstName, _fio.LastName, _fio.MiddleName);
            ViewBag.ExerciseName = db.Exercises.FirstOrDefault(x => x.ExerciseId == visit.ExerciseId).Subject.ToString();
            ViewBag.RoleId = new SelectList(db.PersonRoles, "RoleId", "RoleName", visit.RoleId);
            return View(visit);
        }

        // GET: Visits/Delete/5
        [Authorize(Roles = "Administrator, User")]
        public async Task<ActionResult> VisDelete(int? VisitId, string ReturnUrl)
        {
            if (VisitId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (!String.IsNullOrEmpty(ReturnUrl))
            {
                ViewData["ReturnUrl"] = ReturnUrl;
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
        [Authorize(Roles = "Administrator, User")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int VisitId, string ReturnUrl)
        {
            try
            {
                Visit visit = db.Visits.FirstOrDefault(x => x.VisitId == VisitId);
                if (visit != null)
                {
                    db.Visits.Remove(visit);
                    await db.SaveChangesAsync();
                    TempData["MessageOk"] = "Информация о посещении успешно удалена";
                }
                else
                {
                    string _message = string.Format("Удаление невозможно. Указанный объект отсутствует в базе данных.");
                    TempData["MessageError"] = _message;
                }

                if (String.IsNullOrEmpty(ReturnUrl))
                {
                    return RedirectToAction("VisShow");
                }
                else return Redirect(ReturnUrl);

            }
            catch (Exception ex)
            {
                ViewBag.ErMes = ex.Message;
                ViewBag.ErStack = ex.StackTrace;
                return View("Error");
            }
        }


        public ActionResult PreVisitorPartialList(int? ExerciseId, string PreSearchString, int? PreRoleSearchString, int? page)
        {
            if (ExerciseId == 0 || ExerciseId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var _visitors = (from visitor in db.Visits
                             where (visitor.ExerciseId == ExerciseId)
                                        && (visitor.Person.FirstName.ToUpper().Contains(PreSearchString.ToUpper())
                                            || visitor.Person.LastName.ToUpper().Contains(PreSearchString.ToUpper())
                                            || String.IsNullOrEmpty(PreSearchString))
                                        && (visitor.RoleId == PreRoleSearchString || PreRoleSearchString == null)
                                        && (visitor.FactVisit == false)
                             select new
                             {
                                 VisitId = visitor.VisitId,
                                 VisitorId = visitor.VisitorId,
                                 ExerciseId = visitor.ExerciseId,
                                 PersonFIO = visitor.Person.FirstName + " " + visitor.Person.LastName + " " + visitor.Person.MiddleName,
                                 FirstName = visitor.Person.FirstName,
                                 LastName = visitor.Person.LastName,
                                 RoleId = visitor.RoleId,
                                 RoleName = visitor.PersonRole.RoleName,
                                 Note = visitor.Note,
                                 FactVisit = visitor.FactVisit
                             }).OrderBy(v => v.FirstName).ThenBy(v => v.LastName).AsEnumerable().Select(v => new Visit
                             {
                                 VisitId = v.VisitId,
                                 VisitorId = v.VisitorId,
                                 ExerciseId = v.ExerciseId,
                                 PersonFIO = v.PersonFIO,
                                 RoleId = v.RoleId,
                                 RoleName = v.RoleName,
                                 Note = v.Note,
                                 FactVisit = v.FactVisit
                             });

            ViewData["SearchString"] = PreSearchString;
            ViewData["RoleSearchString"] = PreRoleSearchString;
            ViewData["ExerciseId"] = ExerciseId;
            ViewBag.RoleList = db.PersonRoles.ToList();

            int pageSize = 50;
            int pageNumber = (page ?? 1);

            return PartialView(_visitors.ToList().ToPagedList(pageNumber, pageSize));
        }


        public ActionResult VisitorPartialList(int? ExerciseId, string SearchString, int? RoleSearchString, int? page)
        {
            if (ExerciseId == 0 || ExerciseId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var _visitors = (from visitor in db.Visits
                             where (visitor.ExerciseId == ExerciseId)
                                        && (visitor.Person.FirstName.ToUpper().Contains(SearchString.ToUpper())
                                            || visitor.Person.LastName.ToUpper().Contains(SearchString.ToUpper())
                                            || String.IsNullOrEmpty(SearchString))
                                        && (visitor.RoleId == RoleSearchString || RoleSearchString == null)
                                        && (visitor.FactVisit == true)
                             select new
                             {
                                 VisitId = visitor.VisitId,
                                 VisitorId = visitor.VisitorId,
                                 ExerciseId = visitor.ExerciseId,
                                 //EventId = db.Exercises.Where(e => e.ExerciseId == visitor.ExerciseId).Select(x => x.EventId).FirstOrDefault(),
                                 EventId = visitor.Exercise.EventId,
                                 StartTime = visitor.Exercise.StartTime,
                                 PersonFIO = visitor.Person.FirstName + " " + visitor.Person.LastName + " " + visitor.Person.MiddleName,
                                 FirstName = visitor.Person.FirstName,
                                 LastName = visitor.Person.LastName,
                                 RoleId = visitor.RoleId,
                                 RoleName = visitor.PersonRole.RoleName,
                                 Note = visitor.Note,
                                 FactVisit = visitor.FactVisit,
                             }).OrderBy(v => v.FirstName).ThenBy(v => v.LastName).AsEnumerable().Select(v => new Visit
                             {
                                 VisitId = v.VisitId,
                                 VisitorId = v.VisitorId,
                                 ExerciseId = v.ExerciseId,
                                 EventId = v.EventId,
                                 PersonFIO = v.PersonFIO,
                                 RoleId = v.RoleId,
                                 RoleName = v.RoleName,
                                 Note = v.Note,
                                 FactVisit = v.FactVisit,
                                 StartTime = v.StartTime
                             });

            ViewData["SearchString"] = SearchString;
            ViewData["RoleSearchString"] = RoleSearchString;
            ViewData["ExerciseId"] = ExerciseId;

            ViewData["EndTime"] = Convert.ToDateTime(db.Exercises.FirstOrDefault(x => x.ExerciseId == ExerciseId).EndTime.ToString());

            ViewBag.RoleList = db.PersonRoles.ToList();

            int pageSize = 50;
            int pageNumber = (page ?? 1);

            return PartialView(_visitors.ToList().ToPagedList(pageNumber, pageSize));
        }

        public ActionResult AllVisitsPartial(int? RoleSearchString, int? VisitorId, int? EventId, int? ExerciseId, DateTime? StartDate, int? page, string SortBy, string FilterMode)
        {

            bool _fact = String.IsNullOrEmpty(FilterMode) || FilterMode == "Fact" ? true : false;

            ViewData["RoleSearchString"] = RoleSearchString;
            ViewData["VisitorId"] = VisitorId;
            ViewData["EventId"] = EventId;
            ViewData["ExerciseId"] = ExerciseId;
            ViewData["StartDate"] = StartDate;


            ViewBag.SortExName = SortBy == "ExName" ? "ExName desc" : "ExName";
            ViewBag.SortPersonFIO = SortBy == "PersonFIO" ? "PersonFIO desc" : "PersonFIO";
            ViewBag.SortRoleName = SortBy == "RoleName" ? "RoleName desc" : "RoleName";
            ViewBag.SortStartTime = SortBy == "StartTime" ? "StartTime desc" : "StartTime";
            ViewBag.SortEventName = SortBy == "EventName" ? "EventName desc" : "EventName";

            db.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s));//========================================

            var visits = (from visit in db.Visits
                          join ex in db.Exercises on visit.ExerciseId equals ex.ExerciseId
                          join person in db.Persons on visit.VisitorId equals person.PersonId
                          join role in db.PersonRoles on visit.RoleId equals role.RoleId
                          where (visit.FactVisit == _fact || _fact == false) &&
                                (role.RoleId == RoleSearchString || RoleSearchString == null) &&
                                (DbFunctions.TruncateTime(ex.StartTime) == StartDate || StartDate == null) &&
                                (person.PersonId == VisitorId || VisitorId == null) &&
                                (ex.ExerciseId == ExerciseId || ExerciseId == null) &&
                                (ex.Event.EventId == EventId || EventId == null)

                          select new
                          {
                              VisitId = visit.VisitId,
                              VisitorId = visit.VisitorId,
                              ExerciseId = ex.ExerciseId,
                              RoleId = role.RoleId,
                              PersonFIO = person.FirstName + " " + person.LastName + " " + person.MiddleName,
                              ExName = ex.Subject,
                              RoleName = role.RoleName,
                              Note = visit.Note,
                              StartTime = ex.StartTime,
                              EventId = ex.EventId,
                              EventName = ex.Event.EventName,
                              FactVisit = visit.FactVisit

                          }).AsEnumerable().Select(x => new Visit
                          {
                              VisitId = x.VisitId,
                              VisitorId = x.VisitorId,
                              ExerciseId = x.ExerciseId,
                              PersonFIO = x.PersonFIO.Trim(),
                              RoleId = x.RoleId,
                              ExName = x.ExName,
                              RoleName = x.RoleName,
                              Note = x.Note,
                              StartTime = x.StartTime,
                              EventId = x.EventId,
                              EventName = x.EventName,
                              FactVisit = x.FactVisit
                          });


            switch (SortBy)
            {
                case "ExName desc":
                    visits = visits.OrderByDescending(x => x.ExName);
                    ViewData["SortColumn"] = "ExName";
                    break;
                case "ExName":
                    visits = visits.OrderBy(x => x.ExName);
                    ViewData["SortColumn"] = "ExName";
                    break;

                case "PersonFIO desc":
                    visits = visits.OrderByDescending(x => x.PersonFIO);
                    ViewData["SortColumn"] = "PersonFIO";
                    break;
                case "PersonFIO":
                    visits = visits.OrderBy(x => x.PersonFIO);
                    ViewData["SortColumn"] = "PersonFIO";
                    break;

                case "RoleName desc":
                    visits = visits.OrderByDescending(x => x.RoleName);
                    ViewData["SortColumn"] = "RoleName";
                    break;
                case "RoleName":
                    visits = visits.OrderBy(x => x.RoleName);
                    ViewData["SortColumn"] = "RoleName";
                    break;

                case "StartTime desc":
                    visits = visits.OrderByDescending(x => x.StartTime);
                    ViewData["SortColumn"] = "StartTime";
                    break;
                case "StartTime":
                    visits = visits.OrderBy(x => x.StartTime);
                    ViewData["SortColumn"] = "StartTime";
                    break;

                case "EventName desc":
                    visits = visits.OrderByDescending(x => x.EventName);
                    ViewData["SortColumn"] = "EventName";
                    break;
                case "EventName":
                    visits = visits.OrderBy(x => x.EventName);
                    ViewData["SortColumn"] = "EventName";
                    break;

                default:
                    visits = visits.OrderByDescending(x => x.StartTime);
                    break;
            }


            int pageSize = 50;
            int pageNumber = (page ?? 1);

            return PartialView(visits.ToList().ToPagedList(pageNumber, pageSize));

        }

        public ActionResult PersonsPartialList(string FilterMode, int? ExerciseId, string SearchString, int? page, string SortBy)
        {
            ViewBag.SortPersonFIO = SortBy == "PersonFIO" ? "PersonFIO desc" : "PersonFIO";
            ViewBag.SortAge = SortBy == "Age" ? "Age desc" : "Age";

            IEnumerable<PersonsViewModel> _persons;

            switch (FilterMode)
            {
                case "Potential":
                    _persons = (from pot in db.PotentialСlients
                                join person in db.Persons on pot.PersonId equals person.PersonId
                                join ev in db.Events on pot.EventId equals ev.EventId
                                where (ev.Exercise.Any(z => z.ExerciseId == ExerciseId))
                                        && (person.FirstName.Contains(SearchString)
                                            || person.LastName.Contains(SearchString)
                                            || person.Note.Contains(SearchString)
                                            || person.PersonCommunication.Select(ss => ss.Communication).Any(zz => zz.Address_Number.Contains(SearchString))
                                            || string.IsNullOrEmpty(SearchString))
                                select new
                                {
                                    PersonFIO = person.FirstName + " " + person.LastName + " " + person.MiddleName,
                                    PersonId = person.PersonId,
                                    DateOfBirth = person.DateOfBirth,
                                    Note = pot.Infoes,
                                    PotentialId = pot.ClientId
                                }).AsEnumerable().Select(p => new PersonsViewModel
                                {
                                    PersonFIO = p.PersonFIO.TrimStart(),
                                    PersonId = p.PersonId,
                                    PersonAge = AgeMethods.GetAge(p.DateOfBirth, false),
                                    PersonMonth = AgeMethods.GetTotalMonth(p.DateOfBirth),
                                    DateOfBirth = p.DateOfBirth,
                                    Note = p.Note,
                                    PotentialId = p.PotentialId
                                });

                    ViewData["FMode"] = "potential";
                    break;

                case "All":
                     _persons = (from person in db.Persons
                            where (person.FirstName.Contains(SearchString)
                                || person.LastName.Contains(SearchString)
                                || person.Note.Contains(SearchString)
                                || person.PersonCommunication.Select(ss => ss.Communication).Any(zz => zz.Address_Number.Contains(SearchString))
                                || string.IsNullOrEmpty(SearchString))
                            select new
                            {
                                PersonFIO = person.FirstName + " " + person.LastName + " " + person.MiddleName,
                                PersonId = person.PersonId,
                                DateOfBirth = person.DateOfBirth,
                                Note = person.Note
                            }).AsEnumerable().Select(p => new PersonsViewModel
                             {
                                 PersonFIO = p.PersonFIO.TrimStart(),
                                 PersonId = p.PersonId,
                                 PersonAge = AgeMethods.GetAge(p.DateOfBirth, false),
                                 PersonMonth = AgeMethods.GetTotalMonth(p.DateOfBirth),
                                 DateOfBirth = p.DateOfBirth,
                                 Note = p.Note
                             });
                     ViewData["FMode"] = "all";
                    break;
                
                default:
                    _persons = (from reserve in db.Reserves
                                join person in db.Persons on reserve.PersonId equals person.PersonId
                                join _event in db.Events on reserve.EventId equals _event.EventId
                                join _ex in db.Exercises on _event.EventId equals _ex.EventId
                                where (_ex.ExerciseId == ExerciseId || ExerciseId == null) &&
                                (person.FirstName.Contains(SearchString)
                                    || person.LastName.Contains(SearchString)
                                    || person.Note.Contains(SearchString)
                                    || person.PersonCommunication.Select(ss => ss.Communication).Any(zz => zz.Address_Number.Contains(SearchString))
                                    || string.IsNullOrEmpty(SearchString))
                                select new
                                {
                                    PersonFIO = person.FirstName + " " + person.LastName + " " + person.MiddleName,
                                    PersonId = person.PersonId,
                                    DateOfBirth = person.DateOfBirth,
                                    RoleId = reserve.RoleId,
                                    Note = reserve.Note,
                                    ReserveId = reserve.ReserveId
                                }).AsEnumerable().Select(p => new PersonsViewModel
                                {
                                    PersonFIO = p.PersonFIO.TrimStart(),
                                    PersonId = p.PersonId,
                                    PersonAge = AgeMethods.GetAge(p.DateOfBirth, false),
                                    PersonMonth = AgeMethods.GetTotalMonth(p.DateOfBirth),
                                    DateOfBirth = p.DateOfBirth,
                                    RoleId = p.RoleId,
                                    Note = p.Note,
                                    ReserveId = p.ReserveId
                                });
                    ViewData["FMode"] = "reserve";
                    break;
            }

            switch (SortBy)
            {
                case "PersonFIO desc":
                    _persons = _persons.OrderByDescending(x => x.PersonFIO);
                    ViewData["SortColumn"] = "PersonFIO";
                    break;
                case "PersonFIO":
                    _persons = _persons.OrderBy(x => x.PersonFIO);
                    ViewData["SortColumn"] = "PersonFIO";
                    break;

                case "Age desc":
                    _persons = _persons.OrderByDescending(x => x.DateOfBirth);
                    ViewData["SortColumn"] = "Age";
                    break;
                case "Age":
                    _persons = _persons.OrderBy(x => x.DateOfBirth);
                    ViewData["SortColumn"] = "Age";
                    break;

                default:
                    _persons = _persons.OrderBy(x => x.PersonFIO);
                    break;
            }

            ViewData["SearchString"] = SearchString;
            ViewData["ExerciseId"] = ExerciseId;

            ViewBag.RoleList = db.PersonRoles.ToList();

            int pageSize = 50;
            int pageNumber = (page ?? 1);

            return PartialView(_persons.ToList().ToPagedList(pageNumber, pageSize));
        }



        [Authorize(Roles = "Administrator, User")]
        public JsonResult AddPreVisitAjax(int? ExId, string PersonsJSON)
        {
            if (ExId == null || String.IsNullOrEmpty(PersonsJSON))
            {
                return Json(new { Result = false, Message = "Ошибка валидации модели" }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                int _clientroleid = db.PersonRoles.First(r => r.RoleName.ToUpper() == (string)"Клиент".ToUpper()).RoleId;
                string _jsonObject = PersonsJSON.Replace(@"\", string.Empty);
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                IList<JSONVisitorModel> jsonObject = serializer.Deserialize<IList<JSONVisitorModel>>(_jsonObject);
                List<Visit> _visitlist = new List<Visit>();

                foreach (var Visitor in jsonObject)
                {
                    if (db.Visits.Where(v => v.Person.PersonId == Visitor.PersonId && v.ExerciseId == ExId).Count() > 0)
                    {
                        string _pers = db.Persons.Where(x => x.PersonId == Visitor.PersonId).Select(x => x.FirstName + " " + x.LastName + " " + x.MiddleName).FirstOrDefault();
                        return Json(new { Result = false, Message = String.Format("{0} уже присутствует в списке посещений", _pers) }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        Visit _visit = new Visit()
                        {
                            VisitorId = Visitor.PersonId,
                            ExerciseId = ExId ?? 0,
                            RoleId = Visitor.RoleId == 0 ? _clientroleid : Visitor.RoleId,
                            FactVisit = false
                        };
                        _visitlist.Add(_visit);
                    }
                }

                db.Visits.AddRange(_visitlist);
                db.SaveChanges();

                return Json(new { Result = true, Message = "Инофрмация о посещении добавлена" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string we = ex.Message;
                return Json(new { Result = false, Message = "Ошибка добавления информации о посещении" }, JsonRequestBehavior.AllowGet);
            }
        }



        [Authorize(Roles = "Administrator, User")]
        public JsonResult AddVisitAjax(int? ExId, int? RoleId, int? PersonId, string Note)
        {
            if (ExId == null || RoleId == null || PersonId == null)
            {
                return Json(new { Result = false, Message = "Ошибка валидации модели" }, JsonRequestBehavior.AllowGet);
            }
            try
            {

                if (db.Visits.Where(v => v.Person.PersonId == PersonId && v.ExerciseId == ExId).Count() > 0)
                {
                    return Json(new { Result = false, Message = "Данная персона уже присутствует в списке посещений" }, JsonRequestBehavior.AllowGet);
                }

                Visit _visit = new Visit()
                {
                    VisitorId = PersonId ?? 0,
                    ExerciseId = ExId ?? 0,
                    RoleId = RoleId ?? 0,
                    Note = Note
                };

                db.Visits.Add(_visit);
                db.SaveChanges();

                return Json(new { Result = true, Message = "Инофрмация добавлена" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string we = ex.Message;
                return Json(new { Result = false, Message = "Ошибка добавления информации" }, JsonRequestBehavior.AllowGet);
            }
        }



        [Authorize(Roles = "Administrator, User")]
        public JsonResult AjaxFactChange(int[] VisitsArray)
        {
            if (VisitsArray == null || VisitsArray.Length == 0)
            {
                return Json(new { Result = false, Message = "Ошибка валидации модели" }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                db.Visits.Where(x => VisitsArray.Contains(x.VisitId)).ToList().ForEach(x => x.FactVisit = true);
                db.SaveChanges();

                return Json(new { Result = true, Message = "Инофрмация о факте посещения обновлена" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string we = ex.Message;
                return Json(new { Result = false, Message = "Ошибка обновления информации о факте посещения" }, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize(Roles = "Administrator, User")]
        public JsonResult AjaxChangeRole(int? VisitId, int? RoleId)
        {
            if (VisitId == null || VisitId == 0 || RoleId == null || RoleId == 0)
            {
                return Json(new { Result = false, Message = "Неверные входные параметры" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                Visit _visit = db.Visits.Find(VisitId);
                _visit.RoleId = RoleId ?? 1;

                db.SaveChanges();

                return Json(new { Result = true, Message = "Роль изменена" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = "Во время выполнения запроса произошла критическая ошибка:" + ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, User")]
        public JsonResult AjaxUpdateInfoes(int? VisitId, string Infoes)
        {
            if (VisitId == null)
            {
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                Visit _visit = db.Visits.Find(VisitId);
                if (_visit == null)
                {
                    return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
                }

                _visit.Note = Infoes;

                db.Entry(_visit).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
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
