using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.International.Converters.PinYinConverter;

namespace YFPos.Utils
{
    public class ChineseHelper
    {
        /// <summary>
        /// 获取拼音首字母组合
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public List<Char> GetChineseCharPinYinFirstLetters(Char ch)
        {
            var hls = new List<Char>();
            if (ChineseChar.IsValidChar(ch))
            {
                var cc = new ChineseChar(ch);
                foreach (string py in cc.Pinyins)
                {
                    if (!hls.Contains(py[0]))
                    {
                        hls.Add(py[0]);
                    }
                }
            }
            return hls;
        }

        /// <summary>
        /// 获取默认拼音首字母
        /// </summary>
        /// <param name="ch">汉字</param>
        /// <returns></returns>
        public static Char[] GetFirstLetters(Char ch)
        {
            if (ChineseChar.IsValidChar(ch))
            {
                var cc = new ChineseChar(ch);
                var items = from item in cc.Pinyins.Take(cc.PinyinCount)
                            select item[0];
                return items.ToArray();
            }
            return new char[0];
        }

        /// <summary>
        /// 获取字符串默认拼音首字母
        /// </summary>
        /// <param name="chs">中文字符串</param>
        /// <returns></returns>
        public String GetChineseStringPinYinDefaultFirstLetter(string chs)
        {
            var result = string.Empty;
            foreach (char ch in chs)
            {
                if (ChineseChar.IsValidChar(ch))
                {
                    var cc = new ChineseChar(ch);
                    result += cc.Pinyins[0][0];
                }
            }
            return result;
        }


        public static Char[] GetinYinDefaultFirstLetter(char ch)
        {
            if (ChineseChar.IsValidChar(ch))
            {
                var cc = new ChineseChar(ch);
                return cc.Pinyins.Select(item => item[0]).ToArray();
            }
            else
            {
                return new char[0];
            }
        }

        /// <summary>
        /// 获取默认拼音（无声调）
        /// </summary>
        /// <returns></returns>
        public String GetDefaultPinyinsWithOutSound(string chs)
        {
            var result = string.Empty;
            foreach (char ch in chs)
            {
                if (ChineseChar.IsValidChar(ch))
                {
                    var py = new ChineseChar(ch).Pinyins[0];
                    result += py.Remove(py.Length - 1);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取中国大写数字
        /// </summary>
        /// <param name="n">单个数字</param>
        /// <returns>中国大写数字</returns>
        public static char GetUpperDigit(char n)
        {
            switch (n)
            {
                case '1':
                    return '壹';
                case '2':
                    return '贰';
                case '3':
                    return '叁';
                case '4':
                    return '肆';
                case '5':
                    return '伍';
                case '6':
                    return '陆';
                case '7':
                    return '柒';
                case '8':
                    return '捌';
                case '9':
                    return '玖';
                default:
                    return '零';
            }
        }

        /// <summary>
        /// 获取大写数字
        /// </summary>
        /// <param name="digits">数字串（整数，亿以内）</param>
        /// <returns></returns>
        public static string GetUpperDigits(string digits)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < digits.Length; i++)
            {
                char digitChar = digits[i];
                if (digitChar != '0'
                    || digits.Length == 1
                    || (i - 1 >= 0 && digits[i - 1] != '0' && i + 1 < digits.Length && digits[i + 1] != '0'))
                {
                    sb.Append(GetUpperDigit(digitChar));
                }
                if (digitChar == '0')
                {
                    continue;
                }
                switch ((digits.Length - i - 1) % 6)
                {
                    case 1:
                        sb.Append("拾");
                        break;
                    case 2:
                        sb.Append("佰");
                        break;
                    case 3:
                        sb.Append("仟");
                        break;
                    case 4:
                        sb.Append("万");
                        break;
                    case 5:
                        sb.Append("亿");
                        break;
                }
            }
            return sb.ToString();
        }


        /// <summary>
        /// 转换人民币大小金额
        /// </summary>
        /// <param name="num">金额</param>
        /// <returns>返回大写形式</returns>
        public static string GetAmountCH(decimal num)
        {
            string str1 = "零壹贰叁肆伍陆柒捌玖";            //0-9所对应的汉字
            string str2 = "万仟佰拾亿仟佰拾万仟佰拾元角分"; //数字位所对应的汉字
            string str3 = "";    //从原num值中取出的值
            string str4 = "";    //数字的字符串形式
            string str5 = "";  //人民币大写金额形式
            int i;    //循环变量
            int j;    //num的值乘以100的字符串长度
            string ch1 = "";    //数字的汉语读法
            string ch2 = "";    //数字位的汉字读法
            int nzero = 0;  //用来计算连续的零值是几个
            int temp;            //从原num值中取出的值

            num = Math.Round(Math.Abs(num), 2);    //将num取绝对值并四舍五入取2位小数
            str4 = ((long)(num * 100)).ToString();        //将num乘100并转换成字符串形式
            j = str4.Length;      //找出最高位
            if (j > 15) { return "溢出"; }
            str2 = str2.Substring(15 - j);   //取出对应位数的str2的值。如：200.55,j为5所以str2=佰拾元角分

            //循环取出每一位需要转换的值
            for (i = 0; i < j; i++)
            {
                str3 = str4.Substring(i, 1);          //取出需转换的某一位的值
                temp = Convert.ToInt32(str3);      //转换为数字
                if (i != (j - 3) && i != (j - 7) && i != (j - 11) && i != (j - 15))
                {
                    //当所取位数不为元、万、亿、万亿上的数字时
                    if (str3 == "0")
                    {
                        ch1 = "";
                        ch2 = "";
                        nzero = nzero + 1;
                    }
                    else
                    {
                        if (str3 != "0" && nzero != 0)
                        {
                            ch1 = "零" + str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                        else
                        {
                            ch1 = str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                    }
                }
                else
                {
                    //该位是万亿，亿，万，元位等关键位
                    if (str3 != "0" && nzero != 0)
                    {
                        ch1 = "零" + str1.Substring(temp * 1, 1);
                        ch2 = str2.Substring(i, 1);
                        nzero = 0;
                    }
                    else
                    {
                        if (str3 != "0" && nzero == 0)
                        {
                            ch1 = str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                        else
                        {
                            if (str3 == "0" && nzero >= 3)
                            {
                                ch1 = "";
                                ch2 = "";
                                nzero = nzero + 1;
                            }
                            else
                            {
                                if (j >= 11)
                                {
                                    ch1 = "";
                                    nzero = nzero + 1;
                                }
                                else
                                {
                                    ch1 = "";
                                    ch2 = str2.Substring(i, 1);
                                    nzero = nzero + 1;
                                }
                            }
                        }
                    }
                }
                if (i == (j - 11) || i == (j - 3))
                {
                    //如果该位是亿位或元位，则必须写上
                    ch2 = str2.Substring(i, 1);
                }
                str5 = str5 + ch1 + ch2;

                if (i == j - 1 && str3 == "0")
                {
                    //最后一位（分）为0时，加上“整”
                    str5 = str5 + '整';
                }
            }
            if (num == 0)
            {
                str5 = "零元整";
            }
            return str5;
        }

        /// <summary>
        /// 转换人民币大小金额
        /// </summary>
        /// <param name="num">金额</param>
        /// <returns>返回大写形式</returns>
        public static string GetAmountCH(string numstr)
        {
            try
            {
                decimal num = Convert.ToDecimal(numstr);
                return GetAmountCH(num);
            }
            catch
            {
                return "非数字形式！";
            }
        }
    }
}
