using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace YFPos.Utils
{
    public class HttpClient
    {
        #region 字段属性
        /// <summary>
        /// 编码格式
        /// </summary>
        static Encoding httpEncoding = Encoding.UTF8;
        /// <summary>
        /// 编码格式
        /// </summary>
        public static Encoding HttpEncoding
        {
            get
            {
                return HttpClient.httpEncoding;
            }
            set
            {
                HttpClient.httpEncoding = value;
            }
        }
        #endregion

        #region Post
        /// <summary>
        /// Post文件和键值对
        /// </summary>
        /// <param name="url"></param>
        /// <param name="keyValues"></param>
        /// <param name="files"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Post(string url, NameValueCollection keyValues, FileInfo[] files)
        {
            string content = null;
            try
            {
                //边界线
                string boundary = DateTime.Now.Ticks.ToString("x");
                //创建Request对象并指定Url地址
                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
                request.Timeout = 60000;
                request.Proxy = null;
                request.Method = "POST"; //采用POST的请求
                request.ContentType = string.Format("multipart/form-data; boundary=---------------------------{0}", boundary);

                var sb = new StringBuilder();
                //发送键值对
                foreach (string key in keyValues.AllKeys)
                {
                    sb.AppendFormat("-----------------------------{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n", boundary, key, keyValues[key]);
                }
                //转化成字节数组
                byte[] buffer = httpEncoding.GetBytes(sb.ToString());
                //请求流对象
                Stream stream = request.GetRequestStream();
                //发送流
                stream.Write(buffer, 0, buffer.Length);

                if (files != null)
                {
                    //发送文件
                    foreach (var file in files)
                    {
                        //零时存储文字段
                        string temp = string.Format("-----------------------------{0}\r\nContent-Disposition: form-data; name=\"file\"; filename=\"{1}\"\r\nContent-Type: image/jpeg\r\n\r\n"
                            , boundary, file.Name);
                        buffer = httpEncoding.GetBytes(temp);
                        stream.Write(buffer, 0, buffer.Length);
                        using (var fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                        {
                            buffer = new byte[file.Length];
                            fs.Read(buffer, 0, buffer.Length);
                            stream.Write(buffer, 0, buffer.Length);
                        }
                        //结尾符号
                        buffer = httpEncoding.GetBytes(Environment.NewLine);
                        stream.Write(buffer, 0, buffer.Length);
                    }
                }
                //结尾
                buffer = httpEncoding.GetBytes(string.Format("-----------------------------{0}--\r\n", boundary));
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
                stream.Close();
                //------------接收服务器返回的数据------------
                //得到服务器放回的Response对象
                var response = request.GetResponse() as HttpWebResponse;
                using (var sr = new StreamReader(response.GetResponseStream(), httpEncoding))
                {
                    content = sr.ReadToEnd();
                }
                response.Close();
            }
            catch (WebException ex)
            {
                LogHelper.WriteLog(LogCategorys.EXCEPTION,
                   string.Format("{0} {1}", ex.Message, url));
                //throw new Exception(ex.Message + url);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(LogCategorys.EXCEPTION, string.Format("{0} {1}", ex.Message, url));
                //throw new Exception(ex.Message + url);
            }
            return content;
        }

        /// <summary>
        /// Get Response Stream Content
        /// 客户端调用此方法发送相关参数，请求服务器反馈的数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="values"></param>
        /// <returns>Response Content</returns>
        private static string Post(string url, params string[] args)
        {
            string content = null;
            try
            {
                //转化成字节数组，准备发送
                byte[] postData = httpEncoding.GetBytes(string.Join("&", args));
                //---------发送--------
                //创建Request对象并指定Url地址
                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
                request.Timeout = 60000;
                request.Proxy = null;
                request.Method = "POST"; //采用POST的请求
                request.ContentType = "application/x-www-form-urlencoded"; //内容类型
                request.ContentLength = postData.Length; //指定POST请求的长度

                //开始发送字节流
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(postData, 0, postData.Length);
                    stream.Flush();
                    stream.Close();
                }
                //------------接收服务器返回的数据------------
                //得到服务器放回的Response对象
                var response = request.GetResponse() as HttpWebResponse;
                using (var sr = new StreamReader(response.GetResponseStream(), httpEncoding))
                {
                    content = sr.ReadToEnd();
                }
                response.Close();
            }
            catch (WebException ex)
            {
                throw new Exception(ex.Message + url);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + url);
            }
            return content;
        }

        /// <summary>  
        /// 创建POST方式的HTTP请求New  
        /// </summary>  
        public static string PostHttpRsp(string url, string jsonStr, out string rspcode,
            int timeout = 60000, int readWriteTimeout = 10000)
        {
            rspcode = "";
            byte[] data = Encoding.UTF8.GetBytes(jsonStr);

            //设置https验证方式//modify by ljm 遇到https请求地址，跳过验证
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback(CheckValidationResult);
            }
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentLength = data.Length;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Timeout = timeout;
            request.ReadWriteTimeout = readWriteTimeout;
            request.Proxy = null;

            //发送POST数据  
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            var webRsp = request.GetResponse() as HttpWebResponse;
            if (webRsp != null)
            {
                rspcode = ((int)webRsp.StatusCode).ToString();
                return GetResponseString(webRsp);
            }
            else
            {
                return null;
            }
        }

        /// <summary>  
        /// 创建POST方式的HTTP请求  
        /// </summary>  
        public static string PostHttpResponse(string url, string jsonStr,
            int timeout = 60000, int readWriteTimeout = 10000)
        {
            byte[] data = Encoding.UTF8.GetBytes(jsonStr);

            //设置https验证方式//modify by ljm 遇到https请求地址，跳过验证
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback(CheckValidationResult);
            }
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentLength = data.Length;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Timeout = timeout;
            request.ReadWriteTimeout = readWriteTimeout;
            request.Proxy = null;

            //发送POST数据  
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            return GetResponseString(request.GetResponse() as HttpWebResponse);
        }

        /// <summary>
        /// 获取请求的数据
        /// </summary>
        private static string GetResponseString(HttpWebResponse webresponse)
        {
            using (Stream s = webresponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(s, Encoding.UTF8);
                return reader.ReadToEnd();
            }
        }

        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //直接确认，否则打不开    
            return true;
        }
        #endregion

        #region Get

        /// <summary>
        /// 下载HTML页面
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetContent(string url, int timeout = 60000, string token = "")
        {
            string content = string.Empty;
            //YfPosHttpResponse.RootObject rsp = new YfPosHttpResponse.RootObject();
            try
            {
                if (WebRequest.Create(url) is HttpWebRequest request)
                {
                    request.Timeout = timeout;
                    request.ReadWriteTimeout = timeout;
                    request.Method = "GET";
                    if (token.Length > 0)
                    {
                        request.Headers.Set("token", token);
                    }
                    //request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0)";
                    //#region 构造请求头
                    //WebHeaderCollection headers = new WebHeaderCollection();
                    ////协议版本号
                    //headers.Set("yfposrpc", "1.0");
                    //HeadSender sender = new HeadSender { storeId="",termId="",operId="" };
                    ////发送方身份                    
                    //headers.Set("sender", JsonHelper.ObjectToJson(sender));
                    ////身份识别
                    //headers.Set("token", "");
                    ////时间戳
                    //headers.Set("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));                                     
                    //#endregion
                    //request.Headers = headers;
                    using (var response = request.GetResponse() as HttpWebResponse)
                    using (var sr = new StreamReader(response.GetResponseStream(), httpEncoding))
                    {
                        content = sr.ReadToEnd();
                    }
                }
            }
            //catch (WebException ex)
            //{
            //    //ExceptionPolicy.HandleException(ex, "Policy");
            //    ////YfPosHttpResponse.Body body = new YfPosHttpResponse.Body();
            //    ////YfPosHttpResponse.Error error = new YfPosHttpResponse.Error { message = ex.Message };
            //    ////body.error = error;
            //    ////rsp.body = body;
            //    ////return JsonHelper.ObjectToJson(rsp);
            //    //throw new Exception(ex.Message + url);
            //    throw new Exception(string.Format("{0} {1}", ex.Message, url));
            //}
            catch (Exception ex)
            {
                ex.SaveLog(url);
                throw;
            }
            return content;
        }
        public static string PostByJson(string url, string jsonStr, int timeout = 60000, string token = "")
        {
            byte[] data = Encoding.UTF8.GetBytes(jsonStr);

            //跳过验证
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback(CheckValidationResult);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
            }
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentLength = data.Length;
            request.ContentType = "application/json";
            request.Timeout = timeout;
            request.ReadWriteTimeout = 10000;
            if (token.Length > 0)
            {
                request.Headers.Set("token", token);
            }

            //发送POST数据  
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            return GetResponseString(request.GetResponse() as HttpWebResponse);
        }

        /// <summary>
        /// 带header的post
        /// </summary>
        /// <param name="url"></param>
        /// <param name="jsonStr"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static string PostByJson(string url, string jsonStr,
            Dictionary<string, string> dictHeaders, int timeout = 60000)
        {
            byte[] data = Encoding.UTF8.GetBytes(jsonStr);

            //跳过验证
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback =
                        new RemoteCertificateValidationCallback(CheckValidationResult);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
            }
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentLength = data.Length;
            request.ContentType = "application/json";
            request.Timeout = timeout;
            request.ReadWriteTimeout = 10000;
            if (dictHeaders?.Count > 0)
            {
                foreach (var item in dictHeaders)
                {
                    request.Headers.Set(item.Key, item.Value);
                }
            }

            //发送POST数据  
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            return GetResponseString(request.GetResponse() as HttpWebResponse);
        }

        public static string Get(string url, string reqstr, string token)
        {
            string content = string.Empty;
            try
            {
                string fullUrl = string.Format("{0}?{1}", url, reqstr);
                var request = HttpWebRequest.Create(fullUrl) as HttpWebRequest;
                request.Timeout = 10000;
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers.Add("token", token);
                using (var response = request.GetResponse() as HttpWebResponse)
                using (var sr = new StreamReader(response.GetResponseStream(), httpEncoding))
                {
                    content = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0} {1}", ex.Message, url));
            }
            return content;
        }

        /// <summary>
        /// put方式
        /// </summary>
        /// <param name="url"></param>
        /// <param name="reqstr"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string PutJson(string url, string jsonStr, string token = "", int timeout = 60000)
        {
            string content = string.Empty;
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(jsonStr);
                var request = HttpWebRequest.Create(url) as HttpWebRequest;
                request.Timeout = timeout;
                request.Method = "PUT";
                request.ContentType = "application/json";
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Add("token", token);
                }
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                using (var sr = new StreamReader(response.GetResponseStream(), httpEncoding))
                {
                    content = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0} {1}", ex.Message, url));
            }
            return content;
        }
        public static string PostHttp(string url, string body, string contentType = "application/x-www-form-urlencoded", int timeOut = 60000)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.ContentType = contentType;
            httpWebRequest.Method = "POST";
            httpWebRequest.Timeout = timeOut;

            byte[] btBodys = Encoding.UTF8.GetBytes(body);
            httpWebRequest.ContentLength = btBodys.Length;
            httpWebRequest.GetRequestStream().Write(btBodys, 0, btBodys.Length);

            //HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            //StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string responseContent = string.Empty; //streamReader.ReadToEnd();

            using (var response = httpWebRequest.GetResponse() as HttpWebResponse)
            using (var sr = new StreamReader(response.GetResponseStream(), httpEncoding))
            {
                responseContent = sr.ReadToEnd();
            }

            return responseContent;
        }

        /// <summary>
        /// 请求并获取Json应答
        /// </summary>
        /// <param name="serverPortal"></param>
        /// <param name="methodName"></param>
        /// <param name="argsStr"></param>
        /// <returns></returns>
        public static string PostJsonContent(string url, object req, int timeOut = 60000)
        {
            JsonSerializerSettings jsonSetting = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var str = JsonConvertUtils.ObjToJson(req, Formatting.None, jsonSetting);

            var item = new HttpItem()
            {
                ContentType = "application/x-www-form-urlencoded",
                Method = "POST",
                Postdata = "request=" + str,
                URL = url,
                Timeout = timeOut,
                Encoding = httpEncoding,
                PostEncoding = httpEncoding,
            };
            var helper = new HttpHelper();
            var result = helper.GetHtml(item).Html;
            return result;
        }
        #endregion

        /// <summary>
        /// 获取短连接
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetShortUrl(string shortLinkReqUrl, string url)
        {
            var shortUrl = "";
            try
            {
                var shortLinkResult = PostByJson(shortLinkReqUrl,
                   Newtonsoft.Json.JsonConvert.SerializeObject(new { lengthLink = url }));
                var shortLinkModel = JsonConvertUtils.DesAnonymousType(shortLinkResult, new { shortLink = "" });
                if (shortLinkModel != null)
                {
                    shortUrl = shortLinkModel.shortLink;
                }
            }
            catch (Exception e)
            {
                e.SaveLog();
                shortUrl = "";
            }

            return shortUrl;
        }
    }
}
