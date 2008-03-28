using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using DotNetMatrix;

namespace libWrist
{
    public class TransformParser
    {

        public static void TestParse(string filename)
        {
            if (!File.Exists(filename))
                throw new ArgumentException("File does not exist. (" + filename + ")");

            using (StreamReader r = new StreamReader(filename))
            {
                parseDatFile(r,"ulna");
            }  
        }




        public static void parseDatFile(StreamReader filestream, string boneName)
        {
            TM final = new TM();
            while (!filestream.EndOfStream)
            {
                string line = filestream.ReadLine();
                //check if it matches the boneName
                if (line.Contains(boneName) && line.StartsWith("Bone name: "))
                {
                    TM next_tm = dispatch(filestream, boneName);
                    final = next_tm * final;
                    //TODO: add tm to front of list of transforms....

                }
            }
            Console.WriteLine("final result");
            final.printToConsole();
        }

        public static TM dispatch(StreamReader filestream, string boneName)
        {
            const string centerRotationRegex = @"^\s*center\ of\ rotation\:\s+([-\d\.e+]+)\s+([-\d\.e+]+)\s+([-\d\.e+]+)\s*$";
            const string rotationAnglesRegex = @"^\s*rotation\ angles\:\s+([-\d\.e+]+)\s+([-\d\.e+]+)\s+([-\d\.e+]+)\s*$";
            const string translationRegex = @"^\s*translation\ value\:\s+([-\d\.e+]+)\s+([-\d\.e+]+)\s+([-\d\.e+]+)\s*$";
            const string translationBothRegex = @"^\s*translation\:\s+([-\d\.e+]+)\s+([-\d\.e+]+)\s+([-\d\.e+]+)\s*$";

            //lets figure out the transform type
            TM rt = new TM();
            string transformLine = filestream.ReadLine();
            string transformType = Regex.Match(transformLine, @"^Transform\ type\:\ (.*)$").Groups[1].Value.Trim();
            switch (transformType)
            {
                case "default rotation":
                    double[] centerRotation = new double[3];
                    double[] rotationAngles = new double[3];

                    string centRotationLine = filestream.ReadLine();
                    Match m = Regex.Match(centRotationLine, centerRotationRegex);
                    centerRotation[0] = Double.Parse(m.Groups[1].Value);
                    centerRotation[1] = Double.Parse(m.Groups[2].Value);
                    centerRotation[2] = Double.Parse(m.Groups[3].Value);

                    string rotationAnglesLines = filestream.ReadLine();
                    m = Regex.Match(rotationAnglesLines, rotationAnglesRegex);
                    rotationAngles[0] = Double.Parse(m.Groups[1].Value);
                    rotationAngles[1] = Double.Parse(m.Groups[2].Value);
                    rotationAngles[2] = Double.Parse(m.Groups[3].Value);
                    //TODO: something with these values
                    rt.rotateAboutCenter(rotationAngles, centerRotation);
                    break;
                case "default translation":
                    double[] translation = new double[3];

                    string translationLine = filestream.ReadLine();
                    m = Regex.Match(translationLine, translationRegex);
                    translation[0] = Double.Parse(m.Groups[1].Value);
                    translation[1] = Double.Parse(m.Groups[2].Value);
                    translation[2] = Double.Parse(m.Groups[3].Value);
                    //TODO: Something with these values
                    rt.setTranslation(translation);
                    break;
                case "optimized both":
                    centerRotation = new double[3];
                    rotationAngles = new double[3];
                    translation = new double[3];

                    centRotationLine = filestream.ReadLine();
                    m = Regex.Match(centRotationLine, centerRotationRegex);
                    centerRotation[0] = Double.Parse(m.Groups[1].Value);
                    centerRotation[1] = Double.Parse(m.Groups[2].Value);
                    centerRotation[2] = Double.Parse(m.Groups[3].Value);

                    rotationAnglesLines = filestream.ReadLine();
                    m = Regex.Match(rotationAnglesLines, rotationAnglesRegex);
                    rotationAngles[0] = Double.Parse(m.Groups[1].Value);
                    rotationAngles[1] = Double.Parse(m.Groups[2].Value);
                    rotationAngles[2] = Double.Parse(m.Groups[3].Value);

                    translationLine = filestream.ReadLine();
                    m = Regex.Match(translationLine, translationBothRegex);
                    translation[0] = Double.Parse(m.Groups[1].Value);
                    translation[1] = Double.Parse(m.Groups[2].Value);
                    translation[2] = Double.Parse(m.Groups[3].Value);
                    TM rot = new TM();
                    TM t = new TM();
                    rot.rotateAboutCenter(rotationAngles, centerRotation);
                    t.setTranslation(translation);
                    rt = t * rot;
                    //TODO: Something with these values
                    break;
                default:
                    //error, there should be no other type
                    throw new FormatException("Invalid format for transform file. Unknown transform type: " + transformType);
            }
            Console.WriteLine("Bone_trsf_step");
            rt.printToConsole();
            return rt;
        }

        //public static double[][] parseDatFile(StreamReader filestream)
        //{
        //    //char[] div = { ' ', '\t', ',' };
        //    System.Collections.ArrayList dat = new System.Collections.ArrayList(100);

        //    Regex reg = new Regex(@"([-\d\.e+]+)");
        //    while (!filestream.EndOfStream)
        //    {
        //        string line = filestream.ReadLine().Trim();
        //        if (line.Length == 0) continue;

        //        MatchCollection m = reg.Matches(line);
        //        double[] values = new double[m.Count];
        //        for (int i = 0; i < m.Count; i++)
        //        {
        //            values[i] = Double.Parse(m[i].Value);
        //        }
        //        dat.Add(values);
        //    }
        //    return (double[][])dat.ToArray(typeof(double[]));
        //}
    }
}
