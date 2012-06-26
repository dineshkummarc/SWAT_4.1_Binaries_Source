using System;
using System.Diagnostics;
using System.Threading;

namespace SWAT.Tests
{
    public class ProcessKiller
    {
        public string ProcessName { get; set; }
        public int Delay
        {
            get { return delay; }
            set { delay = value; }
        }
        
        private Thread killThread;
        private int delay = 1000;

        public ProcessKiller(string processName)
        {
            ProcessName = processName;
        }

        public void Kill()
        {
            Kill(0);
        }

        public void Kill(int delayStart)
        {
            Thread.Sleep(delayStart);
            foreach (Process process in Process.GetProcessesByName(ProcessName))
            {
                try
                {
                    process.Kill();
                }
                catch { }
            }

            try
            {
                DateTime timeout = DateTime.Now.AddSeconds(5);
                while (DateTime.Now < timeout && Process.GetProcessesByName(ProcessName).Length > 0)
                {
                    Thread.Sleep(0);
                }
            }
            catch { }
        }

        public void KillDelayed()
        {
            Kill(Delay);
        }

        public void KillAsync()
        {
			ClearPreviousThread();
            killThread = new Thread(new ThreadStart(Kill));
            killThread.Start();
        }

        public void KillAsyncDelayed()
        {
			ClearPreviousThread();
            killThread = new Thread(new ThreadStart(KillDelayed));
            killThread.Start();
        }

        public static void Kill(string processName)
        {
            ProcessKiller killer = new ProcessKiller(processName);
            killer.Kill();
        }
		
		public static void KillAsync(string processName)
        {
            ProcessKiller killer = new ProcessKiller(processName);
            killer.KillAsync();
        }

        public static void KillAsyncDelayed(string processName, int delay)
        {
            ProcessKiller killer = new ProcessKiller(processName);
            killer.Delay = delay;
            killer.KillAsyncDelayed();
        }
		
		private void ClearPreviousThread()
		{
			try
			{
				if (killThread != null && killThread.IsAlive)
				{
					killThread.Abort();
				}
			}
			catch {}
		}
    }
}
