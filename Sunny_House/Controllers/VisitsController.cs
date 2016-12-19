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

namespace Sunny_House.Controllers
{
    [Authorize(Roles = "Administrator, User")]
    public class VisitsController : Controller
    {
        private SunnyModel db = new SunnyModel();

        // GET: Visits
        public ActionResult VisShow()
        {
            ViewData["Roles"] = new SelectList(db.PersonRoles, "RoleId", "RoleName");

            return View();
        }


        // GET: Visits/Create
        public ActionResult VisCreate()
        {
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
                    if (db.Visits.Where(v => v.Person.PersonId == visit.VisitorId).Count() > 0)
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
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VisEdit([Bind(Include = "VisitorId,ExerciseId,VisitId,RoleId")] Visit visit)
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
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int VisitId, string ReturnUrl)
        {
            try
            {
                Visit visit = await db.Visits.FindAsync(VisitId);
                db.Visits.Remove(visit);
                await db.SaveChangesAsync();

                TempData["MessageOk"] = "Информация о посещении успешно удалена";

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


        public ActionResult VisitorPartialList(int? ExerciseId, string SearchString, int? RoleSearchString, int? page)
        {
            if (ExerciseId == 0 || ExerciseId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var _visitors = (from visitor in db.Visits
                             where (visitor.ExerciseId == ExerciseId) &&
                                    ((visitor.Person.FirstName.ToUpper().Contains(SearchString.ToUpper()) || String.IsNullOrEmpty(SearchString)) ||
                                      visitor.Person.LastName.ToUpper().Contains(SearchString.ToUpper()) || String.IsNullOrEmpty(SearchString)) &&
                                    (visitor.RoleId == RoleSearchString || RoleSearchString == null)
                             select new
                             {
                                 VisitId = visitor.VisitId,
                                 VisitorId = visitor.VisitorId,
                                 ExerciseId = visitor.ExerciseId,
                                 PersonFIO = visitor.Person.FirstName + " " + visitor.Person.LastName + " " + visitor.Person.MiddleName,
                                 FirstName = visitor.Person.FirstName,
                                 LastName = visitor.Person.LastName,
                                 RoleName = visitor.PersonRole.RoleName
                             }).OrderBy(v => v.FirstName).ThenBy(v => v.LastName).AsEnumerable().Select(v => new Visit
                             {
                                 VisitId = v.VisitId,
                                 VisitorId = v.VisitorId,
                                 ExerciseId = v.ExerciseId,
                                 PersonFIO = v.PersonFIO,
                                 RoleName = v.RoleName
                             });


            ViewData["SearchString"] = SearchString;
            ViewData["RoleSearchString"] = RoleSearchString;
            ViewData["ExerciseId"] = ExerciseId;

            int pageSize = 50;
            int pageNumber = (page ?? 1);

            return PartialView(_visitors.ToList().ToPagedList(pageNumber, pageSize));
        }

        public ActionResult AllVisitsPartial(string SearchString, int? RoleSearchString, int? page, string SortBy)
        {

            ViewBag.SortExName = SortBy == "ExName" ? "ExName desc" : "ExName";
            ViewBag.SortPersonFIO = SortBy == "PersonFIO" ? "PersonFIO desc" : "PersonFIO";
            ViewBag.SortRoleName = SortBy == "RoleName" ? "RoleName desc" : "RoleName";

            var visits = (from visit in db.Visits
                          join ex in db.Exercises on visit.ExerciseId equals ex.ExerciseId
                          join person in db.Persons on visit.VisitorId equals person.PersonId
                          join role in db.PersonRoles on visit.RoleId equals role.RoleId
                          where (role.RoleId == RoleSearchString || RoleSearchString == null) &&
                                    ((person.FirstName.ToUpper().Contains(SearchString.ToUpper()) || string.IsNullOrEmpty(SearchString)) ||
                                    (person.LastName.ToUpper().Contains(SearchString.ToUpper()) || string.IsNullOrEmpty(SearchString)) ||
                                    (ex.Subject.ToUpper().Contains(SearchString.ToUpper()) || string.IsNullOrEmpty(SearchString)))
                          select new
                          {
                              VisitId = visit.VisitId,
                              VisitorId = visit.VisitorId,
                              ExerciseId = ex.ExerciseId,
                              RoleId = role.RoleId,
                              PersonFIO = person.FirstName + " " + person.LastName + " " + person.MiddleName,
                              ExName = ex.Subject,
                              RoleName = role.RoleName

                          }).AsEnumerable().Select(x => new Visit
                          {
                              VisitId = x.VisitId,
                              VisitorId = x.VisitorId,
                              ExerciseId = x.ExerciseId,
                              PersonFIO = x.PersonFIO.Trim(),
                              RoleId = x.RoleId,
                              ExName = x.ExName,
                              RoleName = x.RoleName
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

                default:
                    visits = visits.OrderBy(x => x.PersonFIO);
                    break;
            }



            ViewData["SearchString"] = SearchString;
            ViewData["RoleSearchString"] = RoleSearchString;

            int pageSize = 50;
            int pageNumber = (page ?? 1);

            return PartialView(visits.ToList().ToPagedList(pageNumber, pageSize));

        }

        public ActionResult PersonsPartialList(string FilterMode, int? ExerciseId, string SearchString, int? page, string SortBy)
        {
            ViewBag.SortPersonFIO = SortBy == "PersonFIO" ? "PersonFIO desc" : "PersonFIO";
            ViewBag.SortAge = SortBy == "Age" ? "Age desc" : "Age";

            IEnumerable<PersonsViewModel> _persons;

            if (FilterMode == "Reserve" || String.IsNullOrEmpty(FilterMode))
            {
                _persons = (from reserve in db.Reserves
                            join person in db.Persons on reserve.PersonId equals person.PersonId
                            join _event in db.Events on reserve.EventId equals _event.EventId
                            join _ex in db.Exercises on _event.EventId equals _ex.EventId
                            where (_ex.ExerciseId == ExerciseId || ExerciseId == null) &&
                            ((person.FirstName.Contains(SearchString) || string.IsNullOrEmpty(SearchString)) || (person.LastName.Contains(SearchString) || string.IsNullOrEmpty(SearchString)))
                            select new
                            {
                                PersonFIO = person.FirstName + " " + person.LastName + " " + person.MiddleName,
                                PersonId = person.PersonId,
                                DateOfBirth = person.DateOfBirth
                            }).AsEnumerable().Select(p => new PersonsViewModel
                            {
                                PersonFIO = p.PersonFIO.TrimStart(),
                                PersonId = p.PersonId,
                                PersonAge = AgeMethods.GetAge(p.DateOfBirth),
                                PersonMonth = AgeMethods.GetTotalMonth(p.DateOfBirth),
                                DateOfBirth = p.DateOfBirth
                            });
            }
            else
            {
                _persons = (from person in db.Persons
                            where (person.FirstName.Contains(SearchString) || string.IsNullOrEmpty(SearchString)) || (person.LastName.Contains(SearchString) || string.IsNullOrEmpty(SearchString))
                            select new
                            {
                                PersonFIO = person.FirstName + " " + person.LastName + " " + person.MiddleName,
                                PersonId = person.PersonId,
                                DateOfBirth = person.DateOfBirth,
                            }).AsEnumerable().Select(p => new PersonsViewModel
                             {
                                 PersonFIO = p.PersonFIO.TrimStart(),
                                 PersonId = p.PersonId,
                                 PersonAge = AgeMethods.GetAge(p.DateOfBirth),
                                 PersonMonth = AgeMethods.GetTotalMonth(p.DateOfBirth),
                                 DateOfBirth = p.DateOfBirth
                             });
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
            ViewData["Roles"] = new SelectList(db.PersonRoles, "RoleId", "RoleName");

            int pageSize = 50;
            int pageNumber = (page ?? 1);

            return PartialView(_persons.ToList().ToPagedList(pageNumber, pageSize));
        }

        public JsonResult AddVisitAjax(int? ExId, int? RoleId, int? PersonId)
        {
            if (ExId == null || RoleId == null || PersonId == null)
            {
                return Json(new { Result = false, Message = "Ошибка валидации модели" }, JsonRequestBehavior.AllowGet);
            }
            try
            {

                if (db.Visits.Where(v => v.Person.PersonId == PersonId).Count() > 0)
                {
                    return Json(new { Result = false, Message = "Данная персона уже присутствует в списке посещений" }, JsonRequestBehavior.AllowGet);
                }

                Visit _visit = new Visit()
                {
                    VisitorId = PersonId ?? 0,
                    ExerciseId = ExId ?? 0,
                    RoleId = RoleId ?? 0
                };
                    
                db.Visits.Add(_visit);
                db.SaveChanges();

                return Json(new { Result = true, Message = "Инофрмация о песещении добавлена" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                string we = ex.Message;
                return Json(new { Result = false, Message = "Ошибка добавления добавления информации о посещении" }, JsonRequestBehavior.AllowGet);
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
