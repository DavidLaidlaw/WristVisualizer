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

        public static void ConvertVRMLToIV(string vrmlFilename)
        {
            string ivFilename = Path.Combine(Path.GetDirectoryName(vrmlFilename),
                Path.GetFileNameWithoutExtension(vrmlFilename) + ".iv");
            ConvertVRMLToIV(vrmlFilename, ivFilename);
        }

        public static void ConvertVRMLToIV(string vrmlFilename, string ivFilename)
        {
            TesselatedObject data = parseVRMLFile(vrmlFilename);
            WriteTesselatedObjectToIVFile(data, ivFilename);
        }

        private const string IV_FILE_HEADER = @"#VRML V1.0 ascii
#
Separator {
    Coordinate3 {
        point [";

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


        public static TesselatedObject parseVRMLFile(string filename)
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

            int s = pointRegex.Match(full).Index;
            int e = closingRegex.Match(full, s + 5).Index;
            string ptsSection = full.Substring(s + 7, e - s - 7);
            
            s = coordIndexRegex.Match(full).Index;
            e = closingRegex.Match(full, s + 12).Index;
            string connSection = full.Substring(s + 12, e - s - 12);

            double[] scale = { 1.0, 1.0, 1.0 };
            Match scaleMatch = Regex.Match(full, @"scale ([\d\.]+) ([\d\.]+) ([\d\.]+)");
            if (scaleMatch.Success)
            {
                scale[0] = Double.Parse(scaleMatch.Groups[1].Value);
                scale[1] = Double.Parse(scaleMatch.Groups[1].Value);
                scale[2] = Double.Parse(scaleMatch.Groups[1].Value);
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
