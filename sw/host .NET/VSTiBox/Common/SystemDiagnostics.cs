
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;        
using System.Management;
using Microsoft.VisualBasic.Devices;
using System.Diagnostics;
using System.Windows.Forms;
using OpenHardwareMonitor;
using OpenHardwareMonitor.Hardware;

namespace VSTiBox
{
    class SystemDiagnostics
    {
        private PerformanceCounter mCpuCounter = new PerformanceCounter();
        private PerformanceCounter mRamCounter = new PerformanceCounter("Memory", "Available MBytes");
        private ComputerInfo mComputerInfo = new ComputerInfo();

        private OpenHardwareMonitor.Hardware.Computer computer;
        
        private SystemDiagnostics()
        {
            mCpuCounter.CategoryName = "Processor";
            mCpuCounter.CounterName = "% Processor Time";
            mCpuCounter.InstanceName = "_Total";
            computer = new OpenHardwareMonitor.Hardware.Computer();
            computer.CPUEnabled = true;
            computer.Open();
        }

        /// <summary>
        /// Singleton pattern implementation to get Class instance
        /// </summary>
        public static SystemDiagnostics Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        /// <summary>
        /// Nested class for singleton implementation
        /// </summary>
        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly SystemDiagnostics instance = new SystemDiagnostics();
        }


            // Call this method every time you need to know the current cpu usage. 
        public static float CurrentCpuUsage
        {
            get
            {
                return Instance.mCpuCounter.NextValue();
            }
        }

        public static float CPUTemperature
        {
            get
            {
                var cpuHardware = Instance.computer.Hardware.First(x => x.HardwareType == HardwareType.CPU);
                cpuHardware.Update();
                var cpuTempSensor = cpuHardware.Sensors.FirstOrDefault(x => x.SensorType == SensorType.Temperature);
                if ((cpuTempSensor == null) || (cpuTempSensor.Value == null))
                {
                    return 0.0f;
                }
                else
                {
                    return (float)cpuTempSensor.Value;
                }
            }
        }

        // Call this method every time you need to get the amount of the available RAM in Mb 
        public static float AvailableRAM_MB()
        {
            return Instance.mRamCounter.NextValue();
        }

        public static ulong TotalPhysicalMemory
        {
            get
            {
                return Instance.mComputerInfo.TotalPhysicalMemory;
            }
        }

        public static ulong AvailablePhysicalMemory
        {
            get
            {
                return Instance.mComputerInfo.AvailablePhysicalMemory;
            }
        }
    }
}
