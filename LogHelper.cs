using System;
using System.IO;
using System.Threading;

namespace YFPos.Utils
{
    /// <summary>
    /// 日志类别
    /// </summary>
    public enum LogCategorys
    {
        NORMAL = 0,
        HTTP_JSON = 1,
        DATABASE = 2,
        PROMOTION = 3,
        JMS = 4,
        INITIAL = 5,
        EXCEPTION = 6,
        PAY_PuKang = 7,
        PAY = 8,
        SOCKET = 9,
        PRINT = 10,
        YB_HNXT = 11,
        MEMBER = 12,
        HANGUP = 13,
        CFD = 14,
        WEB = 15,
        ABA = 16
    }

    /// <summary>
    /// 企业库 Logging Application Block 封装
    /// </summary>
    public class LogHelper
    {
        static readonly object objToLock = new object();
        /// <summary>
        /// 如果当前工作目录不为程序启动目录，则设置到程序启动目录
        /// </summary>
        public static void SetCurrentDirectory2BaseDirectory()
        {

            if (Directory.GetCurrentDirectory() != AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\'))
            {
                Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg"></param>
        public static void WriteLog(string format, params object[] arg)
        {
           // LogEntry le = new LogEntry();
           // le.Message = string.Format(format, arg);
           // Logger.Write(le);
            WriteLog(LogCategorys.NORMAL, format, arg);
        }

        /// <summary>
        /// 写日志 不带params参数的
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg"></param>
        public static void WriteLog(string format)
        {
            // LogEntry le = new LogEntry();
            // le.Message = string.Format(format, arg);
            // Logger.Write(le);
            WriteLog(LogCategorys.NORMAL, format);
        }

        #region 废弃代码
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="message"></param>
        //public static void WriteLog(string message)
        //{
        //    LogEntry le = new LogEntry();
        //    le.Message = message;
        //    Logger.Write(le);
        //}
        #endregion

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="logCategory">日志类别(参考EnumLogCategory)</param>
        /// <param name="format">格式化字符串</param>
        /// <param name="arg">参数</param>
        public static void WriteLog(LogCategorys logCategory, string format, params object[] arg)
        {            
            try
            {
                if (arg != null && arg.Length>0)
                {
                    format = string.Format(format, arg);
                }
            }
            catch (Exception e)
            {
                format = string.Format("format异常{0},{1}", e.Message, format);
            }
            WriteLog(logCategory, format);
        }

        /// <summary>
        /// 记录日志 不带params参数的
        /// </summary>
        /// <param name="logCategory">日志类别(参考EnumLogCategory)</param>
        /// <param name="format">格式化字符串</param>
        /// <param name="arg">参数</param>
        public static void WriteLog(LogCategorys logCategory, string format)
        {

                //异步写文件,提高速度
                ThreadPool.QueueUserWorkItem(s =>
               {
                    //创建以日期命名的日志文件
                    lock (objToLock)
                   {
                       var wContent = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss:fff} -- {format}";
                        //创建log目录
                        string FilePath = $"{Environment.CurrentDirectory}\\log\\{logCategory}";
                       try
                       {
                           if (!Directory.Exists(FilePath))
                           {
                               Directory.CreateDirectory(FilePath);
                           }

                           string FileName = $"{FilePath}\\{logCategory}.{DateTime.Now:yyyy-MM-dd}.log";
                           if (File.Exists(FileName))
                           {
                               using (var sw = new StreamWriter(FileName, true))
                               {
                                   sw.WriteLine(wContent);
                               }
                           }
                           else
                           {
                               using (var myFs = new FileStream(FileName, FileMode.Create, FileAccess.Write,
                                   FileShare.Write))
                               using (var mySw = new StreamWriter(myFs))
                               {
                                   mySw.Write(wContent + Environment.NewLine);
                               }
                           }
                       }
                       catch (Exception e)
                       {
                           Console.WriteLine("Write Log Error:{0}", e.Message);
                       }
                   }
               });              
        }

        #region 废弃代码
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="logCategory">日志类别(参考EnumLogCategory)</param>
        /// <param name="message">日志信息</param>
        //public static void WriteLog(string logCategory, string message)
        //{
        //    LogEntry le = new LogEntry();
        //    le.Categories.Add(logCategory);
        //    le.Message = message;
        //    Logger.Write(le);
        //}
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="logCategory">日志类别(参考EnumLogCategory)</param>
        /// <param name="message">日志信息</param>
        //public static void WriteLog(string logCategory, string format, object arg0)
        //{
        //    LogEntry le = new LogEntry();
        //    le.Categories.Add(logCategory);
        //    le.Message = string.Format(format, arg0);
        //    Logger.Write(le);
        //}
        #endregion
    }
}
