using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;

namespace YFPos.Utils
{

    public class ConfigurationUpdater
    {
        /// <summary>
        /// 更新数据库连接字符串
        /// </summary>
        /// <param name="name"></param>
        /// <param name="connectionString"></param>
        public static void UpdateConnectionStrings(string name, string connectionString)
        {
            UpdateConnectionStrings(name, connectionString, "System.Data.SqlClient");
        }
        /// <summary>
        /// 更新数据库连接字符串
        /// </summary>
        /// <param name="name"></param>
        /// <param name="connectionString"></param>
        /// <param name="providerName">System.Data.SqlClient</param>
        public static void UpdateConnectionStrings(string name, string connectionString, string providerName)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (ConfigurationManager.ConnectionStrings[name] != null)
            {
                config.ConnectionStrings.ConnectionStrings.Remove(name);
            }
            var newConnectionSettings = new ConnectionStringSettings(name, connectionString, providerName);
            config.ConnectionStrings.ConnectionStrings.Add(newConnectionSettings);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("ConnectionStrings");
        }

        /// <summary>
        /// 更新AppSettings
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void UpdateAppSettings(string name, string value)
        {
            if (ConfigurationManager.AppSettings[name] == value)
            {
                return;
            }
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (ConfigurationManager.AppSettings[name] != null)
            {
                config.AppSettings.Settings.Remove(name);
            }
            config.AppSettings.Settings.Add(name, value);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        /// <summary>
        /// 移除节点
        /// </summary>
        /// <param name="name">节点名称</param>
        public static void RemoveAppSettings(string name)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (ConfigurationManager.AppSettings[name] != null)
            {
                config.AppSettings.Settings.Remove(name);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
        }
    }
}
