using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace AndroidBatteryStatus
{
    public class AdbCommand
    {
        public AdbCommand() { }

        public string RunCommand(string cmd)
        {
            Process adb = null;
            try
            {
                ProcessStartInfo procInfo = new ProcessStartInfo(@"adb.exe");
                procInfo.CreateNoWindow = true;
                procInfo.RedirectStandardError = true;
                procInfo.RedirectStandardOutput = true;
                procInfo.UseShellExecute = false;
                procInfo.Arguments += cmd;
                adb = Process.Start(procInfo);
                adb.WaitForExit();

                string stdErr = adb.StandardError.ReadToEnd();

                if (!String.IsNullOrEmpty(stdErr))
                {
                    return "ERROR: " + stdErr;
                }

                string stdOut = adb.StandardOutput.ReadToEnd();

                if (!String.IsNullOrEmpty(stdOut))
                {
                    return stdOut;
                }

                return "ERROR: No output returned from adb!";
            }
            catch (ThreadAbortException) { return "ERROR: ThreadAbortException"; }
            catch (ThreadInterruptedException) { return "ERROR: ThreadInterruptedException"; }
            catch (System.ComponentModel.Win32Exception)
            {
                return "ERROR: Cannot find 'adb' executable.";
            }
            finally
            {
                if (adb != null && !adb.HasExited)
                {
                    adb.Kill();
                }
            }
        }
    }
}
