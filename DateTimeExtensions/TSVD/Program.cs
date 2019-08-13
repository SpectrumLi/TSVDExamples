
using DateTimeExtensions;
using DateTimeExtensions.WorkingDays;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSVD
{
    class Program
    {
        static void Main(string[] args)
        {
            //Arrange
            var culture = new WorkingDayCultureInfo("en-US");
            var startDate = new DateTime(2018, 5, 1);
            int n = 10;
            List<int> list = new List<int>(n);
            for (int i = 0; i < n; i++) list.Add(i);
            //Act
            Parallel.ForEach(list, (i) =>
            {
                try
                {
                    startDate.AddWorkingDays(i, culture);
                }
                catch (Exception exp) { }
            });

            var midsummerDay = new NthDayOfWeekAfterDayHoliday("Midsummer Day", 1, DayOfWeek.Saturday, 6, 20);
            Parallel.ForEach(list, (i) =>
            {
                try
                {
                    midsummerDay.GetInstance(1900 + i);
                }
                catch (Exception exp) { }
            });

        }
    }
}
