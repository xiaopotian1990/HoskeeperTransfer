using System;
using System.Collections.Generic;
using System.Text;

namespace HoskeeperTransfer
{
    public static class DateTimeEx
    {

        /// <summary>
        /// 日期转换成本地unix时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long ToLocalUnixTimestamp(this DateTime dateTime)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            long timeStamp = (long)(dateTime - startTime).TotalSeconds;

            return timeStamp * 1000;
        }

        /// <summary>        
        /// 时间戳转为C#格式时间        
        /// </summary>        
        /// <param name=”timeStamp”></param>        
        /// <returns></returns>        
        public static DateTime ToDateTime(this long timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            TimeSpan toNow = new TimeSpan(timeStamp);
            return dtStart.AddSeconds(timeStamp/1000);
        }
    }
}
