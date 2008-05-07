using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using libWrist;
using libCoin3D;

namespace WristVizualizer
{    
    class FullWristController : Controller
    {
        private bool _showErrors = false;
        private ColoredBone[] _colorBones;
        private Separator[] _bones;
        private Separator[] _inertias;
        private Wrist _wrist;
        private TransformMatrix[][] _transformMatrices;
        private int _currentPositionIndex;
        private int _lastPositionIndex;
        private int _fixedBoneIndex;
        private int _lastFixedBoneIndex;

        private Separator _root;

        //animation stuff
        private bool _animatePositionChanges;
        private AnimationController _animationController;
        private int _FPS;
        private double _animateDuration;
        private TransformMatrix[] _tmCurrentPositionRelativeMotion;

        //GUI stuff
        private FullWristControl _wristControl;

        public FullWristController()
        {
            setupControl();
            setupControlEventListeners();
            _root = new Separator();
            _bones = new Separator[Wrist.NumBones];
            _colorBones = new ColoredBone[Wrist.NumBones];
            _inertias = new Separator[Wrist.NumBones];
            _tmCurrentPositionRelativeMotion = new TransformMatrix[Wrist.NumBones];

            //defaults
            _FPS = 15;
            _animateDuration = 0.5;
        }

        public new void CleanUp()
        {
            removeControlEventListeners();
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
                loadTransforms();
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
                    _wristControl.disableFixingBone(i);
            }

            for (int i = 0; i < Wrist.NumBones; i++)
            {
                string fname = _wrist.bpaths[i];
                if (File.Exists(fname))
                {
                    _bones[i] = new Separator();
                    _colorBones[i] = new ColoredBone(fname);
                    _bones[i].addNode(_colorBones[i]);
                    _root.addChild(_bones[i]);
                }
                else
                {
                    _bones[i] = null;
                    _wristControl.disableBone(i);
                }
            }

            //loadDistanceMaps();
        }

        public void loadDistanceMaps()
        {
            CTmri[] mri = new CTmri[Wrist.NumBones];

            for (int i = 0; i < Wrist.NumBones; i++)
            {
                string basefolder = Path.Combine(Path.Combine(_wrist.subjectPath,_wrist.neutralSeries),"DistanceFields");
                string folder = String.Format("{0}{1}_mri",Wrist.ShortBoneNames[i],_wrist.neutralSeries.Substring(1,3));
                if (Directory.Exists(Path.Combine(basefolder, folder)))
                {
                    mri[i] = new CTmri(Path.Combine(basefolder, folder));
                    mri[i].loadImageData();
                }
                else
                    mri[i] = null;
            }

            //try and create color scheme....
            for (int i = 0; i < Wrist.NumBones; i++)
            {
                int[] colors = createColormap(mri, i);
                
                //now set that color
                _colorBones[i].setColorMap(colors);
            }
        }

