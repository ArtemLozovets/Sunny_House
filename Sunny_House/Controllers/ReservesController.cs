﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Sunny_House.Models;
using System.Globalization;
using System.Data.Entity.SqlServer;
using PagedList;
using PagedList.Mvc;
using Sunny_House.Methods;

namespace Sunny_House.Controllers
{
    [Authorize(Roles = "Administrator, User, Presenter")]
    public class ReservesController : Controller
    {
        private SunnyModel db = new SunnyModel();

        #region Блок работы с резервированием =============================================

        // GET: Reserves
        public ActionResult ResShow(int? PersonId)
        {
            if (PersonId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            db.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s)); //Debug information------------------------------------

            Person _person = db.Persons.FirstOrDefault(p => p.PersonId == PersonId);
            ViewData["PersonFIO"] = String.Format("{0} {1} {2}", _person.FirstName, _person.LastName, _person.MiddleName);
            return View();
        }

        // GET: Reserves/Create
        [Authorize(Roles = "Administrator, User")]
        public ActionResult ResCreate(int? PersonId)
        {
            if (PersonId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person _person = db.Persons.FirstOrDefault(p => p.PersonId == PersonId);
            ViewData["PersonFIO"] = string.Format("{0} {1} {2}", _person.FirstName, _person.LastName, _person.MiddleName);
            ViewBag.RoleId = new SelectList(db.PersonRoles, "RoleId", "RoleName");
            return View();
        }

        // POST: Reserves/Create
        [HttpPost]
        [Authorize(Roles = "Administrator, User")]
        [ValidateAntiForgeryToken]
        public ActionResult ResCreate([Bind(Include = "PersonId,RoleId,Note,EventId")] Reserve reserve)
        {
            //Исключаем время бронирования из валидации модели
            ModelState.Remove("ReserveDate");

            if (ModelState.IsValid)
            {
                try
                {
                    db.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s)); //Debug information------------------------------------

                    if (db.Reserves.Where(x => x.PersonId == reserve.PersonId && x.EventId == reserve.EventId).Count() == 0)
                    {
                        reserve.ReserveDate = DateTime.Now;
                        db.Reserves.Add(reserve);
                        db.SaveChanges();
                        TempData["MessageOk"] = "Информация о бронировании успешно добавлена";
                        return RedirectToAction("ResShow", "Reserves", new { PersonId = reserve.PersonId });
                    }
                    else
                    {
                        Person _person = db.Persons.FirstOrDefault(p => p.PersonId == reserve.PersonId);
                        ViewData["PersonFIO"] = string.Format("{0} {1} {2}", _person.FirstName, _person.LastName, _person.MiddleName);
                        ViewData["EventName"] = db.Events.FirstOrDefault(e => e.EventId == reserve.EventId).EventName;
                        TempData["MessageError"] = "Для данной персоны уже было забронировано место!";
                        ViewBag.RoleId = new SelectList(db.PersonRoles, "RoleId", "RoleName", reserve.RoleId);
                        return View(reserve);
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
                Person _person = db.Persons.FirstOrDefault(p => p.PersonId == reserve.PersonId);
                ViewData["PersonFIO"] = string.Format("{0} {1} {2}", _person.FirstName, _person.LastName, _person.MiddleName);
                ViewData["EventName"] = db.Events.FirstOrDefault(e => e.EventId == reserve.EventId).EventName;
                TempData["MessageError"] = "Ошибка валидации модели";
            }

            ViewBag.RoleId = new SelectList(db.PersonRoles, "RoleId", "RoleName", reserve.RoleId);
            return View(reserve);
        }

        // GET: Reserves/Edit/5
        [Authorize(Roles = "Administrator, User")]
        public ActionResult ResEdit(int? ReserveId)
        {
            if (ReserveId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reserve reserve = db.Reserves.First(e => e.ReserveId == ReserveId);
            if (reserve == null)
            {
                return HttpNotFound();
            }

            Person _person = db.Persons.FirstOrDefault(p => p.PersonId == reserve.PersonId);
            ViewData["PersonFIO"] = string.Format("{0} {1} {2}", _person.FirstName, _person.LastName, _person.MiddleName);
            ViewData["EventName"] = reserve.Event.EventName;
            ViewBag.RoleId = new SelectList(db.PersonRoles, "RoleId", "RoleName", reserve.RoleId);
            return View(reserve);
        }

        // POST: Reserves/Edit/5
        [HttpPost]
        [Authorize(Roles = "Administrator, User")]
        [ValidateAntiForgeryToken]
        public ActionResult ResEdit([Bind(Include = "ReserveId,PersonId,RoleId,Note,EventId,ReserveDate")] Reserve reserve)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s)); //Debug information------------------------------------

