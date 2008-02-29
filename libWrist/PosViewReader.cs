using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace libWrist
{
    public class PosViewReader
    {
        private string _basePath;
        private string _ivFilePath;
        private int _numBones;
        private string[] _ivFileNames;
        private string[] _kinematicFileNames;

        //optional settings
        private bool _showHams;
        private bool _setColor;
        private bool _loadLigaments;
        private string _baseFiberPath;
        private string _fiberName;
        private int _numFibers;

        private string[] _labels = null;

        public PosViewReader(string posFilename)
        {
            parsePosFile(posFilename);
            string iniFile = Path.Combine(Path.GetDirectoryName(posFilename),Path.GetFileNameWithoutExtension(posFilename)+".ini");
            parsePosViewINIFiles(iniFile);
        }

        #region Public Interfaces
        public string[] IvFileNames
        {
            get
            {
                string[] names = new string[_numBones];
                for (int i = 0; i < _numBones; i++)
                    names[i] = Path.Combine(Path.Combine(_basePath, _ivFilePath), _ivFileNames[i]);
                return names;
            }
        }

        public int NumBones
        {
            get { return _numBones; }
        }

        public bool ShowHams
        {
            get { return _showHams; }
        }

        public bool SetColor
        {
            get { return _setColor; }
        }

        public bool LoadLigaments
        {
            get { return _loadLigaments; }
        }

        public int NumberLigamentFibers
        {
            get { return _numFibers; }
        }

        public string[] LigamentFiberFilenames
        {
            get { return new string[0]; }  //TODO: Fix, need Mikes help with ligament file format
        }

        public string[] RTFileNames
        {
            get
            {
                string[] names = new string[_numBones];
                for (int i = 0; i < _numBones; i++)
                    names[i] = _kinematicFileNames[i] + ".RTp";
                return names;
            }
        }

        public string[] HAMFileNames
        {
            get
            {
                string[] names = new string[_numBones];
                for (int i = 0; i < _numBones; i++)
                    names[i] = _kinematicFileNames[i] + ".HAMp";
                return names;
            }
        }

        public string[] Labels
        {
            get { return _labels; }
        }

        public bool HasLables
        {
            get { return (_labels != null && _labels.Length > 0); }
        }

        #endregion

        #region Parse Config Files
        private void parsePosFile(string fname)
        {
            try
            {
                using (StreamReader reader = new StreamReader(fname))
                {

                    _basePath = reader.ReadLine().Trim();
                    _ivFilePath = reader.ReadLine().Trim();
                    _numBones = Int32.Parse(reader.ReadLine().Trim());
                    _ivFileNames = new string[_numBones];
                    _kinematicFileNames = new string[_numBones];
                    for (int i = 0; i < _numBones; i++)
                        _ivFileNames[i] = reader.ReadLine().Trim();

                    for (int i = 0; i < _numBones; i++)
                        _kinematicFileNames[i] = reader.ReadLine().Trim();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidDataException(String.Format("Pos file ({0}) is in an invalid format!", fname), ex);
            }
        }

        private enum ParseSection
        {
            none, global, label
        }

        private void parsePosViewINIFiles(string fname)
        {
            if (!File.Exists(fname))
                return;

            string headingRegex = "^\\[(\\S+)\\]$";
            string keyRegex = "(?<key>[\\w]+)=(?<value>.*)$";

            Regex heading = new Regex(headingRegex);
            Regex key = new Regex(keyRegex);
            ParseSection section = ParseSection.none;

            System.Collections.ArrayList labels = new System.Collections.ArrayList();

            try
            {
                using (StreamReader reader = new StreamReader(fname))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        Match m = heading.Match(line);
                        if (m.Success)  //it was a heading!
                        {
                            section = parseSectionHeading(m.Groups[1].Value);
                            continue;
                        }

                        if (section == ParseSection.none) continue; //if we are out of a section, move on

                        //not a header check for a body match
                        m = key.Match(line);
                        if (!m.Success)
                            continue;

                        string k = m.Groups["key"].Value;
                        string v = m.Groups["value"].Value;
                        switch (section)
                        {
                            case ParseSection.global:
                                parseGlobalKeyValuePair(k, v);
                                break;
                            case ParseSection.label:
                                labels.Add(v);
                                break;
                        }
                    }
                    _labels = (string[])labels.ToArray(typeof(string));
                }
            }
            catch (Exception ex)
            {
                throw new InvalidDataException(String.Format("Pos ini file ({0}) is in an invalid format!", fname), ex);
            }
        }

        private void parseGlobalKeyValuePair(string key, string value)
        {
            key = key.ToLower().Trim();
            switch (key)
            {
                case "showham":
                    _showHams = value.Equals("1") ? true : false;
                    break;
                case "setcolor":
                    _setColor = value.Equals("1") ? true : false;
                    break;
                case "loadligaments":
                    _loadLigaments = value.Equals("1") ? true : false;
                    break;
                case "basefiberpath":
                    _baseFiberPath = value;
                    break;
                case "fibername":
                    _fiberName = value;
                    break;
                case "numfibers":
                    _numFibers = Int32.Parse(value);
                    break;
            }
        }

        private ParseSection parseSectionHeading(string heading)
        {
            heading = heading.ToLower().Trim();
            switch (heading)
            {
                case "global":
                    return ParseSection.global;
                case "label":
                    return ParseSection.label;
                default:
                    return ParseSection.none;
            }
        }
        #endregion

    }
}
