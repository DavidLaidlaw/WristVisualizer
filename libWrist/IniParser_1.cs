using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace libWrist
{
    public class IniParser
    {
        public static Dictionary<string, Dictionary<string, string>> parseINIFilesStrict(string fname)
        {
            if (!File.Exists(fname))
                return null;

            string headingRegex = @"^\[(\S+)\]$";
            string keyRegex = @"(?<key>[\w]+)=(?<value>.*)$";

            Regex heading = new Regex(headingRegex);
            Regex key = new Regex(keyRegex);

            Dictionary<string, string> curentKeys = null;
            Dictionary<string, Dictionary<string, string>> categories = new Dictionary<string, Dictionary<string, string>>();
            try
            {
                using (StreamReader reader = new StreamReader(fname))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (line.IndexOf(';') >= 0) //remove comments from line
                            line = line.Substring(0, line.IndexOf(';') - 1);
                        Match m = heading.Match(line);
                        if (m.Success)  //it was a heading!
                        {
                            curentKeys = new Dictionary<string, string>();
                            categories.Add(m.Groups[1].Value.Trim(), curentKeys);
                            continue;
                        }

                        if (curentKeys ==null) continue; //if we are out of a section, move on

                        //not a header check for a body match
                        m = key.Match(line);
                        if (!m.Success)
                            continue;

                        string k = m.Groups["key"].Value;
                        string v = m.Groups["value"].Value;

                        curentKeys.Add(k, v);
                    }
                    return categories;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidDataException(String.Format("ini file ({0}) is in an invalid format!", fname), ex);
            }
        }

        public static Dictionary<string, Dictionary<string, List<string>>> parseINIFilesWithRepeatKeys(string fname)
        {
            return parseINIFilesWithRepeatKeys(fname, EqualityComparer<string>.Default);
        }

        public static Dictionary<string, Dictionary<string, List<string>>> parseINIFilesWithRepeatKeys(string fname, IEqualityComparer<string> comparer)
        {
            if (!File.Exists(fname))
                return null;

            try
            {
                using (StreamReader reader = new StreamReader(fname))
                {
                    return parseINIFilesWithRepeatKeys(reader, comparer);
                }
            }
            catch (InvalidDataException ex)
            {
                throw new InvalidDataException(String.Format("ini file ({0}) is in an invalid format!", fname), ex.InnerException);
            }
        }

        public static Dictionary<string, Dictionary<string, List<string>>> parseINIFilesWithRepeatKeys(StreamReader reader)
        {
            return parseINIFilesWithRepeatKeys(reader, EqualityComparer<string>.Default);
        }

        public static Dictionary<string, Dictionary<string, List<string>>> parseINIFilesWithRepeatKeys(StreamReader reader, IEqualityComparer<string> comparer)
        {
            string headingRegex = @"^\[(\S+)\]$";
            string keyRegex = @"^[ \t]*(?<key>[ \.\/\\\*\$\w]+)[ \t]*=(?<value>.*)$";

            Regex heading = new Regex(headingRegex);
            Regex key = new Regex(keyRegex);

            Dictionary<string, List<string>> curentKeys = null;
            Dictionary<string, Dictionary<string, List<string>>> categories = new Dictionary<string, Dictionary<string, List<string>>>(comparer);
            try
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line.IndexOf(';') >= 0) //remove comments from line
                        line = line.Substring(0, line.IndexOf(';'));
                    if (line.Trim().Length == 0) continue; //skip blank lines

                    Match m = heading.Match(line);
                    if (m.Success)  //it was a heading!
                    {
                        curentKeys = new Dictionary<string, List<string>>(comparer);
                        categories.Add(m.Groups[1].Value.Trim(), curentKeys);
                        continue;
                    }

                    if (curentKeys == null) continue; //if we are out of a section, move on

                    //not a header check for a body match
                    m = key.Match(line);
                    if (m.Success)
                    {
                        string k = m.Groups["key"].Value.Trim();
                        string v = m.Groups["value"].Value;
                        if (!curentKeys.ContainsKey(k))
                            curentKeys[k] = new List<string>();
                        curentKeys[k].Add(v);
                    }
                    else
                    {
                        //This is outside of the strict INI format definition, but I want it for this setup
                        string k = line.Trim();
                        if (!curentKeys.ContainsKey(k)) //only need to have one entry, ignore repeats
                            curentKeys[k] = null;
                    }
                }
                return categories;
            }
            catch (Exception ex)
            {
                throw new InvalidDataException("ini file is in an invalid format!", ex);
            }
        }

        
    }
}
