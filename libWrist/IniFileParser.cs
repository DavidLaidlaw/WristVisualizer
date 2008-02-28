using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace libWrist
{
    /// <summary>
    /// Parse settings from ini-like files
    /// </summary>
    public class IniFileParser
    {
        static private Regex _iniKeyValuePatternRegex;
        private string _iniFileName;

        static IniFileParser()
        {
            _iniKeyValuePatternRegex = new Regex(
                @"((\s)*(?<Key>([^\=^\s^\n]+))[\s^\n]*
                # key part (surrounding whitespace stripped)
                \=
                (\s)*(?<Value>([^\n^\s]+(\n){0,1})))
                # value part (surrounding whitespace stripped)
                ",
                RegexOptions.IgnorePatternWhitespace |
                RegexOptions.Compiled |
                RegexOptions.CultureInvariant);
        }
        

        public IniFileParser(string iniFileName)
        {
            _iniFileName = iniFileName;
        }

        public string ParseFileReadValue(string key)
        {
            using (StreamReader reader =
                  new StreamReader(_iniFileName))
            {
                do
                {
                    string line = reader.ReadLine();
                    Match match =
                        _iniKeyValuePatternRegex.Match(line);
                    if (match.Success)
                    {
                        string currentKey =
                                match.Groups["Key"].Value as string;
                        if (currentKey != null &&
                       currentKey.Trim().CompareTo(key) == 0)
                        {
                            string value =
                              match.Groups["Value"].Value as string;
                            return value;
                        }
                    }

                }
                while (reader.Peek() != -1);
            }
            return null;
        }

        public string IniFileName
        {
            get { return _iniFileName; }
        }
    }
}
