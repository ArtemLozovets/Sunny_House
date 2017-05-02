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


namespace Sunny_House.Controllers
{
    [Authorize(Roles = "Administrator, User, Presenter")]
    public class PersonRelationsController : Controller
    {
        private SunnyModel db = new SunnyModel();

        #region Зв'язки персон
        // GET: PersonRelations
        public async Task<ActionResult> PRShow()
        {
            var personRelations = db.PersonRelations.Include(p => p.Person).Include(p => p.Person1);
            return View(await personRelations.ToListAsync());
        }

        // GET: PersonRelations/Create
        [Authorize(Roles = "Administrator, User")]
        public ActionResult PRCreate(int? PersonId)
        {
            if (PersonId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Person _person = db.Persons.FirstOrDefault(p => p.PersonId == PersonId);
            string _personname = String.Format("{0} {1} {2}", _person.FirstName, _person.LastName, _person.MiddleName);
            ViewData["PersonName"] = _personname;

            Response.Cookies["PRPersonId"].Value = PersonId.ToString();

            PersonRelation _personrel = new PersonRelation() { PersonId = _person.PersonId };

            db.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s)); //Debug Information

            ViewBag.Relation12 = db.RelationTypesCatalogs
                .Where(c => c.TypeSex.ToString() == _person.Sex.ToString() || c.TypeSex.ToString() == "No_Gender")
                .Select(c => new { c.RelType })
                .Distinct()
                .OrderBy(c => c.RelType);

            return View(_personrel);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, User")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PRCreate([Bind(Include = "Id,PersonId,RelPersonId,Relation12,Relation21")] PersonRelation personRelation)
        {
            db.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s)); //Debug Information

            Person _person = db.Persons.FirstOrDefault(p => p.PersonId == personRelation.PersonId);
            string _personname = String.Format("{0} {1} {2}", _person.FirstName, _person.LastName, _person.MiddleName);
            ViewData["PersonName"] = _personname;

            ViewBag.Relation12 = db.RelationTypesCatalogs
              .Where(c => c.TypeSex.ToString() == _person.Sex.ToString() || c.TypeSex.ToString() == "No_Gender")
              .Select(c => new { c.RelType })
              .Distinct()
              .OrderBy(c => c.RelType);

