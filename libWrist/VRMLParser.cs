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
            double[,] Points;
            int[,] Connections;
        }

        public VRMLParser(string fname)
        {
            fname = @"C:\Functional\E02751\S15R\WRL.files\E02751_15R_rad 1_001.wrl";
            DateTime start = DateTime.Now;
            start = DateTime.Now;
            parseVRMLFile(fname);
            Console.WriteLine("Parse4: {0}", DateTime.Now - start);

        }

        private static TesselatedObject parseVRMLFile(string filename)
        {
            TesselatedObject vrmlData = new TesselatedObject();
            string full;
            using (StreamReader r = new StreamReader(filename))
            {
                full = r.ReadToEnd();
            }
            DateTime start = DateTime.Now;
            Regex reg = new Regex(@"point\s+\[");
            Match m = reg.Match(full);

            int s = m.Index;
            Regex reg2 = new Regex("]");
            int e = reg2.Match(full, s + 5).Index;
            string ptsSection = full.Substring(s + 7, e - s - 7);

            Regex reg3 = new Regex(@"coordIndex\s+\[");
            s = reg3.Match(full).Index;
            e = reg2.Match(full, s + 12).Index;
            string connSection = full.Substring(s + 12, e - s - 12);
            Console.WriteLine("\tReg Substrings: {0}", DateTime.Now - start);

            start = DateTime.Now;
            double[] scale = { 1.0, 1.0, 1.0 };
            Match m2 = Regex.Match(full, @"scale ([\d\.]+) ([\d\.]+) ([\d\.]+)");
            if (m2.Success)
            {
                scale[0] = Double.Parse(m2.Groups[1].Value);
                scale[1] = Double.Parse(m2.Groups[1].Value);
                scale[2] = Double.Parse(m2.Groups[1].Value);
            }
            Console.WriteLine("\tScale: {0}", DateTime.Now - start);

            start = DateTime.Now;
            parsePts(ptsSection, scale);
            Console.WriteLine("\tPts: {0}", DateTime.Now - start);
            start = DateTime.Now;
            parseConn(connSection);
            Console.WriteLine("\tConn: {0}", DateTime.Now - start);
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
