using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace WristVizualizer
{
    class RegistrySettings
    {
        //static RegistryKey a = Registry.CurrentUser.OpenSubKey(
        public static string getSettingString(string name)
        {
            RegistryKey root = Registry.CurrentUser.OpenSubKey(String.Format("{0}\\{1}", Application.CompanyName, Application.ProductName));
            if (root == null)
                return "";
            return (string)root.GetValue(name, "");
        }

        public static void saveSetting(string name, string value)
        {
            RegistryKey root = Registry.CurrentUser.CreateSubKey(String.Format("{0}\\{1}", Application.CompanyName, Application.ProductName));
            root.SetValue(name, value, RegistryValueKind.String);
        }
    }
}
