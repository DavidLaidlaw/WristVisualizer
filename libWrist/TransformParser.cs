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
                parseDatFile(r);
            }  
        }

        public static void parseDatFile(StreamReader filestream)
        {
            const string boneLineRegex = @"^Bone\ name\:\ (\w+)\s*$";
            Hashtable transforms = new Hashtable(15);
            TM final = new TM();
            while (!filestream.EndOfStream)
            {
                string line = filestream.ReadLine();

                //check if it is a line for a bone
                Match m = Regex.Match(line, boneLineRegex);
                if (m.Success)
                {
                    string boneName = m.Groups[1].Value.Trim();
                    //make sure that there is an identity transform if this is the first encounter
                    if (!transforms.ContainsKey(boneName))
                        transforms[boneName] = new TM();

                    TM next_tm = dispatch(filestream);
                    transforms[boneName] = next_tm * (TM)transforms[boneName]; //add to list
                }
            }
            Console.WriteLine("final result");
            ((TM)transforms["ulna"]).printToConsole();
        }

        public static TM dispatch(StreamReader filestream)
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
                    rt.rotateAboutCenter(rotationAngles, centerRotation);
                    break;
                case "default translation":
                    double[] translation = new double[3];

                    string translationLine = filestream.ReadLine();
                    m = Regex.Match(translationLine, translationRegex);
                    translation[0] = Double.Parse(m.Groups[1].Value);
                    translation[1] = Double.Parse(m.Groups[2].Value);
                    translation[2] = Double.Parse(m.Groups[3].Value);
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
                    break;
                default:
                    //error, there should be no other type
                    throw new FormatException("Invalid format for transform file. Unknown transform type: " + transformType);
            }
            Console.WriteLine("Bone_trsf_step");
            rt.printToConsole();
            return rt;
        }
    }
}
