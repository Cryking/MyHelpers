using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YFPos.Utils
{
    /// <summary>
    /// 字符串帮助类
    /// </summary>
    public static class StringUtils
    {
        #region //判断部分
        /// <summary>Returns true if the two strings match.</summary>
        /// <param name="firstString">First string</param>
        /// <param name="secondString">Second string</param>
        /// <param name="ignoreCase">Should case (upper/lower) be ignored?</param>
        /// <returns>True or False</returns>
        /// <remarks>The strings are trimmed and compared in a case-insensitive, culture neutral fashion.</remarks>
        public static bool Compare(string firstString, string secondString, bool ignoreCase)
        {
            int pos = string.Compare(firstString.Trim(), secondString.Trim(), ignoreCase, CultureInfo.InvariantCulture);
            return (pos == 0);
        }
        /// <summary>
        /// 是否包含中文字符
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsChineseLetter(string text)
        {
            bool isChinese = false;
            for (int i = 0; i < text.Length; i++)
            {
                Regex rx = new Regex("^[\u4e00-\u9fa5]$");
                if (rx.IsMatch(text[i].ToString(CultureInfo.InvariantCulture)))
                {
                    isChinese = true;
                    break;
                }
            }
            return isChinese;
        }


        /// <summary>
        /// 判断字符串中是否为数字、字母、汉字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumericEnCn(this string value)
        {
            if (value != null)
            {
                return Regex.IsMatch(value, @"^[a-zA-Z0-9\u4e00-\u9fa5 ]*$");
            }
            return false;
        }

        /// <summary>
        /// 字段串是否为Null或为""(空)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEmpty(this string str)
        {
            if (str == null || str.Trim() == string.Empty)
                return true;

            return false;
        }

        /// <summary>
        /// 判断字符串是否为数字
        /// </summary>
        /// <returns></returns>
        public static bool IsNumeric(string value)
        {
            if (value != null)
            {
                if (value.Length > 0 && value.Length <= 11 && Regex.IsMatch(value, @"^[-]?[0-9]*[.]?[0-9]*$"))
                {
                    if ((value.Length < 10) || (value.Length == 10 && value[0] == '1') || (value.Length == 11 && value[0] == '-' && value[1] == '1'))
                    {
                        return true;
                    }
                }
            }
            return false;

        }

        /// <summary>
        /// 是否为Double类型
        /// </summary>
        /// <returns></returns>
        public static bool IsDouble(this string value)
        {
            if (value != null)
            {
                return Regex.IsMatch(value, @"^([0-9])[0-9]*(\.\w*)?$");
            }
            return false;
        }

        /// <summary>
        /// 字符串是否是字符和数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsCharAndNumber(this string str)
        {
            if (String.IsNullOrEmpty(str))
                return false;

            Regex regex = new Regex(@"^[a-zA-Z0-9]*$", RegexOptions.Compiled);
            return regex.IsMatch(str);
        }

        /// <summary>
        /// 判断字符串是否为文件名
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool IsFileName(this string filename)
        {
            filename = filename.Trim();
            int index = filename.LastIndexOf(".", StringComparison.Ordinal);//获取最后一个.的位置
            if (index > 0)
            {
                string endStr = filename.Substring(index);//获取最后一个.后面的所有字符
                return endStr.IsCharAndNumber();//判断.后字符是否为字符或数组，如果是，则认为是文件类型，否则为否
            }
            return false;
        }

        /// <summary>
        /// 判断文件名是否为浏览器可以直接显示的图片文件名
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>是否可以直接显示</returns>
        public static bool IsImageName(this string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return false;

            filename = filename.Trim();
            if (filename.EndsWith(".") || filename.IndexOf(".", StringComparison.Ordinal) == -1)
                return false;

            string extname = filename.Substring(filename.LastIndexOf(".", StringComparison.Ordinal) + 1).ToLower();
            //增加两种格式，与资源播放器一致
            return (extname == "jpg" || extname == "jpeg" || extname == "png" || extname == "bmp" || extname == "gif" || extname == "img" || extname == "image");
        }

        /// <summary>
        /// 检测是否符合email格式
        /// </summary>
        /// <param name="strEmail">要判断的email字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsEmail(this string strEmail)
        {
            return Regex.IsMatch(strEmail, @"^[\w\.]+([-]\w+)*@[A-Za-z0-9-_]+[\.][A-Za-z0-9-_]");
        }

        /// <summary>
        /// 检测是否是正确的Url
        /// </summary>
        /// <param name="strUrl">要验证的Url</param>
        /// <returns>判断结果</returns>
        public static bool IsURL(this string strUrl)
        {
            return Regex.IsMatch(strUrl, @"^(http|https)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{1,10}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&%\$#\=~_\-]+))*$");
        }

        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(this string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        /// <summary>
        /// 判断字符串是否包含所匹配的字符
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="charts">匹配字符组</param>
        /// <returns></returns>
        public static bool IsHasChars(this string source, string charts)
        {
            return charts.Any(chart => source.IndexOf(chart) > -1);
        }

        #endregion

        /// <summary>
        /// 字符转为数字decimal
        /// </summary>
        /// <param name="sValue"></param>
        /// <returns></returns>
        public static decimal ConverStrToInt(string sValue)
        {
            decimal dRet = 0m;
            if (!decimal.TryParse(sValue, out dRet))
            {
                dRet = 0m;
            }

            return dRet;
        }

        /// <summary>
        /// 隐私格式化
        ///     自定义规则： startIndex + mosaicCount + mosaicWord(起始位置，格式化长度，格式化替换字符)
        ///     字段说明：mosaicCount x 代表不定长
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public static string Mosaic(string mosaicFormat, string content)
        {
            string result = string.Empty;
            if (mosaicFormat.Trim() == string.Empty 
                || string.IsNullOrEmpty(content?.Trim()))
                return content;
            List<string> dataList = mosaicFormat.Split('+').ToList();
            if (dataList.Count != 3)
                return content;
            //起始位置
            int startIndex = 0;
            //格式化长度
            int mosaicCount = 0;
            //格式化替换字符
            string mosaicWord = dataList[2];
            try
            {
                startIndex = Convert.ToInt32(dataList[0]);
                if (dataList[1] == "x")
                {
                    mosaicCount = content.Length - 1;
                }
                else
                {
                    mosaicCount = Convert.ToInt32(dataList[1]);
                }
            }
            catch
            {
                return content;
            }

            if ((startIndex + mosaicCount) < 0)
                return content;

            if ((startIndex + mosaicCount) > content.Length)
                return content;

            //起始字符
            string firstWord = content.Substring(0, startIndex);
            //结束字符
            string EndWord = content.Substring(firstWord.Length + mosaicCount, content.Length - (firstWord.Length + mosaicCount));
            //替代字符
            string mosaicWords = string.Empty;
            for (int i = 0; i < mosaicCount; i++)
            {
                mosaicWords = mosaicWords + mosaicWord;
            }

            result = firstWord + mosaicWords + EndWord;

            return result;
        }

        /// <summary>
        /// 是否为电话号码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static bool IsPhone(this string phone)
        {
            return Regex.IsMatch(phone, "^(((13[0-9])|(14[579])|(15([0-3]|[5-9]))|(16[6])|(17[01235678])|(18[0-9])|(19[189]))\\d{8})$");
        }

        /// <summary>
        /// 只能是中文
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsChinese(string text)
        {
            Regex rx = new Regex("^[\u4e00-\u9fa5]+$");
            return rx.IsMatch(text);
        }

        /// <summary>
        /// 只能是数字
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsNum(string text) {
            Regex rx = new Regex("[^0-9.-]+");
            return rx.IsMatch(text);
        }

        /// <summary>
        /// 获取指定后缀的字符串
        /// </summary>
        /// <param name="sourceStr"></param>
        /// <param name="length"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string GetBySuffixStr(string sourceStr, int length=0,string suffix="...")
        {
            var retStr = sourceStr;
            if (retStr.Length > length)
            {
                retStr = sourceStr.Substring(0, length)+ suffix;
            }
            return retStr;
        }
        private static char[] constant =     {
        '0','1','2','3','4','5','6','7','8','9',
        'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
        'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z' };

        /// <summary>
        /// 生成随机字符串
        /// </summary>
        /// <param name="Length">字符串长度</param>
        /// <returns></returns>
        public static string GenerateRandomNumber(int Length = 32)
        {
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(62);
            Random rd = new Random();
            for (int i = 0; i < Length; i++)
            {
                newRandom.Append(constant[rd.Next(62)]);
            }
            return newRandom.ToString();
        }
    }
}
