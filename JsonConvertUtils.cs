using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;

namespace YFPos.Utils
{
    public class JsonConvertUtils
    {
        #region json
        /// <summary>
        /// 对象转JSON
        /// </summary>
        /// <returns></returns>
        public static string ObjToJson(object obj)
        {
            if (obj == null)
                return null;
            //日期转换器
            var timeConverter = new IsoDateTimeConverter();
            //日期转换格式
            timeConverter.DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
            return JsonConvert.SerializeObject(obj, timeConverter);
        }

        /// <summary>
        /// json转为匿名对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="anonymousTypeObject"></param>
        /// <returns></returns>
        public static T DesAnonymousType<T>(string json, T anonymousTypeObject)
        {
            return JsonConvert.DeserializeAnonymousType(json, anonymousTypeObject);
        }

        /// <summary>
        /// 对象转JSON
        /// </summary>
        /// <returns></returns>
        public static string ObjToJson(object obj, Newtonsoft.Json.Formatting formatting, JsonSerializerSettings settings)
        {
            if (obj == null)
                return null;
            return JsonConvert.SerializeObject(obj, formatting, settings);
        }

        /// <summary>
        /// 对象转JSON,首字母小写
        /// </summary>
        /// <returns></returns>
        public static string ObjToJsonLowercase(object obj)
        {
            if (obj == null)
                return null;
            JsonConverter[] converters = {
                new IsoDateTimeConverter()
                { DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss" }
            };
            return JsonConvert.SerializeObject(obj, Formatting.None,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Converters = converters
                });
        }

        /// <summary>
        /// JSON转对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="json">json字符串</param>
        /// <returns></returns>
        public static T JsonToObject<T>(string json, string dateFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss")
        {
            //日期转换器
            var timeConverter = new IsoDateTimeConverter();
            //日期转换格式
            timeConverter.DateTimeFormat = dateFormat;
            T deserializedProduct = JsonConvert.DeserializeObject<T>(json, timeConverter);
            return deserializedProduct;
        }
        /// <summary>
        /// JSON转对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="json">json字符串</param>
        /// <returns></returns>
        public static T JsonToObject<T>(string json)
        {
            //日期转换器
            var timeConverter = new IsoDateTimeConverter();
            T deserializedProduct=default(T);
            try
            {
                //日期转换格式
                timeConverter.DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
                deserializedProduct = JsonConvert.DeserializeObject<T>(json, timeConverter);
            }
            catch(Exception e)
            {
                deserializedProduct = JsonConvert.DeserializeObject<T>(json);
            }

            return deserializedProduct;
        }

        /// <summary>
        /// 深度拷贝对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">对象实例</param>
        /// <returns>拷贝结果</returns>
        public static T CopyObject<T>(T obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            T deserializedProduct = JsonConvert.DeserializeObject<T>(json);
            return deserializedProduct;
        }

        /// <summary>
        /// JSON转xlm
        /// </summary>
        public static XmlDocument JsonToXml(string json, string RootElementName, bool writeArrayAttribute)
        {
            XmlDocument xmldoc = new XmlDocument();
            XmlNode xmlNode = JsonConvert.DeserializeXmlNode(json, RootElementName);
            xmldoc.LoadXml(xmlNode.InnerXml);
            return xmldoc;
        }

        public static string XmlToJson(XmlDocument doc)
        {
            return JsonConvert.SerializeXmlNode(doc);
        }

        #endregion
    }

    /// <summary>
    /// 类对应属性COPY扩展方法,暂时还不支持嵌套对象的赋值
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// 将泛型中的第两个对象值COPY到第一个对象中(将对象S赋值到T中)
        /// </summary>
        /// <typeparam name="T">要赋值对象</typeparam>
        /// <typeparam name="S">赋值来源</typeparam>
        /// <param name="FromObj">对象值来源</param>
        /// <param name="ToObj">要赋值对象</param>
        /// <returns></returns>
        public static T PropertyMapTo<T, S>(this S FromObj,T ToObj) where T:class
        {
            if (FromObj == null)
            {
                return default(T);
            }
            Type tpFrom = FromObj.GetType();
            List<System.Reflection.PropertyInfo> fromPro = new List<System.Reflection.PropertyInfo>(tpFrom.GetProperties());
            if (fromPro != null && fromPro.Count > 0)
            {
                fromPro.ForEach(item => {
                    var toProInfo = ToObj.GetType().GetProperties().Where(toItem=>string.Equals(toItem.Name,item.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                    if (toProInfo != null && toProInfo.CanWrite)
                    {
                        var value = item.GetValue(FromObj,null);
                        if (!toProInfo.PropertyType.IsGenericType)
                        { 
                            //非泛型
                            toProInfo.SetValue(ToObj, value == null || string.IsNullOrEmpty(value.ToString()) ? null : Convert.ChangeType(value, toProInfo.PropertyType), null);
                        }
                        else
                        {
                            //泛型Nullable<>
                            Type genericTypeDefinition = toProInfo.PropertyType.GetGenericTypeDefinition();
                            if (genericTypeDefinition == typeof(Nullable<>))
                            {
                                var valueTemp = value==null || string.IsNullOrEmpty(value.ToString()) ? null : Convert.ChangeType(value, Nullable.GetUnderlyingType(toProInfo.PropertyType));
                                try
                                {
                                    toProInfo.SetValue(ToObj, valueTemp, null);
                                }
                                catch { }
                            }
                        }
                    }
                });
            }
            return ToObj;
        }
    }
}
