using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using libWrist;
using libCoin3D;

namespace WristVizualizer
{
    partial class LoadTextureDialog : Form
    {
        private static string _LastImagePath = "";
        private static CT _LastMRI = null;

        private ExaminerViewer _viewer;
        private Separator _root;
        private Separator[] _bones;
        private Texture _texture;
        private TransformParser _transformParser;
        private TextureController _controller;

        CropValuesParser _cvParser;
        string _subject;
        string _subjectPath;
        string _seriesKey;
        string _stackSeriesKey;
        int _seriesNumber;
        WristFilesystem.Sides _side;
        KinematicFileTypes _kinematicFileType = KinematicFileTypes.AUTO_REGISTR;
        Modes _mode = Modes.AUTOMATIC;

        int _minX, _maxX, _minY, _maxY, _minZ, _maxZ;

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
            loadVolumeRender.Enabled = false;

            labelErrorCropValues.Text = "";
            labelErrorSubject.Text = "";
            labelErrorKinematicFile.Text = "";
            labelErrorStackFileDir.Text = "";
            labelErrorSeries.Text = "";
            labelErrorImageFile.Text = "";

            string lastSubject = RegistrySettings.getSettingString("TextureLastSubjectDirectory");
            string lastSeries = RegistrySettings.getSettingString("TextureLastSeriesKey");
            if (lastSubject.Length > 0)
                textBoxSubjectDirectory.Text = lastSubject;
            //hopefully everything is loaded here, so lets check
            if (listBoxSeries.Items.Count > 0)
            {
                listBoxSeries.Select();  //make this the active control
                //highlight the last series
                int index = listBoxSeries.Items.IndexOf(lastSeries);
                if (index >= 0)
                    listBoxSeries.SelectedIndex = index;
            }
        }

        private void buttonBrowseImage_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.SelectedPath = textBoxSubjectDirectory.Text;
            if (folder.ShowDialog() == DialogResult.Cancel)
                return;

