using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;

namespace LeHandUI
{
    public struct StartupValues
    {
        public static string[] keynames =
        {
            "FontSize",
            "StartupFile",
            "ShowLineNumbers"
        };
        public int StartFontSize;
        public int StartupFileId;
        public int ShowLineNumbers;
    }
    public class LHregistry
    {
        static string FileNameKey = "HKEY_CURRENT_USER\\Software\\LeHand\\Filenames";
        static string StartupValKey = "HKEY_CURRENT_USER\\Software\\LeHand\\StartupData";
        public static StartupValues GetStartupValues()
        {
            StartupValues values = new StartupValues();

            values.StartFontSize = 14;
            values.StartupFileId = -1;
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("Software\\LeHand\\StartupData", true);
            if (rk == null)
            {
                rk = Registry.CurrentUser.CreateSubKey("Software\\LeHand\\StartupData");
            }
            string[] names = rk.GetValueNames();
            for (int i = 0; i < names.Length; i++)
            {
                if (names[i] == StartupValues.keynames[0])
                    values.StartFontSize = (int)rk.GetValue(names[i]);
                if (names[i] == StartupValues.keynames[1])
                    values.StartupFileId = (int)rk.GetValue(names[i]);
                if (names[i] == StartupValues.keynames[2])
                    values.ShowLineNumbers = (int)rk.GetValue(names[i]);
            }

            return values;
        }

        public static void SetStartupValues(StartupValues values)
        {
            if (values.StartFontSize > 0)
            {
                Registry.SetValue(StartupValKey, StartupValues.keynames[0],
                    values.StartFontSize, RegistryValueKind.DWord);
            }

            if (values.StartFontSize >= 0)
            {
                Registry.SetValue(StartupValKey, StartupValues.keynames[1],
                    values.StartupFileId, RegistryValueKind.DWord);
            }

            Registry.SetValue(StartupValKey, StartupValues.keynames[2],
                values.ShowLineNumbers, RegistryValueKind.DWord);
        }
        
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

            return strbit;
        }
        public static void SetFile(string filename, int id)
        {
            Registry.SetValue(FileNameKey, id.ToString(), filename);
        }

        public static void RemoveFile(int id)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("Software\\LeHand\\Filenames", true);
            string[] names = rk.GetValueNames();
            rk.DeleteValue(names[id]);
            
            
        }
        public static string GetFile(int id)
        {
            string s = (string)Registry.GetValue(FileNameKey, id.ToString(), "");
            return s;
        }


    }
}
