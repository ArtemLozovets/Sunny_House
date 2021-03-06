﻿using System;
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
using System.Globalization;
using System.Data.Entity.SqlServer;
using Sunny_House.Methods;

namespace Sunny_House.Controllers
{
    [Authorize(Roles = "Administrator, User, Presenter")]
    public class EventsController : Controller
    {
        private SunnyModel db = new SunnyModel();

        [HttpGet]
        public ActionResult EShow(string FilterMode, string SearchString, string SearchStartDate, string SearchEndDate, int? page, string SortBy)
        {
            db.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s)); //Debug Ingormation --------------------------

            ViewBag.SortEventName = SortBy == "EventName" ? "EventName desc" : "EventName";
            ViewBag.SortStartTime = SortBy == "StartTime" ? "StartTime desc" : "StartTime";
            ViewBag.SortEndTime = SortBy == "EndTime" ? "EndTime desc" : "EndTime";
            ViewBag.SortDescription = SortBy == "Description" ? "Description desc" : "Description";
            ViewBag.SortAdministrator = SortBy == "Administrator" ? "Administrator desc" : "Administrator";
            ViewBag.SortPlacesCount = SortBy == "PlacesCount" ? "PlacesCount desc" : "PlacesCount";

            DateTime? _startDate = (String.IsNullOrEmpty(SearchStartDate)) ? Convert.ToDateTime("1900-01-01") : Convert.ToDateTime(SearchStartDate);
            DateTime? _endDate = (String.IsNullOrEmpty(SearchEndDate)) ? Convert.ToDateTime("2900-01-01") : Convert.ToDateTime(SearchEndDate);

            //Получаем ID роли "Клиент"
            int _clientroleid = db.PersonRoles.First(r => r.RoleName.ToUpper() == (string)"Клиент".ToUpper()).RoleId;

            var _events = (from _event in db.Events
                           join person in db.Persons on _event.AdministratorId equals person.PersonId
                           where (_endDate >= _event.StartTime && _startDate <= _event.EndTime) &&
                           ((_event.EventName.Contains(SearchString) || string.IsNullOrEmpty(SearchString)) ||
                            (person.FirstName.Contains(SearchString) || string.IsNullOrEmpty(SearchString)) ||
                            (person.LastName.Contains(SearchString) || string.IsNullOrEmpty(SearchString)) ||
                            (_event.Description.Contains(SearchString) || string.IsNullOrEmpty(SearchString)) ||
                            (_event.Note.Contains(SearchString) || string.IsNullOrEmpty(SearchString)))
                           select new
                           {
                               EventId = _event.EventId,
                               EventName = _event.EventName,
                               StartTime = _event.StartTime,
                               EndTime = _event.EndTime,
                               Description = _event.Description,
                               AdministratorId = _event.AdministratorId,
                               FirstName = person.FirstName,
                               LastName = person.LastName,
                               MiddleName = person.MiddleName,
                               Note = _event.Note,
                               PlacesCount = _event.PlacesCount,
                               Percent = ((from res1 in db.Reserves
                                           where (res1.EventId == _event.EventId) && (res1.RoleId == _clientroleid)
                                           select res1.EventId).Count() * 100) / _event.PlacesCount,
                               ResCount = (from res1 in db.Reserves
                                           where (res1.EventId == _event.EventId) && (res1.RoleId == _clientroleid)
                                           select res1.EventId).Count()
                           })
                           .AsEnumerable().Select(e => new Event
                           {
                               EventId = e.EventId,
                               EventName = e.EventName,
                               StartTime = e.StartTime,
                               EndTime = e.EndTime,
                               Description = e.Description,
                               AdministratorId = e.AdministratorId,
                               FirstName = e.FirstName,
                               LastName = e.LastName,
                               MiddleName = e.MiddleName,
                               Note = e.Note,
                               PlacesCount = e.PlacesCount,
                               Percent = e.Percent,
                               ResCount = e.ResCount
                           });

