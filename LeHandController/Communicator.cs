using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;

namespace LeHandController
{
    public class Communicator
    {
        public Communicator()
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
            Process process = new Process();
            process.StartInfo.FileName = "C:/Users/Niek Aukes/source/repos/BtProject/Release/BtProject.exe";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.OutputDataReceived += new DataReceivedEventHandler
            (
                delegate (object sender, DataReceivedEventArgs e)
                {
                    // append the new data to the data already read-in
                    Debug.WriteLine(e.Data);
                }
            );
            process.Start();
            process.BeginOutputReadLine();

            process.StandardInput.WriteLine("connect");
        }
    }
}
