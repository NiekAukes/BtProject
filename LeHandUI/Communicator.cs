using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Threading;

namespace LeHandUI
{
	public struct DataPacket
	{

	}
    public class Communicator
    {
        public enum status
        {
            S_OK,
            S_Error
        }

        public static Process process = new Process();
        public static string LatestLog = "";
        private static NamedPipeClientStream dataStream;
        private static NamedPipeClientStream errorStream;

		/*DataPacket ProcessData(Stream stream)
		{
			stream.

			/*
				Data format: uses 16 bit data chunks
					--Header
					0x0008 (Protocol for identifying type)
					0x0003 (Length of data)

					--Data
					0xA064
					0x74B4
					etc...

					--Footer
					0xFFFF
			

			
			bool Active = true;
			short Length = -1;
			short Protocol = -1;
			int[] databuf = { 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF };
			DataPacket ret;
			while (Active)
			{
				if (Data.size() != 0)
				{
					unsigned short chunk = Data.front(); Data.pop();

					if (expected == Expectedtype::Protocol)
					{
						if (chunk < 32)
						{
							//normal
							Protocol = chunk;
							if (chunk < 12)
							{
								ret.finger = new Axis();
								ret.finger->nAxisNum = chunk - 2;
							}
							else if (chunk < 32)
							{
								//other undefined
							}
							expected = Expectedtype::Length;
						}
						//out of range
					}
					else if (expected == Expectedtype::Length)
					{
						Length = chunk;
						expected = Expectedtype::Data;
					}
					else if (expected == Expectedtype::Data)
					{
						if (chunk != (unsigned short)(0xFFFF)) //not a footer
					{
							if (Protocol < 7)
							{
								//data is of double type
								for (int p = 0; p < 4; p++)
								{
									if (databuf[p] == (short)0xFFFF)
									{
										//free space
										databuf[p] = chunk;
										break;
									}
								}

							}
						}
					else
						{
							//do final things and return

							expected = Expectedtype::Protocol;

							if (Protocol < 12)
							{
								double* value = new double(0);
								for (int p = 0; p < 4; p++) //convert data buffer to double
								{
									if (databuf[p] != 0xFFFF)
									{
										*((short*)value + p) = databuf[p];
									}
									else break;
								}
								ret.finger->val = *value;
								if (out != nullptr)
								*out = ret;
								return 0;
							}
						}
					}

				}
				else
				{
					return 0;
				}
			}

			return 0;
		}
	*/
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

            dataStream = new NamedPipeClientStream("\\\\.\\LeHand\\Data");
            errorStream = new NamedPipeClientStream("\\\\.\\LeHand\\error");

            dataStream.Connect();
            errorStream.Connect();

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
