using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;

namespace LeHandUI
{
    
    class LHregistry
    {
        static string keyName = "HKEY_CURRENT_USER\\Software\\LeHand\\Filenames";
        public static string[] GetAllFilenames()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("Software\\LeHand\\Filenames");
            string[] names = rk.GetValueNames();
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = (string)rk.GetValue(names[i]);
            }

            return names;
        }
        public static int[] GetAllFileIds()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("Software\\LeHand\\Filenames");
            string[] names = rk.GetValueNames();
            int[] ret = new int[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                ret[i] = Convert.ToInt32(names[i]);
            }

            return ret;
        }
        public static int GetIdFromPath(string filepath)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("Software\\LeHand\\Filenames");
            string[] names = rk.GetValueNames();
            int[] ret = new int[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                if((string)rk.GetValue(names[i]) == filepath)
                {
                    return Convert.ToInt32(names[i]);
                }
            }

            return -1;
        }
        public static string getSimpleName(string strin)
        {
            string[] strbits = strin.Split('\\');
            string strbit = strbits[strbits.Length - 1];
            return null;
        }
        public static void SetFile(string filename, int id)
        {
            Registry.SetValue(keyName, id.ToString(), filename);
        }

        public static void RemoveFile(int id)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("Software\\LeHand\\Filenames");
            rk.DeleteValue(id.ToString());
        }
        public static string GetFile(int id)
        {
            string s = (string)Registry.GetValue(keyName, id.ToString(), "");
            return s;
        }


    }
}
