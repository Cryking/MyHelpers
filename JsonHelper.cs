using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace YFPos.Utils
{
    public class JsonHelper
    {
        /// <summary>
        /// 定义转换的字符编码
        /// </summary>
        private static Encoding m_Encoding = Encoding.UTF8;

        /// <summary>
        /// 生成Json格式
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ObjectToJson(object obj)
        {
            string result = string.Empty;
            try
            {
                var json = new DataContractJsonSerializer(obj.GetType());

                using (var ms = new MemoryStream())
                {
                    json.WriteObject(ms, obj);
                    result = m_Encoding.GetString(ms.ToArray());
                }
            }
            catch (Exception ex)
            {
                ex.SaveLog();
                throw;
            }
            return result;
        }

        /// <summary>
        /// 对象转JSON 属性首字母小写
        /// </summary>
        /// <returns></returns>
        public static string ObjToJsonCamelCase(object obj)
        {
            if (obj == null)
                return null;
            //首字母小写
            var jsonSetting = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            //日期转换器
            var timeConverter = new IsoDateTimeConverter();
            //日期转换格式
            timeConverter.DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
            jsonSetting.Converters.Add(timeConverter);

            return JsonConvert.SerializeObject(obj, Formatting.None, jsonSetting);
        }

        /// <summary>
        /// 生成Json格式MemoryStream
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>MemoryStream</returns>
        public static MemoryStream ObjectToJsonStream(object obj)
        {
            MemoryStream result = null;
            try
            {
                var json = new DataContractJsonSerializer(obj.GetType());
                result = new MemoryStream();
                json.WriteObject(result, obj);
            }
            catch (Exception ex)
            {
                ex.SaveLog();
                throw;
            }
            return result;
        }

        /// <summary>
        /// 获取Json的Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonContent"></param>
        /// <returns></returns>
        public static T JsonToObject<T>(string jsonContent)
        {
            T obj = Activator.CreateInstance<T>();
            using (var ms = new MemoryStream(m_Encoding.GetBytes(jsonContent)))
            {
                var serializer = new DataContractJsonSerializer(obj.GetType());
                return (T)serializer.ReadObject(ms);
            }
        }

        /// <summary>
        /// 获取Json的Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonContent"></param>
        /// <returns></returns>
        public static T JsonToObject<T>(Stream jsonStream)
        {
            T obj = Activator.CreateInstance<T>();
            jsonStream.Position = 0;
            var serializer = new DataContractJsonSerializer(obj.GetType());
            obj = (T)serializer.ReadObject(jsonStream);
            jsonStream.Dispose();
            return obj;
        }

        /// <summary>
        /// 将json数据转换成实体类(列表)  
        /// </summary>
        /// <returns></returns>
        public static List<T> GetObjectByJson<T>(string jsonString)
        {
            // 实例化DataContractJsonSerializer对象，需要待序列化的对象类型
            List<T> ls;
            try
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<T>));
                //把Json传入内存流中保存
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
                // 使用ReadObject方法反序列化成对象
                object ob = serializer.ReadObject(stream);
                ls = (List<T>)ob;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return ls;
        }
    }
}
