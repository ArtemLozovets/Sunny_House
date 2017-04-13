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

        private const int EmailId = 6;

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
                                   from perrel in db.PersonRelations.Where(pid => personcomm.PersonId == pid.PersonId || personcomm.PersonId == pid.RelPersonId)
                                   from comm in db.Communications.Where(x=>x.Id == personcomm.CommunicationId && x.TypeOfCommunicationId == EmailId)
                                   where (PersonsIDS.Contains(perrel.PersonId) || PersonsIDS.Contains(perrel.RelPersonId))
                                   select new
                                   {
                                       PersonId = perrel.PersonId,
                                       Address_Number = comm.Address_Number
                                   }).GroupBy(x=>x.PersonId).Select(x=>x.FirstOrDefault()).Select(i=>i.Address_Number).Distinct().ToList();

                    if (_result.Count() == 0)
                    {
                        return Json(new { Result = false, Message = "Адреса электронной почты не найдены"}, JsonRequestBehavior.AllowGet);
                    }

                    return Json(new { Result = true, Message = "All Right!", EmList = _result }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = "Ошибка выполнения запроса. Сервер вернул значение: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        //private static bool ValidEmail(string email)
        //{
        //    if (email == "")
        //        return false;
        //    else
        //        return new System.Text.RegularExpressions.Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,6}$").IsMatch(email);
        //}
    }
}