        private int[] createColormap(CTmri[] mri, int boneIndex)
        {
            double dDist = Double.MaxValue;
            float[,] pts = _colorBones[boneIndex].getVertices();
            int dim0 = pts.GetLength(0);
            int dim1 = pts.GetLength(1);
            double[] dDistances = new double[Wrist.NumBones - 1];

            int[] colors = new int[dim0];

            //for each vertex           
            for (int i = 0; i < dim0; i++)
            {
                int m = 0; 
                for (int j = 0; j < Wrist.NumBones; j++)
                {
                    if (j == boneIndex) continue;

                    double dX = (pts[i, 0] - mri[j].CoordinateOffset[0]) / mri[j].voxelSizeX;
                    double dY = (pts[i, 1] - mri[j].CoordinateOffset[1]) / mri[j].voxelSizeY;
                    double dZ = (pts[i, 2] - mri[j].CoordinateOffset[2]) / mri[j].voxelSizeZ;

                    double xBound = 96.9; //get the boundaries of the distance cube
                    double yBound = 96.9;//
                    double zBound = 96.9; //

                    ////////////////////////////////////////////////////////
                    //is surface point picked inside of the cube?

                    if (dX >= 3.1 && dX <= xBound && dY >= 3.1
                        && dY <= yBound && dZ >= 3.1 && dZ <= zBound)
                    {

                        //dDist = distMRI[j]->sample(MRI_INTERP_CUBIC, dX, dY, dZ, 0, 0, 0, 0);
                        dDist = mri[j].sample_s_InterpCubit(dX, dY, dZ);
                        //dDist = mri[j].getVoxel_s((int)Math.Floor(dX), (int)Math.Floor(dY), (int)Math.Floor(dZ), 0);
                        //if (cubicdDist - dDist > 2)
                        //{
                        //    Console.WriteLine("Difference of {0}", cubicdDist - dDist);
                        //}
                    }
                    else
                        dDist = Double.MaxValue;

                    dDistances[m] = dDist;
                    m++;
                }

                //find smallest
                double min = Double.MaxValue;
                for (int im = 0; im < m; im++)
                {
                    if (dDistances[im] < min) min = dDistances[im];
                }
                dDist = min;

                double sat;
                int GB;

                // a parameter could be used instead of plain 3
                if (dDist < 0 || dDist > 3)
                {
                    sat = 0;
                    GB = 255; //make us white :)
                }
                else
                {
                    sat = (1 - (dDist / 3));
                    GB = (int)(dDist * 255.0/3.0);
                    //Console.WriteLine("{0}",GB);
                }


                //convert to packed RGB color....how?
                int packedColor;
                int col = System.Drawing.Color.FromArgb(255, GB, GB).ToArgb();

                //bit correction needed to move from 0xAARRGGBB -> 0xRRGGBBAA
                col = (col << 8) | 0x000000FF;
                packedColor = col;

                colors[i] = packedColor;
            }


            return colors;
        }

        private void loadTransforms()
        {
            int numPos = _wrist.motionFiles.Length;
            _transformMatrices = new TransformMatrix[numPos][];

            for (int i = 0; i < numPos; i++)
            {
                _transformMatrices[i] = new TransformMatrix[Wrist.NumBones];
                //TODO: create DatParser.parseMotionFileToTM()
                TransformRT[] tfm = DatParser.parseMotionFileToRT(_wrist.motionFiles[i]);
                for (int j = 0; j < Wrist.NumBones; j++)
                {
                    _transformMatrices[i][j] = new TransformMatrix(tfm[j]);
                }
            }
        }

        public bool AnimatePositionTransitions
        {
            get { return _animatePositionChanges; }
            set { _animatePositionChanges = value; }
        }

        public int FPS
        {
            get { return _FPS; }
            set { _FPS = value; }
        }

        public double AnimationDuration
        {
            get { return _animateDuration; }
            set { _animateDuration = value; }
        }

        public bool ShowErrors
        {
            get { return _showErrors; }
            set { _showErrors = value; }
        }

        public override Control Control
        {
            get { return _wristControl; }
        }

        public override Separator Root
        {
            get { return _root; }
        }

        private void populateSeriesList()
        {
            _currentPositionIndex = 0;
            _wristControl.clearSeriesList();
            _wristControl.addToSeriesList(_wrist.neutralSeries);
            _wristControl.addToSeriesList(_wrist.series);
            _wristControl.selectedSeriesIndex = 0;
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
            _wristControl = new FullWristControl();
            _wristControl.setupControl(Wrist.LongBoneNames, true);
        }

        private void setupControlEventListeners()
        {
            _wristControl.BoneHideChanged += new BoneHideChangedHandler(_control_BoneHideChanged);
            _wristControl.FixedBoneChanged += new FixedBoneChangedHandler(_control_FixedBoneChanged);
            _wristControl.SelectedSeriesChanged += new SelectedSeriesChangedHandler(_control_SelectedSeriesChanged);
        }

