using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace libWrist
{
    public class VRMLParser
    {
        public struct TesselatedObject
        {
            public double[,] Points;
            public int[,] Connections;
        }

        private TesselatedObject _localObject;

        public VRMLParser(string fname)
        {
            _localObject = parseVRMLFile(fname);
        }

        public double[,] Points
        {
            get { return _localObject.Points; }
        }
        public int[,] Connections
        {
            get { return _localObject.Connections; }
        }

        public static void ConvertVRMLToIV(string vrmlFilename, bool autoFixMimics10Scale)
        {
            string ivFilename = Path.Combine(Path.GetDirectoryName(vrmlFilename),
                Path.GetFileNameWithoutExtension(vrmlFilename) + ".iv");
            ConvertVRMLToIV(vrmlFilename, ivFilename, autoFixMimics10Scale);
        }

        public static void ConvertVRMLToIV(string vrmlFilename, string ivFilename, bool autoFixMimics10Scale)
        {
            TesselatedObject data = parseVRMLFile(vrmlFilename, autoFixMimics10Scale);
            WriteTesselatedObjectToIVFile(data, ivFilename);
        }

        private const string IV_FILE_HEADER = @"#VRML V1.0 ascii
#
Separator {
    Coordinate3 {
        point [";

        private const string MIMICS_10_SCALE_KEY = @"#coordinates written in 1mm / 10000";
        private const string MIMICS_13_SCALE_KEY = @"#coordinates written in 1mm / 0";

        private static void WriteTesselatedObjectToIVFile(TesselatedObject vrmlData, string ivFilename)
        {
            using (StreamWriter writer = new StreamWriter(ivFilename))
            {
                writer.WriteLine(IV_FILE_HEADER);
                for (int i = 0; i < vrmlData.Points.Length/3; i++)
                    writer.WriteLine("\t\t\t{0} {1} {2},", vrmlData.Points[i, 0], vrmlData.Points[i, 1], vrmlData.Points[i, 2]);

                writer.WriteLine("\t\t]");
                writer.WriteLine("\t}");
                writer.WriteLine("\tIndexedFaceSet {");
                writer.WriteLine("\t\tcoordIndex [");

                for (int i = 0; i < vrmlData.Connections.Length / 3; i++)
                    writer.WriteLine("\t\t\t{0}, {1}, {2}, -1,", vrmlData.Connections[i, 0], vrmlData.Connections[i, 1], vrmlData.Connections[i, 2]);

                writer.WriteLine("\t\t]");
                writer.WriteLine("\t}");
                writer.WriteLine("}");
            }
        }


        public static TesselatedObject parseVRMLFile(string filename) { return parseVRMLFile(filename, true); }
        public static TesselatedObject parseVRMLFile(string filename, bool autoFixMimics10Scale)
        {
            TesselatedObject vrmlData = new TesselatedObject();
            string full;
            using (StreamReader r = new StreamReader(filename))
            {
                full = r.ReadToEnd();
            }

            Regex pointRegex = new Regex(@"point\s+\[");
            Regex closingRegex = new Regex("]");
            Regex coordIndexRegex = new Regex(@"coordIndex\s+\[");

            /* if we are supposed to search for fixing Mimics Scale, 
             * then we need to look before we strip the comments. The key
             * is a comment :)
             */
            bool applyMimics10Scale = false;
            if (autoFixMimics10Scale)
            {
                //check for it, we will re-use this bool value as an indicator of if the scale needs to be changed
                if (full.Contains(MIMICS_10_SCALE_KEY) ||
                    full.Contains(MIMICS_13_SCALE_KEY))
                    applyMimics10Scale = true;
            }

            //Remove all comments from the file
            full = Regex.Replace(full, @"#.*$", "", RegexOptions.Multiline);

            Match pointMatch = pointRegex.Match(full);
            int s = pointMatch.Index;
            int e = closingRegex.Match(full, s + pointMatch.Length).Index;
            string ptsSection = full.Substring(s + pointMatch.Length, e - s - pointMatch.Length);

            Match coordMatch = coordIndexRegex.Match(full);
            s = coordMatch.Index;
            e = closingRegex.Match(full, s + coordMatch.Length).Index;
            string connSection = full.Substring(s + coordMatch.Length, e - s - coordMatch.Length);

            double[] scale = { 1.0, 1.0, 1.0 };
            Match scaleMatch = Regex.Match(full, @"scale\s+([\d\.]+)\s+([\d\.]+)\s+([\d\.]+)");
            if (scaleMatch.Success)
            {
                scale[0] = Double.Parse(scaleMatch.Groups[1].Value);
                scale[1] = Double.Parse(scaleMatch.Groups[1].Value);
                scale[2] = Double.Parse(scaleMatch.Groups[1].Value);
            }

            if (applyMimics10Scale)
            {
                scale[0] *= 1000;
                scale[1] *= 1000;
                scale[2] *= 1000;
            }

            vrmlData.Points = parsePts(ptsSection, scale);
            vrmlData.Connections = parseConn(connSection);
            return vrmlData;
        }

        private static double[,] parsePts(string section)
        {
            return parsePts(section, new double[]{1.0, 1.0, 1.0});
        }

        private static double[,] parsePts(string section, double[] scale)
        {
            string[] parts = section.Split(new char[] { '\t', ' ', ',', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int numPts = parts.Length / 3;
            double[,] pts = new double[numPts,3];
            if (scale[0] == 1 && scale[1] == 1 && scale[2] == 1)
            {
                for (int i = 0; i < numPts; i++)
                {
                    pts[i, 0] = Double.Parse(parts[i * 3]);
                    pts[i, 1] = Double.Parse(parts[i * 3 + 1]);
                    pts[i, 2] = Double.Parse(parts[i * 3 + 2]);
                }
            }
            else
            {
                for (int i = 0; i < numPts; i++)
                {
                    pts[i, 0] = Double.Parse(parts[i * 3]) * scale[0];
                    pts[i, 1] = Double.Parse(parts[i * 3 + 1]) * scale[1];
                    pts[i, 2] = Double.Parse(parts[i * 3 + 2]) * scale[2];
                }
            }
            return pts;
        }

        private static int[,] parseConn(string section)
        {
            string[] parts = section.Split(new char[] { '\t', ' ', ',', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int numConn = parts.Length / 4;
            int[,] conn = new int[numConn, 3];
            for (int i = 0; i < numConn; i++)
            {
                conn[i, 0] = Int32.Parse(parts[i * 4]);
                conn[i, 1] = Int32.Parse(parts[i * 4 + 1]);
                conn[i, 2] = Int32.Parse(parts[i * 4 + 2]);
            }
            return conn;
        }

    }
}
