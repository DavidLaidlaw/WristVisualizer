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

        public enum Database
        {
            DATA,
            DATABASE,
        }

        private struct SeriesInfo
        {
            public string series;
            public int seriesNum;
            public string motionFile;
        }

        private string[] _bnames = { "rad", "uln", "sca", "lun", "trq", "pis", "tpd", "tpm", "cap", "ham", "mc1", "mc2", "mc3", "mc4", "mc5" };
        private string[] _bpaths;

        private string _subjectPath;
        private string _subject;
        private string _ivFolderPath;
        private string _neutralSeriesNum;
        private string _neutralSeries;
        private string _side;
        private string _radius;
        private string _inertiaFile;
        private Database _db;
        private SeriesInfo[] _info;

        //this is precompiled crap
        private string[] _series;

        public Wrist(string pathRadiusIV)
        {
            _radius = pathRadiusIV;
            _bpaths = new string[_bnames.Length];
            setupPaths();
            findAllSeries();
        }

        public Wrist()
        {
        }

        public void setupWrist(string pathRadiusIV)
        {
            _radius = pathRadiusIV;
            _bpaths = new string[_bnames.Length];
            setupPaths();
            findAllSeries();
        }

        public string neutralSeries
        {
            get { return _neutralSeries; }
        }

        public string inertiaFile
        {
            get { return _inertiaFile; }
        }

        public string[] bpaths
        {
            get { return _bpaths; }
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
                    s[i] = _info[i].motionFile;
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

            return _info[positionIndex].motionFile;
        }

        public string getSeriesPath(int positionIndex)
        {
            if (positionIndex >= _info.Length)
                throw new ArgumentOutOfRangeException("Position index exceeds length of array.");

            return Path.Combine(_subjectPath, _info[positionIndex].series);
        }

        private void findAllSeries()
        {
            if (_db == Database.DATA)
                findAllSeries_Data();
            else
                findAllSeries_Database();
        }

        private void findAllSeries_Database()
        {
            DirectoryInfo sub = new DirectoryInfo(_subjectPath);
            DirectoryInfo[] series = sub.GetDirectories("??" + _side);
            System.Collections.ArrayList list = new System.Collections.ArrayList();
            foreach (DirectoryInfo s in series)
            {
                if (!Regex.Match(s.Name, @"(\d\d)" + _side, RegexOptions.IgnoreCase).Success)
                    continue;

                string motion = Path.Combine(s.FullName, _subject + "_Motion" + s.Name + ".dat");
                if (File.Exists(motion))
                {
                    SeriesInfo info = new SeriesInfo();
                    info.motionFile = motion;
                    info.series = s.Name;
                    info.seriesNum = Int32.Parse(s.Name.Substring(0, 2));

                    list.Add(info);
                }
            }
            _info = (SeriesInfo[])list.ToArray(typeof(SeriesInfo));
        }

        private void findAllSeries_Data()
        {
            DirectoryInfo sub = new DirectoryInfo(_subjectPath);
            DirectoryInfo[] series = sub.GetDirectories("S??" + _side);
            System.Collections.ArrayList list = new System.Collections.ArrayList();
            foreach (DirectoryInfo s in series)
            {
                if (!Regex.Match(s.Name,@"S(\d\d)"+_side,RegexOptions.IgnoreCase).Success)                
                    continue;

                string motion = Path.Combine(s.FullName, "Motion" + _neutralSeriesNum + _side + s.Name.Substring(1, 2) + _side + ".dat");
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

        public void setupPaths() { setupPaths(_radius); }
        private void setupPaths(string pathRadiusIV)
        {
            if (isDataStructure(pathRadiusIV))
                setupPaths_Data(pathRadiusIV);
            else if (isDatabaseStructure(pathRadiusIV))
                setupPaths_Database(pathRadiusIV);
            else
                throw new ArgumentException("Bone provided is not a radius");
        }

        private void setupPaths_Database(string pathRadiusIV)
        {
            if (!isDatabaseStructure(pathRadiusIV))
                throw new ArgumentException("Bone provided is not a radius for a database");

            string fname = Path.GetFileNameWithoutExtension(pathRadiusIV);
            string ext = Path.GetExtension(pathRadiusIV);
            _db = Database.DATABASE;

            _ivFolderPath = Path.GetDirectoryName(pathRadiusIV);
            _neutralSeriesNum = "0";
            _neutralSeries = "Neut";
            _side = fname.Substring(10, 1).ToUpper();

            /* For the database, the filestructure is more rigid, so its easier to find 
             * everything
             */

            //first check if the IV files are in the right place
            string folderName = _side.Equals("L") ? "leftiv" : "rightiv";
            if (!Path.GetFileName(_ivFolderPath).ToLower().Equals(folderName))
            {
                throw new ArgumentException("IV Folder is of the wrong filename format");
            }

            _subjectPath = Path.GetDirectoryName(_ivFolderPath);
            _subject = Path.GetFileName(_subjectPath);
            string infoFolder = Path.Combine(_subjectPath,_side.Equals("L") ? "LeftInfo" : "RightInfo");
            _inertiaFile = Path.Combine(infoFolder,_subject + "_inertia_" + _side + ".dat");

            //Now verify that this is the subject path
            if (!Regex.Match(Path.GetFileName(_subjectPath), @"^\d{5}$").Success)
                throw new ArgumentException("Invalid subject path: " + _subjectPath);

            //now setup bonenames & paths
            for (int i = 0; i < _bnames.Length; i++)
                _bpaths[i] = Path.Combine(_ivFolderPath, _subject + "_" + _bnames[i] + "_" + _side + ext);
        }

        private void setupPaths_Data(string pathRadiusIV)
        {
            if (!isDataStructure(pathRadiusIV))
                throw new ArgumentException("Bone provided is not a radius for a data");

            _db = Database.DATA;

            string fname = Path.GetFileNameWithoutExtension(pathRadiusIV);
            string ext = Path.GetExtension(pathRadiusIV);

            _ivFolderPath = Path.GetDirectoryName(pathRadiusIV);
            _neutralSeriesNum = fname.Substring(3, 2);
            _side = fname.Substring(5, 1).ToUpper();

            _neutralSeries = "S" + _neutralSeriesNum + _side;


            //now setup bonenames & paths
            for (int i = 0; i < _bnames.Length; i++)
                _bpaths[i] = Path.Combine(_ivFolderPath, _bnames[i] + _neutralSeriesNum + _side + ext);

            /* Finding the subject path
             * - Try and find the directory of the neutral series and then set the 
             * subject path to its parent directory.
             */

            
            //first check if the neutral series path contains the IV files
            if (Path.GetFileName(_ivFolderPath).ToUpper().Equals(_neutralSeries))
            {
                _subjectPath = Path.GetDirectoryName(_ivFolderPath);
            }
            //else check to see if its one level above
            else if (Path.GetFileName(Path.GetDirectoryName(_ivFolderPath)).ToUpper().Equals(_neutralSeries))
            {
                _subjectPath = Path.GetDirectoryName(Path.GetDirectoryName(_ivFolderPath));
            }
            else
                throw new ArgumentException("Unable to locate subject path");

            _subject = Path.GetFileName(_subjectPath);
            _inertiaFile = Path.Combine(Path.Combine(_subjectPath,_neutralSeries),"inertia"+ _neutralSeriesNum + _side + ".dat");

            //Now verify that this is the subject path
            if (!Regex.Match(Path.GetFileName(_subjectPath), @"E\d{5}").Success)
                throw new ArgumentException("Invalid subject path: " + _subjectPath);

        }

        static public bool isDatabaseStructure(string radiusPath)
        {
            string fname = Path.GetFileNameWithoutExtension(radiusPath);
            return (Regex.Match(fname, @"^\d{5}_rad_[lr]$", RegexOptions.IgnoreCase).Success);
        }

        static public bool isDataStructure(string radiusPath)
        {
            string fname = Path.GetFileNameWithoutExtension(radiusPath);
            return (Regex.Match(fname, @"^rad\d{2}[lr]$", RegexOptions.IgnoreCase).Success);;
        }

        static public bool isRadius(string path)
        {
            return (isDataStructure(path) || isDatabaseStructure(path));
        }

        public bool isRadius()
        {
            return isRadius(_radius);
        }

    }
}
