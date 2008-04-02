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

        public struct CropValues
        {
            string Subject;
            int PositionNumber;
            Wrist.Sides side;
        }

        public CropValuesParser(string cropValuesFilename)
        {
            if (!File.Exists(cropValuesFilename))
                throw new ArgumentException("Crop values file does not exist: " + cropValuesFilename);

            _filename = cropValuesFilename;
            _cropData = new Hashtable();

        }

        private void parseFile()
        {

        }
    }
}
