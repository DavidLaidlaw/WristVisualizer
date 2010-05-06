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

        private int _currentPositionIndex;
        private int _currentTrialIndex;
        private int _fixedBoneIndex;

        private WristPanelLayoutControl _layoutControl;
        private FullWristControl _wristControl;
        private AnimationControl _animationControl;

        //private PosViewControl _posViewControl;

        public XrommController(string filename)
        {
            _xrommFileSys = new XrommFilesystem(filename);
            _fullXromm = new FullXromm(_xrommFileSys);
            _fullXromm.LoadFullJoint();

            _currentPositionIndex = 0;
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
            _animationControl.setupController(frames);
            _animationControl.PlayButtonEnabled = (frames > 1); //no play button if only 1 frame!
            _animationControl.Enabled = (frames > 1);
            _animationControl.StopButtonEnabled = false;

            //TODO: stop timer?
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

        private void ChangeSeries()
        {
        }


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


        public override Separator Root
        {
            get { return _fullXromm.Root; }
        }

        public override Control Control
        {
            get { return _layoutControl; }
        }


        //public override string ApplicationTitle { get { return _firstFilename; } }
        //public override string WatchedFileFilename { get { return _firstFilename; } }
        public override string LastFileFilename { get { return _xrommFileSys.PathFirstIVFile; } }

        public override string ApplicationTitle
        {
            get { return _xrommFileSys.subject + " - " + _xrommFileSys.subjectPath; }
        }
    }
}
