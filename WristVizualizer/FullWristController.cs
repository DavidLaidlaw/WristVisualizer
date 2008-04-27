using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using libWrist;
using libCoin3D;

namespace WristVizualizer
{    
    class FullWristController
    {
        private bool _showErrors = false;
        private Separator[] _bones;
        private Separator[] _inertias;
        private Wrist _wrist;
        private Transform[][] _transforms;
        private int _currentPositionIndex;
        private int _fixedBoneIndex;

        private Separator _root;

        private FullWristControl _control;

        public FullWristController()
        {
            setupControl();
            _root = new Separator();
            _bones = new Separator[Wrist.NumBones];
            _inertias = new Separator[Wrist.NumBones];
        }

        public void loadFullWrist(string radiusFilename)
        {
            //TODO: ShowFullWristControlBox
            //TODO: Block importing a file
            //TODO: Block viewSource
            //TODO: Enable ShowInertia
            //TODO: Enable ShowACS

            //First Try and load the wrist data
            _wrist = new Wrist();
            try
            {
                _wrist.setupWrist(radiusFilename);
                _transforms = DatParser.makeAllTransforms(_wrist.motionFiles, Wrist.NumBones);
                populateSeriesList();
            }
            catch (ArgumentException ex)
            {
                if (_showErrors)
                {
                    string msg = "Error loading wrist kinematics.\n\n" + ex.Message;
                    //TODO: Change to abort,retry, and find way of cancelling load
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                for (int i = 0; i < Wrist.NumBones; i++)
                    _control.disableFixingBone(i);
            }

            for (int i = 0; i < Wrist.NumBones; i++)
            {
                string fname = _wrist.bpaths[i];
                if (File.Exists(fname))
                {
                    _bones[i] = new Separator();
                    _bones[i].addFile(fname, true);
                    _root.addChild(_bones[i]);
                }
                else
                {
                    _bones[i] = null;
                    _control.disableBone(i);
                }
            }
        }

        public bool ShowErrors
        {
            get { return _showErrors; }
            set { _showErrors = value; }
        }

        public FullWristControl Control
        {
            get { return _control; }
        }

        private void populateSeriesList()
        {
            _currentPositionIndex = 0;
            _control.clearSeriesList();
            _control.addToSeriesList(_wrist.neutralSeries);
            _control.addToSeriesList(_wrist.series);
            _control.selectedSeriesIndex = 0;
        }

        public string getTitleCaption()
        {
            return _wrist.subject + _wrist.side + " - " + _wrist.subjectPath;
        }

        public string getFilenameOfFirstFile()
        {
            return Path.Combine(_wrist.subjectPath, _wrist.subject + _wrist.side);
        }

        private void setupControl()
        {
            _control = new FullWristControl();
            _control.setupControl(Wrist.LongBoneNames, true);
        }

        private void setupControlEventListeners()
        {
            _control.BoneHideChanged += new BoneHideChangedHandler(_control_BoneHideChanged);
            _control.FixedBoneChanged += new FixedBoneChangedHandler(_control_FixedBoneChanged);
            _control.SelectedSeriesChanged += new SelectedSeriesChangedHandler(_control_SelectedSeriesChanged);
        }

        private void removeControlEventListeners()
        {
            _control.BoneHideChanged -= new BoneHideChangedHandler(_control_BoneHideChanged);
            _control.FixedBoneChanged -= new FixedBoneChangedHandler(_control_FixedBoneChanged);
            _control.SelectedSeriesChanged -= new SelectedSeriesChangedHandler(_control_SelectedSeriesChanged);
        }

        void _control_SelectedSeriesChanged(object sender, SelectedSeriesChangedEventArgs e)
        {
            if (_currentPositionIndex == e.SelectedIndex)
                return;

            //check if neutral
            if (e.SelectedIndex == 0)
            {
                _currentPositionIndex = 0;
                //do the neutral thing....
                if (_root.hasTransform())
                    _root.removeTransform();
                for (int i = 0; i < _bones.Length; i++)
                {
                    if (_bones[i] == null) continue; //skip missing bone

                    if (_bones[i].hasTransform())
                        _bones[i].removeTransform();
                }
            }
            else
            {
                //int seriesIndex = _wrist.getSeriesIndexFromName((string)seriesListBox.SelectedItem);
                _currentPositionIndex = e.SelectedIndex;
                if (_root.hasTransform())
                    _root.removeTransform();
                Transform t = new Transform();
                //TODO: Fix so this doesn't have to re-parse the motion file from disk each time...
                DatParser.addRTtoTransform(DatParser.parseMotionFile2(_wrist.getMotionFilePath(_currentPositionIndex - 1))[_fixedBoneIndex], t);
                t.invert();
                //_root.addTransform(_transforms[_currentPositionIndex-1][0]); //minus 1 to skip neutral
                _root.addTransform(t);
                for (int i = 0; i < _bones.Length; i++)
                {
                    //skip missing bones
                    if (_bones[i] == null) continue;

                    //remove the old
                    if (_bones[i].hasTransform())
                        _bones[i].removeTransform();

                    _bones[i].addTransform(_transforms[_currentPositionIndex - 1][i]);
                    if (_transforms[_currentPositionIndex - 1][i].isIdentity())
                        _control.hideBone(i);
                }
            }
        }

        void _control_FixedBoneChanged(object sender, FixedBoneChangeEventArgs e)
        {
            _fixedBoneIndex = e.BoneIndex;

            //do nothing for neutral
            if (_currentPositionIndex == 0) return;

            //so now change the top level
            //do the neutral thing....
            if (_root.hasTransform())
                _root.removeTransform();

            Transform t = new Transform();
            DatParser.addRTtoTransform(DatParser.parseMotionFile2(_wrist.getMotionFilePath(_currentPositionIndex - 1))[e.BoneIndex], t);
            t.invert();
            //_root.addTransform(_transforms[_currentPositionIndex-1][0]); //minus 1 to skip neutral
            _root.addTransform(t);
        }

        void _control_BoneHideChanged(object sender, BoneHideChangeEventArgs e)
        {
            if (e.BoneHidden)
                _bones[e.BoneIndex].hide();
            else
                _bones[e.BoneIndex].show();
        }

        public void setInertiaVisibility(bool visible)
        {
            if (visible)
            {
                try
                {
                    //If its checked, then we need to add it
                    TransformRT[] inert = DatParser.parseInertiaFile2(_wrist.inertiaFile);
                    for (int i = 2; i < 10; i++) //skip the long bones
                    {
                        if (_bones[i] == null)
                            continue;

                        _inertias[i] = new Separator();
                        Transform t = new Transform();
                        DatParser.addRTtoTransform(inert[i], t);
                        _inertias[i].addNode(new ACS());
                        _inertias[i].addTransform(t);
                        _bones[i].addChild(_inertias[i]);
                    }
                }
                catch (ArgumentException ex)
                {
                    string msg = "Error loading inertia file.\n\n" + ex.Message;
                    throw new WristVizualizerException(msg, ex);
                }
            }
            else
            {
                //so we want to remove the inertia files
                for (int i = 2; i < 10; i++)
                {
                    _bones[i].removeChild(_inertias[i]);
                    _inertias[i] = null;
                }

            }
        }

        public void setACSVisibility(bool visible)
        {
            if (visible)
            {
                try
                {
                    //If its checked, then we need to add it
                    TransformRT[] inert = DatParser.parseACSFile2(_wrist.acsFile);

                    //only for radius, check if it exists
                    if (_bones[0] == null)
                        return;

                    _inertias[0] = new Separator();
                    Transform t = new Transform();
                    DatParser.addRTtoTransform(inert[0], t);
                    _inertias[0].addNode(new ACS(45)); //longer axes for the radius/ACS
                    _inertias[0].addTransform(t);
                    _bones[0].addChild(_inertias[0]);
                }
                catch (ArgumentException ex)
                {
                    string msg = "Error loading ACS file.\n\n" + ex.Message;
                    throw new WristVizualizerException(msg, ex);
                }
            }
            else
            {
                _bones[0].removeChild(_inertias[0]);
                _inertias[0] = null;
            }
        }
    }
}
