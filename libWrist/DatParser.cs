using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using DotNetMatrix;


namespace libWrist
{
    public struct Transform 
    {
        public GeneralMatrix R;
        public GeneralMatrix T;
    }

    public class DatParser
    {
        public DatParser()
        {
 
            
        }

        public static double[][] parseDatFile(string filename)
        {
            StreamReader r = new StreamReader(filename);
            char[] div = { ' ', '\t', ',' };
            System.Collections.ArrayList dat = new System.Collections.ArrayList(100);
            while (!r.EndOfStream)
            {
                //Console.WriteLine("new Line");
                string line = r.ReadLine().Trim();
                if (line.Length == 0) continue;

                string[] parts = line.Split(div);
                double[] values = new double[parts.Length];
                for (int i=0; i<parts.Length; i++)
                {
                    values[i] = Double.Parse(parts[i]);
                    //Console.WriteLine("Found: " + values[i].ToString());
                }
                dat.Add(values);
            }
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

        public static Transform[] parseMotionFile2(string filename)
        {
            double[][] dat = parseMotionFile(filename);
            Transform[] transforms = new Transform[15];
            for (int i = 0; i < 15; i++)
            {
                transforms[i] = new Transform();
                double[][] tempR = { dat[i * 4], dat[i * 4 + 1], dat[i * 4 + 2] };
                transforms[i].R = new GeneralMatrix(tempR, 3, 3);
                transforms[i].T = new GeneralMatrix(dat[i * 4 + 3], 1);
            }
            return transforms;
        }

    }

}
