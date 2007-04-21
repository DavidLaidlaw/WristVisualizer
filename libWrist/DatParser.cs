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
                    /*
                    t.setRotation(tfm[i].R.Array[0][0], tfm[i].R.Array[0][1], tfm[i].R.Array[0][2],
                        tfm[i].R.Array[1][0], tfm[i].R.Array[1][1], tfm[i].R.Array[1][2],
                        tfm[i].R.Array[2][0], tfm[i].R.Array[2][1], tfm[i].R.Array[2][2]);
                     
                    t.setTranslation(tfm[i].T.Array[0][0], tfm[i].T.Array[0][1], tfm[i].T.Array[0][2]);
                    _bones[i].addTransform(t);
                     */
                    transforms[i][j].setTransform(tfm[j].R.Array[0][0], tfm[j].R.Array[0][1], tfm[j].R.Array[0][2],
                        tfm[j].R.Array[1][0], tfm[j].R.Array[1][1], tfm[j].R.Array[1][2],
                        tfm[j].R.Array[2][0], tfm[j].R.Array[2][1], tfm[j].R.Array[2][2],
                        tfm[j].T.Array[0][0], tfm[j].T.Array[0][1], tfm[j].T.Array[0][2]);
                }
            }
            return transforms;
        }

        public static double[][] parseDatFile(string filename)
        {
            if (!File.Exists(filename))
                throw new ArgumentException("File does not exist. (" + filename + ")");

            StreamReader r = new StreamReader(filename);
            char[] div = { ' ', '\t', ',' };
            System.Collections.ArrayList dat = new System.Collections.ArrayList(100);

            Regex reg = new Regex(@"([-\d\.e+]+)[ \t,]+([-\d\.e+]+)[ \t,]+([-\d\.e+]+)");
            while (!r.EndOfStream)
            {
                //Console.WriteLine("new Line");
                string line = r.ReadLine().Trim();
                if (line.Length == 0) continue;

                Match m = reg.Match(line);
                //string[] parts = line.Split(div);
                double[] values = new double[m.Groups.Count-1];
                for (int i = 0; i < m.Groups.Count-1; i++)
                {
                    //string junk = m.Groups[i+1].Value;
                    values[i] = Double.Parse(m.Groups[i+1].Value);
                    //Console.WriteLine("Found: " + values[i].ToString());
                }
                dat.Add(values);
            }
            r.Close();
            r.Dispose();
            return (double[][])dat.ToArray(typeof(double[]));
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
