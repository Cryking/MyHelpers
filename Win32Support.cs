using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace YFPos.Utils
{
    public class Win32Support
    {
        /// <summary>
        /// 系统时间相关结构
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEMTIME
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

            public DateTime ToDateTime()
            {
                return new DateTime(wYear, wMonth, wDay, wHour, wMinute, wSecond);
            }
        }

        /// <summary>
        /// 设置系统本地时间
        /// </summary>
        [DllImport("Kernel32.dll")]
        public static extern bool SetLocalTime(ref SYSTEMTIME time);

        /// <summary>
        /// 创建指定窗口的线程设置到前台，并且激活该窗口。
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);


        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public extern static IntPtr FindWindow(string lpClassName, string lpWindowName);


        /// <summary>
        /// 设置置顶窗口
        /// </summary>
        /// <param name="windowTitle">要置顶的窗口标题</param>
        public static void SetTopWindow(string windowTitle)
        {
            System.Threading.Thread th = new System.Threading.Thread(delegate ()
            {
                var topCount = 0;//置顶次数
                while (true)
                {
                    var aa = FindWindow(null, windowTitle);
                    if (aa != IntPtr.Zero)
                    {
                        SetForegroundWindow(aa);
                        topCount++;
                        if (topCount > 10)
                        {
                            break;
                        }
                        System.Threading.Thread.Sleep(5 * 1000);
                    }
                    System.Threading.Thread.Sleep( 1000);
                }
            });
            th.Start();
        }
    }
}
