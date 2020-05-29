using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel;

namespace LeHandUI
{
    public struct FileData
    {
        public byte variable;
        public double beginRange, endRange;
        public byte actionId;
        public long arg1;
        public long arg2;
        public FileData(byte var, double begin, double end, byte action, long a1, long a2)
        {
            variable = var;
            beginRange = begin;
            endRange = end;
            actionId = action;
            arg1 = a1;
            arg2 = a2;
        }
        public Logic toLogic()
        {
            Logic logic = new Logic();
            logic.variable = variable;
            logic.beginrange = beginRange;
            logic.endrange = endRange;
            switch(actionId)
            {
                case 0:
                    //Kpress
                    logic.action = new Kpress(Convert.ToChar(arg1));
                    break;
                case 1:
                    //Mpress
                    logic.action = new Mpress((short)arg1);
                    break;
                case 2:
                    //Mpress
                    logic.action = new MMove(BitConverter.Int64BitsToDouble(arg1),
                        BitConverter.Int64BitsToDouble(arg2));
                    break;
            }
            return logic;
        }
       
    }
    class SimpleFileManager
    {
        public static string[] FileNames()
        {
            string[] outstr = Directory.GetFiles(MainWindow.Directory + "\\Files");
            for (int i = 0; i < outstr.Length; i++)
            {
                outstr[i] = LHregistry.getSimpleName(outstr[i]);
                outstr[i] = outstr[i].Split('.')[0];
            }
            return outstr;
        }

        public static FileData[] GetFileData(int id)
        {
            string[] outstr = Directory.GetFiles(MainWindow.Directory + "\\Files");
            if (outstr.Length <= id && File.Exists(outstr[id]))
                return GetFileData(outstr[id]);
            return null;
        }
        public static FileData[] GetFileData(string path)
        {
            FileStream stream;
            if (File.Exists(MainWindow.Directory + "\\Files\\" + path + ".lh"))
            {
                stream = File.OpenRead(MainWindow.Directory + "\\Files\\" + path + ".lh");
                StreamReader reader = new StreamReader(stream);
                string name = "";
                char c = 'a';
                do
                {
                    c = (char)reader.Read();
                    name += c;
                } while (c != '\0');

                string data = reader.ReadToEnd();

                FileData[] ret = new FileData[data.Length / 26];

                int bytesread = 0;
                for (int i = 0; i < data.Length / 26; i++) 
                {
                    string s = data.Substring(bytesread, bytesread + 1);
                    bytesread += 1;
                    ret[i].variable = (byte)s[0];

                    s = data.Substring(bytesread, bytesread + 8);
                    bytesread += 8;
                    ret[i].beginRange = BitConverter.ToDouble(Encoding.ASCII.GetBytes(s), 0);

                    s = data.Substring(bytesread, bytesread + 8);
                    bytesread += 8;
                    ret[i].endRange = BitConverter.ToDouble(Encoding.ASCII.GetBytes(s), 0);

                    s = data.Substring(bytesread, bytesread + 1);
                    bytesread += 1;
                    ret[i].actionId = (byte)s[0];

                    s = data.Substring(bytesread, bytesread + 4); //HELP ERROR HERE

                    bytesread += 4;
                    ret[i].arg1 = BitConverter.ToInt32(Encoding.ASCII.GetBytes(s), 0);

                    s = data.Substring(bytesread, bytesread + 4);
                    bytesread += 4;
                    ret[i].arg1 = BitConverter.ToInt32(Encoding.ASCII.GetBytes(s), 0);
                }
                return ret;
            }
            return null;
        }

        public static void ChangeFile(string name, FileData[] fileData)
        {
            FileStream stream = null;
            if (File.Exists(MainWindow.Directory + "\\Files\\" + name + ".lh"))
               File.Delete(MainWindow.Directory + "\\Files\\" + name + ".lh");
            stream = File.Create(MainWindow.Directory + "\\Files\\" + name + ".lh");
            StreamWriter streamWriter = new StreamWriter(stream);

            streamWriter.Write(name + "\0");
            for (int i = 0; i < fileData.Length; i++)
            {
                streamWriter.Write(fileData[i].variable);
                streamWriter.Write(fileData[i].beginRange);
                streamWriter.Write(fileData[i].endRange);
                streamWriter.Write(fileData[i].actionId);
                streamWriter.Write(fileData[i].arg1);
                streamWriter.Write(fileData[i].arg2);
            }


            streamWriter.Close();
            stream.Close();
        }

        public static void ChangeName(string name, string newName)
        {
            if (File.Exists(MainWindow.Directory + "\\Files\\" + name + ".lh")){

                FileData[] fileDataOfOriginal = GetFileData(name);
                File.Create(MainWindow.Directory + "\\Files\\" + newName + ".lh");

                ChangeFile(newName, fileDataOfOriginal);

                DeleteFile(name);
            }
        }

        public static void DeleteFile(int id)
        {
            string[] outstr = Directory.GetFiles(MainWindow.Directory + "\\Files");
            if (File.Exists(outstr[id]))
                File.Delete(outstr[id]);
        }
        public static void DeleteFile(string name)
        {
            string filepath = MainWindow.Directory + "\\Files\\" + name + ".lh";
            if (File.Exists(filepath)){
                File.Delete(filepath);
            }
                
        }


    }
    class FileManager
    {
        public static bool[] isFileNotSaved = new bool[50];
        public static string[] FileCache = new string[50];
        public static string[] files = new string[50];
        public static int currentFileId = -1;
        public static int currentLoadedIndex = -1;
        /// <summary>
        /// creates new file, returns -1 if failed
        /// </summary>
        /// <param name="filepath">full path of </param>
        /// <returns></returns>
        public static void LoadAllFiles()
        {
            
            string[] filenames = LHregistry.GetAllFilenames();
            int[] fileids = LHregistry.GetAllFileIds();
            for (int i = 0; i < fileids.Length; i++)
            {
                files[fileids[i]] = LHregistry.GetFile(fileids[i]);
            }
        }

        /*public static int Addfile(string filepath)
        {
            for(int i = 0; i < 50; i++)
            {
                if (files[i] == null) {
                    //open file
                    //string[] pathparts = filepath.Split('\\');
                    //string newFilePath = ".\\Resources\\LuaFiles\\" + pathparts[(pathparts.Length - 1)];
                    //files[i] = File.Create(newFilePath);
                    //LHregistry.SetFile(newFilePath, i);
                    return i;
                }
            }
            return -1;
        }*/

        public static string LoadFile(int id)
        {
            string fileContents = "";
            //check if file is cached
            if (!isFileNotSaved[id])
            {
                //load new file
                byte[] cont = new byte[files[id].Length];
                if (files[id] != null)
                {
                    FileStream stream = File.OpenRead(LHregistry.GetFile(id));
                    StreamReader reader = new StreamReader(stream);
                    fileContents = reader.ReadToEnd();
                    reader.Close();
                    stream.Close();

                    
                    isFileNotSaved[id] = false;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                fileContents = FileCache[id];
            }
            currentFileId = id;
            return fileContents;
        }

        public static void Deletefile(int fileid)
        {
            LHregistry.RemoveFile(fileid);
        }

        public static int AddReference(string filepath)
        {
            for (int i = 0; i < 50; i++)
            {
                if (files[i] == null)
                {
                    //open file
                    files[i] = filepath;
                    LHregistry.SetFile(filepath, i);
                    return i;
                }
            }
            return -1;
        }
    }
}