        private void removeControlEventListeners()
        {
            _wristControl.BoneHideChanged -= new BoneHideChangedHandler(_control_BoneHideChanged);
            _wristControl.FixedBoneChanged -= new FixedBoneChangedHandler(_control_FixedBoneChanged);
            _wristControl.SelectedSeriesChanged -= new SelectedSeriesChangedHandler(_control_SelectedSeriesChanged);
        }

        private void removeCurrentTransforms()
        {
            if (_root.hasTransform())
                _root.removeTransform();
            for (int i = 0; i < _bones.Length; i++)
            {
                //skip missing bones & remove the old
                if (_bones[i] != null && _bones[i].hasTransform())
                    _bones[i].removeTransform();

                //remove saved relativeMotion
                _tmCurrentPositionRelativeMotion[i] = null;
            }
        }

        private TransformMatrix[] calculateRelativeMotionFromNeutral(int positionIndex, int fixedBoneIndex)
        {
            TransformMatrix[] relMotions = new TransformMatrix[Wrist.NumBones];

            //check if neutral, if so, do something special....fuck?
            if (positionIndex == 0)
                return relMotions;  //okay, for now return null for all....

            TransformMatrix tmFixedBone = _transformMatrices[positionIndex - 1][fixedBoneIndex];
            for (int i = 0; i < _bones.Length; i++)
            {
                //skip missing bones
                if (_bones[i] == null) 
                    continue;

                if (i == fixedBoneIndex)
                {
                    relMotions[i] = null; //if we are the fixed bone, we don't move, so make us null
                }

                TransformMatrix tmCurrentBone = _transformMatrices[positionIndex - 1][i];
                relMotions[i] = tmFixedBone.Inverse() * tmCurrentBone;
            }
            return relMotions;
        }

        private void animateChangeBlahBlahBlah()
        {
            //setup animations....
            int numFrames = Math.Max((int)(_FPS * _animateDuration), 1); //want at least one frame

            _animationController = new AnimationController();

            HelicalTransform[][] transforms = new HelicalTransform[_bones.Length][];

            //load the transform for the fixed bone of the current index
            TransformMatrix tmFixedBone;
            if (_currentPositionIndex == 0)
                tmFixedBone = new TransformMatrix();
            else
                tmFixedBone = _transformMatrices[_currentPositionIndex - 1][_fixedBoneIndex];

            //save a copy of the lastRElativeMotions
            TransformMatrix[] savedLastPositionRelMotion = _tmCurrentPositionRelativeMotion;
            _tmCurrentPositionRelativeMotion = new TransformMatrix[Wrist.NumBones];

            HelicalTransform[] htRelMotions = new HelicalTransform[_bones.Length];
            for (int i = 0; i < _bones.Length; i++)
            {
                //skip missing bones & the fixed bone, its not moving
                if (_bones[i] == null || i == _fixedBoneIndex) continue;

                //calculate the relative motion for this new position and save it
                TransformMatrix tmCurrentBone = _transformMatrices[_currentPositionIndex - 1][i];
                TransformMatrix tmRelMotion = tmFixedBone.Inverse() * tmCurrentBone;
                _tmCurrentPositionRelativeMotion[i] = tmRelMotion; //save for future

                //now lets load the relative motion for the last position, so we can move gracefully from it :)
                TransformMatrix tmLastRelMotion = savedLastPositionRelMotion[i];
                if (tmLastRelMotion == null)
                    tmLastRelMotion = new TransformMatrix(); //set to identify if we are missing one
                TransformMatrix rmRelMotionLastPosition = tmLastRelMotion.Inverse() * tmRelMotion;

                if (rmRelMotionLastPosition.isIdentity(0.01))  //dirty check to see if we are getting the identity matrix....yar
                    Console.WriteLine("SHIT");
                htRelMotions[i] = rmRelMotionLastPosition.ToHelical();

                //if there is no kinematics for the current Bone, then hide it :)
                if (tmCurrentBone.isIdentity())
                    _wristControl.hideBone(i); //send to the control, so the GUI gets updated, it will call back to the controller to actually hide the bone :)
            }
            _animationController.setupAnimationForLinearInterpolation(_bones, htRelMotions, savedLastPositionRelMotion, numFrames);
            _animationController.LoopAnimation = false;
            _animationController.FPS = _FPS;
            _animationController.Start();
        }

