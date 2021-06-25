using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace YFPos.Utils
{

    /// <summary>
    /// Xml序列化
    /// </summary>
    public static class XmlSerializerHelper
    {
        /// <summary>
        /// 将对象序列化为Xml
        /// </summary>
        /// <typeparam name="T">对象实例对应的类.T的修饰符应当为Public</typeparam>
        /// <param name="obj">对象实例</param>
        /// <returns>序列化后的字符串</returns>
        public static string ToXml<T>(this T obj, string encoding = "GBK")
        {

            XmlSerializer xs = new XmlSerializer(obj.GetType());
            StringBuilder stb = new StringBuilder();
            var settings = new XmlWriterSettings() { Encoding = Encoding.GetEncoding(encoding) };
            settings.OmitXmlDeclaration = false;
            XmlWriter xw = XmlWriter.Create(stb, settings);
            xs.Serialize(xw, obj);
            return stb.ToString();



            //if (obj == null) throw new NullReferenceException();
            //XmlSerializer serializer = new XmlSerializer(typeof(T));

            //using (StringWriter writer = new StringWriter())
            //{
            //    //writer.Encoding = System.Text.Encoding.GetEncoding("GBK");
            //    serializer.Serialize(writer, obj);
            //    return writer.ToString();
            //}



            //var settings = new XmlWriterSettings
            //{
            //    Encoding = Encoding.GetEncoding("GBK"),
            //    OmitXmlDeclaration = false
            //};
            //using (var stringWriter = new StringWriter())
            //{
            //    using (var writer = XmlWriter.Create(stringWriter, settings))
            //    {
            //        var ns = new XmlSerializerNamespaces();
            //        //ns.Add(string.Empty, myns);
            //        var xmlSerializer = new XmlSerializer(obj.GetType(), "root");
            //        xmlSerializer.Serialize(writer, obj);
            //    }
            //    var xmlString = stringWriter.ToString();
            //    return xmlString;
            //}




        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlDocument"></param>
        /// <returns></returns>
        public static T ToInstanceFromXml<T>(this string xmlDocument)
        {
            using (StringReader reader = new StringReader(xmlDocument))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                var obj = serializer.Deserialize(reader);
                if (obj is T)
                {
                    return (T)obj;
                }

                return default(T);
            }
        }

        /// <summary>
        /// 将对象序列化为Xml
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <returns>序列化后的字符串</returns>
        public static string ToXml(this object obj)
        {
            return ToXml(obj, Encoding.UTF8);
        }

        /// <summary>
        /// 将对象序列化为Xml
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <param name="encoding">编码格式</param>
        /// <returns>序列化后的字符串</returns>
        public static string ToXml(this object obj, Encoding encoding)
        {
            if (obj == null)
            {
                throw new ArgumentException("对象实例不能为空");
            }

            if (encoding == null)
            {
                throw new Exception("编码不能为空");
            }

            using (var stream = new MemoryStream())
            {
                var serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(stream, obj);
                stream.Position = 0;
                return encoding.GetString(stream.ToArray());
            }
        }

    }
}
