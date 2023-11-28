using System;
using UnityEngine;

namespace StaGeGames
{
    public static class TimeUtilities
    {

        /// <summary>
        /// Returns current UTC Timestamp in milliseconds since the epoch
        /// </summary>
        /// <returns></returns>
        public static long GetUTCUnixTimestamp()
        {
            return (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc))).TotalMilliseconds;
        }

        public static long GetUTCUnixTimestamp(DateTime dateTarget)
        {
            return (long)(dateTarget.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc))).TotalMilliseconds;
        }

        /// <summary>
        /// retuns the date of timestamp
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns></returns>
        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp);
            return dtDateTime;
        }

        /// <summary>
        /// Returns current UTC Timestamp in string format yyyy-MM-dd HH:mm:ss.fff
        /// </summary>
        /// <returns></returns>
        public static string GetUTCDateTimeString()
        {
            return DateTime.UtcNow.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'fff");
        }

        public static float GetLastTimestampDifferenceInHours()
        {
            DateTime currentDay = DateTime.UtcNow;

            long temp = Convert.ToInt64(PlayerPrefs.GetString("lastQuitSession"));

            DateTime lastSaveDay = DateTime.FromBinary(temp);

            TimeSpan diff = currentDay.Subtract(lastSaveDay);

            return (float)diff.TotalHours;
        }


        public static float GetSessionWaitingIdleSeconds()
        {
            return (GetLastTimestampDifferenceInHours() * 60f * 60f) + 10f;
        }

        public static float GetRemainSecondsToMidnight()
        {
            DateTime current = DateTime.Now; // current time
            DateTime tomorrow = current.AddDays(1).Date; //"next" midnight

            double secondsUntilMidnight = (tomorrow - current).TotalSeconds;

            return (float)secondsUntilMidnight;
        }


        public static double[] GetEveryDayTimestampAndExportAlerts(DateTime fromDate, DateTime toDate, int hour, int min, out long[] milliSeconds)
        {
            fromDate = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, hour, min, 0);
            toDate = new DateTime(toDate.Year, toDate.Month, toDate.Day, hour, min, 0);

            int days = (int)toDate.AddDays(1).Subtract(fromDate).TotalDays;

            Debug.Log("days = " + days);

            double[] stamps = new double[days];
            milliSeconds = new long[days];

            for (int i = 0; i < days; i++)
            {
                DateTime dt = fromDate.AddDays(i + 1);
                stamps[i] = GetUTCUnixTimestamp(dt);// toDate.AddDays(-i).Subtract(fromDate).TotalMilliseconds;

                milliSeconds[i] = (long)dt.Subtract(fromDate).TotalMilliseconds;
            }

            return stamps;
        }

        public static void GetDays(DateTime fromDate, DateTime toDate)
        {
            int days = (int)toDate.AddDays(1).Subtract(fromDate).TotalDays;

            for (int i = 0; i < days; i++)
            {
                DateTime dt = fromDate.AddDays(i + 1);
                if (dt.DayOfWeek == DayOfWeek.Sunday)
                {
                    Debug.Log("Sunday");
                }
            }
        }

        //public static bool IsSameDay()
        //{
        //    long temp = Convert.ToInt64(PlayerPrefs.GetString("todayDate"));

        //    DateTime todayDate = DateTime.FromBinary(temp);

        //    return todayDate.IsToday();
        //}
    }

}