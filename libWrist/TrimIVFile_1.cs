using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace libWrist
{
    public static class TrimIVFile
    {
        private const string CAMERA = "OrthographicCamera";
        private const string MATERIAL = "Material";

        public static void tryStripFile(string filename, bool stripCamera, bool stripMaterial)
        {
            if (!File.Exists(filename))
            {
                MessageBox.Show("Error: File does not exist", "IV Trim", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            try
            {
                TrimIVFile.stripFile(filename, stripCamera, stripMaterial);
            }
            catch (Exception ex)
            {
                string msg = String.Format("Error trimming file ({0}):\n{1}", filename, ex.Message);
                libWrist.ExceptionHandling.HandledExceptionManager.ShowDialog(msg, "", "", ex);
            }
        }

        public static void stripFiles(string[] filenames, bool stripCamera, bool stripMaterial)
        {
            foreach (string file in filenames)
                stripFile(file, stripCamera, stripMaterial);
        }

        public static void stripFile(string filename, bool stripCamera, bool stripMaterial)
        {
            string file; //text for the entire file will go here...!
            try
            {
                StreamReader reader = new StreamReader(filename);
                file = reader.ReadToEnd();
                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception("Error reading in IV file", e);
            }


            //now strip from the string
            if (stripCamera)
            {
                while (file.IndexOf(CAMERA) > 0)
                {
                    int cameraIndex = file.IndexOf(CAMERA);
                    int cameraEndIndex = file.IndexOf("}", cameraIndex);
                    file = file.Remove(cameraIndex, cameraEndIndex - cameraIndex + 1);
                }
            }

            if (stripMaterial)
            {
                while (file.IndexOf(MATERIAL) > 0)
                {
                    int materialIndex = file.IndexOf(MATERIAL);
                    int materialEndIndex = file.IndexOf("}", materialIndex);
                    file = file.Remove(materialIndex, materialEndIndex - materialIndex + 1);
                }
            }

            try
            {
                StreamWriter writer = new StreamWriter(filename, false);
                writer.Write(file);
                writer.Close();
            }
            catch (Exception e)
            {
                throw new Exception("Error writing out moddified IV file", e);
            }
        }
    }
}
