using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace libWrist
{
    public class XrommFilesystem
    {

        private struct TrialInfo
        {
            public string TrialName;
            public int TrialNumber;
            public string KinematicFilename;
        }

        private string[] _bnames;

        private string[] _bpaths;
        private string[] _distanceFieldPaths;

        private string _subjectPath;
        private string _subject;
        private string _ivFolderPath;
        private TrialInfo[] _info;


        public XrommFilesystem(string pathRadiusIV)
        {
            //_bpaths = new string[_bnames.Length];
            //_distanceFieldPaths = new string[_bnames.Length];
            //setupPaths();
            //findAllSeries();
        }

        public static bool IsXrommFile(string[] files)
        {
            return false;
        }

        public XrommFilesystem()
        {
            string test = @"C:\LocalCopies\XROMM\X12345\IV\X12345_tibia_L.iv";
            SetupXromm(test);
        }

        public void SetupXromm(string PathFirstIVFile)
        {
            //check that the file exists
            if (!File.Exists(PathFirstIVFile))
                throw new FileNotFoundException("Can not find IV file specified");

            _ivFolderPath = Path.GetDirectoryName(PathFirstIVFile);
            _subjectPath = Path.GetDirectoryName(_ivFolderPath);
            _subject = Path.GetFileName(_subjectPath);

            string boneFileName = Path.GetFileName(PathFirstIVFile);
            string boneFileNameNoExtension = Path.GetFileNameWithoutExtension(boneFileName);

            Match m = Regex.Match(boneFileNameNoExtension, @"^(X\d{5})_([a-z0-9]+)_([lr])$", RegexOptions.IgnoreCase);
            if (!m.Success)
                throw new WristException("Initial IV file is not in a valid XROMM format");

            string subjectID = m.Groups[1].Value;
            string boneName = m.Groups[2].Value.ToLower();
            string side = m.Groups[3].Value.ToLower();

            if (!String.Equals(_subject, subjectID, StringComparison.InvariantCultureIgnoreCase))
                throw new WristException("Subject specified in IV file name does not match subject folder name");

            //need to now find all of the bones.... (can I limit to IV files only....?)
            List<string> ivFiles = new List<string>();
            List<string> boneNames = new List<string>();
            DirectoryInfo ivFolderDir = new DirectoryInfo(_ivFolderPath);
            FileInfo[] ivFilesFileInfo = ivFolderDir.GetFiles(String.Format("{0}*.iv", _subject));
            foreach (FileInfo file in ivFilesFileInfo)
            {
                m = Regex.Match(file.Name, String.Format(@"^{0}_([a-z0-9]+)_[lr]\.iv$", _subject), RegexOptions.IgnoreCase);
                if (!m.Success)
                    continue;

                ivFiles.Add(file.FullName.ToLower());
                boneNames.Add(m.Groups[1].Value.ToLower());
            }
            ivFiles.Sort();
            boneNames.Sort();

            _bnames = boneNames.ToArray();
            _bpaths = ivFiles.ToArray();

            //now to find all of the trials
            List<TrialInfo> trialInfo = new List<TrialInfo>();
            DirectoryInfo SubjectDir = new DirectoryInfo(_subjectPath);
            DirectoryInfo[] trials = SubjectDir.GetDirectories("Trial_???");
            foreach (DirectoryInfo trialDir in trials)
            {
                m = Regex.Match(trialDir.Name, @"Trial_(\d{3})", RegexOptions.IgnoreCase);
                if (!m.Success)
                    continue;

                string trialNumberString = m.Groups[1].Value.ToLower();
                int trialNumber = Int32.Parse(trialNumberString);

                string kinematicFname = String.Format("{0}_{1}_kinematics.csv", _subject, trialNumberString);
                string kinematicFilePath = Path.Combine(trialDir.FullName, kinematicFname);
                if (File.Exists(kinematicFilePath))
                {
                    TrialInfo info = new TrialInfo();
                    info.KinematicFilename = kinematicFilePath;
                    info.TrialName = ""; //empty for now!
                    info.TrialNumber = trialNumber;

                    trialInfo.Add(info);
                }
            }
            _info = trialInfo.ToArray();
        }

        #region Public Accessors

        /// <summary>
        /// The wrist's subject (ie. E02366 (data) or 12345 (database))
        /// </summary>
        public string subject
        {
            get { return _subject; }
        }

        /// <summary>
        /// Full path to the subject folder. (ie p:\data\young\E02366)
        /// </summary>
        public string subjectPath
        {
            get { return _subjectPath; }
        }



        /// <summary>
        /// An array of full paths to all 15 bones for the wrist
        /// </summary>
        public string[] bpaths
        {
            get { return _bpaths; }
        }

        /// <summary>
        /// An array containingn the full path to the distance fields for each bone
        /// (ie {"p:\Data\...\E00001\S15R\DistanceFields\cap15R_mri", "p:\Data\...\E00001\S15R\DistanceFields\ham15R_mri",...})
        /// </summary>
        public string[] DistanceFieldPaths
        {
            get { return _distanceFieldPaths; }
        }


        /// <summary>
        /// The path to the SeriesNames.ini file (optional) that can give nice names for each series
        /// </summary>
        public string SeriesNamesFilename
        {
            get
            {
                return Path.Combine(_subjectPath, @"SeriesNames.ini");
            }
        }

        public string[] ShortBoneNames
        {
            get { return _bnames; }
        }

        public int NumBones
        {
            get { return _bnames.Length; }
        }

        #endregion

        /*
        #region Public Get Methods
        /// <summary>
        /// Given a series, it will try and find the index for it in the current object. (Case sensitive)
        /// </summary>
        /// <param name="series">Series name to check</param>
        /// <returns>Index to the series, 0 is neutral</returns>
        public int getSeriesIndexFromName(string series)
        {
            //check for neutral
            if (_neutralSeries.Equals(series)) return 0;

            for (int i = 0; i < _info.Length; i++)
                if (_info[i].series.Equals(series)) return i+1;

            throw new ArgumentException("Unable to locate series in list");
        }

        /// <summary>
        /// Returns the full full path to the motion file for the given position index
        /// </summary>
        /// <param name="positionIndex">Index of the position you are looking for</param>
        /// <returns>Full path to motion file</returns>
        public string getMotionFilePath(int positionIndex)
        {
            if (positionIndex >= _info.Length)
                throw new ArgumentOutOfRangeException("Position index exceeds length of array.");

            return _info[positionIndex].motionFile;
        }

        /// <summary>
        /// Returns the full full path to the series folder for the given index
        /// </summary>
        /// <param name="positionIndex">Index of the position you are looking for</param>
        /// <returns>Full apth to the series folder</returns>
        public string getSeriesPath(int positionIndex)
        {
            if (positionIndex >= _info.Length)
                throw new ArgumentOutOfRangeException("Position index exceeds length of array.");

            return Path.Combine(_subjectPath, _info[positionIndex].series);
        }

        #endregion

        */


    }
}
