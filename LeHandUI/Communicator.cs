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
    public class StreamString
    {
        private Stream ioStream;
        private UnicodeEncoding streamEncoding;

        public StreamString(Stream ioStream)
        {
            this.ioStream = ioStream;
            streamEncoding = new UnicodeEncoding();
        }

        public string ReadString()
        {
            int len;
            len = ioStream.ReadByte() * 256;
            len += ioStream.ReadByte();
            var inBuffer = new byte[len];
            ioStream.Read(inBuffer, 0, len);

            return streamEncoding.GetString(inBuffer);
        }

        public int WriteString(string outString)
        {
            byte[] outBuffer = streamEncoding.GetBytes(outString);
            int len = outBuffer.Length;
            if (len > UInt16.MaxValue)
            {
                len = (int)UInt16.MaxValue;
            }
            ioStream.WriteByte((byte)(len / 256));
            ioStream.WriteByte((byte)(len & 255));
            ioStream.Write(outBuffer, 0, len);
            ioStream.Flush();

            return outBuffer.Length + 2;
        }
    }
    
    
    public class DataPacket
	{

        public int id;
        public double val;
	}
    public class Communicator
    {
        enum Expectedtype
        {
            Protocol,
            Length,
            Data,
            Footer
        }

        public enum status
        {
            S_OK,
            S_Error
        }

        public static Process process = new Process();
        public static string LatestLog = "";
        private static NamedPipeClientStream dataStream;
        private static NamedPipeClientStream errorStream;
        static Expectedtype expected;


        static bool Active = true;
        public static DataPacket ProcessData(Stream stream)
        {
            StreamString streamString = new StreamString(stream);
            char[] str = streamString.ReadString().ToCharArray();
            short[] AShort = Array.ConvertAll(str, c => (short)Char.GetNumericValue(c));
            List<short> Data = new List<short>(AShort);
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
			*/

            
            int datlen = Data.Count;
            short Length = -1;
			short Protocol = -1;
			ushort[] databuf = { 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF };
			DataPacket ret = new DataPacket();
            while (Active)
            {
                Debug.WriteLine(str);
                
                /*
                 if (datlen != 0)
                {
                    short chunk = Data[0]; Data.RemoveAt(datlen - 1);
                    datlen = Data.Count;

                    if (expected == Expectedtype.Protocol)
                    {
                        if (chunk < 32)
                        {
                            //normal
                            Protocol = chunk;
                            expected = Expectedtype.Length;
                        }
                        //out of range
                    }
                    else if (expected == Expectedtype.Length)
                    {
                        Length = chunk;
                        expected = Expectedtype.Data;
                    }
                    else if (expected == Expectedtype.Data)
                    {
                        if ((ushort)chunk != (0xFFFF)) //not a footer
                        {
                            if (Protocol < 7)
                            {
                                //data is of double type
                                for (int p = 0; p < 4; p++)
                                {
                                    if (databuf[p] == (ushort)0xFFFF)
                                    {
                                        //free space
                                        databuf[p] = (ushort)chunk;
                                        break;
                                    }
                                }

                            }
                        }
                        else
                        {
                            //do final things and return

                            expected = Expectedtype.Protocol;

                            if (Protocol < 12)
                            {
                                //double* value = new double(0);
                                //for (int p = 0; p < 4; p++) //convert data buffer to double
                                //{
                                //	if (databuf[p] != 0xFFFF)
                                //	{
                                //		*((short*)value + p) = databuf[p];
                                //	}
                                //	else break;
                                //}
                                //ret.finger->val = *value;
                                //if (out != nullptr)
                                //*out = ret;
                                byte[] bytes = new byte[8];
                                foreach (ushort s in databuf)
                                {
                                    bytes.Concat(BitConverter.GetBytes(s));
                                }
                                ret.val = BitConverter.ToDouble(bytes, 0);
                                return ret;
                            }
                        }
                    }

                }
                else
                {
                    return null;
                }*/
            }
			return null;
		}

        static byte[] buf = new byte[1024];
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
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = false;
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
            //process.BeginOutputReadLine();

            dataStream = new NamedPipeClientStream(@".", @"LeHandData");
            errorStream = new NamedPipeClientStream("LeHandError");

            dataStream.Connect();
            //errorStream.Connect();


            process.StandardInput.WriteLine("device discover");

            dataStream.BeginRead(buf, 0, 1024, null, null);
            while (Active)
            {
                if (buf.Length > 20)
                {
                    ProcessData(dataStream);
                }
            }
        }
        public class device
        {
            public static status discover()
            {
                process.StandardInput.WriteLine("device discover");
                process.BeginErrorReadLine();
                if (LatestLog.StartsWith("ERROR"))
                    return status.S_Error;
                return status.S_OK;
            }
            public static status connect()
            {
                return status.S_OK;
            }
            public static status list()
            {
                return status.S_OK;
            }
            public static status auto()
            {
                return status.S_OK;
            }
        }
        public static status start()
        {
            return status.S_OK;
        }
        public static status load()
        {
            return status.S_OK;
        }
        public static status quit()
        {
            process.StandardInput.WriteLine("quit");
            return status.S_OK;
        }

    }
}
