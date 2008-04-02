using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;

namespace libWrist
{
    /// <summary>
    /// This is a class that will read in the crop_values.txt file and parse the 
    /// results.
    /// </summary>
    public class CropValuesParser
    {
        private string _filename;
        private string _subject;
        private int _numPositions;
        private Hashtable _cropData;

        const string _lineRegex = @"^(E\d{5})\s+	# Subject
(\d{2})\s+		# Series Number
([LR])\s+		# L/R Left or Right Wrist
(\d{1,3})\s+	# MinX
(\d{1,3})\s+	# MaxX
(\d{1,3})\s+	# MinY
(\d{1,3})\s+	# MaxY
(\d{1,3})\s+	# MinZ
(\d{1,3})\s+	# MaxZ
([\d\.]+)\s+	# Voxel X & Y
([\d\.]+)		# Voxel Z";

        public struct CropValues
        {
            public string Subject;

            /// <summary>
            /// Key is the Series & Side combined :)
            /// </summary>
            public string Key;
            public int PositionNumber;
            public Wrist.Sides Side;
            public int MinX;
            public int MaxX;
            public int MinY;
            public int MaxY;
            public int MinZ;
            public int MaxZ;
            public double VoxelX;
            public double VoxelY;
            public double VoxelZ;

            /// <summary>
            /// (Optional) Defines the slice that contains the tip of the radial styloid.
            /// Can be usefull for setting all the lengths of radii to the same length.
            /// If not set, should be = -1
            /// </summary>
            public int RadialStyloid;
        }

        public CropValuesParser(string cropValuesFilename)
        {
            if (!File.Exists(cropValuesFilename))
                throw new ArgumentException("Crop values file does not exist: " + cropValuesFilename);

            _filename = cropValuesFilename;
            _subject = "";
            _cropData = new Hashtable();

            using (StreamReader reader = new StreamReader(cropValuesFilename))
            {
                parseFile(reader);
            }
        }

        private void parseFile(StreamReader filestream)
        {
            Regex r = new Regex(_lineRegex, RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            string line;
            while (!filestream.EndOfStream)
            {
                line = filestream.ReadLine().Trim();
                Match m = r.Match(line);
                if (!m.Success)
                    continue; //skip

                CropValues cv = new CropValues();
                cv.Subject = m.Groups[1].Value.ToUpper();
                cv.PositionNumber = Int32.Parse(m.Groups[2].Value);
                cv.Side = m.Groups[3].Value.Equals("L") ? Wrist.Sides.LEFT : Wrist.Sides.RIGHT;
                cv.MinX = Int32.Parse(m.Groups[4].Value);
                cv.MaxX = Int32.Parse(m.Groups[5].Value);
                cv.MinY = Int32.Parse(m.Groups[6].Value);
                cv.MaxY = Int32.Parse(m.Groups[7].Value);
                cv.MinZ = Int32.Parse(m.Groups[8].Value);
                cv.MaxZ = Int32.Parse(m.Groups[9].Value);
                cv.VoxelX = Double.Parse(m.Groups[10].Value);
                cv.VoxelY = cv.VoxelX;
                cv.VoxelZ = Double.Parse(m.Groups[11].Value);
                cv.RadialStyloid = -1; //not used yet
                cv.Key = generatePositionKey(cv.PositionNumber, cv.Side);

                //if this is the first, then we can just save
                if (_subject.Length == 0)
                {
                    _subject = cv.Subject;
                }
                if (_cropData.ContainsKey(cv.Key))
                    throw new ArgumentOutOfRangeException("Duplicate entry in crop values for series & side: " + cv.Key);

                _cropData.Add(cv.Key, cv);
                _numPositions++;
            }
        }

        #region Public Interfaces
        public Hashtable CropData
        {
            get { return _cropData; }
        }

        public string Subject
        {
            get { return _subject; }
        }

        public int NumberPositions
        {
            get { return _numPositions; }
        }
        #endregion

        private string generatePositionKey(int series, Wrist.Sides side)
        {
            return String.Format("{0:00}{1}", series, side == Wrist.Sides.LEFT ? "L" : "R");
        }

        private string generatePositionKey(int series, string side)
        {
            if (side.Length != 1)
                throw new ArgumentException("Side must be a string of length 1, should only be \"L\" or \"R\".", "side");
            return String.Format("{0:00}{1}", series, side);
        }

        public bool hasPosition(string positionKey)
        {
            return _cropData.ContainsKey(positionKey);
        }

        public bool hasPosition(int series, Wrist.Sides side)
        {
            return hasPosition(generatePositionKey(series,side));
        }

        public bool hasPosition(int series, string side)
        {
            return hasPosition(generatePositionKey(series, side));
        }

        public CropValues getCropData(string positionKey)
        {
            return (CropValues)_cropData[positionKey];
        }

        public CropValues getCropData(int series, Wrist.Sides side)
        {
            return getCropData(generatePositionKey(series, side));
        }

        public CropValues getCropData(int series, string side)
        {
            return getCropData(generatePositionKey(series, side));
        }

        public string[] getAllSeries()
        {
            ArrayList keys = new ArrayList();
            IDictionaryEnumerator e = _cropData.GetEnumerator();
            while (e.MoveNext())
            {
                keys.Add(e.Key);
            }
            //keys.Sort();  //Sort the list?
            return (string[])keys.ToArray(typeof(string));
        }
    }
}