            switch (FilterMode)
            {
                case "All":
                    _events = _events.OrderByDescending(e => e.StartTime);
                    break;

                case "Current":
                    _events = _events.Where(e => e.EndTime >= DateTime.Today).OrderBy(e => e.StartTime);
                    break;

                case "Archive":
                    _events = _events.Where(e => e.EndTime < DateTime.Today).OrderByDescending(e => e.StartTime);
                    break;

                default:
                    _events = _events.Where(e => e.EndTime >= DateTime.Today).OrderBy(e => e.StartTime);
                    break;
            }

            switch (SortBy)
            {
                case "EventName desc":
                    _events = _events.OrderByDescending(x => x.EventName);
                    ViewData["SortColumn"] = "EventName";
                    break;
                case "EventName":
                    _events = _events.OrderBy(x => x.EventName);
                    ViewData["SortColumn"] = "EventName";
                    break;
                case "StartTime desc":
                    _events = _events.OrderByDescending(x => x.StartTime);
                    ViewData["SortColumn"] = "StartTime";
                    break;
                case "StartTime":
                    _events = _events.OrderBy(x => x.StartTime);
                    ViewData["SortColumn"] = "StartTime";
                    break;
                case "EndTime desc":
                    _events = _events.OrderByDescending(x => x.EndTime);
                    ViewData["SortColumn"] = "EndTime";
                    break;
                case "EndTime":
                    _events = _events.OrderBy(x => x.EndTime);
                    ViewData["SortColumn"] = "EndTime";
                    break;
                case "Description desc":
                    _events = _events.OrderByDescending(x => x.Description);
                    ViewData["SortColumn"] = "Description";
                    break;
                case "Description":
                    _events = _events.OrderBy(x => x.Description);
                    ViewData["SortColumn"] = "Description";
                    break;
                case "Administrator desc":
                    _events = _events.OrderByDescending(x => x.FirstName).ThenByDescending(x => x.LastName).ThenByDescending(x => x.MiddleName);
                    ViewData["SortColumn"] = "Administrator";
                    break;
                case "Administrator":
                    _events = _events.OrderBy(x => x.FirstName).ThenBy(x => x.LastName).ThenBy(x => x.MiddleName);
                    ViewData["SortColumn"] = "Administrator";
                    break;
                case "PlacesCount desc":
                    _events = _events.OrderByDescending(x => x.PlacesCount);
                    ViewData["SortColumn"] = "PlacesCount";
                    break;
                case "PlacesCount":
                    _events = _events.OrderBy(x => x.PlacesCount);
                    ViewData["SortColumn"] = "PlacesCount";
                    break;
                default:
                    //_persons = _persons.OrderBy(x => x.FirstName);
                    break;
            }

            //Передаем в представление набор параметров для возврата 
            ViewData["ActionName"] = this.ControllerContext.RouteData.Values["action"].ToString();
            ViewData["ControllerName"] = this.ControllerContext.RouteData.Values["controller"].ToString();

            int pageSize = 50;
            int pageNumber = (page ?? 1);

