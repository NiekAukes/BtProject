using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace LeHandUI
{
    class FileManager
    {
        static FileStream[] files = new FileStream[50];
        /// <summary>
        /// creates new file, returns -1 if failed
        /// </summary>
        /// <param name="filepath">full path of </param>
        /// <returns></returns>
        public static int Addfile(string filepath)
        {
            for(int i = 0; i < 50; i++)
            {
                if (!files[i].CanRead) {
                    //open file
                    files[i] = File.Create(filepath);
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

        }
    }
}
