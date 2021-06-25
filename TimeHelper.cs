using System;
using System.Runtime.InteropServices;

namespace YFPos.Utils
{
    public class TimeHelper
    {
        /// <summary>
        /// 系统时间相关结构
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SystemTime
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;

            public void FromDateTime(DateTime dateTime)
            {
                wYear = (ushort)dateTime.Year;
                wMonth = (ushort)dateTime.Month;
                wDayOfWeek = (ushort)dateTime.DayOfWeek;
                wDay = (ushort)dateTime.Day;
                wHour = (ushort)dateTime.Hour;
                wMinute = (ushort)dateTime.Minute;
                wSecond = (ushort)dateTime.Second;
                wMilliseconds = (ushort)dateTime.Millisecond;
            }
        }

        [DllImport("Kernel32.dll")]
        public static extern void GetLocalTime(ref SystemTime st);
        [DllImport("Kernel32.dll", SetLastError=true)]
        public static extern bool SetLocalTime(ref SystemTime st);
        [DllImport("Kernel32.dll")]
        public static extern int FormatMessage(int flag, ref IntPtr source, int msgid, int langid, ref string buf, int size, ref IntPtr args);

        /// <summary>
        /// 设置本地时间
        /// </summary>
        public static void SetLocalTime(DateTime dtime)
        {
            //ClientAuthenticationMembershipProvider
            var st = new SystemTime();
            st.FromDateTime(dtime);
            LogHelper.WriteLog("设置本机时间为:{0}", dtime.ToString("yyyy-MM-dd HH:mm:ss"));
            if (!SetLocalTime(ref st))
            {
                int errorCode = Marshal.GetLastWin32Error();
                if (errorCode != 0)
                {
                    IntPtr tempptr = IntPtr.Zero;
                    string msg = null;
                    FormatMessage(0x1300, ref tempptr, errorCode, 0, ref msg, 255, ref tempptr);
                    LogHelper.WriteLog(LogCategorys.EXCEPTION, $"SetDateTime error：{errorCode}-{msg}");
                }
            }
        }

        /// <summary>
        /// 根据时间戳获得具体时间
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime GetTime(long? timeStamp)
        {
            if (timeStamp==null)
            {
                return DateTime.MinValue;
            }
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime date = start.AddMilliseconds(timeStamp.Value).ToLocalTime();
            return date;
        }


        /// <summary>
        /// 获取时间戳(秒)
        /// </summary>
        /// <returns></returns>
        public static long GetTimestampSeconds()
        {
            return (long)GetTimeSpan().TotalSeconds;
        }

        /// <summary>
        /// 获取时间戳(毫秒)
        /// create by gw 2020.12.08
        /// </summary>
        /// <returns></returns>
        public static long GetTimestampMs()
        {
            return (long)GetTimeSpan().TotalMilliseconds;
        }

        /// <summary>
        /// 获取到当前时间的时间间隔
        /// </summary>
        /// <returns></returns>
        private static TimeSpan GetTimeSpan()
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            return (DateTime.Now - startTime);
        }

        /// <summary>
        /// create by gw 2020.11.5
        /// 字符串转时间
        /// </summary>
        /// <returns></returns>
        public static DateTime ToDateTime<T>(T dateObj) where T : class
        {
            DateTime dateValue = DateTime.MinValue;
            if (!string.IsNullOrEmpty(dateObj?.ToString())
                && DateTime.TryParse(dateObj.ToString(), out dateValue))
            {
                return dateValue;
            }
            else
            {
                return dateValue;
            }
        }

        /// <summary>
        /// 根据生日算年龄
        /// create by gw 2021.4.27
        /// </summary>
        /// <returns></returns>
        public static int GetAge(DateTime birthday)
        {
            return
                (DateTime.Now.Year - birthday.Year + (DateTime.Now.Month > birthday.Month ? 0 : -1));
        }
    }
}
