using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using libWrist;
using libCoin3D;

namespace WristVizualizer
{
    public partial class LoadTextureDialog : Form
    {
        private ExaminerViewer _viewer;
        private Separator _root;
        private Texture _texture;

        CropValuesParser _cvParser;
        string _subject;
        string _seriesKey;
        int _seriesNumber;
        Wrist.Sides _side;
        KinematicFileTypes _kinematicFileType = KinematicFileTypes.AUTO_REGISTR;
        Modes _mode = Modes.AUTOMATIC;

        private enum KinematicFileTypes
        {
            AUTO_REGISTR,
            OUT_RT,
            MOTION
        }

        private enum Modes
        {
            AUTOMATIC,
            MANUAL
        }

        public LoadTextureDialog()
        {
            InitializeComponent();
            labelErrorCropValues.Text = "";
            labelErrorSubject.Text = "";
            labelErrorKinematicFile.Text = "";
            labelErrorStackFileDir.Text = "";
        }

        private void buttonBrowseImage_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.SelectedPath = textBoxSubjectDirectory.Text;
            if (folder.ShowDialog() == DialogResult.Cancel)
                return;

            textBoxSubjectDirectory.Text = folder.SelectedPath;
        }

        private string getBoneFileName(string shortBoneName, Wrist.Sides side)
        {
            string form = @"C:\Ortho\E01424\S15R\Stack.files\{0}15{1}.stack";
            return String.Format(form, shortBoneName.ToLower(), side == Wrist.Sides.LEFT ? "L" : "R");
        }

        private void run()
        {
            string subjectPath = @"C:\Ortho\E01424";
            string subject = System.Text.RegularExpressions.Regex.Match(subjectPath,@"(E\d{5})\\?\s*$").Groups[1].Value;
            int series = 15;
            Wrist.Sides side = Wrist.Sides.RIGHT;

            string seriesKey = "15R";

            string cropValuesFilename = Path.Combine(subjectPath, "crop_values.txt");
            string image = Path.Combine(Path.Combine(subjectPath, "CTScans"), String.Format("{0}_{1:00}", subject, series));

            string ulnaStackFile = getBoneFileName("uln", Wrist.Sides.RIGHT);

            //crop values

            CropValuesParser cvp = new CropValuesParser(cropValuesFilename);
            CropValuesParser.CropValues cv = cvp.getCropData(seriesKey);

            CTmri mri = new CTmri(image);
            mri.setCrop(cv.MinX, cv.MaxX, cv.MinY, cv.MaxY, cv.MinZ, cv.MaxZ);

            double LO_RES_HEIGHT = mri.voxelSizeX;
            double LO_RES_WIDTH = mri.voxelSizeX;
            double RES_DEPTH = mri.voxelSizeZ;

            int sizeX = cv.SizeX;
            int sizeY = cv.SizeY;
            int sizeZ = cv.SizeZ;

            Random r = new Random();

            //lets build an array of bytes (unsigned 8bit data structure)
            Byte[][] voxels = new Byte[sizeZ][];
            for (int i = 0; i < sizeZ; i++)
            {
                voxels[i] = new Byte[sizeX * sizeY];
                for (int j = 0; j < sizeY; j++)
                    for (int k = 0; k < sizeX; k++)
                    {
                        voxels[i][(j * sizeX) + k] = (Byte)mri.getCroppedVoxel(k, j, i);
                    }
            }

            //lets load each bone
            for (int i = 0; i < TextureSettings.ShortBNames.Length; i++)
            {
                double[][] pts = DatParser.parseDatFile(getBoneFileName(TextureSettings.ShortBNames[i], Wrist.Sides.RIGHT));
                Separator bone = Texture.createPointsFileObject(pts, TextureSettings.BoneColors[i]);
                _root.addChild(bone);
            }

            _texture = new Texture(cv.Side== Wrist.Sides.LEFT ? Texture.Sides.LEFT : Texture.Sides.RIGHT, 
                cv.SizeX, cv.SizeY, cv.SizeZ, cv.VoxelX, cv.VoxelY, cv.VoxelZ);
            Separator plane1 = _texture.makeDragerAndTexture(voxels, Texture.Planes.XY_PLANE);
            Separator plane2 = _texture.makeDragerAndTexture(voxels, Texture.Planes.YZ_PLANE);
            _root.addChild(plane1);
            _root.addChild(plane2);
            _root.addChild(_texture.createKeyboardCallbackObject(0));
        }

