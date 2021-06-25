using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace YFPos.Utils
{
    public class Network
    {
        /// <summary>
        /// 检查网络
        /// </summary>
        /// <param name="Description"></param>
        /// <param name="ReservedValue"></param>
        /// <returns></returns>
        /// <summary>
        /// 检查网络
        /// </summary>
        /// <param name="connectionDescription"></param>
        /// <returns></returns>
        [DllImport("sensapi.dll")]
        public extern static bool IsNetworkAlive(out int connectionDescription);

        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<string, string> MacIPs = new Dictionary<string, string>();



        /// <summary>
        /// 等待网络链接
        /// </summary>
        /// <returns></returns>
        public static bool WaitConnectedToInternet()
        {

            int flag;
            while (!IsNetworkAlive(out flag))
            {
                LogHelper.WriteLog(LogCategorys.NORMAL, "脱机，等待网络接通…");
                Thread.Sleep(5000);
            }
            return true;
        }

        /// <summary>
        /// 网络是否可用
        /// </summary>
        /// <returns></returns>
        public static bool IsNetworkAlive()
        {
            int flag;
            return IsNetworkAlive(out flag);
        }

        #region 获取MAC和IP地址的集合
        /// <summary>
        /// 获取MAC和IP地址的集合
        /// </summary>
        /// <returns></returns>
        public static List<HostMacIp> GetMacIps()
        {
            List<HostMacIp> macIPs = new List<HostMacIp>();
            //NetworkInterface[] networks = NetworkInterface.GetAllNetworkInterfaces();
            var networks = from network in NetworkInterface.GetAllNetworkInterfaces()
                           where network.OperationalStatus == OperationalStatus.Up && network.NetworkInterfaceType != NetworkInterfaceType.Loopback && network.NetworkInterfaceType != NetworkInterfaceType.Tunnel
                           select network;
            foreach (NetworkInterface network in networks)
            {
                IPInterfaceProperties ipProperties = network.GetIPProperties();
                foreach (UnicastIPAddressInformation address in ipProperties.UnicastAddresses.Where(n => n.Address.AddressFamily == AddressFamily.InterNetwork))
                {
                    //Console.WriteLine(address.Address.ToString());
                    HostMacIp macIp = new HostMacIp();
                    macIp.IP = address.Address.ToString();
                    macIp.mac = network.GetPhysicalAddress().ToString();
                    macIPs.Add(macIp);
                    break;
                }
            }
            return macIPs;
        }
        #endregion
    }
}
