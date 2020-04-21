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
        FileStream[] files = new FileStream[50];
        /// <summary>
        /// creates new file, returns -1 if failed
        /// </summary>
        /// <param name="filepath">full path of </param>
        /// <returns></returns>
        int Addfile(string filepath)
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
        void Deletefile(int fileid)
        {

        }
    }
}
