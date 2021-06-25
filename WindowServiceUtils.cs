using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace YFPos.Utils
{
    public class WindowServiceUtils
    {
        /// <summary>
        /// 安装服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="serviceFile"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static bool InstallService(string serviceName, string serviceFile, out string errMsg)
        {
            errMsg = string.Empty;
            if (!System.IO.File.Exists(serviceFile))
            {
                errMsg = string.Format("服务程序文件{0}不存在",serviceFile);
                return false;
            }
            ServiceController sc = new ServiceController(serviceName);
            try
            {
                //服务存在
                if (ServiceIsExisted(serviceName))
                {
                    //卸载服务
                    ManagedInstallerClass.InstallHelper(new string[] { serviceFile, "-u" });
                }
                //安装服务
                ManagedInstallerClass.InstallHelper(new string[] { serviceFile });
                //启动服务
                return StartService(serviceName);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return false;
            }
        }


        /// <summary>
        /// 服务是否存在
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>

        private static bool ServiceIsExisted(string serviceName)
        {
            var services = ServiceController.GetServices();
            foreach (ServiceController s in services)
            {
                if (s.ServiceName == serviceName)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="svcName"></param>
        /// <returns></returns>
        public static bool StartService(string svcName)
        {
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController s in services)
            {
                if (s.ServiceName == svcName && s.Status != ServiceControllerStatus.Running)
                {
                    s.Start();
                    s.WaitForStatus(ServiceControllerStatus.Running);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="serviceName"></param>
        public static void StopService(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController s in services)
            {
                if (s.ServiceName == serviceName && s.Status != ServiceControllerStatus.Stopped)
                {
                    s.Stop();
                    s.WaitForStatus(ServiceControllerStatus.Stopped);
                }

            }
        }
    }
}
