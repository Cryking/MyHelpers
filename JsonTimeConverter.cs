using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YFPos.Utils
{


    /// <summary>
    /// Json日期格式化Converter
    /// 格式为：yyyy-MM-dd HH:mm:ss
    /// 
    /// </summary>
    public class DateFormatConverter_yyyy_MM_dd_HH_mm_ss : Newtonsoft.Json.Converters.IsoDateTimeConverter
    {
        public DateFormatConverter_yyyy_MM_dd_HH_mm_ss()
        {
            DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        }

    }
    /// <summary>
    /// Json日期格式化Converter
    /// 格式为：yyyyMMddHHmmssSSS
    /// </summary>
    public class DateFormatConverter_yyyyMMddHHmmssSSS : Newtonsoft.Json.Converters.IsoDateTimeConverter
    {
        public DateFormatConverter_yyyyMMddHHmmssSSS()
        {
            DateTimeFormat = "yyyyMMddHHmmssSSS";
        }
    }
    #region 时间戳转换
    /// <summary>
    /// 毫秒时间戳
    /// </summary>
    public class UnixDateTimeConverterMillisecond : IsoDateTimeConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, /*object existingValue,*/ JsonSerializer serializer)
        {
            var value = reader.Value.ToString();
            if (!new Regex("^(0|[1-9][0-9]*|-[1-9][0-9]*)$").IsMatch(value))
            {
                throw new Exception(String.Format("Unix时间戳转换DateTime必需为全数字，当前值为：{0}.", reader.Value));
            }

            var ticks = 0L;
            ticks = long.Parse(value);


            var date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToLocalTime();
            date = date.AddMilliseconds(ticks);
            return date;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            long ticks;
            if (value is DateTime)
            {
                //var epoc = new DateTime(1970, 1, 1);
                var epoc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToLocalTime();
                var delta = ((DateTime)value) - epoc;
                if (delta.TotalSeconds < 0)
                {
                    throw new ArgumentOutOfRangeException("时间格式错误.1");
                }
                ticks = (long)delta.TotalMilliseconds;
            }
            else
            {
                throw new Exception("时间格式错误.2");
            }
            writer.WriteValue(ticks);
        }
    }
    /// <summary>
    /// 秒时间戳
    /// </summary>
    public class UnixDateTimeConverterSecond : IsoDateTimeConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, /*object existingValue,*/JsonSerializer serializer)
        {
            var value = reader.Value.ToString();
            if (!new Regex("^(0|[1-9][0-9]*|-[1-9][0-9]*)$").IsMatch(value))
            {
                throw new Exception(String.Format("Unix时间戳转换DateTime必需为全数字，当前值为：{0}.", reader.Value));
            }

            var ticks = 0L;
            ticks = long.Parse(value);


            var date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToLocalTime();
            date = date.AddSeconds(ticks);
            return date;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            long ticks;
            if (value is DateTime)
            {
                //var epoc = new DateTime(1970, 1, 1);

                var epoc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToLocalTime();
                var delta = ((DateTime)value) - epoc;
                if (delta.TotalSeconds < 0)
                {
                    throw new ArgumentOutOfRangeException("时间格式错误.1");
                }
                ticks = (long)delta.TotalSeconds;
            }
            else
            {
                throw new Exception("时间格式错误.2");
            }
            writer.WriteValue(ticks);
        }
    }
    #endregion
}