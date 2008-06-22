using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Mono.GetOptions;

namespace WristVizualizer
{
    class CommandLineOptions : Options
    {
        public enum BatchModes
        {
            Normal,
            ContourData
        }

        public string error = "";

        [Option("Path to the subject directory to use")]
        public string subject = null;

        [Option("Manually specify what kind of manual processing to use. Current choices are: ('contour')")]
        public string mode = null;

        [Option("Name of output file", 'o')]
        public string output = null;

        [Option("Position to use for testing given condition.", 'p')]
        public string test_position = null;

        public new void ProcessArgs(string[] args)
        {
            //save existing
            TextWriter stdout = Console.Out;
            string msg;

            //create new
            using (MemoryStream s = new MemoryStream())
            {
                StreamWriter writer = new StreamWriter(s);
                Console.SetOut(writer);

                try
                {
                    base.ProcessArgs(args);
                }
                catch
                {
                    writer.Flush(); //otherwise we don't get our output...
                    s.Seek(0, SeekOrigin.Begin);
                    StreamReader r = new StreamReader(s);
                    msg = r.ReadToEnd();
                    MessageBox.Show(msg, Application.ProductName);
                }                
            }
            Console.SetOut(stdout);
        
        }

        public void showHelp()
        {
            string msg = getHelpMessageAsString();
            MessageBox.Show(msg, Application.ProductName);
        }

        private string getHelpMessageAsString()
        {
            //save existing
            TextWriter stdout = Console.Out;
            string msg;

            //create new
            using (MemoryStream s = new MemoryStream())
            {
                StreamWriter writer = new StreamWriter(s);
                Console.SetOut(writer);

                DoHelp();
                writer.Flush(); //otherwise we don't get our output...
                s.Seek(0, SeekOrigin.Begin);
                StreamReader r = new StreamReader(s);
                msg = r.ReadToEnd();
            }
            Console.SetOut(stdout);
            return msg;
        }

        public bool isBatchMode()
        {
            return (getMode() == BatchModes.ContourData);
        }

        public BatchModes getMode()
        {
            BatchModes mode = BatchModes.Normal;

            //check for contour
            try
            {
                if (isContourDataMode())
                    return BatchModes.ContourData;
            }
            catch (WristVizualizerException ex)
            {
                String msg = "Error: " + ex.Message + "\n" + getHelpMessageAsString();
                MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                System.Environment.Exit(1);
            }
            return mode;
        }

        /// <summary>
        /// Checks if enough options were passed to use ContourData mode
        /// </summary>
        /// <returns></returns>
        private bool isContourDataMode()
        {
            //check that we are in the right mode
            if (mode == null || !mode.Equals("contour"))
                return false;

            //at this point we are trying to be in ContourMode, but did we succeed?
            if (String.IsNullOrEmpty(subject))
                throw new WristVizualizerException("Must specify a subject directory");
            
            if (!Directory.Exists(subject))
                throw new WristVizualizerException("Subject directory does not exist");

            if (String.IsNullOrEmpty(test_position))
                throw new WristVizualizerException("Must specify a position to test");

            if (String.IsNullOrEmpty(output))
                throw new WristVizualizerException("Must specify an output file");

            return true;
        }
    }
}
