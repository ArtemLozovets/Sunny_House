using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sunny_House.Methods
{
    public static class AgeMethods
    {
        public static int? GetAge(DateTime? DateOfBirth)
        {
            if (DateOfBirth == null)
            {
                return null;
            }
            else
            {
                int Age = 0;
                int NowDayNum = 0;
                int DOBDayNum = 0;

                DateTime nowDate = DateTime.Today;
                DateTime DOB = DateOfBirth ?? DateTime.Today;

                Age = nowDate.Year - DOB.Year;

                if (DateTime.IsLeapYear(nowDate.Year) && (nowDate.DayOfYear > 60))
                {
                    NowDayNum = nowDate.DayOfYear - 1;
                }
                else
                {
                    NowDayNum = nowDate.DayOfYear;
                }

                if (DateTime.IsLeapYear(DOB.Year) && (DOB.DayOfYear > 60))
                {
                    DOBDayNum = DOB.DayOfYear - 1;
                }
                else
                {
                    DOBDayNum = DOB.DayOfYear;
                }

                string debugMess = string.Format("Now day:{0} | DOB day:{1}", nowDate.DayOfYear, DOB.DayOfYear);
                System.Diagnostics.Debug.WriteLine(debugMess);

                if (NowDayNum < DOBDayNum) { Age--; }

                return Age;
            }
        }

        public static int? GetTotalMonth(DateTime? DateOfBirth)
        {
            if (DateOfBirth == null)
            {
                return null;
            }
            else
            {
                int TotalMonth = 0;
                DateTime nowDate = DateTime.Today;
                DateTime DOB = DateOfBirth ?? DateTime.Today;

                if (nowDate == DOB) { TotalMonth = 0; }
                else
                {
                    TotalMonth = ((nowDate.Year - DOB.Year) * 12)
                        + nowDate.Month - DOB.Month
                        + (nowDate.Day >= DOB.Day - 1 ? 0 : -1)//поправка на числа
                        + ((DOB.Day == 1 && DateTime.DaysInMonth(nowDate.Year, nowDate.Month) == nowDate.Day) ? 1 : 0);
                    //если начальная дата - 1-е число меяца, а конечная - последнее число, добавляется 1 месяц
                }

                string debugMess = string.Format("Месяцев: ", TotalMonth);
                System.Diagnostics.Debug.WriteLine(debugMess);
                return TotalMonth;
            }
        }

    }
}