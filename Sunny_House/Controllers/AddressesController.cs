using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Sunny_House.Models;
using PagedList;
using PagedList.Mvc;

namespace Sunny_House.Controllers
{
    [Authorize(Roles = "Administrator, User")]
    public class AddressesController : Controller
    {
        private SunnyModel db = new SunnyModel();

        // GET: Форма відображення списку адрес з фільтрами та AJAX
        #region Список адрес
        public ActionResult AdShow(string CitySearchString, string AddressSearchString)
        {
            List<Address> _addresses = db.Addresses.ToList();

            var _cityes = from address in db.Addresses
                          select new SelectListItem { Text = address.City, Value = address.City };
            ViewData["Cityes"] = _cityes.OrderBy(c => c.Value).Distinct().ToList();
            return View();
        }

        // Список адрес 
        public ActionResult AdShowAllPartial(string CitySearchString, string AddressSearchString, int? page, string SortBy)
        {

            ViewBag.SortAlias = SortBy == "Alias" ? "Alias desc" : "Alias";
            ViewBag.SortPostCode = SortBy == "PostCode" ? "PostCode desc" : "PostCode";
            ViewBag.SortCountry = SortBy == "Country" ? "Country desc" : "Country";
            ViewBag.SortRegion = SortBy == "Region" ? "Region desc" : "Region";
            ViewBag.SortArea = SortBy == "Area" ? "Area desc" : "Area";
            ViewBag.SortCity = SortBy == "City" ? "City desc" : "City";
            ViewBag.SortAddressValue = SortBy == "AddressValue" ? "AddressValue desc" : "AddressValue";


            var _addresses = db.Addresses.Where(c => (c.City.Contains(CitySearchString) || String.IsNullOrEmpty(CitySearchString)) &&
                     ((c.AddressValue.ToUpper().Contains(AddressSearchString.ToUpper()) || String.IsNullOrEmpty(AddressSearchString)) ||
                     (c.Alias.ToUpper().Contains(AddressSearchString.ToUpper()) || String.IsNullOrEmpty(AddressSearchString))));

            switch (SortBy)
            {
                case "Alias desc":
                    _addresses = _addresses.OrderByDescending(x => x.Alias);
                    ViewData["SortColumn"] = "Alias";
                    break;
                case "Alias":
                    _addresses = _addresses.OrderBy(x => x.Alias);
                    ViewData["SortColumn"] = "Alias";
                    break;

                case "PostCode desc":
                    _addresses = _addresses.OrderByDescending(x => x.PostCode);
                    ViewData["SortColumn"] = "PostCode";
                    break;
                case "PostCode":
                    _addresses = _addresses.OrderBy(x => x.PostCode);
                    ViewData["SortColumn"] = "PostCode";
                    break;

                case "Country desc":
                    _addresses = _addresses.OrderByDescending(x => x.Country);
                    ViewData["SortColumn"] = "Country";
                    break;
                case "Country":
                    _addresses = _addresses.OrderBy(x => x.Country);
                    ViewData["SortColumn"] = "Country";
                    break;

                case "Region desc":
                    _addresses = _addresses.OrderByDescending(x => x.Region);
                    ViewData["SortColumn"] = "Region";
                    break;
                case "Region":
                    _addresses = _addresses.OrderBy(x => x.Region);
                    ViewData["SortColumn"] = "Region";
                    break;

                case "Area desc":
                    _addresses = _addresses.OrderByDescending(x => x.Area);
                    ViewData["SortColumn"] = "Area";
                    break;
                case "Area":
                    _addresses = _addresses.OrderBy(x => x.Area);
                    ViewData["SortColumn"] = "Area";
                    break;

                case "City desc":
                    _addresses = _addresses.OrderByDescending(x => x.City);
                    ViewData["SortColumn"] = "City";
                    break;
                case "City":
                    _addresses = _addresses.OrderBy(x => x.City);
                    ViewData["SortColumn"] = "City";
                    break;

                case "AddressValue desc":
                    _addresses = _addresses.OrderByDescending(x => x.AddressValue);
                    ViewData["SortColumn"] = "Area";
                    break;
                case "AddressValue":
                    _addresses = _addresses.OrderBy(x => x.AddressValue);
                    ViewData["SortColumn"] = "AddressValue";
                    break;

                default:
                    _addresses = _addresses.OrderByDescending(x => x.AddressId);
                    break;
            }


            int pageSize = 50;
            int pageNumber = (page ?? 1);

            return PartialView(_addresses.ToList().ToPagedList(pageNumber, pageSize));
        }

        // Список адрес для модального вікна
        public ActionResult AdShowPartial(string CitySearchString, string AddressSearchString)
        {
            List<Address> _addresses = db.Addresses.ToList();

            _addresses = _addresses.Where(c => (c.City
                     .Contains(CitySearchString) || CitySearchString == null) && ((c.AddressValue.ToUpper()
                     .Contains(AddressSearchString.ToUpper()) || AddressSearchString == null) || (c.Alias.ToUpper()
                     .Contains(AddressSearchString.ToUpper()) || AddressSearchString == null)))
                     .ToList();

            return PartialView(_addresses);
        }

        #endregion

        // GET: Addresses/Details/5
        public ActionResult AdDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Address address = db.Addresses.Find(id);
            if (address == null)
            {
                return HttpNotFound();
            }
            return View(address);
        }

        #region Відображення пов'язаних з адресою записів

