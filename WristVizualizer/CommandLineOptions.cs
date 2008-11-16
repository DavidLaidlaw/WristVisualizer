using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Mono.GetOptions;

namespace WristVizualizer
{
    class CommandLineOptions : Options
    {
        public string error = "";

        [Option(1,"Path to the {subject} directory to use",'s',"subject")]
        public string Subject = null;

        [Option("Wrist {side} (L/R)", "side")]
        public string SideString = null;

        [Option(1,"Reference bone. Specify three letter {bone code} (ie. 'rad')",'r',"refbone")]
        public string ReferenceBone = null;

        [Option(1,"Test bone. Specify three letter {bone code} (ie. 'rad')",'t',"testbone")]
        public string TestBone = null;

        //[Option(1, "Fixed bone. Specify three letter bone code (ie. 'rad')", 'f', "fixedbone")]
        //public string FixedBone = null;

        [Option("Position List. Specify {index} (0==neutral) or 4 char code",'p',"poslist")]
        public string PositionList = null;

        [Option("Max {distance} for color map", "maxcolordistance")]
        public double MaxColorDistance = 0;

        [Option("Contour {Distances}", 'c', "contours")]
        public string ContourString = null;

        [Option("{Filename} for contour file", "saveContour")]
        public string SaveContourFname = null;

        [Option("{Filename} for patch file", "savePatch")]
        public string SavePatchFname = null;

        [Option("Save area and centroid data to disk. Optionally: specify {directory} to save to (Defaults to <subject>\\<series>\\Distances\\)", "saveArea")]
        public string SaveAreaDirectory = null;

        [Option("Use multi-threaded processing for increased speed", "multithread")]
        public bool MultiThread = false;   


        public bool isBatchMode()
        {
            //check if ANY options were passed to us. If so, thats the mode we run :)
            if (Subject != null) return true;
            if (SideString != null) return true;
            if (ReferenceBone != null) return true;
            if (TestBone != null) return true;
            if (PositionList != null) return true;
            if (ContourString != null) return true;
            //if (FixedBone != null) return true;
            if (MaxColorDistance != 0) return true;

            //if we get here, then nothing was set
            return false;
        }

        public double[] GetCoutourDistances()
        {
            return GetListDoubles(this.ContourString);
        }

        private static double[] GetListDoubles(string doubleString)
        {
            string[] stringparts = doubleString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            double[] distances = new double[stringparts.Length];
            for (int i = 0; i < stringparts.Length; i++)
            {
                double dist;
                if (!Double.TryParse(stringparts[i], out dist))
                    throw new ArgumentException(String.Format("Error parsing list of doubles: '{0}'. Not a valid double", stringparts[i]));
                distances[i] = dist;
            }
            return distances;
        }

        public string[] GetPositionNames()
        {
            return GetPositionNames(this.PositionList);
        }

        private static string[] GetPositionNames(string positionsString)
        {
            string[] positions = positionsString.ToUpper().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < positions.Length; i++)
            {
                if (positions[i].Length == 3) //check for the case where the leading S is missing
                    positions[i] = "S" + positions[i];

                if (!Regex.IsMatch(positions[i], @"^S\d\d[LR]$"))
                    throw new ArgumentException(String.Format("Invalid position: '{0}'", positions[i]));
            }
            return positions;
        }

        public int GetTestBoneIndex()
        {
            int[] indices = GetBoneIndexesFromShortName(this.TestBone);
            if (indices.Length != 1)
                throw new ArgumentException("Can only specify a single test bone");
            return indices[0];
        }

        public int GetReferenceBoneIndex()
        {
            int[] indices = GetBoneIndexesFromShortName(this.ReferenceBone);
            if (indices.Length != 1)
                throw new ArgumentException("Can only specify a single test bone");
            return indices[0];
        }

        //public int GetFixedBoneIndex()
        //{
        //    int[] indices = GetBoneIndexesFromShortName(this.FixedBone);
        //    if (indices.Length != 1)
        //        throw new ArgumentException("Can only specify a single test bone");
        //    return indices[0];
        //}

        public int[] GetTestBoneIndices()
        {
            return GetBoneIndexesFromShortName(this.TestBone);
        }

        public int[] GetReferenceBoneIndices()
        {
            return GetBoneIndexesFromShortName(this.ReferenceBone);
        }

        private static int[] GetBoneIndexesFromShortName(string boneListString)
        {
            string[] parts = boneListString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            int[] boneIndices = new int[parts.Length];
            for (int i = 0; i < parts.Length; i++)
                boneIndices[i] = libWrist.Wrist.GetBoneIndexFromShortName(parts[i]);
            return boneIndices;
        }


        #region Redirect ConsoleOutput To MessageBox
        public new void ProcessArgs(string[] args)
        {
            //save existing
            TextWriter stdout = Console.Out;
            TextWriter stderr = Console.Error;
            string msg;

            //create new
            using (MemoryStream s = new MemoryStream())
            {
                StreamWriter writer = new StreamWriter(s);
                Console.SetOut(writer);
                Console.SetError(writer);
                try
                {
                    base.ProcessArgs(args);
                }
                catch { }
                finally
                {
                    writer.Flush(); //otherwise we don't get our output...
                    s.Seek(0, SeekOrigin.Begin);
                    StreamReader r = new StreamReader(s);
                    msg = r.ReadToEnd();
                    if (msg.Length > 0)
                        MessageBox.Show(msg, Application.ProductName);
                }
            }
            Console.SetOut(stdout);
            Console.SetError(stderr);
        
        }

        [Option("Show this help list", '?', "help")]
        public override WhatToDoNext DoHelp()
        {
            showHelp();
            return WhatToDoNext.AbandonProgram;
        }

        [Option("Show an additional help list", "help2")]
        public override WhatToDoNext DoHelp2()
        {
            showHelp2();
            return WhatToDoNext.AbandonProgram;
        }

        [Option("Display version and licensing information", 'V', "version")]
        public override WhatToDoNext DoAbout()
        {
            showAbout();
            return WhatToDoNext.AbandonProgram;
        }

        [Option("Show usage syntax and exit", "usage")]
        public override WhatToDoNext DoUsage()
        {
            showUsage();
            return WhatToDoNext.AbandonProgram;
        }
        public void showHelp()
        {
            string msg = getMessageAsString("help");
            MessageBox.Show(msg, Application.ProductName);
        }

        public void showHelp2()
        {
            MessageBox.Show(getMessageAsString("help2"), Application.ProductName);
        }

        public void showUsage()
        {
            MessageBox.Show(getMessageAsString("usage"), Application.ProductName);
        }

        public void showAbout()
        {
            MessageBox.Show(getMessageAsString("about"), Application.ProductName);
        }

        private string getMessageAsString(string type)
        {
            //save existing
            TextWriter stdout = Console.Out;
            string msg;

            //create new
            using (MemoryStream s = new MemoryStream())
            {
                StreamWriter writer = new StreamWriter(s);
                Console.SetOut(writer);

                switch (type)
                {
                    case "help":
                        base.DoHelp();
                        break;
                    case "help2":
                        base.DoHelp2();
                        break;
                    case "about":
                        base.DoAbout();
                        break;
                    case "usage":
                        base.DoUsage();
                        break;
                }
                writer.Flush(); //otherwise we don't get our output...
                s.Seek(0, SeekOrigin.Begin);
                StreamReader r = new StreamReader(s);
                msg = r.ReadToEnd();
            }
            Console.SetOut(stdout);
            return msg;
        }
        #endregion


    }
}
