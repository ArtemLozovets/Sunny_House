using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sunny_House.Models;

namespace Sunny_House.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class StatisticController : Controller
    {
        private SunnyModel db = new SunnyModel();
        public ActionResult Payment(int? Year)
        {
            if (Year == null)
            {
                Year = DateTime.Today.Year;
            }

            var _paylist = (from t in db.Payments
                            group t by new { t.Date.Year, t.Date.Month } into g
                            where g.Key.Year == Year
                            select new
                            {
                                Month = g.Key.Month,
                                Total = g.Sum(t => t.Sum)
                            });

            List<decimal?> _sum = new List<decimal?>();
            for (int i = 1; i < 13; i++)
            {
                if (_paylist.FirstOrDefault(x => x.Month == i) != null)
                {
                    _sum.Add(_paylist.FirstOrDefault(x => x.Month == i).Total);
                }
                else _sum.Add(0);
            }


            var _exlist = (from t in db.Exercises
                           group t by new { t.StartTime.Year, t.StartTime.Month } into g
                           where g.Key.Year == Year
                           select new
                           {
                               Month = g.Key.Month,
                               Total = g.Count()
                           });

            List<int?> _ex = new List<int?>();
            for (int i = 1; i < 13; i++)
            {
                if (_exlist.FirstOrDefault(x => x.Month == i) != null)
                {
                    _ex.Add(_exlist.FirstOrDefault(x => x.Month == i).Total);
                }
                else _ex.Add(0);
            }

            ViewBag.sum = _sum;
            ViewBag.ex = _ex;
            ViewBag.Message = String.Format("График платежей за {0} год", Year);

            return View();
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