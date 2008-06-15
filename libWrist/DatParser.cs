using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using DotNetMatrix;
using libCoin3D;


namespace libWrist
{
    public struct TransformRT
    {
        public GeneralMatrix R;
        public GeneralMatrix T;
    }

    public class DatParser
    {
        public DatParser()
        {
   
        }

        public static Transform[][] makeAllTransforms(string[] motionFiles, int numBones)
        {
            Transform[][] transforms = new Transform[motionFiles.Length][];
            for (int i = 0; i < motionFiles.Length; i++)
            {
                transforms[i] = new Transform[numBones];
                TransformRT[] tfm = parseMotionFileToRT(motionFiles[i]);
                for (int j = 0; j < numBones; j++)
                {
                    transforms[i][j] = new Transform();
                    addRTtoTransform(tfm[j], transforms[i][j]);
                }
            }
            return transforms;
        }

        public static double[][] parseDatFile(StreamReader filestream)
        {
            char[] div = { ' ', '\t', ',' };
            System.Collections.ArrayList dat = new System.Collections.ArrayList(100);

            Regex reg = new Regex(@"([-\d\.e+]+)");
            while (!filestream.EndOfStream)
            {
                string line = filestream.ReadLine().Trim();
                if (line.Length == 0) continue;

                MatchCollection m = reg.Matches(line);
                double[] values = new double[m.Count];
                for (int i = 0; i < m.Count; i++)
                {
                    values[i] = Double.Parse(m[i].Value);
                }
                dat.Add(values);
            }
            return (double[][])dat.ToArray(typeof(double[]));
        }

        public static double[][] parseDatFile(string filename)
        {
            if (!File.Exists(filename))
                throw new ArgumentException("File does not exist. (" + filename + ")");

            using (StreamReader r = new StreamReader(filename))
            {
                return parseDatFile(r);
            }            
        }

        public static double[][] parseMotionFile(string filename)
        {
            double[][] dat = parseDatFile(filename);
            if (dat.Length != 60)
                throw new ArgumentException("Motion files should have 60 lines");

            for (int i = 0; i < dat.Length; i++)
            {
                if (dat[i].Length != 3)
                    throw new ArgumentException("Each row of the motion file should have 3 elements. (Line: " + i.ToString() + ")");
            }
            return dat;
        }

        private static double[][] parseInertiaFile(string filename)
        {
            double[][] dat = parseDatFile(filename);
            if (dat.Length != 75)
                throw new ArgumentException("Inertia files should have 75 lines");

            for (int i = 0; i < dat.Length; i++)
            {
                if (dat[i].Length != 3)
                    throw new ArgumentException("Each row of the inertia files should have 3 elements. (Line: " + i.ToString() + ")");
            }
            return dat;
        }

        public static TransformRT[] parseInertiaFileToRT(string filename)
        {
            double[][] dat = parseInertiaFile(filename);
            TransformRT[] inertias = new TransformRT[15];
            for (int i = 0; i < 15; i++)
            {
                inertias[i] = new TransformRT();
                double[][] tempR = { dat[i * 5 + 2], dat[i * 5 + 3], dat[i * 5 + 4] };
                inertias[i].R = new GeneralMatrix(tempR, 3, 3);
                inertias[i].T = new GeneralMatrix(dat[i * 5], 1);
            }
            return inertias;
        }

        private static double[][] parseACSFile(string filename)
        {
            double[][] dat = parseDatFile(filename);
            if (dat.Length != 4)
                throw new ArgumentException("ACS files should have 4 lines");

            for (int i = 0; i < dat.Length; i++)
            {
                if (dat[i].Length != 3)
                    throw new ArgumentException("Each row of the ACS files should have 3 elements. (Line: " + i.ToString() + ")");
            }
            return dat;
        }

        public static TransformRT[] parseACSFileToRT(string filename)
        {
            double[][] dat = parseACSFile(filename);
            TransformRT[] ACS = new TransformRT[1];

            ACS[0] = new TransformRT();
            double[][] tempR = { dat[0], dat[1], dat[2] };
            ACS[0].R = new GeneralMatrix(tempR, 3, 3);
            ACS[0].T = new GeneralMatrix(dat[3], 1);

            return ACS;
        }

        public static TransformRT[] parseMotionFileToRT(string filename)
        {
            double[][] dat = parseMotionFile(filename);
            TransformRT[] transforms = new TransformRT[15];
            for (int i = 0; i < 15; i++)
            {
                transforms[i] = new TransformRT();
                double[][] tempR = { dat[i * 4], dat[i * 4 + 1], dat[i * 4 + 2] };
                transforms[i].R = new GeneralMatrix(tempR, 3, 3);
                transforms[i].T = new GeneralMatrix(dat[i * 4 + 3], 1);
            }
            return transforms;
        }

        public static TransformMatrix[] parseMotionFileToTransformMatrix(string filename)
        {
            double[][] dat = parseMotionFile(filename);
            TransformMatrix[] transforms = new TransformMatrix[Wrist.NumBones];
            for (int i = 0; i < Wrist.NumBones; i++)
            {
                double[][] tempR = { dat[i * 4], dat[i * 4 + 1], dat[i * 4 + 2] };
                transforms[i] = new TransformMatrix(tempR, dat[i * 4 + 3]);
            }

            return transforms;
        }