        /// <summary>
        /// Sets up the scene
        /// </summary>
        /// <param name="viewer">The examiner viewer to display everything in</param>
        /// <returns>a reference to the new root Separator node</returns>
        public Separator setup(ExaminerViewer viewer)
        {
            _viewer = viewer;
            _root = new Separator();
            run();
            _viewer.setSceneGraph(_root);
            return _root;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void textBoxSubjectDirectory_TextChanged(object sender, EventArgs e)
        {
            bool valid = true;
            labelErrorSubject.Text = "";

            if (textBoxSubjectDirectory.Text.Trim().Length == 0)
            {
                //special case for nothing...?
                labelErrorSubject.Text = "";
                valid = false;
            }

            if (valid && !Directory.Exists(textBoxSubjectDirectory.Text))
            {
                //error, report
                labelErrorSubject.Text = "Not a valid directory";
                valid = false;
            }
            string subjectDirectory = textBoxSubjectDirectory.Text.Trim();
            Match m = Regex.Match(subjectDirectory, @"(E\d{5})\\?\s*$");
            if (valid && !m.Success)  //only go here if already valid
            {
                //failure, lets report it
                labelErrorSubject.Text = "Unable to determine subject from directory";
                valid = false;
            }

            //got this far, then things are okay
            if (_mode == Modes.MANUAL)
                return; //below this is all the auto stuff :)

            if (valid)
            {
                _subject = m.Groups[1].Value;
                textBoxCropValuesFilename.Text = Path.Combine(subjectDirectory, "crop_values.txt");
            }
            else
            {
                _subject = "";
                textBoxCropValuesFilename.Text = "";
            }
        }

        private void textBoxCropValuesFilename_TextChanged(object sender, EventArgs e)
        {
            bool valid = true;
            labelErrorCropValues.Text = "";
            if (textBoxCropValuesFilename.Text.Trim().Length == 0)
            {
                labelErrorCropValues.Text = "";
                //special case for nothing...?
                valid = false;
            }

            //check that it exists
            if (valid && !File.Exists(textBoxCropValuesFilename.Text))
            {
                labelErrorCropValues.Text = "No crop values file found";
                valid = false;
            }

            
            if (_mode == Modes.MANUAL)
                return; //below is automatic

            if (valid) //still valid so far....
            {
                //got this far, we should load it....yes?
                //check that the file is a CV file
                try
                {
                   CropValuesParser parser = new CropValuesParser(textBoxCropValuesFilename.Text);
                    _cvParser = parser;
                }
                catch (Exception ex)
                {
                    labelErrorCropValues.Text = "Error reading file: " + ex.Message;
                    valid = false;
                    _cvParser = null;
                }
            }

            if (valid)
            {
                //looks okay, next step            
                loadSeriesList();
            }
            else
            {
                listBoxSeries.DataSource = null;
            }
        }

        private void loadSeriesList()
        {
            listBoxSeries.DataSource = _cvParser.getAllSeries();
        }

        private string generateKinematicsFileName(string subjectPath, string seriesKey, KinematicFileTypes kinType)
        {
            string folder, filename;
            switch (kinType)
            {
                case KinematicFileTypes.AUTO_REGISTR:
                    folder = Path.Combine(subjectPath, "auto_registr_results");
                    filename = String.Format("test{0}.txt", seriesKey);
                    return Path.Combine(folder, filename);
                case KinematicFileTypes.OUT_RT:
                    folder = Path.Combine(subjectPath, "collected_results");
                    filename = String.Format("outRT_{0}.txt", seriesKey);
                    return Path.Combine(folder, filename);
                case KinematicFileTypes.MOTION:
                    folder = Path.Combine(subjectPath, String.Format("S{0}", seriesKey));
                    filename = String.Format("Motion15{0}{1}.dat", seriesKey.Substring(2, 1), seriesKey);
                    return Path.Combine(folder, filename);
                default:
                    return "";
            }
        }

        private void clearCropValueFields()
        {
            maskedTextBoxMinX.Text = "";
            maskedTextBoxMaxX.Text = "";
            maskedTextBoxMinY.Text = "";
            maskedTextBoxMaxY.Text = "";
            maskedTextBoxMinZ.Text = "";
            maskedTextBoxMaxZ.Text = "";
        }

        private void setCropValueFields(CropValuesParser.CropValues cv)
        {
            maskedTextBoxMinX.Text = cv.MinX.ToString();
            maskedTextBoxMaxX.Text = cv.MaxX.ToString();
            maskedTextBoxMinY.Text = cv.MinY.ToString();
            maskedTextBoxMaxY.Text = cv.MaxY.ToString();
            maskedTextBoxMinZ.Text = cv.MinZ.ToString();
            maskedTextBoxMaxZ.Text = cv.MaxZ.ToString();
        }

        private void listBoxSeries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxSeries.SelectedItems.Count == 0)
            {
                if (_mode == Modes.AUTOMATIC)
                {
                    textBoxKinematicFilename.Text = "";
                }
                return;
            }

