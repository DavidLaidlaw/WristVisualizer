using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace libWrist
{
    public class VRMLParser
    {
        List<double[]> _pts;
        List<int[]> _conn;
        public VRMLParser(string fname)
        {
            DateTime start = DateTime.Now;
            parse1();
            Console.WriteLine("Parse1: {0}", DateTime.Now - start);
            start = DateTime.Now;
            parse2();
            Console.WriteLine("Parse2: {0}", DateTime.Now - start);
            start = DateTime.Now;
            parse3();
            Console.WriteLine("Parse3: {0}", DateTime.Now - start);
            start = DateTime.Now;
            parse4();
            Console.WriteLine("Parse4: {0}", DateTime.Now - start);
            parse5();
        }

        private void parse1()
        {
            _pts = new List<double[]>();
            _conn = new List<int[]>();
            string fname = @"C:\Functional\E02751\S15R\WRL.files\E02751_15R_rad 1_001.wrl";
            using (StreamReader r = new StreamReader(fname))
            {
                parse(r);
            }
        }

        private void parse(StreamReader reader)
        {
            bool next = false;
            string line = reader.ReadLine();
            while (line.IndexOf("point [") < 0)
                line = reader.ReadLine();

            while (!next)
            {
                line = reader.ReadLine();
                if (line.IndexOf(']') >= 0)
                {
                    next = true;
                    continue;
                }
                else
                {
                    double x, y, z;
                    string[] parts = line.Split(new char[] { '\t', ' ', ',' }, 3);
                    string[] parts_full = line.Split(new char[] { '\t', ' ', ',' });
                    x = Double.Parse(parts[0]);
                    y = Double.Parse(parts[1]);
                    z = Double.Parse(parts[2]);
                    _pts.Add(new double[] { x, y, z });
                }
            }

            while (line.IndexOf("coordIndex [") < 0)
                line = reader.ReadLine();
            
            next = false;
            while (!next)
            {
                line = reader.ReadLine();
                if (line.IndexOf(']') >= 0)
                {
                    return; //we are done
                }
                else
                {
                    int c1, c2, c3;
                    string[] parts = line.Split(new char[] { '\t', ' ', ',' }, 4);
                    string[] parts_full = line.Split(new char[] { '\t', ' ', ',' });
                    c1 = Int32.Parse(parts[0]);
                    c2 = Int32.Parse(parts[1]);
                    c3 = Int32.Parse(parts[2]);
                    _conn.Add(new int[] { c1, c2, c3 });
                }
            }
        }

        private void parse2()
        {
            _pts = new List<double[]>();
            _conn = new List<int[]>();
            string fname = @"C:\Functional\E02751\S15R\WRL.files\E02751_15R_rad 1_001.wrl";
            string full;
            using (StreamReader r = new StreamReader(fname))
            {
                full = r.ReadToEnd();
            }
            //Regex reg = new Regex(@"(.*)point\s+\[([^\]]*)\](.*)coordIndex\s+\[([\d\s\,\n\r\.-]*)\](.*)");
            Regex reg = new Regex(@"(.*)point \[([^\]]*)\](.*)coordIndex \[([\d\s\,\n\r\.-]*)\](.*)", RegexOptions.Singleline | RegexOptions.Compiled);
            DateTime start = DateTime.Now;
            Match m = reg.Match(full);
            Console.WriteLine("\tRegex: {0}", DateTime.Now - start);
            start = DateTime.Now;
            parsePts(m.Groups[2].Value);
            Console.WriteLine("\tPts: {0}", DateTime.Now - start);
            start = DateTime.Now;
            parseConn(m.Groups[4].Value);
            Console.WriteLine("\tConn: {0}", DateTime.Now - start);
        }

        private void parse3()
        {
            _pts = new List<double[]>();
            _conn = new List<int[]>();
            string fname = @"C:\Functional\E02751\S15R\WRL.files\E02751_15R_rad 1_001.wrl";
            string full;
            using (StreamReader r = new StreamReader(fname))
            {
                full = r.ReadToEnd();
            }
            DateTime start = DateTime.Now;
            int s = full.IndexOf("point [");
            int e = full.IndexOf("]", s + 7);
            string ptsSection = full.Substring(s + 7, e - s - 7);

            s = full.IndexOf("coordIndex [");
            e = full.IndexOf("]", s + 12);
            string connSection = full.Substring(s + 12, e - s - 12);
            Console.WriteLine("\tSubstrings: {0}", DateTime.Now - start);
            start = DateTime.Now;
            parsePts(ptsSection);
            Console.WriteLine("\tPts: {0}", DateTime.Now - start);
            start = DateTime.Now;
            parseConn(connSection);
            Console.WriteLine("\tConn: {0}", DateTime.Now - start);
        }

        private void parse4()
        {
            _pts = new List<double[]>();
            _conn = new List<int[]>();
            string fname = @"C:\Functional\E02751\S15R\WRL.files\E02751_15R_rad 1_001.wrl";
            string full;
            using (StreamReader r = new StreamReader(fname))
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


        }

        private void parse5()
        {
            libCoin3D.Coin3DBase.Init();
            libCoin3D.Separator s = new libCoin3D.Separator();
            string fname = @"C:\Functional\E02751\S15R\WRL.files\E02751_15R_rad 1_001.wrl";
            DateTime start = DateTime.Now;
            //libCoin3D.ColoredBone b = new libCoin3D.ColoredBone(fname);
            Console.WriteLine("Parse5 Coin: {0}", DateTime.Now - start);
        }

        private void parsePts(string section)
        {
            parsePts(section, new double[]{1.0, 1.0, 1.0});
        }

        private void parsePts(string section, double[] scale)
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
        }

        private void parseConn(string section)
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
        }

    }
}
