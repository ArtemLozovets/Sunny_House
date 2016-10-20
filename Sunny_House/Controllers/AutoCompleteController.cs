using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sunny_House.Models;

namespace Sunny_House.Controllers
{
    [Authorize(Roles = "Administrator, User")]
    public class AutoCompleteController : Controller
    {

        private SunnyModel db = new SunnyModel();

        #region Методи підстановки для моделі "Адреси" 
       
        public ActionResult AutocompleteCity(string Term)
        {
           var result = db.Addresses.Where(c => c.City.Contains(Term)).Select(c => c.City).Distinct();
           return Json(result, JsonRequestBehavior.AllowGet);
        }

         public ActionResult AutocompleteAlias(string Term)
         {
             var result = db.Addresses.Where(c => c.Alias.Contains(Term)).Select(c => c.Alias).Distinct();
             return Json(result, JsonRequestBehavior.AllowGet);
         }

         public ActionResult AutocompletePostCode(string Term)
         {
             var result = db.Addresses.Where(c => c.PostCode.Contains(Term)).Select(c => c.PostCode).Distinct();
             return Json(result, JsonRequestBehavior.AllowGet);
         }

        public ActionResult AutocompleteCountry(string Term)
         {
             var result = db.Addresses.Where(c => c.Country.Contains(Term)).Select(c => c.Country).Distinct();
             return Json(result, JsonRequestBehavior.AllowGet);
         }
        
        public ActionResult AutocompleteRegion(string Term)
         {
             var result = db.Addresses.Where(c => c.Region.Contains(Term)).Select(c => c.Region).Distinct();
             return Json(result, JsonRequestBehavior.AllowGet);
         }
            
        public ActionResult AutocompleteArea(string Term)
         {
             var result = db.Addresses.Where(c => c.Area.Contains(Term)).Select(c => c.Area).Distinct();
             return Json(result, JsonRequestBehavior.AllowGet);
         }


        public ActionResult AutocompleteAddressValue(string Term)
         {
             var result = db.Addresses.Where(c => c.AddressValue.Contains(Term)).Select(c => c.AddressValue).Distinct();
             return Json(result, JsonRequestBehavior.AllowGet);
         }
            
        #endregion

        #region Методи підстановки для моделі "Місця ооби"
        public ActionResult AutocompletePlaceName(string Term)
        {
            var result = db.PersonPlaces.Where(c => c.PlaceName.Contains(Term)).Select(c => c.PlaceName).Distinct();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Методи підстановки для моделі "Персони"
        public ActionResult AutocompletePersonFirstName(string Term)
        {
            var result = db.Persons.Where(c => c.FirstName.Contains(Term)).Select(c => c.FirstName).Distinct();
            var mmm = result.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Методи підстановки для моделі "Платежі"
        public ActionResult AutocompletePayer(string Term)
        {
            var result = from payments in db.Payments
                              join payer in db.Persons on payments.PayerId equals payer.PersonId
                              where payer.FirstName.Contains(Term)
                              select payer.FirstName;

            result = result.Distinct();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AutocompleteClient(string Term)
        {
            var result = from payments in db.Payments
                         join client in db.Persons on payments.ClientId equals client.PersonId
                         where client.FirstName.Contains(Term)
                         select client.FirstName;

            result = result.Distinct();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Методи підстановки для моделі "Заходи"
        public ActionResult AutocompleteEventName(string Term)
        {
            var result = from events in db.Events
                         where events.EventName.Contains(Term)
                         select events.EventName;

            result = result.Distinct();

            return Json(result, JsonRequestBehavior.AllowGet);
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