            return View(_events.ToList().ToPagedList(pageNumber, pageSize));
        }

        #region Створення заходу
        // GET: Events/Create
        [Authorize(Roles = "Administrator, User")]
        [HttpGet]
        public ActionResult ECreate()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Administrator, User")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ECreate(Event model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Если дата начала мероприятия больше даты окончания
                    if (model.EndTime < model.StartTime)
                    {
                        var _person = db.Persons.FirstOrDefault(p => p.PersonId == model.AdministratorId);
                        if (_person != null)
                        {
                            string _administartorPIB = string.Format("{0} {1} {2}", _person.FirstName, _person.LastName, _person.MiddleName);
                            ViewData["AdministratorPIB"] = _administartorPIB;
                        }
                        TempData["MessageError"] = "Дата начала мероприятия не может быть больше даты окончания";
                        return View(model);
                    }

                    db.Events.Add(model);
                    var _result = await db.SaveChangesAsync();
                    if (_result > 0)
                    {
                        string _message = "Информация о мероприятии добавлена в базу данных";
                        TempData["MessageOk"] = _message;
                        return RedirectToAction("EShow", "Events");

                    }
                    return View();
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
                if (model.AdministratorId > 0)
                {
                    Person _pers = db.Persons.FirstOrDefault(p => p.PersonId == model.AdministratorId);
                    ViewData["AdministratorPIB"] = string.Format("{0} {1} {2}", _pers.FirstName, _pers.LastName, _pers.MiddleName);
                }

                if (ModelState["StartTime"].Errors.Count > 0)
                {
                    ModelState["StartTime"].Errors.Clear();
                    ModelState.AddModelError("StartTime", "Проверьте правильность ввода даты (ДД/ММ/ГГГГ)");
                }

                if (ModelState["EndTime"].Errors.Count > 0)
                {
                    ModelState["EndTime"].Errors.Clear();
                    ModelState.AddModelError("EndTime", "Проверьте правильность ввода даты (ДД/ММ/ГГГГ)");
                }
                return View(model);
            }
        }

        #endregion

        // GET: Events/Edit/5
        [Authorize(Roles = "Administrator, User")]
        public async Task<ActionResult> EEdit(int? EventId, string ReturnParam)
        {
            if (EventId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = await db.Events.FindAsync(EventId);
            if (@event == null)
            {
                return HttpNotFound();
            }

            ViewData["ReturnParam"] = string.IsNullOrEmpty(ReturnParam) ? "/Events/EShow" : ReturnParam;

            //Получаем ID роли "Клиент"
            int _clientroleid = db.PersonRoles.First(r => r.RoleName.ToUpper() == (string)"Клиент".ToUpper()).RoleId;

            ViewData["ReservedCount"] = db.Reserves.Where(e => e.EventId == @event.EventId && e.RoleId == _clientroleid).Count();

            Person _person = db.Persons.FirstOrDefault(p => p.PersonId == @event.AdministratorId);
            ViewData["AdministratorPIB"] = String.Format("{0} {1} {2}", _person.FirstName, _person.LastName, _person.MiddleName);
            return View(@event);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [Authorize(Roles = "Administrator, User")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EEdit([Bind(Include = "EventId,EventName,StartTime,EndTime,Description,AdministratorId,Note,PlacesCount")] Event @event, string ReturnParam)
        {
            if (ModelState.IsValid)
            {
                //Получаем ID роли "Клиент"
                int _clientroleid = db.PersonRoles.First(r => r.RoleName.ToUpper() == (string)"Клиент".ToUpper()).RoleId;

                ViewData["ReturnParam"] = string.IsNullOrEmpty(ReturnParam) ? "/Events/EShow" : ReturnParam;
                ReturnParam = string.IsNullOrEmpty(ReturnParam) ? "/Events/EShow" : ReturnParam;


                try
                {
                    db.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s)); //Debug Ingormation --------------------------

                    // Если дата начала мероприятия больше даты окончания
                    if (@event.EndTime < @event.StartTime)
                    {
                        var _person = db.Persons.FirstOrDefault(p => p.PersonId == @event.AdministratorId);
                        if (_person != null)
                        {

                            ViewData["ReservedCount"] = db.Reserves.Where(e => e.EventId == @event.EventId && e.RoleId == _clientroleid).Count();

                            string _administartorPIB = string.Format("{0} {1} {2}", _person.FirstName, _person.LastName, _person.MiddleName);
                            ViewData["AdministratorPIB"] = _administartorPIB;
                        }
                        TempData["MessageError"] = "Дата начала мероприятия не может быть больше даты окончания";
                        return View(@event);
                    }


                    if (db.Reserves.Where(e => e.EventId == @event.EventId && e.RoleId == _clientroleid).Count() > @event.PlacesCount)
                    {
                        var _person = db.Persons.FirstOrDefault(p => p.PersonId == @event.AdministratorId);
                        if (_person != null)
                        {
                            ViewData["ReservedCount"] = db.Reserves.Where(e => e.EventId == @event.EventId && e.RoleId == _clientroleid).Count();
                            string _administartorPIB = string.Format("{0} {1} {2}", _person.FirstName, _person.LastName, _person.MiddleName);
                            ViewData["AdministratorPIB"] = _administartorPIB;
                        }
                        TempData["MessageError"] = "Введенное количество мест меньше количества забронированных мест";
                        return View(@event);
                    }

                    db.Entry(@event).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    string _message = "Информация о мероприятии успешно обновлена";
                    TempData["MessageOk"] = _message;

                    return Redirect(ReturnParam);
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
                if (@event.AdministratorId > 0)
                {
                    Person _pers = db.Persons.FirstOrDefault(p => p.PersonId == @event.AdministratorId);
                    ViewData["AdministratorPIB"] = string.Format("{0} {1} {2}", _pers.FirstName, _pers.LastName, _pers.MiddleName);
                }

                if (ModelState["StartTime"].Errors.Count > 0)
                {
                    ModelState["StartTime"].Errors.Clear();
                    ModelState.AddModelError("StartTime", "Проверьте правильность ввода даты (ДД/ММ/ГГГГ)");
                }

                if (ModelState["EndTime"].Errors.Count > 0)
                {
                    ModelState["EndTime"].Errors.Clear();
                    ModelState.AddModelError("EndTime", "Проверьте правильность ввода даты (ДД/ММ/ГГГГ)");
                }
                return View(@event);
            }
        }

        // GET: Events/Delete/5
        [Authorize(Roles = "Administrator, User")]
        public async Task<ActionResult> EDelete(int? EventId, string ReturnParam)
        {
            if (EventId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = await db.Events.FindAsync(EventId);
            if (@event == null)
            {
                return HttpNotFound();
            }

            ViewData["ReturnParam"] = string.IsNullOrEmpty(ReturnParam) ? "/Events/EShow" : ReturnParam;

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("EDelete")]
        [Authorize(Roles = "Administrator, User")]
        [ValidateAntiForgeryToken]
        public ActionResult EDeleteConfirmed(int EventId, string ReturnParam)
        {
            try
            {
                string _message = string.Empty;
                ViewData["ReturnParam"] = string.IsNullOrEmpty(ReturnParam) ? "/Events/EShow" : ReturnParam;
                ReturnParam = string.IsNullOrEmpty(ReturnParam) ? "/Events/EShow" : ReturnParam;

                if (db.Events.FirstOrDefault(e => e.EventId == EventId) != null)
                {
                    if (db.Events.First(i => i.EventId == EventId).Exercise.Any())
                    {
                        _message = string.Format("Информация о мероприятии не может быть удалена. В таблице занятий есть связанные данные.");
                        TempData["MessageError"] = _message;

                        return Redirect(ReturnParam);
                    }

                    if (db.Events.First(i => i.EventId == EventId).Reserve.Any())
                    {
                        _message = string.Format("Информация о мероприятии не может быть удалена. В таблице бронирований есть связанные данные.");
                        TempData["MessageError"] = _message;

                        return Redirect(ReturnParam);
                    }

                    if (db.Events.First(i => i.EventId == EventId).Comment.Any())
                    {
                        _message = string.Format("Информация о мероприятии не может быть удалена. В таблице отзывов есть связанные данные.");
                        TempData["MessageError"] = _message;

                        return Redirect(ReturnParam);
                    }

                    if (db.Events.First(i => i.EventId == EventId).Payment.Any())
                    {
                        _message = string.Format("Информация о мероприятии не может быть удалена. В таблице платежей есть связанные данные.");
                        TempData["MessageError"] = _message;

                        return Redirect(ReturnParam);
                    }

                    Event @event = db.Events.FirstOrDefault(x => x.EventId == EventId);
                    db.Events.Remove(@event);
                    db.SaveChanges();
                    _message = "Информация о мероприятии успешно удалена";
                    TempData["MessageOk"] = _message;
                }
                else
                {
                    _message = string.Format("Информация о мероприятии не может быть удалена. Указанный объект отсутствует в базе данных.");
                    TempData["MessageError"] = _message;
                }

                return RedirectToAction("EShow");
            }
            catch (Exception ex)
            {
                ViewBag.ErMes = ex.Message;
                ViewBag.ErStack = ex.StackTrace;
                return View("Error");
            }
        }


        public ActionResult ResRelPersonsPartial(int? EventId, string SearchString, int? page)
        {
            if (EventId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var _persons = (from person in db.Persons
                            join reserve in db.Reserves on person.PersonId equals reserve.PersonId
                            join role in db.PersonRoles on reserve.RoleId equals role.RoleId
                            where (reserve.EventId == EventId)
                            && (person.FirstName.Contains(SearchString)
                                || person.LastName.Contains(SearchString)
                                || person.PersonCommunication.Select(ss => ss.Communication).Any(zz => zz.Address_Number.Contains(SearchString)
                                || String.IsNullOrEmpty(SearchString)))
                            select new
                            {
                                ReserveId = reserve.ReserveId,
                                PersonId = person.PersonId,
                                PersonFIO = person.FirstName + " " + person.LastName + " " + person.MiddleName,
                                Address_Num = person.PersonCommunication.Select(ss => ss.Communication.Address_Number).ToList(),
                                PersonRole = role.RoleName,
                                RoleId = role.RoleId,
                                DOB = person.DateOfBirth
                            }).OrderByDescending(r => r.ReserveId).AsEnumerable().Select(e => new PersonReserveViewModel
                            {
                                ReserveId = e.ReserveId,
                                PersonId = e.PersonId,
                                PersonFIO = e.PersonFIO.Trim(),
                                PersonAge = AgeMethods.GetAge(e.DOB, false),
                                PersonMonth = AgeMethods.GetTotalMonth(e.DOB),
                                Num_Address = e.Address_Num,
                                PersonRole = e.PersonRole,
                                RoleId = e.RoleId
                            }).ToList();

            ViewData["EventId"] = EventId;
            ViewBag.RoleList = db.PersonRoles.ToList();

            int pageSize = 50;
            int pageNumber = (page ?? 1);

            return PartialView(_persons.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, User")]
        public JsonResult AjaxEventCountChange([Bind(Include = "EventId,EventName,StartTime,EndTime,Description,AdministratorId,Note,PlacesCount")] Event @event)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    //Получаем ID роли "Клиент"
                    int _clientroleid = db.PersonRoles.First(r => r.RoleName.ToUpper() == (string)"Клиент".ToUpper()).RoleId;

                    if (db.Reserves.Where(e => e.EventId == @event.EventId && e.RoleId == _clientroleid).Count() > @event.PlacesCount)
                    {
                        return Json(new { Result = false, Message = "Количество мест не может быть меньше количества бронирований" }, JsonRequestBehavior.AllowGet);
                    }

                    db.Entry(@event).State = EntityState.Modified;
                    db.SaveChanges();

                    //Устанавливаем разделитель дробной части для работы c jQuery
                    NumberFormatInfo nfi = new NumberFormatInfo();
                    nfi.NumberDecimalSeparator = ".";

                    int _resplaces = db.Reserves.Where(e => e.EventId == @event.EventId && e.RoleId == _clientroleid).Count();
                    double _percent = (Convert.ToDouble(_resplaces)) / (Convert.ToDouble(@event.PlacesCount));

                    return Json(new
                    {
                        Result = true,
                        PlacesCount = @event.PlacesCount,
                        ResPlaces = _resplaces,
                        Percent = _percent.ToString(nfi)
                    }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    return Json(new { Result = false, Message = "Во время выполнения запроса произошла критическая ошибка" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Result = false, Message = "Ошибка валидации модели" }, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpPost]
        [Authorize(Roles = "Administrator, User")]
        public JsonResult AjaxUpdateInfoes(int? EventId, string Note)
        {
            if (EventId == null)
            {
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                Event _event = db.Events.FirstOrDefault(x => x.EventId == EventId);
                if (_event == null)
                {
                    return Json(new { Result = false }, JsonRequestBehavior.AllowGet);
                }

                _event.Note = Note;

                db.Entry(_event).State = EntityState.Modified;
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
