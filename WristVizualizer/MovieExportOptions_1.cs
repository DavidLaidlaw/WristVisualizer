using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WristVizualizer
{
    public partial class MovieExportOptions : Form
    {
        public enum SaveType
        {
            IMAGES,
            MOVIE,
            CANCEL
        }

        private SaveType _result;
        private string _defaultPath;
        private string _outputFileName;

        public MovieExportOptions(string defaultPath, decimal startingFPS)
        {
            InitializeComponent();

            _result = SaveType.CANCEL;
            _defaultPath = defaultPath;
            numericUpDownFPS.Value = startingFPS;
            radioMovie_CheckedChanged(this, null);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            getOutputFileName();
            this.Close();
        }

        public SaveType results
        {
            get { return _result; }
        }

        public string OutputFileName
        {
            get { return _outputFileName; }
        }

        public decimal MovieFPS
        {
            get { return numericUpDownFPS.Value; }
        }

        public bool MovieCompress
        {
            get { return checkBoxCompression.Checked; }
        }

        public int SmoothFactor
        {
            get
            {
                if (radioButton2xSmoothing.Checked)
                    return 2;
                if (radioButton3xSmoothing.Checked)
                    return 3;
                return 1;  //default is 1
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            _result = SaveType.CANCEL;
            this.Close();
        }

        private void radioMovie_CheckedChanged(object sender, EventArgs e)
        {
            //set check box enabled status
            checkBoxCompression.Enabled = radioMovie.Checked;
            numericUpDownFPS.Enabled = radioMovie.Checked;
        }

        private void getOutputFileName()
        {
            if (radioImages.Checked)
            {
                //images mode, set mode, then get the directory for output
                _result = SaveType.IMAGES;

                FolderBrowserDialog fb = new FolderBrowserDialog();
                fb.SelectedPath = System.IO.Path.GetDirectoryName(_defaultPath);
#if DEBUG
                fb.SelectedPath = @"C:\Temp\testMovie";
#endif
                fb.ShowNewFolderButton = true;
                fb.Description = "Choose directory to output movie frames to";
                if (DialogResult.OK != fb.ShowDialog())
                {
                    //this was a clear cancel....
                    _result = SaveType.CANCEL;
                    return;
                }
                _outputFileName = fb.SelectedPath;
            }
            else if (radioMovie.Checked)
            {
                //movie mode, set meode, then get the filename
                _result = SaveType.MOVIE;

                SaveFileDialog save = new SaveFileDialog();
                save.DefaultExt = "avi";
                save.AddExtension = true;
                save.Filter = "Avi Files (*.avi)|*.avi|All Files (*.*)|*.*";
                save.CheckPathExists = true;
                save.ValidateNames = true;
                save.InitialDirectory = System.IO.Path.GetDirectoryName(_defaultPath);

                if (save.ShowDialog() == DialogResult.Cancel)
                {
                    _result = SaveType.CANCEL;
                    return;
                }

                _outputFileName = save.FileName;
            }
            else
            {
                throw new ApplicationException("Unreachable code reached.... no idea how");
            }
        }
    }
}