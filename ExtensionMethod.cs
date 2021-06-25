using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YFPos.Utils
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class ExtensionMethod
    {
        /// <summary>
        /// 将异常对象保存到文件
        /// </summary>
        /// <param name="ex">异常对象</param>
        public static void SaveLog(this Exception ex, string msg = "")
        {
            LogHelper.WriteLog(LogCategorys.EXCEPTION, "{0}{1}{3}调用堆栈：{2}",
                msg.Length > 0 ? string.Format("附加信息:{0}", msg) : "", ex.Message, ex.StackTrace, Environment.NewLine);
        }
        /// <summary>
        /// TextBox只允许输入指定字符
        /// </summary>
        /// <param name="txt">System.Windows.Controls.TextBox</param>
        /// <param name="allowInputStr">允许输入的字符</param>
        public static void InputRestricted(this System.Windows.Controls.TextBox txt, string allowInputStr = "0123456789-")
        {
            txt.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Space)
                    e.Handled = true;
            };
            txt.PreviewTextInput += (s, e) =>
            {
                if (!Exists(e.Text, allowInputStr))
                {
                    e.Handled = true;
                }
                else
                    e.Handled = false;
            };
            txt.AddHandler(System.Windows.DataObject.PastingEvent, new System.Windows.DataObjectPastingEventHandler((s, e) =>
            {
                if (e.DataObject.GetDataPresent(typeof(String)))
                {
                    String text = (String)e.DataObject.GetData(typeof(String));
                    if (!Exists(text, allowInputStr))
                    { e.CancelCommand(); }
                }
                else { e.CancelCommand(); }
            }));
        }

        /// <summary>
        /// str中是否包含input的每个字符
        /// </summary>
        /// <param name="input"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        private static bool Exists(string input, string str)
        {
            var result = true;

            foreach (var item in input)
            {
                if (!str.Contains(item))
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// 将List切割成多个List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="groupNum">每个List包含几个Item</param>
        /// <returns></returns>
        public static List<List<T>> GetListGroup<T>(this List<T> list, int groupNum)
        {
            List<List<T>> listGroup = new List<List<T>>();
            for (int i = 0; i < list.Count(); i += groupNum)
            {
                listGroup.Add(list.Skip(i).Take(groupNum).ToList());
            }
            return listGroup;
        }
        /// <summary>
        /// 将List切割成多个List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="groupNum">每个List包含几个Item</param>
        /// <returns></returns>
        public static IList<IList<T>> GetListGroup<T>(this IList<T> list, int groupNum)
        {
           var listGroup = new List<IList<T>>();
            for (int i = 0; i < list.Count(); i += groupNum)
            {
                listGroup.Add(list.Skip(i).Take(groupNum).ToList());
            }
            return listGroup;
        }

        /// <summary>
        /// create by gw 2020.12.23
        /// 获取指定长度的字符串
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string GetLen(this string source, int len)
        {
            if (!string.IsNullOrEmpty(source) && source.Length > len)
            {
                return source.Substring(0, len);
            }
            else
            {
                return source;
            }
        }
    }
}
 