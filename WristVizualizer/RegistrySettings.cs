using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace WristVizualizer
{
    class RegistrySettings
    {
        private const int NUM_SAVED_FILES = 6;
        private const string RECENT_FILES_KEY = "RecentFiles";
        private const string FILENAME_NAME = "Filename";

        //static RegistryKey a = Registry.CurrentUser.OpenSubKey(
        public static string getSettingString(string name)
        {
            RegistryKey root = Registry.CurrentUser.OpenSubKey(String.Format("Software\\{0}\\{1}", Application.CompanyName, Application.ProductName));
            if (root == null)
                return "";
            return (string)root.GetValue(name, "");
        }

        public static void saveSetting(string name, string value)
        {
            RegistryKey root = Registry.CurrentUser.CreateSubKey(String.Format("Software\\{0}\\{1}", Application.CompanyName, Application.ProductName));
            root.SetValue(name, value, RegistryValueKind.String);
        }
        
        public static string[] getAllRecentFiles()
        {
            List<string> recentFiles = new List<string>(NUM_SAVED_FILES);
            int index = 0;
            string filename;
            while ((filename = getRecentFileAtIndex(index)) != null)
            {
                recentFiles.Add(filename);
                index++;
            }
            return recentFiles.ToArray();
        }

        public static void saveMostRecentFile(string filename)
        {
            //look if we have it
            int currentIndex = getIndexRecentFile(filename);
            if (currentIndex == 0) return; //all done, its already first

            //figure out where to start moving previous entries down
            int lastToMove = NUM_SAVED_FILES-2; //-1 so we move the one before the last, -1 for 0 based index
            if (currentIndex > -1)  //if we were already in the list, then we start with moving the one before us
                lastToMove = currentIndex - 1;

            for (int i = lastToMove; i >= 0; i--)
            {
                string current = getRecentFileAtIndex(i);
                if (current != null) //if there is nothing in the slot, don't move it :)
                    setRecentFile(current, i + 1);
            }
            //finally, lets save this to slot0
            setRecentFile(filename, 0);
        }

        private static int getIndexRecentFile(string recentFile)
        {
            int index = 0;
            string filename;
            while ((filename = getRecentFileAtIndex(index)) != null)
            {
                if (filename.ToLower().Equals(recentFile.ToLower()))
                    return index;
                index++;
            }
            //got here, didn't find it, return no luck
            return -1;
        }

        private static string getRecentFileAtIndex(int index)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(String.Format("Software\\{0}\\{1}\\{2}\\{3}",
                Application.CompanyName, Application.ProductName, RECENT_FILES_KEY, index));
            if (key == null)
                return null;

            string filename = (string)key.GetValue(FILENAME_NAME);
            if (String.IsNullOrEmpty(filename))
                return null;

            return filename;
        }
        private static bool hasRecentFileAtIndex(int index)
        {
            return (getRecentFileAtIndex(index) != null);
        }

        private static void setRecentFile(string filename, int index)
        {
            if (index < 0 || index >= NUM_SAVED_FILES)
                throw new IndexOutOfRangeException("Invalid index for saved file");

            RegistryKey key = Registry.CurrentUser.CreateSubKey(String.Format("Software\\{0}\\{1}\\{2}\\{3}",
                Application.CompanyName, Application.ProductName, RECENT_FILES_KEY, index));
            key.SetValue(FILENAME_NAME, filename, RegistryValueKind.String);
        }
    }
}
