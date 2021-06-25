using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace YFPos.Utils
{
    public class HttpSignatureByEC
    {
        private const string RetSuccess = "0";
        private const string RetFailed = "-1";

        private const string Apiversion = "1.0";
        private const string AccessKeyId = "J66OcIujL93feevz";
        private const string AccessKeySecret = "o12kcLzzpUveCFSTmPGqm6WDHWmTtTSp";

        /// <summary>
        /// 获取头部参数json
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetHeaderDict()
        {
            var dict = new Dictionary<string, string>();
            var header = new OpenxHead()
            {
                ApiVersion = Apiversion,
                ContentMd5 = "",
                Date = DateTime.Now.ToString("r")
            };
            header.Authorization = string.Format("OTP {0}:{1}", AccessKeyId, GetSignature(Apiversion, AccessKeySecret, header.Date));
            dict.Add("Authorization", header.Authorization);
            dict.Add("Content-Md5", header.ContentMd5);
            dict.Add("Content-Type", header.ContentType);
            dict.Add("Date", header.Date);
            dict.Add("Api-Version", header.ApiVersion);

            return dict;
        }

        /// <summary>
        /// 获取签名字串
        /// </summary>
        /// <param name="apiVersion"></param>
        /// <param name="apiSecret"></param>
        /// <returns></returns>
        private static string GetSignature(string apiVersion, string apiSecret, string date)
        {
            var sign = string.Format("POST\n\napplication/json\n{0}\n{1}", date, apiVersion);

            var hmacshal = new HMACSHA1(Encoding.UTF8.GetBytes(apiSecret));
            sign = Convert.ToBase64String(hmacshal.ComputeHash(Encoding.UTF8.GetBytes(sign)));

            return sign;
        }

    }

    /// <summary>
    /// 接口签名头部参数
    /// </summary>
    public class OpenxHead
    {
        /// <summary>
        /// 验证字符串
        /// 由OTP + 空格 + AccessKeyId + : + signature构成
        /// 例:OTP x44BR0cEXUxn3Iag:8RioTal+hyyG6IFRes1dzKBIONU=
        /// </summary>
        public string Authorization { get; set; }

        /// <summary>
        /// 当前版本为1.0
        /// 调用OTP接口的版本号
        /// </summary>
        public string ApiVersion { get; set; }

        /// <summary>
        /// 请求的构造时间
        /// 目前只支持GMT格式: EEE, dd MMM yyyy HH:mm:ss z
        /// 如果请求时间和OTP服务器时间相差超过15分钟，脉家会判定此请求不合法
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// HTTP消息体的MD5值
        /// 当前可以为空
        /// </summary>
        public string ContentMd5 { get; set; }

        /// <summary>
        /// 请求内容的MIME类型
        /// 目前请求仅支持 text/json 格式
        /// </summary>
        public string ContentType { get; set; }

        public OpenxHead()
        {
            ContentType = "application/json";
        }
    }
}
