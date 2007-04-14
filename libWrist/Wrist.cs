using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace libWrist
{


    public class Wrist
    {
        public enum Side
        {
            LEFT,
            RIGHT,
        }

        private struct SeriesInfo
        {
            public string series;
            public int seriesNum;
            public string motionFile;
        }

        private string _subjectPath;
        private string _ivFolderPath;
        private string _neutralSeries;
        private string _side;
        private SeriesInfo[] _info;

        public Wrist(string pathRadiusIV)
        {

        }

        public string neturalSeries
        {
            get { return "S"+_neutralSeries+_side; }
        }

        public string[] series
        {
            get
            {
                string[] s = new string[_info.Length];
                for (int i = 0; i < _info.Length; i++)
                    s[i] = _info[i].series;
                return s;
            }
        }

        public string[] motionFiles
        {
            get
            {
                string[] s = new string[_info.Length];
                for (int i = 0; i < _info.Length; i++)
                    s[i] = Path.Combine(getSeriesPath(i), "Motion" + _neutralSeries + _side + _info[i].series.Substring(1, 3) + ".dat");
                return s;
            }
        }

        public int getSeriesIndexFromName(string series)
        {
            for (int i = 0; i < _info.Length; i++)
                if (_info[i].series.Equals(series)) return i;

            throw new ArgumentException("Unable to locate series in list");
        }

        public string getMotionFilePath(int positionIndex)
        {
            if (positionIndex >= _info.Length)
                throw new ArgumentOutOfRangeException("Position index exceeds length of array.");

            return Path.Combine(getSeriesPath(positionIndex), "Motion" + _neutralSeries + _side + _info[positionIndex].series.Substring(1,3) + ".dat");
        }

        public string getSeriesPath(int positionIndex)
        {
            if (positionIndex >= _info.Length)
                throw new ArgumentOutOfRangeException("Position index exceeds length of array.");

            return Path.Combine(_subjectPath, _info[positionIndex].series);
        }

        public void findAllSeries()
        {
            DirectoryInfo sub = new DirectoryInfo(_subjectPath);
            DirectoryInfo[] series = sub.GetDirectories("S??" + _side);
            System.Collections.ArrayList list = new System.Collections.ArrayList();
            foreach (DirectoryInfo s in series)
            {
                if (!Regex.Match(s.Name,@"S(\d\d)"+_side,RegexOptions.IgnoreCase).Success)                
                    continue;

                string motion = Path.Combine(s.FullName,"Motion"+_neutralSeries+_side+s.Name.Substring(1,2)+_side+".dat");
                if (File.Exists(motion))
                {
                    SeriesInfo info = new SeriesInfo();
                    info.motionFile = motion;
                    info.series = s.Name;
                    info.seriesNum = Int32.Parse(s.Name.Substring(1, 2));

                    list.Add(info);
                }
            }
            _info = (SeriesInfo[])list.ToArray(typeof(SeriesInfo));
        }

        public void setupPaths(string pathRadiusIV)
        {
            if (!isRadius(pathRadiusIV))
                throw new ArgumentException("Bone provided is not a radius");

            string fname = Path.GetFileNameWithoutExtension(pathRadiusIV);

            _ivFolderPath = Path.GetDirectoryName(pathRadiusIV);
            _neutralSeries = fname.Substring(3, 2);
            _side = fname.Substring(5, 1).ToUpper();
            //_side = (fname.EndsWith("L", StringComparison.CurrentCultureIgnoreCase)) ? Side.LEFT : Side.RIGHT;

            string neutral = "S"+_neutralSeries+_side;

            /* Finding the subject path
             * - Try and find the directory of the neutral series and then set the 
             * subject path to its parent directory.
             */

            
            //first check if the neutral series path contains the IV files
            if (Path.GetFileName(_ivFolderPath).ToUpper().Equals(neutral))
            {
                _subjectPath = Path.GetDirectoryName(_ivFolderPath);
            }
            //else check to see if its one level above
            else if (Path.GetFileName(Path.GetDirectoryName(_ivFolderPath)).ToUpper().Equals(neutral))
            {
                _subjectPath = Path.GetDirectoryName(Path.GetDirectoryName(_ivFolderPath));
            }
            else
                throw new ArgumentException("Unable to locate subject path");

            //Now verify that this is the subject path
            if (!Regex.Match(Path.GetFileName(_subjectPath), @"E\d{5}").Success)
                throw new ArgumentException("Invalid subject path: " + _subjectPath);

        }

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
            if (!Int32.TryParse(fname.Substring(3, 2), out junk))
                return false;

            //check ends in L or R
            if (!fname.EndsWith("L", StringComparison.CurrentCultureIgnoreCase) && !fname.EndsWith("R", StringComparison.CurrentCultureIgnoreCase))
                return false;

            return true;
        }

    }
}
