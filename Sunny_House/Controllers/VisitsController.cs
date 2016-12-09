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
        public ActionResult VisShow()
        {
            //var visits = db.Visits.Include(v => v.Exercise).Include(v => v.Person).Include(v => v.PersonRole);

            var visits = (from visit in db.Visits
                          join ex in db.Exercises on visit.ExerciseId equals ex.ExerciseId
                          join person in db.Persons on visit.VisitorId equals person.PersonId
                          join role in db.PersonRoles on visit.RoleId equals role.RoleId
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
                              PersonFIO = x.PersonFIO,
                              RoleId = x.RoleId,
                              ExName = x.ExName,
                              RoleName = x.RoleName
                          }).ToList();

            return View(visits);
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


        public ActionResult VisitorPartialList(int? ExerciseId, string SearchString, int? RoleSearchString)
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
                                 RoleName = visitor.PersonRole.RoleName
                             }).AsEnumerable().Select(v => new Visit
                             {
                                 VisitId = v.VisitId,
                                 VisitorId = v.VisitorId,
                                 ExerciseId = v.ExerciseId,
                                 PersonFIO = v.PersonFIO,
                                 RoleName = v.RoleName
                             });

            return PartialView(_visitors.ToList());
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
