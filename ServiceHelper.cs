using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using Microsoft.Win32;

namespace YFPos.Utils
{
    public class ServiceHelper
    {
        /// <summary>
        /// function:获取系统服务所在目录
        /// <param name="serviceName">服务名称</param>
        /// <returns></returns>
        ///  Add BY CHENSHENG 20160108
        /// </summary>
        //-------------------------------
        public static string ServiceIsExisted(string serviceName)
        {
            string result = string.Empty;
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController s in services)
            {
                if (s.ServiceName == serviceName)
                {
                    if (FilePath(s.ServiceName) != "")
                    {
                        string fullName = FilePath(s.ServiceName);
                        result = fullName.Substring(0, fullName.IndexOf("--")).Trim().Trim(new char[] { '\"' });
                        result = result.Remove(result.LastIndexOf("\\"));
                    }
                }
            }
            return result;
        }

        public static string FilePath(string serviceName)
        {
            RegistryKey _Key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\ControlSet001\Services\" + serviceName);
            if (_Key != null)
            {
                object _ObjPath = _Key.GetValue("ImagePath");
                if (_ObjPath != null) return _ObjPath.ToString();
            }
            return "";

        }
        //--------------------------------

        /// <summary>
        /// 获取服务执行目录
        /// </summary>
        /// <param name="strServiceName">服务名称</param>
        /// <returns></returns>
        public static string getServicePath(string strServiceName)
        {
            string s = "Win32_service";
            string result = string.Empty;
            using (var mClass = new ManagementClass(s))
            {
                using (ManagementObjectCollection moc = mClass.GetInstances())
                {
                    foreach (ManagementObject mo in moc)
                    {
                        if (mo["Name"].ToString().Trim().ToUpper() == strServiceName.ToUpper())
                        {
                            string fullName = mo["PathName"].ToString();
                            result = fullName.Substring(0, fullName.IndexOf("--")).Trim().Trim(new char[] { '\"' });
                            result = result.Remove(result.LastIndexOf("\\"));
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="strServiceName">服务名</param>
        /// <returns></returns>
        public static ServiceController getServiceByName(string strServiceName)
        {
            try
            {
                var services = ServiceController.GetServices();
                var sc = services.FirstOrDefault(w => w.ServiceName.ToLower().Equals(strServiceName.ToLower().Trim()));
                if (sc != null && sc.Status != ServiceControllerStatus.Running)
                {
                    sc.Start();
                }
            
                return sc;
            }
            catch (Exception e)
            {
                e.SaveLog();
                return null;
            }
        }

        /// <summary>
        /// 重启服务
        /// </summary>
        /// <param name="svcName"></param>
        /// <returns></returns>
        public static bool ReStartService(string svcName)
        {
            ServiceController[] services = ServiceController.GetServices();
            var selectService = services.FirstOrDefault(w => w.ServiceName.ToLower().Trim() == svcName.ToLower().Trim());

            if (selectService != null)
            {
                try
                {
                    if (selectService.Status != ServiceControllerStatus.Stopped)
                    {
                        selectService.Stop();
                        selectService.WaitForStatus(ServiceControllerStatus.Stopped);
                    }
                    selectService.Start();
                    return true;
                }
                catch (Exception e)
                {
                    LogHelper.WriteLog(LogCategorys.EXCEPTION, "重启服务{0}出错:{1}", svcName, e.Message);
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// 服务是否启动
        /// </summary>
        /// <param name="strServiceName">服务名</param>
        /// <returns></returns>
        public static bool IsServerRunByName(string strServiceName)
        {
            var service = getServiceByName(strServiceName);

            return service != null;
        }

        /// <summary>
        /// 服务是否存在
        /// </summary>
        /// <param name="strServiceName">服务名</param>
        /// <returns></returns>
        public static bool IsServiceExist(string serviceName)
        {
            var services = ServiceController.GetServices();
            var sc = services?.FirstOrDefault(w => w.ServiceName.ToLower().Equals(serviceName.ToLower().Trim()));
            return sc != null;
        }

        /// <summary>
        /// 修正服务路径
        /// 判断服务路径与收银台路径是否一致》不一致》停止服务》卸载》重新安装
        /// </summary>
        /// <param name="strServiceName">服务名称</param>
        /// <returns>修正失败时返回False，其他情况返回True</returns>
        public static bool RepairServicePath(string strServiceName)
        {
            var errMsg = "";
            var result = true;
            try
            {
                var servicePath = GetServicePath(strServiceName);//服务路径
                var yfPosDirectory = System.AppDomain.CurrentDomain.BaseDirectory;//收银台安装目录

                //1.服务路径不存在 或 务路径与收银台路径是否一致
                if (string.IsNullOrEmpty(servicePath) || !servicePath.Contains(yfPosDirectory))
                {
                    if (IsServerRunByName(strServiceName))
                    {
                        //2.停止服务
                        WindowServiceUtils.StopService(strServiceName);
                    }
                    //3.重新安装（InstallService方法会自动卸载服务）
                    WindowServiceUtils.InstallService(strServiceName, yfPosDirectory + "YFPos.WinSerivce.exe", out errMsg);
                }
            }
            catch (Exception ex)
            {
                result = false;
                ex.SaveLog();
            }
            return result;
        }
        /// <summary>
        /// 根据服务名获取服务文件路径
        /// </summary>
        /// <param name="strServiceName">服务名</param>
        /// <returns></returns>
        private static string GetServicePath(string strServiceName)
        {

            string key = @"SYSTEM\CurrentControlSet\Services\" + strServiceName;
            string path = Registry.LocalMachine.OpenSubKey(key).GetValue("ImagePath").ToString();
            //替换掉双引号 
            path = path.Replace("\"", string.Empty);
            var fi = new System.IO.FileInfo(path);
            return fi.FullName;
        }
    }
}