            if (ModelState.IsValid)
            {
                if (personRelation.RelPersonId != 0)
                {
                    try
                    {
                        db.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s)); //Debug Information=======================

                        int _relcount = db.PersonRelations.Count(p => ((p.PersonId == personRelation.PersonId && p.RelPersonId == personRelation
                            .RelPersonId) || (p.RelPersonId == personRelation.PersonId && p.PersonId == personRelation.RelPersonId)) && ((p.Relation12 == personRelation
                            .Relation12 && p.Relation21 == personRelation.Relation21) || (p.Relation21 == personRelation.Relation12 && p.Relation12 == personRelation.Relation21)));

                        if (_relcount > 0)
                        {
                            string _message = "Указанная комбинация взаимоотношений уже существует для выбранных персон";
                            TempData["MessageError"] = _message;
                            return RedirectToAction("ShowRel", "Home", new { PersonId = personRelation.PersonId });
                        }
                        db.PersonRelations.Add(personRelation);
                        int _result = await db.SaveChangesAsync();
                        if (_result > 0)
                        {
                            string _message = "Информация о взаимоотношении добавлена в базу данных";
                            TempData["MessageOk"] = _message;
                            return RedirectToAction("ShowRel", "Home", new { PersonId = personRelation.PersonId });
                        }
                        else
                        {
                            string _message = "Ошибка создания взаимоотношения";
                            TempData["MessageError"] = _message;
                            return RedirectToAction("ShowRel", "Home", new { PersonId = personRelation.PersonId });
                        }
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
                    ModelState["RelPersonId"].Errors.Clear();
                    ModelState.AddModelError("RelPersonId", "Обязательное поле");
                }
            }

            else
            {
                if (personRelation.RelPersonId == 0)
                {
                    ModelState["RelPersonId"].Errors.Clear();
                    ModelState.AddModelError("RelPersonId", "Обязательное поле");
                }
            }

            return View(personRelation);
        }

        // GET: PersonRelations/Delete/5
        [Authorize(Roles = "Administrator, User")]
        public async Task<ActionResult> PRDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //db.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s));
            PersonRelation personRelation = await db.PersonRelations.FirstOrDefaultAsync(k => k.Id == id);
            if (personRelation == null)
            {
                return HttpNotFound();
            }
            return View(personRelation);
        }

        // POST: PersonRelations/Delete/5
        [HttpPost, ActionName("PRDelete")]
        [Authorize(Roles = "Administrator, User")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PRDeleteConfirmed(int id, int? PersonId)
        {
            if (PersonId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                PersonRelation personRelation = await db.PersonRelations.FirstOrDefaultAsync(k => k.Id == id);
                if (personRelation != null)
                {

                    //Шукаємо в якому полі знаходиться ІД персони. PersonId чи RelPersonId.
                    int _relpersonid = personRelation.RelPersonId;
                    int _returnPerson = personRelation.PersonId;
                    if (PersonId == _relpersonid) _returnPerson = _relpersonid;


                    db.PersonRelations.Remove(personRelation);
                    int _result = await db.SaveChangesAsync();
                    if (_result > 0)
                    {
                        string _message = "Информация о взаимоотношении удалена из базы данных";
                        TempData["MessageOk"] = _message;

                        return RedirectToAction("ShowRel", "Home", new { PersonId = _returnPerson });
                    }
                    else
                    {
                        string _message = "Ошибка удаления";
                        TempData["MessageError"] = _message;

                        return RedirectToAction("ShowRel", "Home", new { PersonId = _returnPerson });
                    }
                }
                else
                {
                    string _message = string.Format("Удаление невозможно. Указанный объект отсутствует в базе данных.");
                    TempData["MessageError"] = _message;
                    return RedirectToAction("ShowRel", "Home", new { PersonId = PersonId });
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErMes = ex.Message;
                ViewBag.ErStack = ex.StackTrace;
                return View("Error");
            }
        }

        #endregion

        #region Довідник типів зв'язків
        public ActionResult RTShow(int? page)
        {

            if (Request.Cookies["PRPersonId"] != null)
            {
                ViewData["PRPersonId"] = Server.HtmlEncode(Request.Cookies["PRPersonId"].Value);
                var c = new HttpCookie("PRPersonId");
                c.Expires = DateTime.Now.AddDays(-1);

                Response.Cookies.Add(c);
            }
            else
            {
                ViewData["PRPersonId"] = "-1";
            }

            int pageSize = 50;
            int pageNumber = (page ?? 1);

            return View(db.RelationTypesCatalogs.OrderBy(p => p.RelType).ToList().ToPagedList(pageNumber, pageSize));
        }

        // GET: RelationTypesCatalogs/Create
        [Authorize(Roles = "Administrator, User")]
        public ActionResult RTCreate()
        {
            return View();
        }

        // POST: RelationTypesCatalogs/Create
        [HttpPost]
        [Authorize(Roles = "Administrator, User")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RTCreate([Bind(Include = "RelTypesId,RelType,TypeSex,ReverseRelType,RevTypeSex")] RelationTypesCatalog relationTypesCatalog)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.RelationTypesCatalogs.Add(relationTypesCatalog);
                    await db.SaveChangesAsync();

                    string _message = "Информация о типе взаимоотношений добавлена в базу данных";
                    TempData["MessageOk"] = _message;

                    return RedirectToAction("RTShow");
                }
                catch (Exception ex)
                {
                    ViewBag.ErMes = ex.Message;
                    ViewBag.ErStack = ex.StackTrace;
                    return View("Error");
                }
            }

            return View(relationTypesCatalog);
        }

        // GET: RelationTypesCatalogs/Edit/5
        [Authorize(Roles = "Administrator, User")]
        public async Task<ActionResult> RTEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RelationTypesCatalog relationTypesCatalog = await db.RelationTypesCatalogs.FindAsync(id);
            if (relationTypesCatalog == null)
            {
                return HttpNotFound();
            }
            return View(relationTypesCatalog);
        }

        // POST: RelationTypesCatalogs/Edit/5
        [HttpPost]
        [Authorize(Roles = "Administrator, User")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RTEdit([Bind(Include = "RelTypesId,RelType,TypeSex,ReverseRelType,RevTypeSex")] RelationTypesCatalog relationTypesCatalog)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(relationTypesCatalog).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    
                    string _message = "Информация о типе взаимоотношений обновлена";
                    TempData["MessageOk"] = _message;
                    
                    return RedirectToAction("RTShow");
                }
                catch (Exception ex)
                {
                    ViewBag.ErMes = ex.Message;
                    ViewBag.ErStack = ex.StackTrace;
                    return View("Error");
                }
            }
            return View(relationTypesCatalog);
        }

        // GET: RelationTypesCatalogs/Delete/5
        [Authorize(Roles = "Administrator, User")]
        public async Task<ActionResult> RTDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RelationTypesCatalog relationTypesCatalog = await db.RelationTypesCatalogs.FindAsync(id);
            if (relationTypesCatalog == null)
            {
                return HttpNotFound();
            }
            return View(relationTypesCatalog);
        }

        // POST: RelationTypesCatalogs/Delete/5
        [HttpPost, ActionName("RTDelete")]
        [Authorize(Roles = "Administrator, User")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RTDeleteConfirmed(int id)
        {
            try
            {
                RelationTypesCatalog relationTypesCatalog = db.RelationTypesCatalogs.FirstOrDefault(x=>x.RelTypesId == id);
                if (relationTypesCatalog != null)
                {
                    db.RelationTypesCatalogs.Remove(relationTypesCatalog);
                    await db.SaveChangesAsync();

                    string _message = "Информация о типе взаимоотношений успешно удалена";
                    TempData["MessageOk"] = _message;
                }
                else
                {
                    string _message = string.Format("Удаление невозможно. Указанный объект отсутствует в базе данных.");
                    TempData["MessageError"] = _message;
                }

                return RedirectToAction("RTShow");
            }
            catch (Exception ex)
            {
                ViewBag.ErMes = ex.Message;
                ViewBag.ErStack = ex.StackTrace;
                return View("Error");
            }
            
        }
        #endregion

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
