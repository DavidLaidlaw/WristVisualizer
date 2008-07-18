using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

namespace WristVizualizer
{
    class RegistrySettings
    {
        private const int NUM_SAVED_FILES = 6;
        private const string RECENT_FILES_KEY = "RecentFiles";
        private const string FILENAME_NAME = "Filename";

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
            filename = System.IO.Path.GetFullPath(filename); //make sure its the full path we are saving
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


        /// <summary>
        /// Shortens a pathname by either removing consecutive components of a path
        /// and/or by removing characters from the end of the filename and replacing
        /// then with three elipses (...)
        ///
        /// In all cases, the root of the passed path will be preserved in it's entirety.
        ///
        /// If a UNC path is used or the pathname and maxLength are particularly short,
        /// the resulting path may be longer than maxLength.
        ///
        /// This method expects fully resolved pathnames to be passed to it.
        /// (Use Path.GetFullPath() to obtain this.)
        /// </summary>
        /// <param name="pathname"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string ShortenPathname(string pathname, int maxLength)
        {
            if (pathname.Length <= maxLength)
                return pathname;

            string root = Path.GetPathRoot(pathname);
            if (root.Length > 3)
                root += Path.DirectorySeparatorChar;

            string[] elements = pathname.Substring(root.Length).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            int filenameIndex = elements.GetLength(0) - 1;

            if (elements.GetLength(0) == 1) // pathname is just a root and filename
            {
                if (elements[0].Length > 5) // long enough to shorten
                {
                    // if path is a UNC path, root may be rather long
                    if (root.Length + 6 >= maxLength)
                    {
                        return root + elements[0].Substring(0, 3) + "...";
                    }
                    else
                    {
                        return pathname.Substring(0, maxLength - 3) + "...";
                    }
                }
            }
            else if ((root.Length + 4 + elements[filenameIndex].Length) > maxLength) // pathname is just a root and filename
            {
                root += "...\\";

                int len = elements[filenameIndex].Length;
                if (len < 6)
                    return root + elements[filenameIndex];

                if ((root.Length + 6) >= maxLength)
                {
                    len = 3;
                }
                else
                {
                    len = maxLength - root.Length - 3;
                }
                return root + elements[filenameIndex].Substring(0, len) + "...";
            }
            else if (elements.GetLength(0) == 2)
            {
                return root + "...\\" + elements[1];
            }
            else
            {
                int len = 0;
                int begin = 0;

                for (int i = 0; i < filenameIndex; i++)
                {
                    if (elements[i].Length > len)
                    {
                        begin = i;
                        len = elements[i].Length;
                    }
                }

                int totalLength = pathname.Length - len + 3;
                int end = begin + 1;

                while (totalLength > maxLength)
                {
                    if (begin > 0)
                        totalLength -= elements[--begin].Length - 1;

                    if (totalLength <= maxLength)
                        break;

                    if (end < filenameIndex)
                        totalLength -= elements[++end].Length - 1;

                    if (begin == 0 && end == filenameIndex)
                        break;
                }

                // assemble final string
                for (int i = 0; i < begin; i++)
                {
                    root += elements[i] + '\\';
                }

                root += "...\\";

                for (int i = end; i < filenameIndex; i++)
                {
                    root += elements[i] + '\\';
                }

                return root + elements[filenameIndex];
            }
            return pathname;
        }
    }
}
