using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YFPos.Utils
{
    /// <summary>
    /// 枚举操作工具类
    /// </summary>
    public class EnumHelper
    {
        /// <summary>
        /// 根据枚举名称获得整型值
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="enumName"></param>
        public static string GetCodeFromName(Type enumType, string enumName)
        {
            string retCode = "";

            if (enumType != null && !string.IsNullOrEmpty(enumName) && System.Enum.IsDefined(enumType, enumName))
            {
                try
                {
                    retCode = ((int)System.Enum.Parse(enumType, enumName)).ToString();
                }
                catch
                {

                    retCode = "";
                }
            }

            return retCode;
        }

        /// <summary>
        /// 根据枚举数字获取名称
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetNameFromCode(Type enumType, string code)
        {
            string retName = code??"";
            int enumCode = 0;
            if (!string.IsNullOrEmpty(code) && int.TryParse(code, out enumCode))
            {
                retName = Enum.GetName(enumType, enumCode);
            }
            return retName;
        }
    }
}
