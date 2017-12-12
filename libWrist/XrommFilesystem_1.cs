using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace libWrist
{
    public class XrommFilesystem
    {

        public struct TrialInfo
        {
            public string TrialName;
            public int TrialNumber;
            public string KinematicFilename;
        }

        private string[] _bnames;

        private string[] _bpaths;
        private string[] _distanceFieldPaths;

        private string _pathFirstIVFile;

        private string _subjectPath;
        private string _subject;
        private string _ivFolderPath;
        private string _modelsPath;
        private TrialInfo[] _info;


        public XrommFilesystem(string PathFirstIVFile)
        {
            _pathFirstIVFile = PathFirstIVFile;
            SetupXromm(PathFirstIVFile);

            _distanceFieldPaths = new string[NumBones];

            //_bpaths = new string[_bnames.Length];
            //_distanceFieldPaths = new string[_bnames.Length];
            //setupPaths();
            //findAllSeries();
        }

        [Obsolete("Test method only")]
        public XrommFilesystem()
        {
            string test = @"P:\XROMM\SampleStudy\X00001\Models\IV\X00001_Fem_L.iv";
            SetupXromm(test);
        }

        private void SetupXromm(string PathFirstIVFile)
        {
            //check that the file exists
            if (!File.Exists(PathFirstIVFile))
                throw new FileNotFoundException("Can not find IV file specified");

            _ivFolderPath = Path.GetDirectoryName(PathFirstIVFile);
            _modelsPath = Path.GetDirectoryName(_ivFolderPath);
            _subjectPath = Path.GetDirectoryName(_modelsPath);
            _subject = Path.GetFileName(_subjectPath);

            string boneFileName = Path.GetFileName(PathFirstIVFile);
            string boneFileNameNoExtension = Path.GetFileNameWithoutExtension(boneFileName);

            Match m = Regex.Match(boneFileNameNoExtension, @"^(X[a-z]*\d{5})_([a-z0-9]+)_([lr])$", RegexOptions.IgnoreCase);
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
            DirectoryInfo[] trials = SubjectDir.GetDirectories("Trial???");
            foreach (DirectoryInfo trialDir in trials)
            {
                m = Regex.Match(trialDir.Name, @"Trial(\d{3})", RegexOptions.IgnoreCase);
                if (!m.Success)
                    continue;

                string trialNumberString = m.Groups[1].Value.ToLower();
                int trialNumber = Int32.Parse(trialNumberString);

                string kinematicFilePattern = String.Format("{0}_Trial{1}_*AbsTforms.csv", _subject, trialNumberString);
                DirectoryInfo kinematicDirectory = new DirectoryInfo(Path.Combine(trialDir.FullName, "XROMM"));
                
                //check that there is an XROMM directory
                if (!kinematicDirectory.Exists) continue;

                FileInfo[] possbileFiles = kinematicDirectory.GetFiles(kinematicFilePattern);
                //Danny wants us to report everything we find...
                foreach (FileInfo file in possbileFiles)
                {
                    //we want to grab middle section of the filename to dispaly, so we need a regex
                    string savePattern = String.Format("^{0}_Trial{1}_(.*)AbsTforms.csv$", _subject, trialNumberString);
                    //I don't think this regex can fail, give the filter we used get the files....
                    string subID = Regex.Match(file.Name, savePattern, RegexOptions.IgnoreCase).Groups[1].Value;
                    //remove any trailing '_' that might be there
                    subID = subID.TrimEnd('_');

                    //TODO: Save file information.... e.g. "{0}_Trial{1}_xyzptsBUTTER25_sm125AbsTforms.csv"
                    TrialInfo info = new TrialInfo();
                    info.KinematicFilename = possbileFiles[0].FullName;
                    info.TrialName = String.Format("T{0:00}", trialNumber);
                    if (subID.Length > 0)
                        info.TrialName += String.Format("-{0}", subID);
                    info.TrialNumber = trialNumber;

                    trialInfo.Add(info);
                }
            }
            _info = trialInfo.ToArray();
        }

        #region Static XROMM detection Methods
        public static bool IsXrommFile(string[] filenames)
        {
            if (filenames == null || filenames.Length != 1) return false;
            return IsXrommFile(filenames[0]);
        }
        public static bool IsXrommFile(string PathFirstIVFile)
        {
            try
            {
                //check that the file exists
                if (!File.Exists(PathFirstIVFile))
                    return false;

                string ivFolderPath = Path.GetDirectoryName(PathFirstIVFile);
                string modelsPath = Path.GetDirectoryName(ivFolderPath);
                string subjectPath = Path.GetDirectoryName(modelsPath);
                string subject = Path.GetFileName(subjectPath);

                string boneFileName = Path.GetFileName(PathFirstIVFile);
                string boneFileNameNoExtension = Path.GetFileNameWithoutExtension(boneFileName);

                Match m = Regex.Match(boneFileNameNoExtension, @"^(X[a-z]*\d{5})_([a-z0-9]+)_([lr])$", RegexOptions.IgnoreCase);
                if (!m.Success)
                    return false;

                string subjectID = m.Groups[1].Value;
                string boneName = m.Groups[2].Value.ToLower();
                string side = m.Groups[3].Value.ToLower();

                if (!String.Equals(subject, subjectID, StringComparison.InvariantCultureIgnoreCase))
                    return false;

                string Trial001_Folder = Path.Combine(subjectPath, "Trial001");
                if (!Directory.Exists(Trial001_Folder))
                    return false;

                //Danny says my original assumption that the Trial001 will always have kinematic data is false
                //string kinematicFname = String.Format("{0}_Trial001_xyzptsBUTTER25_sm125AbsTforms.csv", subject);
                //string kinematicFilePath = Path.Combine(Path.Combine(Trial001_Folder, "XROMM"), kinematicFname);

                //return File.Exists(kinematicFilePath);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

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

        public string PathFirstIVFile
        {
            get { return _pathFirstIVFile; }
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

        public TrialInfo[] Trials
        {
            get { return _info; }
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
