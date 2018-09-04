using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Linq;

namespace VSTiBox
{
    public class DistributedThread
    {
        [DllImport("kernel32.dll")]
        public static extern int GetCurrentThreadId();

        [DllImport("kernel32.dll")]
        public static extern int GetCurrentProcessorNumber();

        private ThreadStart threadStart;

        private ParameterizedThreadStart parameterizedThreadStart;

        private Thread thread;

        public int ProcessorAffinity { get; set; }

        public Thread ManagedThread
        {
            get
            {
                return thread;
            }
        }

        private DistributedThread()
        {
            thread = new Thread(DistributedThreadStart);
        }

        public DistributedThread(ThreadStart threadStart)
            : this()
        {
            this.threadStart = threadStart;
        }

        public DistributedThread(ParameterizedThreadStart threadStart)
            : this()
        {
            this.parameterizedThreadStart = threadStart;
        }

        public void Start()
        {
            if (this.threadStart == null) throw new InvalidOperationException();

            thread.Start(null);
        }

        public void Start(object parameter)
        {
            if (this.parameterizedThreadStart == null) throw new InvalidOperationException();

            thread.Start(parameter);
        }

        private void DistributedThreadStart(object parameter)
        {
            try
            {
                // fix to OS thread
                Thread.BeginThreadAffinity();

                // set affinity
                if (ProcessorAffinity != 0)
                {
                    CurrentThread.ProcessorAffinity = new IntPtr(ProcessorAffinity);
                }

                // call real thread
                if (this.threadStart != null)
                {
                    this.threadStart();
                }
                else if (this.parameterizedThreadStart != null)
                {
                    this.parameterizedThreadStart(parameter);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            finally
            {
                // reset affinity
                CurrentThread.ProcessorAffinity = new IntPtr(0xFFFF);
                Thread.EndThreadAffinity();
            }
        }

        private ProcessThread CurrentThread
        {
            get
            {
                int id = GetCurrentThreadId();
                return
                    (from ProcessThread th in Process.GetCurrentProcess().Threads
                     where th.Id == id
                     select th).Single();
            }
        }
    }
}
