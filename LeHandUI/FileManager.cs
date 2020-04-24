﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel;

namespace LeHandUI
{
    struct FileData
    {
    }
    class FileManager
    {
        public static string[] files = new string[50];
        public static int currentFile = -1;
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

            //load new file
            string fileContents = "";
            byte[] cont = new byte[files[id].Length];
            if (files[id] != null)
            {
                FileStream stream = File.OpenRead(LHregistry.GetFile(id));
                StreamReader reader = new StreamReader(stream);
                fileContents = reader.ReadToEnd();
                reader.Close();
                stream.Close();

            }
            else
            {
                return null;
            }
            currentFile = id;
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
