using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace WristVizualizer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            Application.Run(new WristVizualizer());

        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            string log = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "error.log");
            //log = @"C:\Documents and Settings\Evan\My Documents\Visual Studio 2005\Projects\Vizualization\WristVizualizer\bin\Release\test.txt";
            try //there is a chance of failure when writing to the log file
            {
                StreamWriter wr = new StreamWriter(log, true);
                wr.WriteLine(e.Exception.ToString());
                wr.Close();
                wr.Dispose();
            }
            catch { } //if the log fails....well, we are just fucked then

            //throw ex; //rethrow original exception
            MessageBox.Show(e.Exception.Message,"Fatal Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            Application.Exit();
        }
    }
}