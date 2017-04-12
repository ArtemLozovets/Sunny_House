using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sunny_House.Models;

namespace Sunny_House.Controllers
{
    public class MailController : Controller
    {
        // GET: Mail
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Administrator, User")]
        public JsonResult GetEmails(int[] PersonsIDS)
        {
            if (PersonsIDS.Length == 0)
            {
                return Json(new { Result = false, Message = "Входящий список персон пуст" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                using (SunnyModel db = new SunnyModel())
                {

                    var _result = (from personcomm in db.PersonCommunications
                                   join comm in db.Communications on personcomm.CommunicationId equals comm.Id
                                   join commtype in db.TypeOfCommunications on comm.TypeOfCommunicationId equals commtype.Id
                                   where PersonsIDS.Contains(personcomm.PersonId)
                                   select new 
                                   {
                                       Email_Address = comm.Address_Number
                                   }).AsEnumerable().Where(e => ValidEmail(e.Email_Address)).Select(i=>i.Email_Address).ToList();

                    return Json(new { Result = true, Message = "All Right!", EmList = _result }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = "Ошибка выполнения запроса. Сервер вернул значение: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        private static bool ValidEmail(string email)
        {
            if (email == "")
                return false;
            else
                return new System.Text.RegularExpressions.Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,6}$").IsMatch(email);
        }
    }
}