        public PartialViewResult AdRelPersonsPartial(int? id)
        {

            List<Person> _persons = (from person in db.Persons
                                     join personplace in db.PersonPlaces on person.PersonId equals personplace.PersonId
                                     join address in db.Addresses on personplace.AddressId equals address.AddressId
                                     where address.AddressId == id
                                     select person).ToList();

            return PartialView(_persons);
        }

        public PartialViewResult AdRelExercisesPartial(int? id)
        {
            List<Exercise> _exercises = (from exercise in db.Exercises
                                         join address in db.Addresses on exercise.AddressId equals address.AddressId
                                         where address.AddressId == id
                                         select exercise).ToList();
            return PartialView(_exercises);
        }

        public PartialViewResult AdRelCommentsPartial(int? id)
        {
            List<Comment> _comments = (from comment in db.Comments
                                       join address in db.Addresses on comment.AddressId equals address.AddressId
                                       where address.AddressId == id
                                       select comment).ToList();
            return PartialView(_comments);
        }

        #endregion


        #region Створення адреси

        //Створення нової адреси
        [HttpGet]
        public ActionResult AdCreate(int? ObjectId, string RetActionName, string RetControllerName)
        {
            if (ObjectId != null) { ViewBag.ObjectId = ObjectId; }
            ViewData["RetActionName"] = RetActionName;
            ViewData["RetControllerName"] = RetControllerName;

            return View();
        }

        [HttpPost]
        public ActionResult AdCreate(int? ObjectId, Address model, string RetActionName, string RetControllerName)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Addresses.Add(model);

                    TempData["MessageOk"] = "Адрес успешно добавлен в базу данных";

                    db.SaveChanges();

                    if (ObjectId != null)
                    {
                        return RedirectToAction(RetActionName, RetControllerName, new { ObjectId, AddressId = model.AddressId });
                    }
                    else
                    {
                        return RedirectToAction(RetActionName, RetControllerName, new { AddressId = model.AddressId });
                    }
                }

                catch (Exception ex)
                {
                    ViewBag.ErMes = ex.Message;
                    ViewBag.ErStack = ex.StackTrace;
                    return View("Error");
                }
            }
            if (ObjectId != null) { ViewBag.ObjectId = ObjectId; }
            ViewData["RetActionName"] = RetActionName;
            ViewData["RetControllerName"] = RetControllerName;
            return View();
        }

        #endregion

        // GET: Addresses/Edit/5
        public ActionResult AdEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Address address = db.Addresses.Find(id);
            if (address == null)
            {
                return HttpNotFound();
            }
            return View(address);
        }

        // POST: Addresses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdEdit([Bind(Include = "AddressId,Alias,PostCode,Country,Region,Area,City,AddressValue,GeoTag,Note")] Address address)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(address).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["MessageOk"] = "Информация о адресе успешно обновлена";
                    return RedirectToAction("AdShow");
                }
                catch (Exception ex)
                {
                    ViewBag.ErMes = ex.Message;
                    ViewBag.ErStack = ex.StackTrace;
                    return View("Error");
                }

            }
            return View(address);
        }

        #region Редагування місця особи та адреси

        // GET: Addresses/Edit/5
        public ActionResult AdPlaceEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            PersonPlace place = db.PersonPlaces.FirstOrDefault(p => p.Id == id);
            Address address = db.Addresses.FirstOrDefault(p => p.AddressId == place.AddressId);

            if (address == null || address == null)
            {
                return HttpNotFound();
            }

            AdPlaceEditViewModel model = new AdPlaceEditViewModel
            {
                PersonPlace = place,
                Address = address
            };

            return View(model);
        }

        // POST: Addresses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdPlaceEdit(AdPlaceEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {

                        db.Entry(model.PersonPlace).State = EntityState.Modified;
                        db.SaveChanges();

                        db.Entry(model.Address).State = EntityState.Modified;
                        db.SaveChanges();

                        dbContextTransaction.Commit();

                        TempData["MessageOk"] = "Информация о адресе успешно обновлена";
                        return RedirectToAction("ShowPlaces", "Home", new { PersonId = Request.QueryString["PersonId"] });
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
            return View(model);
        }

        #endregion

        // GET: Addresses/Delete/5
        public ActionResult AdDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Address address = db.Addresses.Find(id);
            if (address == null)
            {
                return HttpNotFound();
            }
            return View(address);
        }

        // POST: Addresses/Delete/5
        [HttpPost, ActionName("AdDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {

                if (db.Addresses.First(a => a.AddressId == id).PersonPlace.Any())
                {
                    TempData["MessageError"] = "Удаление адреса невозможно. В таблице персон имеются связанные данные.";
                    return RedirectToAction("AdShow");
                }

                if (db.Addresses.First(a => a.AddressId == id).Exercise.Any())
                {
                    TempData["MessageError"] = "Удаление адреса невозможно. В таблице занятий имеются связанные данные.";
                    return RedirectToAction("AdShow");
                }

                if (db.Addresses.First(a => a.AddressId == id).Comment.Any())
                {
                    TempData["MessageError"] = "Удаление адреса невозможно. В таблице отзывов имеются связанные данные.";
                    return RedirectToAction("AdShow");
                }

                else
                {
                    Address address = db.Addresses.Find(id);
                    db.Addresses.Remove(address);
                    db.SaveChanges();
                    TempData["MessageOk"] = "Информация о адресе успешно удалена";
                    return RedirectToAction("AdShow");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErMes = ex.Message;
                ViewBag.ErStack = ex.StackTrace;
                return View("Error");
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
