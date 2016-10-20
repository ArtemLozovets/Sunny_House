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
   [Authorize(Roles = "Administrator, User")]
    public class CommunicationsController : Controller
    {
        private SunnyModel db = new SunnyModel();


        #region Засоби зв'язку

        // GET: Communications
        public ActionResult ComShow(int? PersonId)
        {
            if (PersonId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            db.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s)); //Debug Information====================

            Person _person = db.Persons.FirstOrDefault(p => p.PersonId == PersonId);
            ViewBag.PersonName = String.Format("{0} {1} {2}", _person.FirstName, _person.LastName, _person.MiddleName);

            List<CommViewModel> _commlist = new List<CommViewModel>();

            var _result = from personcomm in db.PersonCommunications
                          join comm in db.Communications on personcomm.CommunicationId equals comm.Id
                          join commtype in db.TypeOfCommunications on comm.TypeOfCommunicationId equals commtype.Id
                          where personcomm.PersonId == PersonId
                          select new
                          {
                              Id = comm.Id,
                              Address_Number = comm.Address_Number,
                              CommType = commtype.CommType,
                              CommName = comm.CommName,
                              Note = comm.Note
                          };

            foreach (var item in _result)
            {
                CommViewModel _commodel = new CommViewModel();
                _commodel.Id = item.Id;
                _commodel.Address_Number = item.Address_Number;
                _commodel.TypeOfCommunication = item.CommType;
                _commodel.CommName = item.CommName;
                _commodel.Note = item.Note;
                _commlist.Add(_commodel);
            }

            return View(_commlist);
        }

        // GET: Communications/Create
        public ActionResult ComCreate(int? PersonId)
        {
            if (PersonId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Response.Cookies["CommPersonId"].Value = PersonId.ToString();
            Person _person = db.Persons.FirstOrDefault(p => p.PersonId == PersonId);
            ViewBag.PersonName = String.Format("{0} {1} {2}", _person.FirstName, _person.LastName, _person.MiddleName);

            ViewBag.TypeOfCommunicationId = new SelectList(db.TypeOfCommunications, "Id", "CommType");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ComCreate([Bind(Include = "Id,Address_Number,TypeOfCommunicationId,CommName,Note")] Communication communication, int? PersonId)
        {
            if (PersonId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {

                        db.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s)); //Debug Information====================

                        Person _person = db.Persons.FirstOrDefault(p => p.PersonId == PersonId);
                        ViewBag.PersonName = String.Format("{0} {1} {2}", _person.FirstName, _person.LastName, _person.MiddleName);

                        int _personId = PersonId.GetValueOrDefault(); //Convert int? to int -------------------------------------
                        db.Communications.Add(communication);
                        await db.SaveChangesAsync();
                        PersonCommunication _percomm = new PersonCommunication()
                        {
                            PersonId = _personId,
                            CommunicationId = communication.Id
                        };

                        db.PersonCommunications.Add(_percomm);
                        await db.SaveChangesAsync();

                        dbContextTransaction.Commit();

                        TempData["MessageOK"] = "Информация о средстве связи успешно добавлена!";
                        return RedirectToAction("ComShow", new { PersonId = PersonId });
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    ViewBag.ErMes = ex.Message;
                    ViewBag.ErStack = ex.StackTrace;
                    return View("Error");
                }
            }

            ViewBag.TypeOfCommunicationId = new SelectList(db.TypeOfCommunications, "Id", "CommType", communication.TypeOfCommunicationId);
            return View(communication);
        }

        // GET: Communications/Edit/5
        public async Task<ActionResult> ComEdit(int? id, int? PersonId)
        {
            if (id == null || PersonId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Communication communication = await db.Communications.FindAsync(id);
            if (communication == null)
            {
                return HttpNotFound();
            }

            Person _person = db.Persons.FirstOrDefault(p => p.PersonId == PersonId);
            ViewBag.PersonName = String.Format("{0} {1} {2}", _person.FirstName, _person.LastName, _person.MiddleName);
            ViewBag.TypeOfCommunicationId = new SelectList(db.TypeOfCommunications, "Id", "CommType", communication.TypeOfCommunicationId);
            return View(communication);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ComEdit([Bind(Include = "Id,Address_Number,TypeOfCommunicationId,CommName,Note")] Communication communication, int? PersonId)
        {
            if (PersonId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (ModelState.IsValid)
            {
                db.Entry(communication).State = EntityState.Modified;
                await db.SaveChangesAsync();

                TempData["MessageOK"] = "Информация о средстве связи успешно обновлена!";

                return RedirectToAction("ComShow", new { PersonId = PersonId });
            }

            Person _person = db.Persons.FirstOrDefault(p => p.PersonId == PersonId);
            ViewBag.PersonName = String.Format("{0} {1} {2}", _person.FirstName, _person.LastName, _person.MiddleName);
            ViewBag.TypeOfCommunicationId = new SelectList(db.TypeOfCommunications, "Id", "CommType", communication.TypeOfCommunicationId);
            return View(communication);
        }

        // GET: Communications/Delete/5
        public async Task<ActionResult> ComDelete(int? id, int? PersonId)
        {
            if (id == null || PersonId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Communication communication = await db.Communications.FindAsync(id);
            if (communication == null)
            {
                return HttpNotFound();
            }
            Person _person = db.Persons.FirstOrDefault(p => p.PersonId == PersonId);
            ViewBag.PersonName = String.Format("{0} {1} {2}", _person.FirstName, _person.LastName, _person.MiddleName);
            return View(communication);
        }

        // POST: Communications/Delete/5
        [HttpPost, ActionName("ComDelete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ComDeleteConfirmed(int id, int? PersonId)
        {
            if (PersonId == null || id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s)); //Debug Information====================

                    db.PersonCommunications.RemoveRange(db.PersonCommunications.Where(c => c.CommunicationId == id));
                    await db.SaveChangesAsync();

                    Communication communication = await db.Communications.FindAsync(id);
                    db.Communications.Remove(communication);
                    await db.SaveChangesAsync();

                    dbContextTransaction.Commit();

                    TempData["MessageOK"] = "Информация о средстве связи успешно удалена!";

                    return RedirectToAction("ComShow", new { PersonId = PersonId });
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    ViewBag.ErMes = ex.Message;
                    ViewBag.ErStack = ex.StackTrace;
                    return View("Error");
                }
            }
        }

        #endregion

        #region Види зв'язку

        // GET: TypeOfCommunications
        public async Task<ActionResult> TCShow()
        {
            if (Request.Cookies["CommPersonId"] != null) 
            {
                ViewData["CommPersonId"] = Server.HtmlEncode(Request.Cookies["CommPersonId"].Value);
                var c = new HttpCookie("CommPersonId");
                c.Expires = DateTime.Now.AddDays(-1);
              
                Response.Cookies.Add(c);
            }
            else
            {
                ViewData["CommPersonId"] = "-1";
            }

            return View(await db.TypeOfCommunications.ToListAsync());
        }

        // GET: TypeOfCommunications/Create
        public ActionResult TCCreate()
        {
            return View();
        }

        // POST: TypeOfCommunications/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TCCreate([Bind(Include = "Id,CommType")] TypeOfCommunication typeOfCommunication)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.TypeOfCommunications.Add(typeOfCommunication);
                    await db.SaveChangesAsync();

                    TempData["MessageOk"] = "Запись успешно добавлена";
                    
                    return RedirectToAction("TCShow");
                }
                catch (Exception ex)
                {
                    ViewBag.ErMes = ex.Message;
                    ViewBag.ErStack = ex.StackTrace;
                    return View("Error");
                }

            }
            TempData["MessageError"] = "Ошибка валидации модели";
            return View(typeOfCommunication);

        }

        // GET: TypeOfCommunications/Delete/5
        public async Task<ActionResult> TCDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TypeOfCommunication typeOfCommunication = await db.TypeOfCommunications.FindAsync(id);
            if (typeOfCommunication == null)
            {
                return HttpNotFound();
            }
            return View(typeOfCommunication);
        }

        // POST: TypeOfCommunications/Delete/5
        [HttpPost, ActionName("TCDelete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TCDeleteConfirmed(int id)
        {
            try
            {

                if (db.TypeOfCommunications.First(i => i.Id == id).Communication.Any())
                {
                    string _message = "Данная запись не может быть удалена! В таблице \"Communication\" имеются связанные данные. Удалите связанные данные и повторите попытку.";
                    TempData["MessageError"] = _message;
                    return RedirectToAction("TCShow", "Communications");
                }

                TypeOfCommunication typeOfCommunication = await db.TypeOfCommunications.FindAsync(id);
                db.TypeOfCommunications.Remove(typeOfCommunication);
                await db.SaveChangesAsync();

                TempData["MessageOk"] = "Запись успешно удалена";

                return RedirectToAction("TCShow");
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
