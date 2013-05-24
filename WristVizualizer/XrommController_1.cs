using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using libWrist;
using libCoin3D;

namespace WristVizualizer
{
    class XrommController : Controller
    {
        private XrommFilesystem _xrommFileSys;
        private FullXromm _fullXromm;
        private ExaminerViewer _viewer;

        private int _currentTrialIndex;
        private int _fixedBoneIndex;

        private WristPanelLayoutControl _layoutControl;
        private FullWristControl _wristControl;
        private AnimationControl _animationControl;

        public XrommController(ExaminerViewer viewer, string filename)
        {
            _xrommFileSys = new XrommFilesystem(filename);
            _fullXromm = new FullXromm(_xrommFileSys);
            _fullXromm.LoadFullJoint();
            _viewer = viewer;

            _fixedBoneIndex = 0;
            _currentTrialIndex = 0;

            SetupXrommControl();
            ResetAnimationControl();
            setupControlEventListeners();
        }

        #region Setup and Breakdown

        public override void CleanUp()
        {
            removeControlEventListeners();
        }

        private void SetupXrommControl()
        {
            string[] seriesNames = createSeriesListWithNiceNames();
            _layoutControl = new WristPanelLayoutControl();
            _wristControl = new FullWristControl();
            _wristControl.setupControl(_xrommFileSys.ShortBoneNames, true, seriesNames.Length);
            _layoutControl.addControl(_wristControl);

            _animationControl = new AnimationControl();
            _animationControl.EnableInternalTimer();
            _animationControl.FPS = 10; //default            
            _layoutControl.addControl(_animationControl);

            _wristControl.clearSeriesList();
            _wristControl.addToSeriesList(seriesNames);
            _wristControl.selectedSeriesIndex = 0;
        }



        private void setupControlEventListeners()
        {
            _wristControl.BoneHideChanged += new BoneHideChangedHandler(_control_BoneHideChanged);
            _wristControl.FixedBoneChanged += new FixedBoneChangedHandler(_control_FixedBoneChanged);
            _wristControl.SelectedSeriesChanged += new SelectedSeriesChangedHandler(_control_SelectedSeriesChanged);

            _animationControl.TrackbarScroll += new AnimationControl.TrackbarScrollHandler(_animationControl_TrackbarScroll);
        }


        private void removeControlEventListeners()
        {
            _wristControl.BoneHideChanged -= new BoneHideChangedHandler(_control_BoneHideChanged);
            _wristControl.FixedBoneChanged -= new FixedBoneChangedHandler(_control_FixedBoneChanged);
            _wristControl.SelectedSeriesChanged -= new SelectedSeriesChangedHandler(_control_SelectedSeriesChanged);

            _animationControl.TrackbarScroll -= new AnimationControl.TrackbarScrollHandler(_animationControl_TrackbarScroll);
        }
        #endregion

        private void ResetAnimationControl()
        {            
            int frames = _fullXromm.NumPositionsPerTrial[_currentTrialIndex];
            _animationControl.StopTimer();
            _animationControl.setupController(frames);
            _animationControl.PlayButtonEnabled = (frames > 1); //no play button if only 1 frame!
            _animationControl.Enabled = (frames > 1);
            _animationControl.StopButtonEnabled = false;
        }


        #region CallBacks
        void _control_SelectedSeriesChanged(object sender, SelectedSeriesChangedEventArgs e)
        {
            if (_currentTrialIndex == e.SelectedIndex)
                return;

            _currentTrialIndex = e.SelectedIndex;
            ResetAnimationControl();
            _fullXromm.SetToPositionAndFixedBoneAndTrial(_animationControl.currentFrame, _fixedBoneIndex, _currentTrialIndex);
        }

        void _control_FixedBoneChanged(object sender, FixedBoneChangeEventArgs e)
        {
            _fixedBoneIndex = e.BoneIndex;
            _fullXromm.SetToPositionAndFixedBoneAndTrial(_animationControl.currentFrame, _fixedBoneIndex, _currentTrialIndex);
        }

        void _control_BoneHideChanged(object sender, BoneHideChangeEventArgs e)
        {
            _fullXromm.Bones[e.BoneIndex].SetBoneVisibilityManually(!e.BoneHidden);
        }

        void _animationControl_TrackbarScroll()
        {
            _fullXromm.SetToPositionAndFixedBoneAndTrial(_animationControl.currentFrame, _fixedBoneIndex, _currentTrialIndex);
        }

        #endregion


        private string[] createSeriesListWithNiceNames()
        {
            string[] series = new string[_xrommFileSys.Trials.Length + 1]; // +1 for CT position
            series[0] = "CT Scan";

            for (int i = 0; i < _xrommFileSys.Trials.Length; i++)
                series[i + 1] = _xrommFileSys.Trials[i].TrialName;

            //now try and replace ugly names with nice ones :)
            string configFile = _xrommFileSys.SeriesNamesFilename;
            if (System.IO.File.Exists(configFile))
            {
                Dictionary<string, string> lookupTable = IniFileParser.GetIniFileStrings(configFile, "SeriesNames");
                for (int i = 0; i < series.Length; i++)
                {
                    if (lookupTable.ContainsKey(series[i]))
                        series[i] = lookupTable[series[i]];
                }
            }
            return series;
        }

