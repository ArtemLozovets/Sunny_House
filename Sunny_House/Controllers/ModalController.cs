using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sunny_House.Models;
using System.Net;
using System.Data.Entity;
using PagedList;
using PagedList.Mvc;

namespace Sunny_House.Controllers
{
    [Authorize(Roles = "Administrator, User, Presenter")]
    public class ModalController : Controller
    {
        private SunnyModel db = new SunnyModel();

        private List<Person> PersonBySearchString(string PersonSearchString)
        {
            var _persons = (from person in db.Persons
                            from percomm in db.PersonCommunications.Where(pid => person.PersonId == pid.PersonId).Take(1).DefaultIfEmpty()

                            join comm in db.Communications on percomm.CommunicationId equals comm.Id into tmpcomm
                            from comm in tmpcomm.DefaultIfEmpty()
                            where person.FirstName
                              .Contains(PersonSearchString) || person.LastName
                              .Contains(PersonSearchString) || comm.Address_Number.Contains(PersonSearchString)
                            select person).OrderBy(p => p.PersonId);

            return _persons.ToList();
        }

        private List<Person> PersonById(int PersonId)
        {
            if (PersonId == 0)
            {
                var _persons = db.Persons;
                return _persons.ToList();
            }
            else
            {

                var _persons = (from person in db.Persons
                                from perrel in db.PersonRelations.Where(pid => person.PersonId == pid.PersonId || person.PersonId == pid.RelPersonId)
                                where perrel.PersonId == PersonId || perrel.RelPersonId == PersonId
                                select person).OrderBy(p => p.PersonId).Distinct();
                return _persons.ToList();
            }
        }


        public ActionResult ShowModalPersons(string field)
        {
            ViewBag.Mode = field;
            return PartialView();
        }

