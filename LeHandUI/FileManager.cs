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

    }
    class SimpleFileMangaer
    {
        private static int Loopdistance = 26;
        public static string[] FileNames()
        {
            return null;
        }
        public static void AddFile(string name)
        {
            FileStream stream = File.Create(MainWindow.Directory + "\\Files\\" + name + ".txt");
            StreamWriter streamWriter = new StreamWriter(stream);
            streamWriter.WriteLine(name + "\0");
            streamWriter.Close();
            stream.Close();
        }
        public static void DeleteFile(int id)
        {

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
