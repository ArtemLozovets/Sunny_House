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

        public ActionResult Index(int? Year)
        {
            return View();
        }

        public JsonResult GetChart(string mode, int? Year)
        {
            if (Year == null)
            {
                Year = DateTime.Today.Year;
            }

            if (mode == "mpayment" || string.IsNullOrEmpty(mode))
            {
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
                return Json(new { Result = true, Message = String.Format("График платежей за {0} год", Year), ChartData = _sum, Mode = mode }, JsonRequestBehavior.AllowGet);
            }

            if (mode == "mvisit" || string.IsNullOrEmpty(mode))
            {
                var _visitlist = (from t in db.Visits
                                  group t by new { t.Exercise.StartTime.Year, t.Exercise.StartTime.Month, t.FactVisit } into g
                                  where g.Key.Year == Year && g.Key.FactVisit
                                  select new
                                  {
                                      Month = g.Key.Month,
                                      Total = g.Count()
                                  });



                List<int?> _visit = new List<int?>();
                for (int i = 1; i < 13; i++)
                {
                    if (_visitlist.FirstOrDefault(x => x.Month == i) != null)
                    {
                        _visit.Add(_visitlist.FirstOrDefault(x => x.Month == i).Total);
                    }
                    else _visit.Add(0);
                }
                return Json(new { Result = true, Message = String.Format("График посещений за {0} год", Year), ChartData = _visit, Mode = mode }, JsonRequestBehavior.AllowGet);
            }

            if (mode == "mex" || string.IsNullOrEmpty(mode))
            {
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

                return Json(new { Result = true, Message = String.Format("Количество занятий в {0} году", Year), ChartData = _ex, Mode = mode }, JsonRequestBehavior.AllowGet);
            }


            return Json(new { Result = false, Message = "Во время выполнения запроса произошла ошибка", Mode = mode }, JsonRequestBehavior.AllowGet);
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