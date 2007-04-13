using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WristVizualizer
{
    class WristHelper
    {
        static public bool isRadius(string path)
        {
            string fname = Path.GetFileNameWithoutExtension(path);
            //check length
            if (fname.Length != 6) 
                return false;

            //check is rad bone
            if (!fname.Substring(0, 3).ToLower().Equals("rad"))
                return false;

            //check is numeric
            int junk;
            if (!Int32.TryParse(fname.Substring(3, 2),out junk))
                return false;

            //check ends in L or R
            if (!fname.EndsWith("L",StringComparison.CurrentCultureIgnoreCase) && !fname.EndsWith("R",StringComparison.CurrentCultureIgnoreCase))
                return false;

            return true;
        }
    }
}