        public ActionResult PersonsPartialList(string PersonSearchString, string field, int? page)
        {
            ViewBag.Mode = field;
            ViewData["PersonSearchString"] = PersonSearchString;

            int pageSize = 50;
            int pageNumber = (page ?? 1);

            if (!String.IsNullOrEmpty(PersonSearchString))
            {
                var _persons = PersonBySearchString(PersonSearchString);
                return PartialView(_persons.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                int PersonId = 0;
                //Если выбираем плательщика - получить ИД клиента
                if (field == "PayerId")
                {
                    if (HttpContext.Request.Cookies["ClientId"] != null)
                    {
                        PersonId = Convert.ToInt16(Server.HtmlEncode(HttpContext.Request.Cookies["ClientId"].Value));
                    }
                }
                //Если выбираем клиента - получить ИД плательщика
                if (field == "ClientId")
                {
                    if (HttpContext.Request.Cookies["PayerId"] != null)
                    {
                        PersonId = Convert.ToInt16(Server.HtmlEncode(HttpContext.Request.Cookies["PayerId"].Value));
                    }
                }

                // Удаляем cookies
                if (HttpContext.Request.Cookies["PayerId"] != null)
                {
                    var c = new HttpCookie("PayerId");
                    c.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(c);
                }

                if (HttpContext.Request.Cookies["ClientId"] != null)
                {
                    var c = new HttpCookie("ClientId");
                    c.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(c);
                }

                var _persons = PersonById(PersonId);
                return PartialView(_persons.ToPagedList(pageNumber, pageSize));
            }

        }

        [Authorize(Roles = "Administrator, User")]
        public ActionResult PaymentsPersonAsPayer(int? PersonId)
        {
            if (PersonId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                var _result = from payments in db.Payments
                              join person in db.Persons on payments.ClientId equals person.PersonId
                              join @event in db.Events on payments.EventId equals @event.EventId
                              where payments.PayerId == PersonId
                              select new
                              {
                                  FirstName = person.FirstName,
                                  LastName = person.LastName,
                                  MiddleName = person.MiddleName,
                                  eventName = @event.EventName,
                                  Sum = (double)payments.Sum,
                                  PayDate = payments.Date,
                                  Note = payments.Note
                              };

                List<PaymentViewModel> _paylist = new List<PaymentViewModel>();
                foreach (var item in _result)
                {
                    PaymentViewModel _paymodel = new PaymentViewModel();
                    _paymodel.PIB = String.Format("{0} {1} {2}", item.FirstName, item.LastName, item.MiddleName);
                    _paymodel.EventName = item.eventName;
                    _paymodel.Sum = item.Sum;
                    _paymodel.PayDate = item.PayDate;
                    _paymodel.Note = item.Note;
                    _paylist.Add(_paymodel);
                }

                return PartialView(_paylist.OrderByDescending(p => p.PayDate));
            }
        }


        [Authorize(Roles = "Administrator, User")]
        public ActionResult PaymentsPersonAsClient(int? PersonId)
        {
            if (PersonId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                var _result = from payments in db.Payments
                              join person in db.Persons on payments.PayerId equals person.PersonId
                              join @event in db.Events on payments.EventId equals @event.EventId
                              where payments.ClientId == PersonId
                              select new
                              {
                                  FirstName = person.FirstName,
                                  LastName = person.LastName,
                                  MiddleName = person.MiddleName,
                                  eventName = @event.EventName,
                                  Sum = (double)payments.Sum,
                                  PayDate = payments.Date,
                                  Note = payments.Note
                              };

                List<PaymentViewModel> _paylist = new List<PaymentViewModel>();
                foreach (var item in _result)
                {
                    PaymentViewModel _paymodel = new PaymentViewModel();
                    _paymodel.PIB = String.Format("{0} {1} {2}", item.FirstName, item.LastName, item.MiddleName);
                    _paymodel.Sum = item.Sum;
                    _paymodel.EventName = item.eventName;
                    _paymodel.PayDate = item.PayDate;
                    _paymodel.Note = item.Note;
                    _paylist.Add(_paymodel);
                }

                return PartialView(_paylist.OrderByDescending(p => p.PayDate));
            }
        }

        public ActionResult FillReverseRelTypeDDL(string RelType, int RelPersonId)
        {
            db.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s)); //Debug Information
            if (RelPersonId != 0)
            {
                string _relpersonsex = db.Persons.FirstOrDefault(p => p.PersonId == RelPersonId).Sex.ToString();
                var ReverseRelTypes = db.RelationTypesCatalogs.Where(c => c.RelType
                    .Contains(RelType) && (c.RevTypeSex.ToString() == _relpersonsex || c.RevTypeSex.ToString() == "No_Gender"))
                    .OrderBy(c => c.ReverseRelType).Select(c => c.ReverseRelType);
                return Json(ReverseRelTypes, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var ReverseRelTypes = db.RelationTypesCatalogs.Where(c => c.RelType.Contains(RelType)).OrderBy(c => c.ReverseRelType).Select(c => c.ReverseRelType);
                return Json(ReverseRelTypes, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult ShowModalEvents(string field)
        {
            ViewBag.Mode = field;
            return PartialView();
        }

        [HttpGet]
        public ActionResult EventsPartialList(int? page)
        {
            var _events = (from events in db.Events select events).ToList();

            int pageSize = 50;
            int pageNumber = (page ?? 1);

            return PartialView(_events.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult EventsPartialList(string EventsSearchString, DateTime? StartTimeS, DateTime? EndTimeS, int? page)
        {

            ViewData["ExerciseSearchString"] = EventsSearchString;
            ViewData["StartTimeS"] = StartTimeS;
            ViewData["EndTimeS"] = EndTimeS;

            var _events = (from events in db.Events
                           where (events.EventName.ToUpper().Contains(EventsSearchString.ToUpper()) || String.IsNullOrEmpty(EventsSearchString)) &&
                           ((events.StartTime >= StartTimeS || StartTimeS == null) && (events.EndTime <= EndTimeS || EndTimeS == null))
                           select events).ToList();

            int pageSize = 50;
            int pageNumber = (page ?? 1);

            return PartialView(_events.ToPagedList(pageNumber, pageSize));

        }


        public ActionResult ShowModalAddresses(string RetActionName, string RetControllerName)
        {
            var _cityes = from address in db.Addresses
                          select new SelectListItem { Text = address.City, Value = address.City };
            ViewData["Cityes"] = _cityes.OrderBy(c => c.Value).Distinct().ToList();
            ViewData["RetActionName"] = RetActionName;
            ViewData["RetControllerName"] = RetControllerName;

            return PartialView();
        }

        public ActionResult AddressesPartialView(string CitySearchString, string AddressSearchString)
        {
            List<Address> _addresses = db.Addresses.ToList();

            _addresses = _addresses.Where(c => (c.City
                        .Contains(CitySearchString) || CitySearchString == null) && ((c.AddressValue.ToUpper()
                        .Contains(AddressSearchString.ToUpper()) || AddressSearchString == null) || (c.Alias.ToUpper()
                        .Contains(AddressSearchString.ToUpper()) || AddressSearchString == null)))
                        .ToList();

            return PartialView(_addresses);

        }

        public ActionResult ShowModalEvents2Res()
        {
            return PartialView();
        }

        public ActionResult Events2ResPartialList(string EventsSearchString)
        {
            //Получаем ID роли "Клиент"
            int _clientroleid = db.PersonRoles.First(r => r.RoleName.ToUpper() == (string)"Клиент".ToUpper()).RoleId;

            var _events = (from _event in db.Events
                           where _event.EndTime >= DateTime.Today
                           select new
                           {
                               EventId = _event.EventId,
                               EventName = _event.EventName,
                               StartTime = _event.StartTime,
                               EndTime = _event.EndTime,
                               AllPlaces = _event.PlacesCount,
                               FreePLaces = _event.PlacesCount - (from res1 in db.Reserves
                                                                  where res1.EventId == _event.EventId && res1.RoleId == _clientroleid
                                                                  select res1.EventId).Count()
                           });

            if (!String.IsNullOrEmpty(EventsSearchString))
            {
                _events = _events.Where(e => e.EventName.Contains(EventsSearchString));
            }

            List<Events2ResViewModel> _modelList = new List<Events2ResViewModel>();

            foreach (var item in _events)
            {
                Events2ResViewModel _model = new Events2ResViewModel
                {
                    EventId = item.EventId,
                    EventName = item.EventName,
                    StartTime = item.StartTime,
                    EndTime = item.EndTime,
                    AllPlaces = item.AllPlaces,
                    FreePlaces = item.FreePLaces
                };
                _modelList.Add(_model);

            }

            return PartialView(_modelList);
        }


        public ActionResult ShowModalExercises(string field)
        {
            ViewBag.Mode = field;
            return PartialView();
        }

        [HttpGet]
        public ActionResult ExercisesPartialList(string field, int? page)
        {
            ViewBag.Mode = field;
            var _result = db.Exercises.ToList();


            int pageSize = 50;
            int pageNumber = (page ?? 1);

            return PartialView(_result.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult ExercisesPartialList(string field, string ExerciseSearchString, DateTime? StartTimeS, DateTime? EndTimeS, int? page)
        {
            ViewBag.Mode = field;

            ViewData["ExerciseSearchString"] = ExerciseSearchString;
            ViewData["StartTimeS"] = StartTimeS;
            ViewData["EndTimeS"] = EndTimeS;

            var _ex = (from ex in db.Exercises
                       where (ex.Subject.ToUpper().Contains(ExerciseSearchString.ToUpper()) || String.IsNullOrEmpty(ExerciseSearchString)) &&
                       ((ex.StartTime >= StartTimeS || StartTimeS == null) && (ex.EndTime <= EndTimeS || EndTimeS == null))
                       select ex).ToList();

            int pageSize = 50;
            int pageNumber = (page ?? 1);

            return PartialView(_ex.ToPagedList(pageNumber, pageSize));
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