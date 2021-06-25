using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace YFPos.Utils
{
    /// <summary>
    /// ini 文件读写帮助类
    /// </summary>
    public class IniUtils
    {
        // 声明INI文件的写操作函数 WritePrivateProfileString()
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        // 声明INI文件的读操作函数 GetPrivateProfileString()
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// 写入ini文档
        /// </summary>
        /// <param name="path">ini文档路径</param>
        /// <param name="section">片段</param>
        /// <param name="key">关键字</param>
        /// <param name="value">值</param>
        public static void Write(string path, string section, string key, string value)
        {
            // section=配置节，key=键名，value=键值，path=路径
            WritePrivateProfileString(section, key, value, path);
        }
        /// <summary>
        /// 读ini文件
        /// </summary>
        /// <param name="path">ini文档路径</param>
        /// <param name="section">片段</param>
        /// <param name="key">关键字</param>
        /// <returns>值</returns>
        public static string Read(string path, string section, string key)
        {
            // 每次从ini中读取多少字节
            StringBuilder sb = new StringBuilder(255);
            // section=配置节，key=键名，temp=上面，path=路径
            GetPrivateProfileString(section, key, string.Empty, sb, 255, path);
            return sb.ToString();
        }
    }
}
