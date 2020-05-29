using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.TeamFoundation.Common;

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
            if (outstr.Length > id && File.Exists(outstr[id]))
                return GetFileData(outstr[id]);
            return null;
        }
        public static FileData[] GetFileData(string path)
        {
            FileStream stream;
            if (File.Exists(path))
            {
                //open the file
                stream = File.OpenRead(path);
                BinaryReader reader = new BinaryReader(stream);

                //read the name before proceeding
                if (reader.BaseStream.Length <= 0)
                {
                    return null;
                }
                string name = reader.ReadString();
                //char c = 'a';
                //while (c != '\0')
                //{
                //    c = (char)reader.Read();
                //    name += c;
                //};

                //read all other data

                FileData[] ret = new FileData[reader.BaseStream.Length / 34];

                //fill in the filedata
                int bytesread = 0;
                for (int i = 0; i < Math.Floor(reader.BaseStream.Length / 34.0f); i++) 
                {
                    //variable
                    ret[i].variable = reader.ReadByte();

                    //beginrange
                    ret[i].beginRange = reader.ReadDouble();

                    //endrange
                    ret[i].endRange = reader.ReadDouble();

                    //action
                    ret[i].actionId = reader.ReadByte();

                    //arg1
                    ret[i].arg1 = reader.ReadInt64();

                    //arg2
                    ret[i].arg2 = reader.ReadInt64();
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
            BinaryWriter streamWriter = new BinaryWriter(stream);

            streamWriter.Write(name + "\0");
            for (int i = 0; i < fileData.Length; i++)
            {
                byte[] bb = BitConverter.GetBytes(fileData[i].beginRange);
                double d = BitConverter.ToDouble(bb, 0);
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
                DeleteFile(name);

                File.Create(MainWindow.Directory + "\\Files\\" + newName + ".lh");
                ChangeFile(newName, fileDataOfOriginal);

                
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
