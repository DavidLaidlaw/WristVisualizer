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
        private Hashtable _transforms;
        private bool _lastTransformWasOptimizedBoth;
        private string _lastOptimizedBone="";

        private ArrayList _listOfAlignments;
        private ArrayList _listOfTransformHashtables;

        public TransformParser(string filename)
        {
            _listOfAlignments = new ArrayList();
            _listOfTransformHashtables = new ArrayList();

            ParseTransform(filename);
        }

        public Hashtable getFinalTransforms()
        {
            return _transforms;
        }

        public string[] getArrayOfAllignmentSteps()
        {
            return (string[])_listOfAlignments.ToArray(typeof(string));
        }

        public Hashtable[] getArrayOfTransformHashtables()
        {
            return (Hashtable[])_listOfTransformHashtables.ToArray(typeof(Hashtable));
        }

        public static Hashtable ParseAllTransforms(string filename)
        {
            TransformParser tp = new TransformParser(filename);
            return tp.getFinalTransforms();
        }


        private void ParseTransform(string filename)
        {
            if (!File.Exists(filename))
                throw new ArgumentException("File does not exist. (" + filename + ")");

            using (StreamReader r = new StreamReader(filename))
            {
                ParseTransform(r);
            }  
        }

        private void ParseTransform(StreamReader filestream)
        {
            const string boneLineRegex = @"^Bone\ name\:\ (\w+)\s*$";
            _transforms = new Hashtable(15);
            saveCurrentState("Neutral Wrist");
            while (!filestream.EndOfStream)
            {
                string line = filestream.ReadLine();

                //check if it is a line for a bone
                Match m = Regex.Match(line, boneLineRegex);
                if (m.Success)
                {
                    string boneName = m.Groups[1].Value.Trim();
                    //make sure that there is an identity transform if this is the first encounter
                    if (!_transforms.ContainsKey(boneName))
                        _transforms[boneName] = new TransformMatrix();

                    TransformMatrix next_tm = readSingleTransform(filestream);
                    if (_lastTransformWasOptimizedBoth)
                    {
                        //at this point, we are going to save the previous optimization
                        string label;
                        if (_lastOptimizedBone.Length > 0)
                            label = "Align " + _lastOptimizedBone;
                        else
                            label = "DRUJ Alignment";
                        saveCurrentState(label);
                        _lastOptimizedBone = boneName;
                    }
                    _transforms[boneName] = next_tm * (TransformMatrix)_transforms[boneName]; //add to list
                }
            }
            //at the end, see if we need to save again, for this state
            if (_lastTransformWasOptimizedBoth)
            {
                saveCurrentState("Align " + _lastOptimizedBone);
            }
        }

        private void saveCurrentState(string label)
        {
            _listOfAlignments.Add(label);
            Hashtable htCopy = (Hashtable)_transforms.Clone();
            _listOfTransformHashtables.Add(htCopy);
        }

        private TransformMatrix readSingleTransform(StreamReader filestream)
        {
            const string centerRotationRegex = @"^\s*center\ of\ rotation\:\s+([-\d\.e+]+)\s+([-\d\.e+]+)\s+([-\d\.e+]+)\s*$";
            const string rotationAnglesRegex = @"^\s*rotation\ angles\:\s+([-\d\.e+]+)\s+([-\d\.e+]+)\s+([-\d\.e+]+)\s*$";
            const string translationRegex = @"^\s*translation\ value\:\s+([-\d\.e+]+)\s+([-\d\.e+]+)\s+([-\d\.e+]+)\s*$";
            const string translationBothRegex = @"^\s*translation\:\s+([-\d\.e+]+)\s+([-\d\.e+]+)\s+([-\d\.e+]+)\s*$";

            //lets figure out the transform type
            TransformMatrix rt = new TransformMatrix();
            string transformLine = filestream.ReadLine();
            string transformType = Regex.Match(transformLine, @"^Transform\ type\:\ (.*)$").Groups[1].Value.Trim();
            switch (transformType)
            {
                case "default rotation":
                    _lastTransformWasOptimizedBoth = false;
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
                    _lastTransformWasOptimizedBoth = false;
                    double[] translation = new double[3];

                    string translationLine = filestream.ReadLine();
                    m = Regex.Match(translationLine, translationRegex);
                    translation[0] = Double.Parse(m.Groups[1].Value);
                    translation[1] = Double.Parse(m.Groups[2].Value);
                    translation[2] = Double.Parse(m.Groups[3].Value);
                    rt.setTranslation(translation);
                    break;
                case "optimized both":
                    _lastTransformWasOptimizedBoth = true;
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
                    TransformMatrix rot = new TransformMatrix();
                    TransformMatrix t = new TransformMatrix();
                    rot.rotateAboutCenter(rotationAngles, centerRotation);
                    t.setTranslation(translation);
                    rt = t * rot;
                    break;
                default:
                    //error, there should be no other type
                    throw new FormatException("Invalid format for transform file. Unknown transform type: " + transformType);
            }
            return rt;
        }

        public static void addTfmMatrixtoTransform(TransformMatrix tfm, libCoin3D.Transform transform)
        {
            transform.setTransform(tfm.Array[0][0], tfm.Array[0][1], tfm.Array[0][2],
                tfm.Array[1][0], tfm.Array[1][1], tfm.Array[1][2],
                tfm.Array[2][0], tfm.Array[2][1], tfm.Array[2][2],
                tfm.Array[0][3], tfm.Array[1][3], tfm.Array[2][3]);
        }
    }
}