        public override bool CanEditBoneColors { get { return true; } }
        public override void EditBoneColorsShowDialog()
        {
            EditBoneColors edit = new EditBoneColors(_fullXromm);
            DialogResult r = edit.ShowDialog();
            if (r != DialogResult.OK)
                return;
            for (int i = 0; i < _xrommFileSys.NumBones; i++)
            {
                if (_fullXromm.Bones[i].IsValidBone && edit.IsColorChanged(i))
                    _fullXromm.Bones[i].SetColor(edit.GetNewBoneColor(i));
            }
        }

        #region Movie Export
        public override bool CanSaveToMovie
        {
            // we can only save if we are in animation mode, which is defined by the control existing
            get { return (_animationControl != null); }
        }
        public override void saveToMovie()
        {
            //save starting state & stop playback
            bool startPlaying = _animationControl.AnimationTimer.Enabled;
            _animationControl.AnimationTimer.Stop();

            //show save dialogue 
            MovieExportOptions dialog = new MovieExportOptions(_xrommFileSys.subjectPath, _animationControl.FPS);
            dialog.ShowDialog();

            //okay, now lets figure out what we are doing here
            switch (dialog.results)
            {
                case MovieExportOptions.SaveType.CANCEL:
                    //nothing to do, we were canceled
                    break;
                case MovieExportOptions.SaveType.IMAGES:
                    //save images
                    string outputDir = dialog.OutputFileName;
                    //save it to a movie
                    _viewer.cacheOffscreenRenderer();
                    for (int i = 0; i < _animationControl.NumberOfFrames; i++)
                    {
                        _fullXromm.SetToPositionAndFixedBoneAndTrial(i, _fixedBoneIndex, _currentTrialIndex);  //change to current frame
                        string fname = System.IO.Path.Combine(outputDir, String.Format("outfile{0:d3}.jpg", i));
                        _viewer.saveToJPEG(fname);
                    }
                    _viewer.clearOffscreenRenderer();
                    break;
                case MovieExportOptions.SaveType.MOVIE:
                    //save movie
                    try
                    {
                        AviFile.AviManager aviManager = new AviFile.AviManager(dialog.OutputFileName, false);
                        int smooth = dialog.SmoothFactor;
                        _viewer.cacheOffscreenRenderer(smooth); //TODO: Check that output is multiple of 4!!!!
                        _fullXromm.SetToPositionAndFixedBoneAndTrial(0, _fixedBoneIndex, _currentTrialIndex); ; //set to first frame, so we can grab it.
                        System.Drawing.Bitmap frame = getSmoothedFrame(smooth);
                        AviFile.VideoStream vStream = aviManager.AddVideoStream(dialog.MovieCompress, (double)dialog.MovieFPS, frame);
                        for (int i = 1; i < _animationControl.NumberOfFrames; i++) //start from frame 1, frame 0 was added when we began
                        {
                            _fullXromm.SetToPositionAndFixedBoneAndTrial(i, _fixedBoneIndex, _currentTrialIndex);
                            vStream.AddFrame(getSmoothedFrame(smooth));
                        }
                        aviManager.Close();  //close out and save
                        _viewer.clearOffscreenRenderer();
                    }
                    catch (Exception ex)
                    {
                        string msg = "Error saving to movie file.\n\n" + ex.Message;
                        libWrist.ExceptionHandling.HandledExceptionManager.ShowDialog(msg, "", "", ex);
                    }
                    break;
            }

            //wrap us up, resume frame and playing status
            _fullXromm.SetToPositionAndFixedBoneAndTrial(_animationControl.currentFrame, _fixedBoneIndex, _currentTrialIndex);
            if (startPlaying)
                _animationControl.AnimationTimer.Start();
        }

        private System.Drawing.Bitmap getSmoothedFrame(int smoothFactor)
        {
            System.Drawing.Image rawImage = _viewer.getImage();
            if (smoothFactor == 1)
                return (System.Drawing.Bitmap)rawImage;

            System.Drawing.Image finalImage = new System.Drawing.Bitmap(rawImage.Size.Width / smoothFactor, rawImage.Size.Height / smoothFactor);
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(finalImage))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(rawImage, 0, 0, rawImage.Size.Width / smoothFactor, rawImage.Size.Height / smoothFactor);
            }
            rawImage.Dispose();
            return (System.Drawing.Bitmap)finalImage;
        }

        #endregion


        public override Separator Root
        {
            get { return _fullXromm.Root; }
        }

        public override Control Control
        {
            get { return _layoutControl; }
        }


        //public override string WatchedFileFilename { get { return _firstFilename; } }
        public override string LastFileFilename { get { return _xrommFileSys.PathFirstIVFile; } }

        public override string ApplicationTitle
        {
            get { return _xrommFileSys.subject + " - " + _xrommFileSys.subjectPath; }
        }
    }
}
