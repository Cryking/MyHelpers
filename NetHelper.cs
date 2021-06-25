using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;

namespace YFPos.Utils
{
    /// <summary>
    /// 网络辅助工具
    /// </summary>
    public class NetHelper
    {
        /// <summary>
        /// ping测试
        /// </summary>
        /// <param name="ip"></param>
        public static bool PingTest(string ip, out string msg)
        {
            msg = string.Empty;
            bool isSuccess = false;
            if (ip.ToLower().Contains("http"))
            {
                ip = GetDomainName(ip);
            }
            if (ip.Length > 0)
            {
                try
                {
                    using (var ping = new Ping())
                    {
                        PingReply reply = ping.Send(ip);
                        isSuccess = (reply.Status == IPStatus.Success);
                        msg = reply.Status.ToString();
                    }
                }
                catch (PingException e)
                {
                    msg = e.Message;
                }
            }

            return isSuccess;
        }

        /// <summary>
        /// 根据网址获取域名
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private static string GetDomainName(string url)
        {
            var domainName = "";

            string p = @"(http|https)://(?<domain>[^(:|/]*)";
            Regex reg = new Regex(p, RegexOptions.IgnoreCase);
            Match m = reg.Match(url);
            if (m.Groups.Count > 0)
            {
                domainName = m.Groups["domain"].Value;
            }

            return domainName;
        }
    }
}
