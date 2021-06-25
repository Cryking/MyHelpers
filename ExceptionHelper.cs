using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace YFPos.Utils
{
    /// <summary>
    /// 异常处理工具类
    /// </summary>
    public class ExceptionHelper
    {
        public static string WebExceptionProcess(WebException e)
        {
            var errmsg = "";
            if (e.Response != null)
            {
                using (WebResponse response = e.Response)
                {
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        //var rspObj = JsonConvertUtils.DesAnonymousType(text, new { code = "", message = "" });
                        if (!string.IsNullOrEmpty(text))
                        {
                            errmsg = text;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(errmsg))
            {
                errmsg = e.Message;
            }

            return errmsg;
        }
    }
}
