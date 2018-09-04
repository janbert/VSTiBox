using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;

namespace VSTiBox.Common
{
    class CpuTemperatureMonitor
    {
        public double CurrentValue { get; set; }
        public string InstanceName { get; set; }
        public static List<CpuTemperatureMonitor> Temperatures
        {
            get
            {
                List<CpuTemperatureMonitor> result = new List<CpuTemperatureMonitor>();
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"root\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature");
                foreach (ManagementObject obj in searcher.Get())
                {
                    Double temp = Convert.ToDouble(obj["CurrentTemperature"].ToString());
                    temp = (temp - 2732) / 10.0;
                    result.Add(new CpuTemperatureMonitor { CurrentValue = temp, InstanceName = obj["InstanceName"].ToString() });
                }
                return result;
            }
        }
    }
}

