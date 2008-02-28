using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using DotNetMatrix;
using libCoin3D;


namespace libWrist
{
    public struct TransformMatrix
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
                TransformMatrix[] tfm = parseMotionFile2(motionFiles[i]);
                for (int j = 0; j < numBones; j++)
                {
                    transforms[i][j] = new Transform();
                    addRTtoTransform(tfm[j], transforms[i][j]);
                    /*
                    transforms[i][j].setTransform(tfm[j].R.Array[0][0], tfm[j].R.Array[0][1], tfm[j].R.Array[0][2],
                        tfm[j].R.Array[1][0], tfm[j].R.Array[1][1], tfm[j].R.Array[1][2],
                        tfm[j].R.Array[2][0], tfm[j].R.Array[2][1], tfm[j].R.Array[2][2],
                        tfm[j].T.Array[0][0], tfm[j].T.Array[0][1], tfm[j].T.Array[0][2]); */
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

        public static TransformMatrix[] parseInertiaFile2(string filename)
        {
            double[][] dat = parseInertiaFile(filename);
            TransformMatrix[] inertias = new TransformMatrix[15];
            for (int i = 0; i < 15; i++)
            {
                inertias[i] = new TransformMatrix();
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

        public static TransformMatrix[] parseACSFile2(string filename)
        {
            double[][] dat = parseACSFile(filename);
            TransformMatrix[] ACS = new TransformMatrix[1];

            ACS[0] = new TransformMatrix();
            double[][] tempR = { dat[0], dat[1], dat[2] };
            ACS[0].R = new GeneralMatrix(tempR, 3, 3);
            ACS[0].T = new GeneralMatrix(dat[3], 1);

            return ACS;
        }

        public static TransformMatrix[] parseMotionFile2(string filename)
        {
            double[][] dat = parseMotionFile(filename);
            TransformMatrix[] transforms = new TransformMatrix[15];
            for (int i = 0; i < 15; i++)
            {
                transforms[i] = new TransformMatrix();
                double[][] tempR = { dat[i * 4], dat[i * 4 + 1], dat[i * 4 + 2] };
                transforms[i].R = new GeneralMatrix(tempR, 3, 3);
                transforms[i].T = new GeneralMatrix(dat[i * 4 + 3], 1);
            }
            return transforms;
        }

        public static void addRTtoTransform(TransformMatrix tfm, Transform transform)
        {
            transform.setTransform(tfm.R.Array[0][0], tfm.R.Array[0][1], tfm.R.Array[0][2],
                tfm.R.Array[1][0], tfm.R.Array[1][1], tfm.R.Array[1][2],
                tfm.R.Array[2][0], tfm.R.Array[2][1], tfm.R.Array[2][2],
                tfm.T.Array[0][0], tfm.T.Array[0][1], tfm.T.Array[0][2]);       
        }
    }
}
