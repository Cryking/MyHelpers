using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YFPos.Utils
{
    /// <summary>
    /// 注册表修改
    /// </summary>
    public class RegUtils
    {
        public const string MySqlSubKeyName = @"Software\YfPos";
        public const string MySqlRegistryKeyName = "MySqlPwd";
        private const string DefaultMySqlPwd = "abc";

        /// <summary>
        ///设置应用程序为开机启动 
        /// </summary>
        /// <param name="keyName">键名</param>
        /// <param name="filePath">应用程序</param>
        /// <returns>是否成功</returns>
        public static bool SetAutoRun(string keyName,
            string filePath)
        {
            try
            {
                RegistryKey runKey = Registry.LocalMachine.OpenSubKey(
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                runKey.SetValue(keyName, filePath);
                runKey.Close();
            }
            catch(Exception ex)
            {
                ex.SaveLog();
                return false;
            }
            return true;
        }
        /// <summary>
        /// 从注册表中获取mysql密码
        /// </summary>
        /// <returns></returns>
        public static string GetLocalMySqlPwd()
        {
            try
            {
                using (RegistryKey pwdKey = Registry.CurrentUser.OpenSubKey(MySqlSubKeyName))
                {
                    if (pwdKey == null)
                    {
                        return DefaultMySqlPwd;
                    }
                    else
                    {
                        var pwd = pwdKey.GetValue(MySqlRegistryKeyName);
                        if (pwd == null)
                        {
                            SetMysqlPwd(DefaultMySqlPwd);
                            return DefaultMySqlPwd;
                        }
                        else
                        {
                            return pwd.ToString();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.SaveLog();
                return DefaultMySqlPwd;
            }
        }

        /// <summary>
        /// 设置mysql密码到注册表
        /// </summary>
        /// <param name="password"></param>
        public static void SetMysqlPwd(string password)
        {
            RegistryKey pwdKey = Registry.CurrentUser.OpenSubKey(MySqlSubKeyName, true);

            if (pwdKey == null)
            {
                pwdKey = Registry.CurrentUser.CreateSubKey(MySqlSubKeyName);

            }
            //设置键值
            pwdKey.SetValue(MySqlRegistryKeyName, password);

            pwdKey.Close();
        }
    }
}