        public static double[][] parsePosViewRTFileToDouble(string filename)
        {
            if (!File.Exists(filename))
                throw new ArgumentException("File does not exist. (" + filename + ")");

            int numTransforms;
            double[][] data;
            try
            {
                using (StreamReader r = new StreamReader(filename))
                {
                    numTransforms = Int32.Parse(r.ReadLine().Trim());
                    data = parseDatFile(r);
                }
                //check size of array
                if (data.Length != numTransforms*4)
                    throw new InvalidDataException(String.Format("RT file ({0}) is in an invalid format! Header says {1} transforms, but found {2}!",
                        filename,numTransforms,data.Length/4));

                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i].Length != 3)
                        throw new ArgumentException("Each row of the RT files should have 3 elements. (Line: " + (i+1).ToString() + ")");
                }

                return data;
            }
            catch (Exception ex)
            {
                throw new InvalidDataException(String.Format("RT file ({0}) is in an invalid format!", filename), ex);
            }
        }

        public static TransformRT[] parsePosViewRTFile(string filename)
        {
            double[][] dat = parsePosViewRTFileToDouble(filename);
            int numTransforms = (int)dat.Length/4;
            TransformRT[] transforms = new TransformRT[numTransforms];
            for (int i = 0; i < numTransforms; i++)
            {
                transforms[i] = new TransformRT();
                double[][] tempR = { dat[i * 4], dat[i * 4 + 1], dat[i * 4 + 2] };
                transforms[i].R = new GeneralMatrix(tempR, 3, 3);
                transforms[i].T = new GeneralMatrix(dat[i * 4 + 3], 1);
            }
            return transforms;
        }

        public static Transform[] parsePosViewRTFileToTransforms(string filename)
        {
            TransformRT[] tfm = parsePosViewRTFile(filename);
            Transform[] transforms = new Transform[tfm.Length];
            for (int i = 0; i < tfm.Length; i++)
            {
                transforms[i] = new Transform();
                addRTtoTransform(tfm[i], transforms[i]);
            }
            return transforms;
        }

        public static double[][] parsePosViewHAMFile(string filename)
        {
            double[][] dat = parseDatFile(filename);

            for (int i = 0; i < dat.Length; i++)
            {
                if (dat[i].Length != 8)
                    throw new ArgumentException("Each row of the PosView HAM files should have 8 elements. (Line: " + i.ToString() + ")");
            }
            return dat;
        }

        public static TransformRT[] parseRTFileWithHeaderToRT(string filename)
        {
            double[][] dat = parseRTFileWithHeaderToDouble(filename);
            int numTransforms = (int)dat.Length / 4;
            TransformRT[] transforms = new TransformRT[numTransforms];
            for (int i = 0; i < numTransforms; i++)
            {
                transforms[i] = new TransformRT();
                double[][] tempR = { dat[i * 4], dat[i * 4 + 1], dat[i * 4 + 2] };
                transforms[i].R = new GeneralMatrix(tempR, 3, 3);
                transforms[i].T = new GeneralMatrix(dat[i * 4 + 3], 1);
            }
            return transforms;
        }

        public static double[][] parseRTFileWithHeaderToDouble(string filename)
        {
            if (!File.Exists(filename))
                throw new ArgumentException("File does not exist. (" + filename + ")");

            double[][] dat;
            int positions;

            using (StreamReader r = new StreamReader(filename))
            {
                string header = r.ReadLine();
                positions = Int32.Parse(header);
                dat = parseDatFile(r);
            } 
            //now lets validate the dat size.
            if (dat.Length != positions * 4)
                throw new ArgumentException(String.Format("Invalid RT file with header. Positions defined in header ({0}) does not correlate with lines in file ({1})", positions, dat.Length));
            for (int i = 0; i < dat.Length; i++)
            {
                if (dat[i].Length != 3)
                    throw new ArgumentException("Each row of the RT file w/header should have 3 elements. (Line: " + i.ToString() + ")");
            }
            return dat;
        }

        public static int[][] parseDistvColorFile(string filename, int numPositions, int numVertices)
        {
            if (!File.Exists(filename))
                throw new ArgumentException("File does not exist. (" + filename + ")");
            int[][] dat = new int[numPositions][];

            using (StreamReader sr = new StreamReader(filename))
            {
                BinaryReader r = new BinaryReader(sr.BaseStream);
                for (int i = 0; i < numPositions; i++)
                {
                    dat[i] = new int[numVertices];
                    for (int j=0; j<numVertices; j++)
                        dat[i][j] = (int)r.ReadUInt32();
                }
            }
            return dat;
        }

        public static void addRTtoTransform(TransformRT tfm, Transform transform)
        {
            transform.setTransform(tfm.R.Array[0][0], tfm.R.Array[0][1], tfm.R.Array[0][2],
                tfm.R.Array[1][0], tfm.R.Array[1][1], tfm.R.Array[1][2],
                tfm.R.Array[2][0], tfm.R.Array[2][1], tfm.R.Array[2][2],
                tfm.T.Array[0][0], tfm.T.Array[0][1], tfm.T.Array[0][2]);       
        }
    }
}
