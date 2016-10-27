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
    public class HomeController : Controller
    {
        private SunnyModel db = new SunnyModel();

        public ActionResult Index()
        {
            if (User.IsInRole("Administrator"))
            { return RedirectToAction("ShowUsers", new { area = "Administrator", controller = "UsersManagement" }); }

            if (User.IsInRole("User"))
            { return RedirectToAction("ShowPersons", new { area = "", controller = "Home" }); }

            if (User.IsInRole("Blockeduser"))
            { return View("~/Areas/Administrator/Views/UsersManagement/ForbiddenError.cshtml"); }

            return RedirectToAction("Login", new { area = "Administrator", controller = "UsersManagement" });
        }

        #region  Список осіб
        [HttpGet]
        public ActionResult ShowPersons(int? PersonId, string SearchString, int? page, string SortBy, string ReturnUrl)
        {
            //db.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s)); //Debug information------------------------------------

            ViewBag.SortFirstName = SortBy == "FirstName" ? "FirstName desc" : "FirstName";
            ViewBag.SortLastName = SortBy == "LastName" ? "LastName desc" : "LastName";
            ViewBag.SortMiddleName = SortBy == "MiddleName" ? "MiddleName desc" : "MiddleName";
            ViewBag.SortAge = SortBy == "Age" ? "Age desc" : "Age";
            ViewBag.SortNum_Address = SortBy == "Num_Address" ? "Num_Address desc" : "Num_Address";


            var _persons = from person in db.Persons
                           from percomm in db.PersonCommunications.Where(pid => person.PersonId == pid.PersonId).Take(1).DefaultIfEmpty()

                           join comm in db.Communications on percomm.CommunicationId equals comm.Id into tmpcomm
                           from comm in tmpcomm.DefaultIfEmpty()
                           select new
                           {
                               PersonId = person.PersonId,
                               FirstName = person.FirstName,
                               LastName = person.LastName,
                               MiddleName = person.MiddleName,
                               DateOfBirth = person.DateOfBirth,
                               Num_Address = tmpcomm.FirstOrDefault().Address_Number
                           };

            if (!String.IsNullOrEmpty(SearchString))
            {

                _persons = from person in db.Persons
                           from percomm in db.PersonCommunications.Where(pid => person.PersonId == pid.PersonId).Take(1).DefaultIfEmpty()

                           join comm in db.Communications on percomm.CommunicationId equals comm.Id into tmpcomm
                           from comm in tmpcomm.DefaultIfEmpty()
                           where person.FirstName
                             .Contains(SearchString) || person.LastName
                             .Contains(SearchString) || comm.Address_Number.Contains(SearchString)
                           select new
                           {
                               PersonId = person.PersonId,
                               FirstName = person.FirstName,
                               LastName = person.LastName,
                               MiddleName = person.MiddleName,
                               DateOfBirth = person.DateOfBirth,
                               Num_Address = tmpcomm.FirstOrDefault().Address_Number
                           };
            }
            else
            {
                if (PersonId != null)
                {
                    if (db.PersonRelations.Where(p => p.PersonId == PersonId || p.RelPersonId == PersonId).Select(p => p.Id).Count() > 0)
                    {
                        _persons = (from person in _persons
                                    from perrel in db.PersonRelations.Where(pid => person.PersonId == pid.PersonId || person.PersonId == pid.RelPersonId)
                                    where perrel.PersonId == PersonId || perrel.RelPersonId == PersonId
                                    select person).Distinct();
                    }
                    else
                    {
                        _persons = _persons.Where(p => p.PersonId == PersonId);
                    }
                }

            }

            switch (SortBy)
            {
                case "FirstName desc":
                    _persons = _persons.OrderByDescending(x => x.FirstName);
                    ViewData["SortColumn"] = "FirstName";
                    break;
                case "FirstName":
                    _persons = _persons.OrderBy(x => x.FirstName);
                    ViewData["SortColumn"] = "FirstName";
                    break;
                case "LastName desc":
                    _persons = _persons.OrderByDescending(x => x.LastName);
                    ViewData["SortColumn"] = "LastName";
                    break;
                case "LastName":
                    _persons = _persons.OrderBy(x => x.LastName);
                    ViewData["SortColumn"] = "LastName";
                    break;
                case "MiddleName desc":
                    _persons = _persons.OrderByDescending(x => x.MiddleName);
                    ViewData["SortColumn"] = "MiddleName";
                    break;
                case "MiddleName":
                    _persons = _persons.OrderBy(x => x.MiddleName);
                    ViewData["SortColumn"] = "MiddleName";
                    break;
                case "Age desc":
                    _persons = _persons.OrderByDescending(x => x.DateOfBirth);
                    ViewData["SortColumn"] = "Age";
                    break;
                case "Age":
                    _persons = _persons.OrderBy(x => x.DateOfBirth);
                    ViewData["SortColumn"] = "Age";
                    break;
                case "Num_Address desc":
                    _persons = _persons.OrderByDescending(x => x.Num_Address);
                    ViewData["SortColumn"] = "Num_Address";
                    break;
                case "Num_Address":
                    _persons = _persons.OrderBy(x => x.Num_Address);
                    ViewData["SortColumn"] = "Num_Address";
                    break;
                default:
                    //_persons = _persons.OrderBy(x => x.FirstName);
                    break;
            }


            List<PersonsViewModel> _personsviewmodellist = new List<PersonsViewModel>();

            foreach (var item in _persons)
            {
                PersonsViewModel _personsviewmodel = new PersonsViewModel();
                _personsviewmodel.PersonId = item.PersonId;
                _personsviewmodel.FirstName = item.FirstName;
                _personsviewmodel.LastName = item.LastName;
                _personsviewmodel.MiddleName = item.MiddleName;
                _personsviewmodel.DateOfBirth = item.DateOfBirth;
                _personsviewmodel.PersonAge =  AgeMethods.GetAge(item.DateOfBirth);
                _personsviewmodel.PersonMonth = AgeMethods.GetTotalMonth(item.DateOfBirth);
                _personsviewmodel.Num_Address = item.Num_Address;

                _personsviewmodellist.Add(_personsviewmodel);
            }

            if (!string.IsNullOrEmpty(ReturnUrl))
            {
                ViewBag.ReturnUrl = ReturnUrl;
            }

            int pageSize = 50;
            int pageNumber = (page ?? 1);

            return View(_personsviewmodellist.ToPagedList(pageNumber, pageSize));

        }

        #endregion

        #region  Список взаимосвязей
        [HttpGet]
        public ActionResult ShowRel(int? PersonId)
        {
            if (PersonId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {

                db.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s)); //Debug information------------------------------------

                Person Person = db.Persons.FirstOrDefault(p => p.PersonId == PersonId);
                if (Person != null)
                {

                    ViewBag.PersonName = String.Format("{0} {1} {2}", Person.FirstName, Person.LastName, Person.MiddleName);
                    ViewData["PersonId"] = PersonId;

                    List<RelationViewModel> _rellist = new List<RelationViewModel>();

                    var _result = from relation in db.PersonRelations
                                  join person in db.Persons on relation.PersonId equals person.PersonId
                                  where relation.RelPersonId == PersonId
                                  select new
                                  {
                                      Id = relation.Id,
                                      FirstName = person.FirstName,
                                      LastName = person.LastName,
                                      MiddleName = person.MiddleName,
                                      RelationName = relation.Relation12
                                  };

                    foreach (var item in _result)
                    {
                        RelationViewModel _relmodel = new RelationViewModel();
                        _relmodel.RelationId = item.Id;
                        _relmodel.FirstName = item.FirstName;
                        _relmodel.LastName = item.LastName;
                        _relmodel.MiddleName = item.MiddleName;
                        _relmodel.RelationName = item.RelationName;
                        _rellist.Add(_relmodel);
                    }


                    var _resultRev = from relation in db.PersonRelations
                                     join person in db.Persons on relation.RelPersonId equals person.PersonId
                                     where relation.PersonId == PersonId
                                     select new
                                     {
                                         Id = relation.Id,
                                         FirstName = person.FirstName,
                                         LastName = person.LastName,
                                         MiddleName = person.MiddleName,
                                         RelationName = relation.Relation21
                                     };

                    foreach (var item in _resultRev)
                    {
                        RelationViewModel _relmodel = new RelationViewModel();
                        _relmodel.RelationId = item.Id;
                        _relmodel.FirstName = item.FirstName;
                        _relmodel.LastName = item.LastName;
                        _relmodel.MiddleName = item.MiddleName;
                        _relmodel.RelationName = item.RelationName;
                        _rellist.Add(_relmodel);
                    }


                    return View(_rellist);
                }
                else
                {
                    return RedirectToAction("ShowPerson", "Home");
                }
            }
        }

        #endregion

        #region  Список платежів по платнику
        [HttpGet]
        public ActionResult ShowPayments(int? PersonId)
        {
            if (PersonId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            Person Person = db.Persons.FirstOrDefault(p => p.PersonId == PersonId);
            if (Person != null)
            {
                ViewBag.PersonName = String.Format("{0} {1} {2}", Person.FirstName, Person.LastName, Person.MiddleName);
                ViewBag.PersonId = Person.PersonId;
                return View();
            }
            else
            {
                string _message = string.Format("Информация о персоне \"{0} {1} {2} \" отсутствует в базе данных", Person.FirstName, Person.LastName, Person.MiddleName);
                TempData["MessageError"] = _message;

                return RedirectToAction("ShowPersons", "Home");
            }
        }
        #endregion

        #region Список платежів
        [HttpGet]
        public ActionResult ShowAllPayments(string PayerSearchString, string ClientSearchString, int? page, string SortBy)
        {
            // db.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s)); //Debug Information ------------------------------------

            ViewBag.SortPayerPIB = SortBy == "PayerPIB" ? "PayerPIB desc" : "PayerPIB";
            ViewBag.SortPIB = SortBy == "PIB" ? "PIB desc" : "PIB";
            ViewBag.SortEventName = SortBy == "EventName" ? "EventName desc" : "EventName";
            ViewBag.SortSum = SortBy == "Sum" ? "Sum desc" : "Sum";
            ViewBag.SortPayDate = SortBy == "PayDate" ? "PayDate desc" : "PayDate";

            var _result = from payments in db.Payments
                          join payer in db.Persons on payments.PayerId equals payer.PersonId
                          join client in db.Persons on payments.ClientId equals client.PersonId
                          join @event in db.Events on payments.EventId equals @event.EventId
                          select new
                          {
                              Id = @payments.PaymentId,
                              pId = payer.PersonId,
                              pFirstName = payer.FirstName,
                              pLastName = payer.LastName,
                              pMiddleName = payer.MiddleName,
                              cId = client.PersonId,
                              cFirstName = client.FirstName,
                              cLastName = client.LastName,
                              cMiddleName = client.MiddleName,
                              eventName = @event.EventName,
                              Sum = (decimal)payments.Sum,
                              PayDate = payments.Date,
                              Note = payments.Note
                          };


            _result = _result.Where(r => ((r.pFirstName
                .Contains(PayerSearchString) || PayerSearchString == null) || (r.pLastName
                .Contains(PayerSearchString) || PayerSearchString == null)) && ((r.cFirstName
                .Contains(ClientSearchString) || ClientSearchString == null) || (r.cLastName
                .Contains(ClientSearchString) || ClientSearchString == null)));

            switch (SortBy)
            {
                case "PayerPIB desc":
                    _result = _result.OrderByDescending(x => x.pFirstName).ThenByDescending(x => x.pLastName).ThenByDescending(x => x.pMiddleName);
                    ViewData["SortColumn"] = "PayerPIB";
                    break;
                case "PayerPIB":
                    _result = _result.OrderBy(x => x.pFirstName).ThenBy(x => x.pLastName).ThenBy(x => x.pMiddleName);
                    ViewData["SortColumn"] = "PayerPIB";
                    break;
                case "PIB desc":
                    _result = _result.OrderByDescending(x => x.cFirstName).ThenByDescending(x => x.cLastName).ThenByDescending(x => x.cMiddleName);
                    ViewData["SortColumn"] = "PIB";
                    break;
                case "PIB":
                    _result = _result.OrderBy(x => x.cFirstName).ThenBy(x => x.cLastName).ThenBy(x => x.cMiddleName);
                    ViewData["SortColumn"] = "PIB";
                    break;
                case "EventName desc":
                    _result = _result.OrderByDescending(x => x.eventName);
                    ViewData["SortColumn"] = "EventName";
                    break;
                case "EventName":
                    _result = _result.OrderBy(x => x.eventName);
                    ViewData["SortColumn"] = "EventName";
                    break;
                case "Sum desc":
                    _result = _result.OrderByDescending(x => x.Sum);
                    ViewData["SortColumn"] = "Sum";
                    break;
                case "Sum":
                    _result = _result.OrderBy(x => x.Sum);
                    ViewData["SortColumn"] = "Sum";
                    break;
                case "PayDate desc":
                    _result = _result.OrderByDescending(x => x.PayDate);
                    ViewData["SortColumn"] = "PayDate";
                    break;
                case "PayDate":
                    _result = _result.OrderBy(x => x.PayDate);
                    ViewData["SortColumn"] = "PayDate";
                    break;
                default:
                    _result = _result.OrderByDescending(x => x.PayDate);
                    break;
            }

            List<PaymentViewModel> _paylist = new List<PaymentViewModel>();
            foreach (var item in _result)
            {
                PaymentViewModel _payment = new PaymentViewModel
                {
                    Id = item.Id,
                    PayerId = item.pId,
                    ClientId = item.cId,
                    PayerPIB = String.Format("{0} {1} {2}", item.pFirstName, item.pLastName, item.pMiddleName),
                    PIB = String.Format("{0} {1} {2}", item.cFirstName, item.cLastName, item.cMiddleName),
                    EventName = item.eventName,
                    Sum = (double)item.Sum,
                    PayDate = item.PayDate,
                    Note = item.Note
                };
                _paylist.Add(_payment);
            }
            if (_paylist.Count() == 0) { TempData["MessageError"] = "Платежи не найдены"; }


            int pageSize = 50;
            int pageNumber = (page ?? 1);

            return View(_paylist.ToPagedList(pageNumber, pageSize));
        }
        #endregion

        #region Редагування платежу

        // GET: Payments/Edit/5
        public async Task<ActionResult> PaymentEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = await db.Payments.FindAsync(id);
            if (payment == null)
            {
                return HttpNotFound();
            }

            Person _payer = db.Persons.FirstOrDefault(p => p.PersonId == payment.PayerId);
            ViewData["PayerPIB"] = String.Format("{0} {1} {2}", _payer.FirstName, _payer.LastName, _payer.MiddleName);

            Person _client = db.Persons.FirstOrDefault(p => p.PersonId == payment.ClientId);
            ViewData["ClientPIB"] = String.Format("{0} {1} {2}", _client.FirstName, _client.LastName, _client.MiddleName);

            Event _event = db.Events.FirstOrDefault(p => p.EventId == payment.EventId);
            ViewData["EventName"] = String.Format("{0}", _event.EventName);

            return View(payment);
        }

        // POST: Payments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PaymentEdit([Bind(Include = "PaymentId,Sum,Date,Note,PayerId,ClientId,EventId")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(payment).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    TempData["MessageOk"] = "Информация о платеже успешно изменена";

                    return RedirectToAction("ShowAllPayments");
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
                if (payment.PayerId > 0)
                {
                    Person _payer = db.Persons.FirstOrDefault(p => p.PersonId == payment.PayerId);
                    ViewData["PayerPIB"] = String.Format("{0} {1} {2}", _payer.FirstName, _payer.LastName, _payer.MiddleName);
                }
                if (payment.ClientId > 0)
                {
                    Person _client = db.Persons.FirstOrDefault(p => p.PersonId == payment.ClientId);
                    ViewData["ClientPIB"] = String.Format("{0} {1} {2}", _client.FirstName, _client.LastName, _client.MiddleName);
                }

                if (payment.EventId > 0)
                {
                    Event _event = db.Events.FirstOrDefault(p => p.EventId == payment.EventId);
                    ViewData["EventName"] = String.Format("{0}", _event.EventName);
                }

                if (ModelState["Date"].Errors.Count > 0)
                {
                    ModelState["Date"].Errors.Clear();
                    ModelState.AddModelError("Date", "Проверьте правильность ввода даты (ДД/ММ/ГГГГ)");
                }
            }

            return View(payment);
        }

        #endregion

        #region Видалення платежу

        // GET: Payments/Delete/5
        public ActionResult PaymentDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("PaymentDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult PaymentDeleteConfirmed(int id)
        {
            try
            {
                Payment payment = db.Payments.Find(id);
                db.Payments.Remove(payment);
                db.SaveChanges();

                TempData["MessageOk"] = "Информация о платеже успешно удалена";

                return RedirectToAction("ShowAllPayments", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.ErMes = ex.Message;
                ViewBag.ErStack = ex.StackTrace;
                return View("Error");
            }

        }

        #endregion

        #region Створення персони
        [HttpGet]
        public ActionResult AddPerson()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPerson([Bind(Include = "PersonId,FirstName,LastName,MiddleName,Sex,DateOfBirth,Note,CreateDate")] Person model)
        {
            ModelState.Remove("CreateDate");

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.DateOfBirth > DateTime.Now)
                    {
                        string _message = string.Format("Дата рождения не может быть больше текущей даты");
                        TempData["MessageError"] = _message;

                        return RedirectToAction("ShowPersons", "Home");
                    }
                    model.CreateDate = DateTime.Now;
                    db.Persons.Add(model);
                    var result = await db.SaveChangesAsync();
                    if (result > 0)
                    {
                        string _message = string.Format("\"{0} {1} {2} \" добавлен(на) в базу данных", model.FirstName, model.LastName, model.MiddleName);
                        TempData["MessageOk"] = _message;

                        return RedirectToAction("ShowPersons", "Home");
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
                if (ModelState["DateOfBirth"].Errors.Count > 0)
                {
                    ModelState["DateOfBirth"].Errors.Clear();
                    ModelState.AddModelError("DateOfBirth", "Проверьте правильность ввода даты (ДД/ММ/ГГГГ)");
                }
                else
                {
                    //Вывод всех сообщений об ошибках валидации модели
                    foreach (var item in ModelState)
                    {
                        foreach (var error in item.Value.Errors)
                        {
                            TempData["MessageError"] = error.ErrorMessage;
                            return RedirectToAction("ShowPersons", "Home", new { @PersonId = model.PersonId });
                        }

                    }
                }
            }
            return View();

        }
        #endregion

        #region Редагування персони
        public async Task<ActionResult> PersonEdit(int? PersonId)
        {
            if (PersonId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = await db.Persons.FindAsync(PersonId);
            if (person == null)
            {
                return HttpNotFound();
            }
            ViewBag.PersonName = String.Format("{0} {1} {2}", person.FirstName, person.LastName, person.MiddleName);
            return View(person);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PersonEdit([Bind(Include = "PersonId,FirstName,LastName,MiddleName,Sex,DateOfBirth,Note,CreateDate")] Person model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s)); //Debug information------------------------------------

                    db.Entry(model).State = EntityState.Modified;
                    var result = await db.SaveChangesAsync();
                    if (result > 0)
                    {
                        string _message = string.Format("Информация о \"{0} {1} {2} \" успешно обновлена", model.FirstName, model.LastName, model.MiddleName);
                        TempData["MessageOk"] = _message;

                        return RedirectToAction("ShowPersons", "Home", new { PersonId = model.PersonId });
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
                if (ModelState["DateOfBirth"].Errors.Count > 0)
                {
                    ModelState["DateOfBirth"].Errors.Clear();
                    ModelState.AddModelError("DateOfBirth", "Проверьте правильность ввода даты (ДД/ММ/ГГГГ)");
                }
                else
                {
                    //Вывод всех сообщений об ошибках валидации модели
                    foreach (var item in ModelState)
                    {
                        foreach (var error in item.Value.Errors)
                        {
                            TempData["MessageError"] = error.ErrorMessage;
                        }
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region Видалення персони

        public async Task<ActionResult> PersonDelete(int? PersonId)
        {
            if (PersonId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = await db.Persons.FindAsync(PersonId);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        [HttpPost, ActionName("PersonDelete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int PersonId)
        {

            using (var DbContextTransaction = db.Database.BeginTransaction())
            {

                try
                {
                    Person person = await db.Persons.FindAsync(PersonId);

                    if (db.Persons.First(i => i.PersonId == PersonId).Payment.Any() || db.Persons.First(i => i.PersonId == PersonId).Payment1.Any())
                    {
                        string _message = string.Format("Персона \"{0} {1} {2} \" не может быть удалена, так как имеются связанные данные в таблице \"Payment\"", person.FirstName, person.LastName, person.MiddleName);
                        TempData["MessageError"] = _message;

                        return RedirectToAction("ShowPersons", "Home", new { @PersonId = PersonId });
                    }

                    if (db.Persons.First(i => i.PersonId == PersonId).PersonRelation.Any() || db.Persons.First(i => i.PersonId == PersonId).PersonRelation1.Any())
                    {
                        string _message = string.Format("Персона \"{0} {1} {2} \" не может быть удалена, так как имеются связанные данные в таблице \"PersonRelation\"", person.FirstName, person.LastName, person.MiddleName);
                        TempData["MessageError"] = _message;

                        return RedirectToAction("ShowPersons", "Home", new { @PersonId = PersonId });
                    }

                    if (db.Persons.First(i => i.PersonId == PersonId).Event.Any())
                    {
                        string _message = string.Format("Персона \"{0} {1} {2} \" не может быть удалена, так как имеются связанные данные в таблице \"Event\"", person.FirstName, person.LastName, person.MiddleName);
                        TempData["MessageError"] = _message;

                        return RedirectToAction("ShowPersons", "Home", new { @PersonId = PersonId });
                    }

                    else
                    {

                        db.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s)); //Debug Information====================


                        //Шукаємо всі записи в таблиці PersonRelation, що пов'язані з персоною
                        List<PersonCommunication> _percommlist = db.PersonCommunications.Where(p => p.PersonId == PersonId).ToList();

                        //Видаляємо діапазон записів з таблиці PersonRelation, що пов'язані з персоною
                        db.PersonCommunications.RemoveRange(db.PersonCommunications.Where(c => c.PersonId == PersonId));
                        await db.SaveChangesAsync();

                        //Видаляємо записи з таблиці Communication, що були пов'язані с персоною
                        foreach (var item in _percommlist)
                        {
                            Communication communication = await db.Communications.FindAsync(item.CommunicationId);
                            db.Communications.Remove(communication);
                            await db.SaveChangesAsync();
                        }

                        // Видаляємо персону
                        db.Persons.Remove(person);
                        await db.SaveChangesAsync();

                        DbContextTransaction.Commit(); //Commit TRansaction

                        string _message = string.Format("Персона \"{0} {1} {2} \" удалена из базы данных", person.FirstName, person.LastName, person.MiddleName);
                        TempData["MessageOk"] = _message;

                        return RedirectToAction("ShowPersons", "Home");
                    }

                }
                catch (Exception ex)
                {
                    DbContextTransaction.Rollback();
                    ViewBag.ErMes = ex.Message;
                    ViewBag.ErStack = ex.StackTrace;
                    return View("Error");
                }
            }
        }

        #endregion

        #region Створення платежу
        [HttpGet]
        public ActionResult AddPayment()
        {
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPayment(Payment model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Payment _payment = new Payment
                    {
                        PayerId = model.PayerId,
                        ClientId = model.ClientId,
                        EventId = model.EventId,
                        Sum = (decimal)model.Sum,
                        Date = model.Date,
                        Note = model.Note
                    };

                    db.Payments.Add(_payment);
                    var result = await db.SaveChangesAsync();
                    string _message = "Информация о платеже добавлена в базу данных.";
                    TempData["MessageOk"] = _message;

                    return RedirectToAction("ShowAllPayments", "Home");
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
                if (model.PayerId > 0)
                {
                    Person _pers = db.Persons.FirstOrDefault(p => p.PersonId == model.PayerId);
                    ViewData["PayerPIB"] = string.Format("{0} {1} {2}", _pers.FirstName, _pers.LastName, _pers.MiddleName);
                }
                if (model.ClientId > 0)
                {
                    Person _pers = db.Persons.FirstOrDefault(p => p.PersonId == model.ClientId);
                    ViewData["ClientPIB"] = string.Format("{0} {1} {2}", _pers.FirstName, _pers.LastName, _pers.MiddleName);
                }

                if (model.EventId > 0)
                {
                    Event _event = db.Events.FirstOrDefault(p => p.EventId == model.EventId);
                    ViewData["EventName"] = String.Format("{0}", _event.EventName);
                }

                if (ModelState["Date"].Errors.Count > 0)
                {
                    ModelState["Date"].Errors.Clear();
                    ModelState.AddModelError("Date", "Проверьте правильность ввода даты (ДД/ММ/ГГГГ)");
                }
            }
            return View(model);
        }
        #endregion

        #region Список місць особи
        public ActionResult ShowPlaces(int? PersonId)
        {
            if (PersonId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person Person = db.Persons.FirstOrDefault(p => p.PersonId == PersonId);
            if (Person != null)
            {
                ViewBag.PersonName = String.Format("{0} {1} {2}", Person.FirstName, Person.LastName, Person.MiddleName);
                ViewBag.PersonId = Person.PersonId;
                var _result = from places in db.PersonPlaces
                              join person in db.Persons on places.PersonId equals person.PersonId
                              join address in db.Addresses on places.AddressId equals address.AddressId
                              where places.PersonId == PersonId
                              select new
                              {
                                  Id = places.Id,
                                  PlaceName = places.PlaceName,
                                  Alias = address.Alias,
                                  PostCode = address.PostCode,
                                  Country = address.Country,
                                  Region = address.Region,
                                  Area = address.Area,
                                  City = address.City,
                                  AddressValue = address.AddressValue,
                                  GeoTag = address.GeoTag,
                                  Note = address.Note
                              };

                List<PlacesViewModel> _placesList = new List<PlacesViewModel>();
                foreach (var item in _result)
                {
                    PlacesViewModel _place = new PlacesViewModel
                    {
                        Id = item.Id,
                        PlaceName = item.PlaceName,
                        Alias = item.Alias,
                        PostCode = item.PostCode,
                        Country = item.Country,
                        Region = item.Region,
                        Area = item.Area,
                        City = item.City,
                        AddressValue = item.AddressValue,
                        GeoTag = item.GeoTag,
                        Note = item.Note
                    };
                    _placesList.Add(_place);
                }

                return View(_placesList);
            }
            else
            {
                return RedirectToAction("ShowPersons", "Home");
            }
        }
        #endregion

        #region Створення місця особи
        [HttpGet]
        public ActionResult AddPlace(int? ObjectId, int? AddressId)
        {
            if (ObjectId == null) { return RedirectToAction("ShowPersons", "Home"); }
            else
            {
                Person _person = db.Persons.FirstOrDefault(p => p.PersonId == ObjectId);
                if (_person != null)
                {
                    var _cityes = from address in db.Addresses
                                  select new SelectListItem { Text = address.City, Value = address.City };
                    ViewData["Cityes"] = _cityes.Distinct().ToList();


                    ViewBag.PersonName = String.Format("{0} {1} {2}", _person.FirstName, _person.LastName, _person.MiddleName);

                    if (AddressId != null)
                    {
                        Address _address = db.Addresses.FirstOrDefault(a => a.AddressId == AddressId);
                        string _addressString = string.Format("{0} {1}", _address.City, _address.AddressValue);
                        ViewData["AddressText"] = _addressString;

                        if (_address != null)
                        {

                            PersonPlace _place = new PersonPlace() { PersonId = _person.PersonId, AddressId = _address.AddressId };
                            return View(_place);
                        }
                        else
                        {
                            PersonPlace _place = new PersonPlace() { PersonId = _person.PersonId };
                            return View(_place);
                        }
                    }
                    else
                    {
                        PersonPlace _place = new PersonPlace() { PersonId = _person.PersonId };
                        return View(_place);
                    }
                }
                else
                {
                    TempData["MessageError"] = "Персона не найдена";

                    return RedirectToAction("ShowPersons", "Home");
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPlace([Bind(Include = "PersonId, AddressId, PlaceName")] int? ObjectId, PersonPlace model)
        {
            if (ModelState.IsValid)
            {
                if (model.AddressId != 0)
                {
                    try
                    {
                        PersonPlace _place = new PersonPlace
                        {
                            PersonId = model.PersonId,
                            AddressId = model.AddressId,
                            PlaceName = model.PlaceName
                        };

                        db.PersonPlaces.Add(_place);
                        var result = await db.SaveChangesAsync();

                        string _message = "Контактная информация добавлена";
                        TempData["MessageOk"] = _message;

                        return RedirectToAction("ShowPlaces", "Home", new { PersonId = model.PersonId });
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

                    var _cityes = from address in db.Addresses
                                  select new SelectListItem { Text = address.City, Value = address.City };
                    ViewData["Cityes"] = _cityes.Distinct().ToList();

                    ViewData["PlaceName"] = model.PlaceName;
                    ModelState["AddressId"].Errors.Clear();
                    ModelState.AddModelError("AddressId", "Выберите адрес из списка в правой панели");

                    return View(model);

                }
            }
            return View();
        }

        #endregion

        #region Видалення місця особи

        public ActionResult PersonPlaceDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PersonPlace personPlace = db.PersonPlaces.FirstOrDefault(i => i.Id == id);
            if (personPlace == null)
            {
                return HttpNotFound();
            }
            return View(personPlace);
        }

        // POST: PersonPlaces/Delete/5
        [HttpPost, ActionName("PersonPlaceDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, int? PersonId, bool? RelAddressDel)
        {
            if (PersonId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (RelAddressDel == false)
                    {
                        PersonPlace _personPlace = db.PersonPlaces.FirstOrDefault(i => i.Id == id);
                        db.PersonPlaces.Remove(_personPlace);
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                        TempData["MessageOk"] = "Запись успешно удалена из базы данных";
                    }
                    else
                    {
                        PersonPlace _personPlace = db.PersonPlaces.FirstOrDefault(i => i.Id == id);

                        if (db.Addresses
                                .First(a => a.AddressId == _personPlace.AddressId)
                                .PersonPlace.Any(i => i.Id != id) || db.Addresses
                                .First(a => a.AddressId == _personPlace.AddressId)
                                .Exercise.Any())
                        {
                            db.PersonPlaces.Remove(_personPlace);
                            db.SaveChanges();
                            dbContextTransaction.Commit();

                            TempData["MessageOk"] = "Информация о месте персоны удалена из базы данных";
                            TempData["MessageError"] = "Удаление адреса невозможно. В таблице персон или занятий имеются связанные данные";
                        }
                        else
                        {
                            db.PersonPlaces.Remove(_personPlace);
                            db.SaveChanges();

                            Address _delAddress = db.Addresses.First(a => a.AddressId == _personPlace.AddressId);
                            db.Addresses.Remove(_delAddress);
                            db.SaveChanges();

                            TempData["MessageOk"] = "Информация о месте персоны и связанный адрес удалены из базы данных";

                            dbContextTransaction.Commit();
                        }

                    }
                    return RedirectToAction("ShowPlaces", "Home", new { PersonId = PersonId });
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

        #region DISPOSING
        protected override void Dispose(bool disposing)
        {

            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}