        private void hideBonesWithNoKinematicsForPosition(int positionIndex)
        {
            if (positionIndex==0)
                return; //no bone can be missing in neutral

            for (int i=0; i<_bones.Length; i++) {
            //only check if the bone exists
            if (_bones[i]==null)
                continue;

                if (_transformMatrices[positionIndex-1][i].isIdentity())
                    _wristControl.hideBone(i); //send to the control, so the GUI gets updated, it will call back to the controller to actually hide the bone :)
            }
        }

        private void setTransformsForCurrentPositionAndFixedBone()
        {
            if (_animatePositionChanges)
            {
                animateChangeBlahBlahBlah();
                return;
            }

            //first remove the old transforms, if they exist
            removeCurrentTransforms();

            //check if neutral, if so, we are done
            if (_currentPositionIndex == 0)
                return;

            //save to current positions, then apply
            _tmCurrentPositionRelativeMotion = calculateRelativeMotionFromNeutral(_currentPositionIndex, _fixedBoneIndex);
            for (int i = 0; i < _bones.Length; i++)
            {
                _bones[i].addTransform(_tmCurrentPositionRelativeMotion[i].ToTransform());
            }
            hideBonesWithNoKinematicsForPosition(_currentPositionIndex);
            

            //TransformMatrix tmFixedBone = _transformMatrices[_currentPositionIndex - 1][_fixedBoneIndex];
            //for (int i = 0; i < _bones.Length; i++)
            //{
            //    //skip missing bones
            //    if (_bones[i] == null || i == _fixedBoneIndex) continue;

            //    TransformMatrix tmCurrentBone = _transformMatrices[_currentPositionIndex - 1][i];
            //    TransformMatrix tmRelMotion = tmFixedBone.Inverse() * tmCurrentBone;
            //    _bones[i].addTransform(tmRelMotion.ToTransform());

            //    _tmCurrentPositionRelativeMotion[i] = tmRelMotion; //save for later...?

            //    if (tmCurrentBone.isIdentity())
            //        _wristControl.hideBone(i); //send to the control, so the GUI gets updated, it will call back to the controller to actually hide the bone :)
            //}
        }

        void _control_SelectedSeriesChanged(object sender, SelectedSeriesChangedEventArgs e)
        {
            if (_currentPositionIndex == e.SelectedIndex)
                return;

            //save current position
            _lastPositionIndex = _currentPositionIndex;
            _currentPositionIndex = e.SelectedIndex;

            setTransformsForCurrentPositionAndFixedBone();
        }

        void _control_FixedBoneChanged(object sender, FixedBoneChangeEventArgs e)
        {
            _lastFixedBoneIndex = _fixedBoneIndex;
            _fixedBoneIndex = e.BoneIndex;

            setTransformsForCurrentPositionAndFixedBone();
        }

        void _control_BoneHideChanged(object sender, BoneHideChangeEventArgs e)
        {
            _colorBones[e.BoneIndex].setHidden(e.BoneHidden);
        }

        public void setInertiaVisibility(bool visible)
        {
            if (visible)
            {
                try
                {
                    //If its checked, then we need to add it
                    TransformRT[] inert = DatParser.parseInertiaFileToRT(_wrist.inertiaFile);
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
                    TransformRT[] inert = DatParser.parseACSFileToRT(_wrist.acsFile);

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
