using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;

namespace YFPos.Utils
{
    public class SystemHelper
    {
        //获取内存可用大小
        public static double GetAvalidMemery()
        {
            var ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            ramCounter.NextValue();
            Thread.Sleep(150);
            return ramCounter.NextValue();
        }

        //获取当前CPU使用率
        public static double GetCPURate()
        {
            var  cpuCounter =  new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue();
            Thread.Sleep(150);
            return cpuCounter.NextValue();
        }
    }
}