                    db.Entry(reserve).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["MessageOk"] = "Информация о бронировании успешно изменена";
                    return RedirectToAction("ResShow", new { PersonId = reserve.PersonId });
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
                foreach (var item in ModelState)
                {
                    foreach (var error in item.Value.Errors)
                    {
                        TempData["MessageError"] = error.ErrorMessage;
                    }

                }

                Person _person = db.Persons.FirstOrDefault(p => p.PersonId == reserve.PersonId);
                ViewData["PersonFIO"] = string.Format("{0} {1} {2}", _person.FirstName, _person.LastName, _person.MiddleName);

                ViewData["EventName"] = db.Events.FirstOrDefault(e => e.EventId == reserve.EventId).EventName;
                ViewBag.RoleId = new SelectList(db.PersonRoles, "RoleId", "RoleName", reserve.RoleId);

                reserve = db.Reserves.First(e => e.ReserveId == reserve.ReserveId);
                return View(reserve);

            }


        }

        // GET: Reserves/Delete/5
        [Authorize(Roles = "Administrator, User")]
        public ActionResult ResDelete(int? ReserveId)
        {
            if (ReserveId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reserve reserve = db.Reserves.First(e => e.ReserveId == ReserveId);
            if (reserve == null)
            {
                return HttpNotFound();
            }
            return View(reserve);
        }

        // POST: Reserves/Delete/5
        [HttpPost, ActionName("ResDelete")]
        [Authorize(Roles = "Administrator, User")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int ReserveId)
        {
            try
            {
                Reserve reserve = db.Reserves.FirstOrDefault(e => e.ReserveId == ReserveId);
                if (reserve != null)
                {
                    db.Reserves.Remove(reserve);
                    db.SaveChanges();
                    TempData["MessageOk"] = "Информация о бронировании успешно удалена";
                }
                else
                {
                    string _message = string.Format("Удаление невозможно. Указанный объект отсутствует в базе данных.");
                    TempData["MessageError"] = _message;
                }
                return RedirectToAction("ResShow", new { PersonId = reserve.PersonId });
            }
            catch (Exception ex)
            {
                ViewBag.ErMes = ex.Message;
                ViewBag.ErStack = ex.StackTrace;
                return View("Error");
            }
        }

        public ActionResult ResShowOfEvent(int? EventId)
        {
            if (EventId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Получаем ID роли "Клиент"
            int _clientroleid = db.PersonRoles.First(r => r.RoleName.ToUpper() == (string)"Клиент".ToUpper()).RoleId;

            //Передаем в представление набор параметров для возврата 
            ViewData["ActionName"] = this.ControllerContext.RouteData.Values["action"].ToString();
            ViewData["ControllerName"] = this.ControllerContext.RouteData.Values["controller"].ToString();
            ViewData["ParameterName"] = "EventId";
            ViewData["ParameterValue"] = EventId.ToString();


            Event @event = db.Events.Find(EventId);
            int _allplaces = @event.PlacesCount;
            int _resplaces = db.Reserves.Where(e => e.EventId == EventId && e.RoleId == _clientroleid).Count();
            double _percent = (Convert.ToDouble(_resplaces)) / (Convert.ToDouble(_allplaces));

            ViewData["AllCount"] = _allplaces;
            ViewData["ReservedCount"] = _resplaces;
            ViewBag.RoleId = new SelectList(db.PersonRoles, "RoleId", "RoleName", null);

            //Устанавливаем разделитель дробной части для работы c jQuery
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            ViewData["FillPercent"] = _percent.ToString(nfi);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);

        }

        public ActionResult ResShowAll(string SearchString)
        {
            List<Reserve> _reserves = db.Reserves.ToList();
            return View(_reserves);
        }

        public JsonResult AjaxDelete(int? ReserveId, int? EventId)
        {
            if (ReserveId == null || ReserveId == 0 || EventId == null || EventId == 0)
            {
                return Json(new { Result = false, Message = "Не указан идентификатор бронирования или мероприятия" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                Reserve reserve = db.Reserves.First(e => e.ReserveId == ReserveId);
                db.Reserves.Remove(reserve);
                db.SaveChanges();

                //Получаем ID роли "Клиент"
                int _clientroleid = db.PersonRoles.First(r => r.RoleName.ToUpper() == (string)"Клиент".ToUpper()).RoleId;
                var @event = db.Events.Find(EventId);
                var _allplaces = @event.PlacesCount;
                var _resplaces = db.Reserves.Where(e => e.EventId == EventId && e.RoleId == _clientroleid).Count();
                double _percent = (Convert.ToDouble(_resplaces)) / (Convert.ToDouble(_allplaces));

                return Json(new { Result = true, Message = "Информация о бронировании удалена", percent = _percent, resplaces = _resplaces, allplaces = _allplaces }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { Result = false, Message = "Во время выполнения запроса произошла критическая ошибка" }, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize(Roles = "Administrator, User")]
        public JsonResult AjaxChangeRole(int? ReserveId, int? RoleId, int? EventId)
        {
            if (ReserveId == null || ReserveId == 0 || RoleId == null || RoleId == 0 || EventId == null || EventId == 0)
            {
                return Json(new { Result = false, Message = "Неверные входные параметры" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var reserve = new Reserve() { ReserveId = ReserveId ?? 1, RoleId = RoleId ?? 1 };
                var _res = db.Reserves.FirstOrDefault(r => r.ReserveId == ReserveId);
                _res.RoleId = RoleId ?? 1;

                db.SaveChanges();

                //Получаем ID роли "Клиент"
                int _clientroleid = db.PersonRoles.First(r => r.RoleName.ToUpper() == (string)"Клиент".ToUpper()).RoleId;
                var @event = db.Events.Find(EventId);
                var _allplaces = @event.PlacesCount;
                var _resplaces = db.Reserves.Where(e => e.EventId == EventId && e.RoleId == _clientroleid).Count();
                double _percent = (Convert.ToDouble(_resplaces)) / (Convert.ToDouble(_allplaces));

                return Json(new { Result = true, Message = "Роль изменена", percent = _percent, resplaces = _resplaces, allplaces = _allplaces }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = "Во время выполнения запроса произошла критическая ошибка:" + ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Блок работы с потенциальными клиентами ------------------------------------------

        // GET: PotentialСlient
        public ActionResult ShowPotentialPartial(int? EventId, string PTCSearchString, int? page)
        {
            if (EventId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var potentialClients = (from client in db.PotentialСlients
                                    join role in db.PersonRoles on client.RoleId equals role.RoleId into joinedrole
                                    from _role in joinedrole.DefaultIfEmpty()
                                    join person in db.Persons on client.PersonId equals person.PersonId
                                    where (client.EventId == EventId) &&
                                          (person.FirstName.Contains(PTCSearchString)
                                             || person.LastName.Contains(PTCSearchString)
                                             || _role.RoleName.Contains(PTCSearchString)
                                             || string.IsNullOrEmpty(PTCSearchString)
                                          )
                                    select new
                                    {
                                        PersonId = person.PersonId,
                                        EventId = client.EventId,
                                        ClientId = client.ClientId,
                                        PersonFIO = person.FirstName + " " + person.LastName + " " + person.MiddleName,
                                        RoleName = _role.RoleName,
                                        Infoes = client.Infoes,
                                        DOB = person.DateOfBirth
                                    }).AsEnumerable().Select(c => new PotentialClientsViewModel
                                    {
                                        PersonId = c.PersonId,
                                        EventId = c.EventId,
                                        ClientId = c.ClientId,
                                        PersonFIO = c.PersonFIO.Trim(),
                                        RoleName = c.RoleName,
                                        Infoes = c.Infoes,
                                        PersonAge = AgeMethods.GetAge(c.DOB, false),
                                        PersonMonth = AgeMethods.GetTotalMonth(c.DOB)
                                    }).OrderByDescending(c => c.ClientId);

            ViewData["SearchString"] = PTCSearchString;

            int pageSize = 50;
            int pageNumber = (page ?? 1);

            //Передаем в представление набор параметров для возврата 
            ViewData["ActionName"] = "ResShowOfEvent";
            ViewData["ControllerName"] = "Reserves";
            ViewData["ParameterName"] = "EventId";
            ViewData["ParameterValue"] = EventId;

            return PartialView(potentialClients.ToList().ToPagedList(pageNumber, pageSize));
        }

        public ActionResult ShowModalClient(int? EventId, int? MinAgeYears, int? MaxAgeYears, int? MinAgeMonths, int? MaxAgeMonths, int? RoleId, string PTSearch, int? PTEventId)
        {

            if (EventId == null || MinAgeYears == null || MaxAgeYears == null || MinAgeMonths == null || MaxAgeMonths == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewData["EventId"] = EventId;
            ViewData["MinAgeYears"] = MinAgeYears;
            ViewData["MinAgeMonths"] = MinAgeMonths;
            ViewData["MaxAgeYears"] = MaxAgeYears;
            ViewData["MaxAgeMonths"] = MaxAgeMonths;
            ViewData["PTSearch"] = PTSearch;
            ViewData["PTEventId"] = PTEventId;

            if (RoleId != null) { ViewData["RoleId"] = RoleId; }
            else { ViewData["RoleId"] = "0"; }

            return PartialView();
        }


        public ActionResult PartialClientinModal(int? EventId, int? MinAgeYears, int? MaxAgeYears, int? MinAgeMonths, int? MaxAgeMonths, int? RoleId, int? page, string PTSearch, int? PTEventId)
        {
            if (EventId == null || MinAgeYears == null || MaxAgeYears == null || MinAgeMonths == null || MaxAgeMonths == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            RoleId = RoleId != 0 ? RoleId : null;
            PTSearch = String.IsNullOrEmpty(PTSearch) ? null : PTSearch.Trim();

            db.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s)); //Debug information------------------------------------

            var _persons = (from person in db.Persons
                            join _visit in db.Visits on person.PersonId equals _visit.VisitorId into _vstmp
                            from _visit in _vstmp.DefaultIfEmpty()
                            join reserve in db.Reserves on person.PersonId equals reserve.PersonId into resjoined
                            from pr in resjoined.DefaultIfEmpty()
                            join role in db.PersonRoles on pr.RoleId equals role.RoleId into rolejoined
                            from rl in rolejoined.DefaultIfEmpty()

                            from percomm in db.PersonCommunications.Where(pid => person.PersonId == pid.PersonId).DefaultIfEmpty()
                            join comm in db.Communications on percomm.CommunicationId equals comm.Id into tmpcomm
                            from comm in tmpcomm.DefaultIfEmpty()

                            where (pr.RoleId == RoleId || RoleId == null)
                                    && ((_visit.Exercise.EventId == PTEventId && _visit.FactVisit) || PTEventId == null || PTEventId == 0)
                                    && (SqlFunctions.DateDiff("day", pr.ReserveDate, DateTime.Today) <= 365 || RoleId == null)
                                    && (person.FirstName.Contains(PTSearch)
                                            || person.LastName.Contains(PTSearch)
                                            || person.Note.Contains(PTSearch)
                                            || comm.Address_Number.Contains(PTSearch)
                                            || String.IsNullOrEmpty(PTSearch))
                                    && (!person.Reserve.Any(x => x.PersonId == person.PersonId && x.EventId == EventId))
                                    && (!person.PotentialClient.Any(x => x.PersonId == person.PersonId && x.EventId == EventId))

                            select new
                            {
                                PersonId = person.PersonId,
                                PersonFIO = person.FirstName + " " + person.LastName + " " + person.MiddleName,
                                DateOfBirth = person.DateOfBirth,
                                Note = person.Note
                            }).Distinct().AsEnumerable().Select(e => new PersonsViewModel
                            {
                                PersonId = e.PersonId,
                                PersonFIO = e.PersonFIO.Trim(),
                                PersonAge = AgeMethods.GetAge(e.DateOfBirth, false),
                                PersonMonth = AgeMethods.GetTotalMonth(e.DateOfBirth),
                                DateOfBirth = e.DateOfBirth,
                                Note = e.Note
                            }).Where(a => (((a.PersonAge == MinAgeYears && a.PersonMonth >= MinAgeMonths) || (a.PersonAge > MinAgeYears))
                                                && ((a.PersonAge == MaxAgeYears && a.PersonMonth <= MaxAgeMonths) || (a.PersonAge < MaxAgeYears))
                                                || (a.PersonAge == null || a.PersonMonth == null)));

            if (MinAgeYears > 0 || MinAgeMonths > 0 || MaxAgeYears < 99 || MaxAgeMonths < 11)
            {
                _persons = _persons.Where(a => a.PersonAge != null);
            }

            int pageSize = 50;
            int pageNumber = (page ?? 1);

            ViewData["EventId"] = EventId;
            ViewData["MinAgeYears"] = MinAgeYears;
            ViewData["MinAgeMonths"] = MinAgeMonths;
            ViewData["MaxAgeYears"] = MaxAgeYears;
            ViewData["MaxAgeMonths"] = MaxAgeMonths;
            if (RoleId != null) { ViewData["RoleId"] = RoleId; }
            else { ViewData["RoleId"] = "0"; }

            return PartialView(_persons.ToList().ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = "Administrator, User")]
        public JsonResult AddPotentialClient(int? EventId, int RoleId, int[] PersonsIDS)
        {
            if (EventId == null || PersonsIDS.Length == 0)
            {
                return Json(new { Result = false, Message = "Ошибка валидации модели" }, JsonRequestBehavior.AllowGet);
            }
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (RoleId == 0)
                    {
                        //Получаем ID роли "Клиент"
                        RoleId = db.PersonRoles.First(r => r.RoleName.ToUpper() == (string)"Клиент".ToUpper()).RoleId;
                    }

                    var _EventId = EventId ?? 0;
                    foreach (var item in PersonsIDS)
                    {
                        PotentialСlient _PTC = new PotentialСlient()
                        {
                            EventId = _EventId,
                            PersonId = item,
                            RoleId = RoleId
                        };
                        db.PotentialСlients.Add(_PTC);
                        db.SaveChanges();
                    }

                    dbContextTransaction.Commit();
                    return Json(new { Result = true, Message = "Добавление выполнено успешно" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    string we = ex.Message;
                    dbContextTransaction.Rollback();
                    return Json(new { Result = false, Message = "Ошибка добавления персон" }, JsonRequestBehavior.AllowGet);
                }
            }
        }


        [HttpPost]
        [Authorize(Roles = "Administrator, User")]
        public JsonResult AjaxUpdateInfoes(int? PTCId, string Infoes)
        {
            if (PTCId == null)
            {
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                PotentialСlient _client = db.PotentialСlients.Find(PTCId);
                if (_client == null)
                {
                    return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
                }

                _client.Infoes = Infoes;

                db.Entry(_client).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        [Authorize(Roles = "Administrator, User")]
        public JsonResult AjaxPTCMoveToReserve(int? PTCId)
        {
            if (PTCId == null)
            {
                return Json(new { Result = false, Message = "Ошибка валидации модели" }, JsonRequestBehavior.AllowGet);
            }
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    PotentialСlient _client = db.PotentialСlients.Find(PTCId);
                    if (_client == null)
                    {
                        return Json(new { Result = false, Message = "Потенциальный клиент не найден" }, JsonRequestBehavior.AllowGet);
                    }

                    if (db.Reserves.Where(v => v.PersonId == _client.PersonId && v.EventId == _client.EventId).Count() > 0)
                    {
                        string _pers = db.Persons.Where(x => x.PersonId == _client.PersonId).Select(x => x.FirstName + " " + x.LastName + " " + x.MiddleName).FirstOrDefault();
                        return Json(new { Result = false, Message = String.Format("{0} уже есть в списке забронированных персон", _pers) }, JsonRequestBehavior.AllowGet);
                    }

                    //Получаем ID роли "Клиент"
                    int _clientroleid = db.PersonRoles.First(r => r.RoleName.ToUpper() == (string)"Клиент".ToUpper()).RoleId;

                    Event @event = db.Events.Find(_client.EventId);
                    int _allplaces = @event.PlacesCount;
                    int _resplaces = db.Reserves.Where(e => e.EventId == _client.EventId && e.RoleId == _clientroleid).Count();
                    if (_allplaces - _resplaces == 0 && _client.RoleId == _clientroleid)
                    {
                        return Json(new { Result = false, Message = "Свободные места отсутствуют" }, JsonRequestBehavior.AllowGet);
                    }


                    //Формирование отзыва при нажатии кнопки "Бронь" на странице потенциальных клиентов
                    int sourceID = db.CommentSources.Where(x => x.SourceName == "Бронирование").Select(x => x.SourceId).FirstOrDefault();

                    var _commData = (from comm in db.PotentialСlients
                                     join ev in db.Events on comm.EventId equals ev.EventId
                                     join fio in db.Persons on comm.PersonId equals fio.PersonId
                                     where comm.ClientId == PTCId
                                     select new
                                     {
                                         EventName = ev.EventName,
                                         EventId = ev.EventId,
                                         SignPersonId = comm.PersonId,
                                         InfoesText = comm.Infoes,
                                         PersonFIO = (string)(fio.FirstName + " " + fio.LastName + " " + fio.MiddleName).Trim()
                                     }).FirstOrDefault();

                    if ((sourceID != 0) && (_commData != null))
                    {
                        string CommentText = String.Format("Бронь; {0}; {1}; {2}", _commData.PersonFIO, _commData.EventName, _commData.InfoesText);

                        Comment _comm = new Comment();
                        _comm.Date = DateTime.Now;
                        _comm.Rating = 5;
                        _comm.SourceId = sourceID;
                        _comm.EventId = _commData.EventId;
                        _comm.SignPersonId = _commData.SignPersonId;
                        _comm.Text = CommentText;
                        db.Comments.Add(_comm);
                        db.SaveChanges();
                    }
                    //----------------

                    Reserve _reserve = new Reserve
                    {
                        PersonId = _client.PersonId,
                        EventId = _client.EventId,
                        RoleId = _client.RoleId,
                        ReserveDate = DateTime.Now
                    };

                    db.Reserves.Add(_reserve);
                    db.SaveChanges();

                    db.PotentialСlients.Remove(_client);
                    db.SaveChanges();
                    dbContextTransaction.Commit();

                    @event = db.Events.Find(_client.EventId);
                    _allplaces = @event.PlacesCount;
                    _resplaces = db.Reserves.Where(e => e.EventId == _client.EventId && e.RoleId == _clientroleid).Count();
                    double _percent = (Convert.ToDouble(_resplaces)) / (Convert.ToDouble(_allplaces));

                    return Json(new { Result = true, Message = "Бронирование выполнено успешно", percent = _percent, resplaces = _resplaces, allplaces = _allplaces }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    return Json(new { Result = false, Message = "Ошибка бронирования персоны " + ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost, ActionName("AjaxPTCDelete")]
        [Authorize(Roles = "Administrator, User")]
        public JsonResult AjaxPTCDeleteConfirmed(int? PTCId)
        {
            if (PTCId == null)
            {
                return Json(new { Result = false, Message = "Ошибка валидации модели" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                PotentialСlient _client = db.PotentialСlients.Find(PTCId);
                db.PotentialСlients.Remove(_client);
                db.SaveChanges();
                return Json(new { Result = true, Message = "Удаление выполнено успешно" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Result = false, Message = "Ошибка удаления записи" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "Administrator, User")]
        public ActionResult DeleteAll(int? EventId)
        {

            if (EventId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var Records = db.PotentialСlients.Where(e => e.EventId == EventId);
            db.PotentialСlients.RemoveRange(Records);
            db.SaveChanges();
            return RedirectToAction("ResShowOfEvent", "Reserves", new { @EventId });
        }


        [HttpGet]
        [Authorize(Roles = "Administrator, User")]
        public ActionResult PTCRefusing(int? PersonId, int? ClientId, int? ReserveId, int? EventId, string Mode)
        {
            if (PersonId == null || EventId == null || (ClientId == null && Mode == "Pot") || (ReserveId == null && Mode == "Res"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Получаем ID источника "Бронирование"
            int _sourceid = db.CommentSources.First(s => s.SourceName.ToUpper() == (string)"Бронирование".ToUpper()).SourceId;

            int SignerId = Mode == "Pot" ? ClientId ?? 0 : ReserveId ?? 0;

            //Формируем текст отзыва при отказе из списка потенциальных клиентов
            string CommentText = String.Empty;
            if (Mode == "Pot")
            {
                var _commData = (from comm in db.PotentialСlients
                                 join ev in db.Events on comm.EventId equals ev.EventId
                                 join fio in db.Persons on comm.PersonId equals fio.PersonId
                                 where comm.ClientId == SignerId
                                 select new
                                 {
                                     EventName = ev.EventName,
                                     EventId = ev.EventId,
                                     SignPersonId = comm.PersonId,
                                     InfoesText = comm.Infoes,
                                     PersonFIO = (string)(fio.FirstName + " " + fio.LastName + " " + fio.MiddleName).Trim()
                                 }).FirstOrDefault();
                if (_commData != null)
                {
                    CommentText = String.Format("Отказ; {0}; {1}; {2}", _commData.PersonFIO, _commData.EventName, _commData.InfoesText);
                }
            }

            CommentViewModel _comment = new CommentViewModel();
            _comment.EventId = EventId ?? null;
            _comment.SourceId = _sourceid;
            _comment.Date = DateTime.Now;
            _comment.SourceName = "Бронирование";
            _comment.EventName = db.Events.FirstOrDefault(e => e.EventId == EventId).EventName.ToString();
            _comment.SignPersonId = SignerId;
            _comment.Text = CommentText;

            List<RelationViewModel> _rellist = new List<RelationViewModel>();

            var _person = (from person in db.Persons
                           where (person.PersonId == PersonId)
                           select new RelationViewModel
                           {
                               PersonId = person.PersonId,
                               PersonFIO = person.FirstName + " " + person.LastName + " " + person.MiddleName
                           }).Single();

            _rellist.Add(_person);

            var _result = (from relation in db.PersonRelations
                           join person in db.Persons on relation.PersonId equals person.PersonId
                           where relation.RelPersonId == PersonId
                           select new RelationViewModel
                           {
                               PersonId = person.PersonId,
                               PersonFIO = person.FirstName + " " + person.LastName + " " + person.MiddleName
                           }).ToList();

            _rellist.AddRange(_result);

            var _resultRev = (from relation in db.PersonRelations
                              join person in db.Persons on relation.RelPersonId equals person.PersonId
                              where relation.PersonId == PersonId
                              select new RelationViewModel
                              {
                                  PersonId = person.PersonId,
                                  PersonFIO = person.FirstName + " " + person.LastName + " " + person.MiddleName
                              }).ToList();

            _rellist.AddRange(_resultRev);


            ViewBag.SignPersonId = new SelectList(_rellist, "PersonId", "PersonFIO");


            return View(_comment);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, User")]
        [ValidateAntiForgeryToken]
        public ActionResult PTCRefusing(CommentViewModel model, int? ClientId, int? ReserveId, int? PersonId, string Mode)
        {
            if (PersonId == null || (ClientId == null && Mode == "Pot") || (ReserveId == null && Mode == "Res"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid)
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s)); //Debug information------------------------------------

                        Comment _comment = new Comment
                        {
                            SourceId = model.SourceId,
                            EventId = model.EventId,
                            Date = model.Date,
                            Text = model.Text,
                            SignPersonId = model.SignPersonId,
                            Rating = model.Rating
                        };

                        db.Comments.Add(_comment);
                        db.SaveChanges();


                        if (Mode == "Pot")
                        {
                            PotentialСlient _client = db.PotentialСlients.Find(ClientId);
                            db.PotentialСlients.Remove(_client);
                        }
                        if (Mode == "Res")
                        {
                            Reserve _res = db.Reserves.Find(ReserveId);
                            db.Reserves.Remove(_res);
                        }

                        db.SaveChanges();
                        dbContextTransaction.Commit();
                        TempData["MessageRefusingOk"] = "Операция выполнена успешно";
                        return RedirectToAction("ResShowOfEvent", "Reserves", new { EventId = model.EventId });

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

            TempData["MessageError"] = "Ошибка валидации модели";
            List<RelationViewModel> _rellist = new List<RelationViewModel>();

            var _person = (from person in db.Persons
                           where (person.PersonId == PersonId)
                           select new RelationViewModel
                           {
                               PersonId = person.PersonId,
                               PersonFIO = person.FirstName + " " + person.LastName + " " + person.MiddleName
                           }).Single();

            _rellist.Add(_person);

            var _result = (from relation in db.PersonRelations
                           join person in db.Persons on relation.PersonId equals person.PersonId
                           where relation.RelPersonId == PersonId
                           select new RelationViewModel
                           {
                               PersonId = person.PersonId,
                               PersonFIO = person.FirstName + " " + person.LastName + " " + person.MiddleName
                           }).ToList();

            _rellist.AddRange(_result);

            var _resultRev = (from relation in db.PersonRelations
                              join person in db.Persons on relation.RelPersonId equals person.PersonId
                              where relation.PersonId == PersonId
                              select new RelationViewModel
                              {
                                  PersonId = person.PersonId,
                                  PersonFIO = person.FirstName + " " + person.LastName + " " + person.MiddleName
                              }).ToList();

            _rellist.AddRange(_resultRev);


            ViewBag.SignPersonId = new SelectList(_rellist, "PersonId", "PersonFIO");


            return View(model);
        }


        public ActionResult PTReserve4Person(int? PersonId)
        {
            if (PersonId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            db.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s)); //Debug information------------------------------------

            Person _person = db.Persons.FirstOrDefault(p => p.PersonId == PersonId);
            ViewData["PersonFIO"] = String.Format("{0} {1} {2}", _person.FirstName, _person.LastName, _person.MiddleName);

            var _reserves = (from reserves in db.Reserves
                             join _event in db.Events on reserves.EventId equals _event.EventId
                             join person in db.Persons on reserves.PersonId equals person.PersonId
                             join role in db.PersonRoles on reserves.RoleId equals role.RoleId
                             select new
                             {
                                 ReserveId = reserves.ReserveId,
                                 PersonId = person.PersonId,
                                 EventName = _event.EventName,
                                 EventId = _event.EventId,
                                 RoleName = role.RoleName,
                                 Note = reserves.Note
                             }
                             ).Where(p => p.PersonId == PersonId || PersonId == null);


            List<ReservesViewModel> _resviewModelList = new List<ReservesViewModel>();
            foreach (var item in _reserves)
            {
                ReservesViewModel _reserveViewModel = new ReservesViewModel
                {
                    ReserveId = item.ReserveId,
                    EventName = item.EventName,
                    RoleName = item.RoleName,
                    EventId = item.EventId,
                    Note = item.Note
                };
                _resviewModelList.Add(_reserveViewModel);

            };

            return PartialView(_resviewModelList);
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
