using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace EmbeddedOffice
{
    /// <summary>
    /// Summary description for Win32Helper.
    /// </summary>
    public class Win32Helper
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        public static int GetProcessIdByWindowTitle(string appTitle)
        {
            var processes = Process.GetProcesses().ToList();

            var ws = processes.Where(p => p.ToString().Contains("WORD")).ToList();

            foreach (var process1 in ws)
            {
                List<string> sList = new List<string>();
                foreach (ProcessModule module in process1.Modules)
                {
                    sList.Add(module.FileName);
                }
                sList.Sort();
                var txt = sList.Aggregate("", (a, b) => a + "\r\n" + b);

            }


            var process = processes.SingleOrDefault(p => p.MainWindowTitle.Equals(appTitle));
            if (process != null)
                return process.Id;

            return -1;
        }

        public static int GetEmtyWordProcess()
        {
            var processes = Process.GetProcesses().ToList();

            var ws = processes.Where(p => p.ProcessName == "WINWORD").SingleOrDefault(w => string.IsNullOrEmpty(w.MainWindowTitle));
            if (ws == null)
                return -1;
            return ws.Handle.ToInt32();
        }
    }
}