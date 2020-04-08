using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;

namespace LeHandUI
{
    public class Communicator
    {
        public enum status
        {
            S_OK,
            S_Error
        }

        public static Process process = new Process();
        public static string LatestLog = "";
        public static void Init()
        {
            //Process Device = new Process
            //{
            //    StartInfo =
            //    {
            //        FileName = "C:/Users/Niek Aukes/source/repos/BtProject/Release/BtProject.exe",
            //        CreateNoWindow = false,
            //        UseShellExecute = false
            //    }
            //};
            
            process.StartInfo.FileName = "LeHand.exe";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;
            process.OutputDataReceived += new DataReceivedEventHandler
            (
                delegate (object sender, DataReceivedEventArgs e)
                {
                    // append the new data to the data already read-in
                    Debug.WriteLine(e.Data);
                    LatestLog = e.Data;
                }
            );
            process.Start();
            process.BeginOutputReadLine();

            process.StandardInput.WriteLine("connect");
        }

        public class device
        {
            public status discover()
            {
                process.StandardInput.WriteLine("device discover");
                process.BeginErrorReadLine();
                if (LatestLog.StartsWith("ERROR"))
                    return status.S_Error;
                return status.S_OK;
            }
            public status connect()
            {
                return status.S_OK;
            }
            public status list()
            {
                return status.S_OK;
            }
            public status auto()
            {
                return status.S_OK;
            }
        }
        public status start()
        {
            return status.S_OK;
        }
        public status load()
        {
            return status.S_OK;
        }

    }
}
