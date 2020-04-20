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
        static string keyName = "HKEY_CURRENT_USER\\LeHand\\Filenames";
        public static string[] GetAllFilenames()
        {
            RegistryKey rk = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default);
            rk.OpenSubKey("LeHand\\Filenames");
            string[] names = rk.GetSubKeyNames();
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = (string)rk.GetValue(names[i]);
            }

            return names;
        }
        public static void SetFile(string filename, int id)
        {
            Registry.SetValue(keyName, id.ToString(), filename);
        }

        public static void RemoveFile(int id)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("\\LeHand\\Filenames");
            //Registry.
        }
        public static string GetFile(int id)
        {
            string s = (string)Registry.GetValue(keyName, id.ToString(), "");
            return s;
        }


    }
}