            textBoxSubjectDirectory.Text = folder.SelectedPath;
        }

        private string getBoneFileName(string shortBoneName)
        {
            string bFileName = String.Format("{0}{1}.stack", shortBoneName.ToLower(), _stackSeriesKey);
            return Path.Combine(textBoxStackFileDirectory.Text, bFileName);
        }

        private string getRadiusSeries(string stackFileDiretory)
        {
            //First try and find it by looking at the directory
            Match m = Regex.Match(stackFileDiretory, @"[\\\/]S(\d{2}[LR])([\\\/]|$)");
            if (m.Success)
            {
                return m.Groups[1].Value;
            }

            //now try by looking for a file with the right name
            DirectoryInfo dir = new DirectoryInfo(stackFileDiretory);
            if (!dir.Exists)
                throw new ArgumentException("Invalid stack file directory");

            foreach (FileInfo file in dir.GetFiles("rad???.stack"))
            {
                Match m2 = Regex.Match(file.Name, @"^rad(\d{2}[LR])\.stack$", RegexOptions.IgnoreCase);
                if (m2.Success)
                    return m2.Groups[1].Value;
            }

            throw new ArgumentException("Unable to find radius stack file in directory. (" + stackFileDiretory + ")");
        }

        private void validate()
        {
            //TODO: Get manual mode working, at all :)
            if (_mode == Modes.MANUAL)
                throw new NotImplementedException("Can not yet load in manual mode");

            //check image file
            if (!Directory.Exists(textBoxImageFile.Text) && !File.Exists(textBoxImageFile.Text))
                throw new ArgumentException("No image file found");

            //check min/max crop data
            NumericUpDown[] boxes = new NumericUpDown[4];
            boxes[0] = numericUpDownMinX;
            boxes[1] = numericUpDownMaxX;
            boxes[2] = numericUpDownMinY;
            boxes[3] = numericUpDownMaxY;
            foreach (NumericUpDown box in boxes)
            {
                if (box.Text.Trim().Length == 0)
                    box.Text = "0";
                else if (Int32.Parse(box.Text) < 0)
                    numericUpDownMinX.Text = "0";
                else if (Int32.Parse(box.Text) > 512)
                    numericUpDownMinX.Text = "512";
            }

            if (Int32.Parse(numericUpDownMinZ.Text) < 0)
                numericUpDownMinZ.Text = "0";
            
            //are we a left or right?
            if (_mode == Modes.MANUAL)
            {
                _stackSeriesKey = getRadiusSeries(textBoxStackFileDirectory.Text);
            }
            else
            {
                //check that we have a radius
                if (!File.Exists(Path.Combine(textBoxStackFileDirectory.Text,String.Format("rad{0}.stack",_stackSeriesKey))))
                    throw new ArgumentException("Unable to find radius stack file in directory. (" + textBoxStackFileDirectory.Text + ")");
            }

            //TODO: Check image files, etc.
        }

        private void parseCropValues()
        {
            _minX = Int32.Parse(numericUpDownMinX.Text);
            _maxX = Int32.Parse(numericUpDownMaxX.Text);
            _minY = Int32.Parse(numericUpDownMinY.Text);
            _maxY = Int32.Parse(numericUpDownMaxY.Text);
            _minZ = Int32.Parse(numericUpDownMinZ.Text);
            _maxZ = Int32.Parse(numericUpDownMaxZ.Text);
        }

        private CT run()
        {
            _bones = new Separator[TextureSettings.ShortBNames.Length];
            _subjectPath = textBoxSubjectDirectory.Text.Trim();
            
            //TODO: Figure out the image type....
            parseCropValues();
            
            CT mri;
            //check if we have this MRI saved!!!, dirty cache
            //TODO: Check if the crop values are compatable!!!
            if (false && _LastImagePath.ToLower().Equals(textBoxImageFile.Text.Trim().ToLower()))
                mri = _LastMRI;
            else
            {
                //pass crop values now, for faster read :)
                mri = CT.SmartLoad(textBoxImageFile.Text);
                mri.setCrop(_minX, _maxX, _minY, _maxY, _minZ, _maxZ);
                if (mri.Layers == 1)  //the default case, we want to load the only layer, echo 0
                    mri.loadImageData();
                else //for other cases, we should try and load layer 5, the layer used by the Wrist Registration Code.
                    mri.loadImageData(5); //TODO: Option for loading different image layers, check for at least 6, etc.
                _LastMRI = mri;
                _LastImagePath = textBoxImageFile.Text.Trim(); //save filename, to use in cache
            }

            Byte[][] voxels = mri.getCroppedRegionScaledToBytes((mri.Layers==1) ? 0 : 5);
            int min = 1000;
            int max = -10;
            for (int i = 0; i < voxels[0].Length; i++)
            {
                if (voxels[0][i] < min) min = voxels[0][i];
                if (voxels[0][i] > max) max = voxels[0][i];
            }

            Hashtable transforms = null;
            _transformParser = null;
            if (File.Exists(textBoxKinematicFilename.Text))
            {
                switch (_kinematicFileType)
                {
                    case KinematicFileTypes.AUTO_REGISTR:
                        _transformParser = new TransformParser(textBoxKinematicFilename.Text);
                        transforms = _transformParser.getFinalTransforms();
                        break;
                    case KinematicFileTypes.OUT_RT:
                        throw new NotImplementedException("Can't yet read OutRT files");
                    case KinematicFileTypes.MOTION:
                        throw new NotImplementedException("Can't yet read Motion files");
                }
            }

            //lets load each bone
            for (int i = 0; i < TextureSettings.ShortBNames.Length; i++)
            {
                double[][] pts = DatParser.parseDatFile(getBoneFileName(TextureSettings.ShortBNames[i]));
                _bones[i] = Texture.createPointsFileObject(pts, TextureSettings.BoneColors[i]);
                //try and load transforms
                if (transforms != null && transforms.ContainsKey(TextureSettings.TransformBNames[i]))
                {
                    Transform tfrm = new Transform();
                    TransformParser.addTfmMatrixtoTransform((TransformMatrix)transforms[TextureSettings.TransformBNames[i]], tfrm);
                    _bones[i].addTransform(tfrm);
                }
                _root.addChild(_bones[i]);
            }

            _texture = new Texture(_side == WristFilesystem.Sides.LEFT ? Texture.Sides.LEFT : Texture.Sides.RIGHT, 
                mri.Cropped_SizeX, mri.Cropped_SizeY, mri.Cropped_SizeZ, mri.voxelSizeX, mri.voxelSizeY, mri.voxelSizeZ);
            Separator plane1 = _texture.makeDragerAndTexture(voxels, Texture.Planes.XY_PLANE);
            Separator plane2 = _texture.makeDragerAndTexture(voxels, Texture.Planes.YZ_PLANE);

            _root.addChild(plane1);
            _root.addChild(plane2);
            _root.addChild(_texture.createKeyboardCallbackObject(_viewer.Parent_HWND));

            //returning mri in order to pass it into the texture controller, to contrust a texture for volume rendering
            return mri;
        }

        /// <summary>
        /// Sets up the scene
        /// </summary>
        /// <param name="viewer">The examiner viewer to display everything in</param>
        /// <returns>a reference to the new Texture Conroller</returns>
        public TextureController setup(ExaminerViewer viewer)
        {
            _viewer = viewer;
            _root = new Separator();
            CT mri=run();
            _viewer.setSceneGraph(_root);
            
            if (checkBoxEnableStepping.Checked){
                _controller = new TextureController(_root, _bones, _transformParser, loadVolumeRender.Checked);
                if (loadVolumeRender.Checked)
                {
                    //if the check box is unchecked it won't bother loading mri stuff
                    _controller.setMRI(mri, (_side == WristFilesystem.Sides.LEFT));
                }
             }
            else
                _controller = new TextureController(_root, null, null, loadVolumeRender.Checked); 
            return _controller;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                validate();
            }
            catch (Exception ex)
            {
                string msg = "Error loading texture.\n" + ex.Message;
                libWrist.ExceptionHandling.HandledExceptionManager.ShowDialog(msg, "", "", ex);
                return;
            }
            //save last Path
            RegistrySettings.saveSetting("TextureLastSubjectDirectory", textBoxSubjectDirectory.Text.Trim());
            if (_seriesKey.Length>0)
                RegistrySettings.saveSetting("TextureLastSeriesKey", _seriesKey);
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
                loadSeriesList();
            }
            else
            {
                _subject = "";
                textBoxCropValuesFilename.Text = "";
                clearSeriesList();
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
        }

        private void clearSeriesList()
        {
            listBoxSeries.DataSource = null;
        }

        /// <summary>
        /// Given a filename for an image, determine the series number and add it to the list
        /// It is added as a left and/or right depending on if a left or right wrist is present
        /// </summary>
        /// <param name="list"></param>
        /// <param name="FileName"></param>
        /// <param name="hasLeft"></param>
        /// <param name="hasRight"></param>
        private void loadSeriesListHelper(ArrayList list, string FileName, bool hasLeft, bool hasRight)
        {
            Match m = Regex.Match(FileName, @"E\d{5}_(\d{2})$");
            if (m.Success)
            {
                if (hasLeft && !list.Contains(m.Groups[1].Value + "L"))
                    list.Add(m.Groups[1].Value + "L");
                if (hasRight && !list.Contains(m.Groups[1].Value + "R"))
                    list.Add(m.Groups[1].Value + "R");
                return; 
            }

            //check for cropped file...
            m = Regex.Match(FileName, @"E\d{5}_(\d{2}[RL])", RegexOptions.IgnoreCase);
            if (m.Success)
            {
                string series = m.Groups[1].Value;
                if (hasLeft && series.ToLower().EndsWith("l") && !list.Contains(series))
                    list.Add(series);
                if (hasRight && series.ToLower().EndsWith("r") && !list.Contains(series))
                    list.Add(series);
                return;
            }
        }

        private void loadSeriesList()
        {
            string imDir = Path.Combine(textBoxSubjectDirectory.Text, "CTScans");
            DirectoryInfo dir = new DirectoryInfo(imDir);
            if (!dir.Exists)
            {
                clearSeriesList();
                labelErrorSeries.Text = "Unable to locate any imges";
                return;
            }
            bool hasLeft = canLocateStackfileDirectory(WristFilesystem.Sides.LEFT);
            bool hasRight = canLocateStackfileDirectory(WristFilesystem.Sides.RIGHT);

            ArrayList list = new ArrayList();
            foreach (DirectoryInfo d in dir.GetDirectories(String.Format("{0}_???", _subject)))
            {
                loadSeriesListHelper(list, d.Name, hasLeft, hasRight);
            }
            foreach (FileInfo f in dir.GetFiles(String.Format("{0}_???", _subject)))
            {
                loadSeriesListHelper(list, f.Name, hasLeft, hasRight);
            }

            list.Sort();  //Sort the list?
            if (list.Contains("15R"))
            {
                list.Remove("15R");
                list.Insert(0, "15R");
            }
            if (list.Contains("15L"))
            {
                list.Remove("15L");
                list.Insert(0, "15L");
            }
            listBoxSeries.DataSource = (string[])list.ToArray(typeof(string));
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
            numericUpDownMinX.Text = "";
            numericUpDownMaxX.Text = "";
            numericUpDownMinY.Text = "";
            numericUpDownMaxY.Text = "";
            numericUpDownMinZ.Text = "";
            numericUpDownMaxZ.Text = "";
        }

        private void setCropValueFields(CropValuesParser.CropValues cv)
        {
            numericUpDownMinX.Text = cv.MinX.ToString();
            numericUpDownMaxX.Text = cv.MaxX.ToString();
            numericUpDownMinY.Text = cv.MinY.ToString();
            numericUpDownMaxY.Text = cv.MaxY.ToString();
            numericUpDownMinZ.Text = cv.MinZ.ToString();
            numericUpDownMaxZ.Text = cv.MaxZ.ToString();
        }

        private void listBoxSeries_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxSeries.SelectedItems.Count == 0)
            {
                if (_mode == Modes.AUTOMATIC)
                {
                    textBoxKinematicFilename.Text = "";
                    textBoxImageFile.Text = "";
                }
                return;
            }

            if (_mode == Modes.MANUAL)
                return; //all automatic below

            _seriesKey = (string)listBoxSeries.SelectedItem;
            _seriesNumber = Int32.Parse(_seriesKey.Substring(0, 2));
            _side = (_seriesKey.Substring(2, 1).ToUpper().Equals("L")) ? WristFilesystem.Sides.LEFT : WristFilesystem.Sides.RIGHT;

            //update kinematics file
            textBoxKinematicFilename.Text = generateKinematicsFileName(textBoxSubjectDirectory.Text, _seriesKey, _kinematicFileType);
            //set crop values:
            if (_cvParser != null && _cvParser.hasPosition(_seriesKey))
                setCropValueFields(_cvParser.getCropData(_seriesKey));
            else
                clearCropValueFields();

            //update image file
            textBoxImageFile.Text = Path.Combine(Path.Combine(textBoxSubjectDirectory.Text, "CTScans"), String.Format("{0}_{1:00}", _subject, _seriesNumber));
            if (!File.Exists(textBoxImageFile.Text) && !Directory.Exists(textBoxImageFile.Text))
                textBoxImageFile.Text = textBoxImageFile.Text + (_side == WristFilesystem.Sides.LEFT ? "L" : "R");

            //try and find stackFile Directory
            string neutralSeriesDir = String.Format("S15{0}",_seriesKey.Substring(2,1));
            _stackSeriesKey = neutralSeriesDir.Substring(1, 3);
            string stackPath1 = Path.Combine(textBoxSubjectDirectory.Text, neutralSeriesDir);
            string stackPath2 = Path.Combine(stackPath1,"Stack.files");
            if (canLocateRadiusStackFileInDirectory(stackPath2)) //try Stack.files directory first
                textBoxStackFileDirectory.Text = stackPath2;
            else if (canLocateRadiusStackFileInDirectory(stackPath1))
                textBoxStackFileDirectory.Text = stackPath1;
            else //error, can't locate, lets just default to Stack.files path
                textBoxStackFileDirectory.Text = stackPath2;
        }

        private bool canLocateStackfileDirectory(WristFilesystem.Sides side)
        {
            string s = (side == WristFilesystem.Sides.LEFT) ? "L" : "R";
            string neutralSeriesDir = "S15" + s;
            string stackPath1 = Path.Combine(textBoxSubjectDirectory.Text, neutralSeriesDir);
            string stackPath2 = Path.Combine(stackPath1, "Stack.files");
            if (canLocateRadiusStackFileInDirectory(stackPath2, s)) //try Stack.files directory first
                return true;
            else if (canLocateRadiusStackFileInDirectory(stackPath1, s))
                return true;
            else
                return false;
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

        private bool canLocateRadiusStackFileInDirectory(string stackFileDir)
        {
            return canLocateRadiusStackFileInDirectory(stackFileDir, _seriesKey.Substring(2, 1));
        }
        private bool canLocateRadiusStackFileInDirectory(string stackFileDir, string side)
        {
            string radName = String.Format("rad15{0}.stack", side);
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

            if (valid && !canLocateRadiusStackFileInDirectory(textBoxStackFileDirectory.Text))
            {
                labelErrorStackFileDir.Text = "Unable to locate radius stack file in dir.";
                valid = false;
            }
        }

        private void textBoxImageFile_TextChanged(object sender, EventArgs e)
        {
            labelErrorImageFile.Text = "";
            bool valid = true;

            if (textBoxImageFile.Text.Trim().Length == 0)
            {
                valid = false;
            }

            if (valid && !File.Exists(textBoxImageFile.Text) && !Directory.Exists(textBoxImageFile.Text))
            {
                labelErrorImageFile.Text = "No image found";
            }
        }

        #region Public Properties
        public string DisplayTitle
        {
            get { return _subject + "_" + _seriesKey + " - " + _subjectPath; }
        }

        public TextureController Controller
        {
            get { return _controller; }
        }

        public bool EnableSteppingRegistration
        {
            get { return checkBoxEnableStepping.Checked; }
        }
        #endregion


        private void listBoxSeries_DoubleClick(object sender, EventArgs e)
        {
            //make it so double clicking a series loads that series
            this.buttonOK_Click(sender, null);
        }

        private void loadVolumeRender_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxEnableStepping_CheckedChanged(object sender, EventArgs e)
        {
            loadVolumeRender.Enabled = checkBoxEnableStepping.Checked;
        }
    }
}