            if (_mode == Modes.MANUAL)
                return; //all automatic below

            _seriesKey = (string)listBoxSeries.SelectedItem;
            _seriesNumber = Int32.Parse(_seriesKey.Substring(0, 2));
            _side = (_seriesKey.Substring(2, 1).ToUpper().Equals("L")) ? Wrist.Sides.LEFT : Wrist.Sides.RIGHT;

            //update kinematics file
            textBoxKinematicFilename.Text = generateKinematicsFileName(textBoxSubjectDirectory.Text, _seriesKey, _kinematicFileType);
            //set crop values:
            if (_cvParser != null)
                setCropValueFields(_cvParser.getCropData(_seriesKey));
            else
                clearCropValueFields();
            //try and find stackFile Directory
            string neutralSeriesDir = String.Format("S15{0}",_seriesKey.Substring(2,1));
            string stackPath1 = Path.Combine(textBoxSubjectDirectory.Text, neutralSeriesDir);
            string stackPath2 = Path.Combine(stackPath1,"Stack.files");
            if (canLocateRadiusStackFile(stackPath2)) //try Stack.files directory first
                textBoxStackFileDirectory.Text = stackPath2;
            else if (canLocateRadiusStackFile(stackPath1))
                textBoxStackFileDirectory.Text = stackPath1;
            else //error, can't locate, lets just default to Stack.files path
                textBoxStackFileDirectory.Text = stackPath2;
        }

        private void radioButtonKinematics_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonKinematicAutoRegistr.Checked)
                _kinematicFileType = KinematicFileTypes.AUTO_REGISTR;
            else if (radioButtonKinematicRT.Checked)
                _kinematicFileType = KinematicFileTypes.OUT_RT;
            else if (radioButtonKinematicMotion.Checked)
                _kinematicFileType = KinematicFileTypes.MOTION;

            listBoxSeries_SelectedIndexChanged(this, null);
        }

        private void textBoxKinematicFilename_TextChanged(object sender, EventArgs e)
        {
            labelErrorKinematicFile.Text = "";
            if (textBoxKinematicFilename.Text.Trim().Length == 0)
            {
                //special case, nothing
                return;
            }

            if (!File.Exists(textBoxKinematicFilename.Text))
            {
                labelErrorKinematicFile.Text = "Kinematic file does not exist, defaulting to global position";
                return;
            }
        }

        private void radioButtonMode_CheckedChanged(object sender, EventArgs e)
        {
            _mode = (radioButtonAutomatic.Checked) ? Modes.AUTOMATIC : Modes.MANUAL;
        }

        private bool canLocateRadiusStackFile(string stackFileDir)
        {
            string radName = String.Format("rad15{0}.stack", _seriesKey.Substring(2, 1));
            string fullRadName = Path.Combine(stackFileDir, radName);
            return (File.Exists(fullRadName));
        }

        private void textBoxStackFileDirectory_TextChanged(object sender, EventArgs e)
        {
            labelErrorStackFileDir.Text = "";
            bool valid = true;
            if (textBoxStackFileDirectory.Text.Trim().Length == 0)
            {
                valid = false;
            }

            if (!Directory.Exists(textBoxStackFileDirectory.Text))
            {
                labelErrorStackFileDir.Text = "Directory does not exist";
                valid = false;
            }

            if (valid && !canLocateRadiusStackFile(textBoxStackFileDirectory.Text))
            {
                labelErrorStackFileDir.Text = "Unable to locate radius stack file in dir.";
                valid = false;
            }
        }
    }
}