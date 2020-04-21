using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace LeHandUI
{
    struct FileData
    {
    }
    class FileManager
    {
        static FileStream[] files = new FileStream[50];
        /// <summary>
        /// creates new file, returns -1 if failed
        /// </summary>
        /// <param name="filepath">full path of </param>
        /// <returns></returns>
        public static void LoadAllFiles()
        {
            string[] filenames = LHregistry.GetAllFilenames();
            int[] fileids = LHregistry.GetAllFileIds();
            for(int i = 0; i < fileids.Length; i++)
            {
                files[fileids[i]] = File.Open(filenames[i], FileMode.Open);
            }
        }

        public static int Addfile(string filepath)
        {
            for(int i = 0; i < 50; i++)
            {
                if (!files[i].CanRead) {
                    //open file
                    files[i] = File.Create(filepath);
                    LHregistry.SetFile(filepath, i);
                    return i;
                }
            }
            return -1;
        }

        public static string LoadFile(int id) 
        {
            string fileContents;
            using (StreamReader reader = new StreamReader(files[id]))
            {
                fileContents = reader.ReadToEnd();
            }

            return fileContents;
        }

        public static void Deletefile(int fileid)
        {
            return;
        }

        public static int AddReference(string filepath)
        {
            for (int i = 0; i < 50; i++)
            {
                if (files[i] == null)
                {
                    //open file
                    files[i] = File.Open(filepath, FileMode.Open);
                    LHregistry.SetFile(filepath, i);
                    return i;
                }
            }
            return -1;
        }
    }
}
