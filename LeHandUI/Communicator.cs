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
using System.Security.Cryptography;

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
    public static class CommunicatorHelper
    {
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
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

        public static Process process = null;
        public static string LatestLog = "";
        private static NamedPipeClientStream dataStream;
        private static NamedPipeClientStream errorStream;
        static Expectedtype expected;


        static bool Active = true;
        
        public static DataPacket ProcessData(ushort[] data)
        {
            List<ushort> Data = new List<ushort>(data);
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
            ushort Length = 65535;
			ushort Protocol = 65535;
			ushort[] databuf = { 0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF };
			DataPacket ret = new DataPacket();
            while (Active)
            {


                if (datlen != 0)
                {
                    ushort chunk = Data[0]; Data.RemoveAt(0);
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
                                IEnumerable<byte> ex = new byte[0];
                                foreach (ushort s in databuf)
                                {
                                    byte[] con = BitConverter.GetBytes(s);
                                    ex = ex.Concat(con);
                                }
                                ret.val = BitConverter.ToDouble(ex.ToArray(), 0);
                                return ret;
                            }
                        }
                    }

                }
                else
                {
                    return null;
                }
            }
			return null;
		}
        static int shortsread = 0;
        static int charsread = 0;
        static byte[] buf = new byte[1024];
        static IAsyncResult readres;
        public static void DistributeData()
        {
            while (Active)
            {
                dataStream.EndRead(readres);
                ushort[] shortbuf = new ushort[512];
                for (int i = 0; i < 512; i++)
                {
                    shortbuf[i] = BitConverter.ToUInt16(buf, (i * 2 + shortsread) % 1023);
                }

                if (Enumerable.Contains<ushort>(shortbuf, 65535))
                {
                    //still shit to do
                    ushort[] dat = CommunicatorHelper.SubArray<ushort>(shortbuf, 0, 7);
                    DataPacket pack = ProcessData(dat);
                    shortsread += 7;
                    if (pack != null)
                        Debug.WriteLine("detected a new message" + pack.val.ToString()); 
                }
                else
                {

                }

                for (int i = 0; i < 1024; i++)
                {

                }
                readres = dataStream.BeginRead(buf, 0, 1024, null, null);

                Thread.Sleep(200);
            }
        }
        static Thread distribution = null;
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
            process = new Process();


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

            readres = dataStream.BeginRead(buf, 0, 1024, null, null);
            distribution = new Thread(new ThreadStart(DistributeData));
            distribution.Start();
            return;
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

            if (process != null)
            {
                process.StandardInput.WriteLine("quit");
                return status.S_OK;
            }
            else
            {
                return status.S_Error;
            }
        }

    }
}
