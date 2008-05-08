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

        //Distance Fields
        private CTmri[] _distanceFields;
        private int[][][] _calculatedDistanceMaps;

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
        }

        private void readInDistanceFieldsIfNotLoaded()
        {
            if (_distanceFields != null)
                return;

            _distanceFields = new CTmri[Wrist.NumBones];

            for (int i = 0; i < Wrist.NumBones; i++)
            {
                string basefolder = Path.Combine(Path.Combine(_wrist.subjectPath, _wrist.neutralSeries), "DistanceFields");
                string folder = String.Format("{0}{1}_mri", Wrist.ShortBoneNames[i], _wrist.neutralSeries.Substring(1, 3));
                if (Directory.Exists(Path.Combine(basefolder, folder)))
                {
                    _distanceFields[i] = new CTmri(Path.Combine(basefolder, folder));
                    _distanceFields[i].loadImageData();
                }
                else
                    _distanceFields[i] = null;
            }
        }

        public void loadDistanceMapsForCurrentPosition()
        {
            loadDistanceMapsForPosition(_currentPositionIndex);
        }

        private void loadDistanceMapsForPosition(int positionIndex)
        {
            //setup save space if it doesn't exist
            if (_calculatedDistanceMaps == null)
                _calculatedDistanceMaps = new int[Wrist.NumBones][][];

            DateTime t1 = DateTime.Now;
            readInDistanceFieldsIfNotLoaded();
            Console.WriteLine("Time to read MRI: {0}", ((TimeSpan)(DateTime.Now - t1)));
            t1 = DateTime.Now;

            //try and create color scheme....
            for (int i = 0; i < Wrist.NumBones; i++)
            {
                //setup space if it doesn't exist
                if (_calculatedDistanceMaps[i] == null)
                    _calculatedDistanceMaps[i] = new int[_transformMatrices.Length + 1][]; //add one extra for neutral :)

                //read in the colors if not yet loaded
                if (_calculatedDistanceMaps[i][positionIndex] == null)
                    _calculatedDistanceMaps[i][positionIndex] = createColormap(_distanceFields, i, positionIndex);
                Console.WriteLine("Created colormap {0}: {1}", i, ((TimeSpan)(DateTime.Now - t1)));

                //now set that color
                _colorBones[i].setColorMap(_calculatedDistanceMaps[i][positionIndex]);
                Console.WriteLine("Applied colormap {0}: {1}", i, ((TimeSpan)(DateTime.Now - t1)));
                t1 = DateTime.Now;
            }
        }

        private bool hasDistanceMapsForPosition(int positionIndex)
        {
            if (_calculatedDistanceMaps == null)
                return false;

            //only check the radius, it should be a good enough check....
            if (_calculatedDistanceMaps[0] == null)
                return false;

            return (_calculatedDistanceMaps[0][positionIndex] != null);
        }

        private void clearDistanceMapsForAllBones()
        {
            for (int i = 0; i < Wrist.NumBones; i++)
            {
                if (_colorBones[i] != null)
                    _colorBones[i].clearColorMap();
            }
        }

        private TransformMatrix[] calculateRelativeMotionForDistanceMaps(int boneIndex, int positionIndex, int[] boneInteraction)
        {
            TransformMatrix[] tmRelMotions = new TransformMatrix[Wrist.NumBones]; //for each position
            if (positionIndex == 0) //no transforms needed for the neutral position, we are all set :)
                return tmRelMotions;

            /* Check if we are missing kinematics for the bone, if so, then we can not
             * calculate distance maps (we don't know where the bone is, so we just return all null)
             */
            if (_transformMatrices[positionIndex - 1][boneIndex] == null ||
                _transformMatrices[positionIndex - 1][boneIndex].isIdentity())
                return tmRelMotions;

            TransformMatrix tmBone = _transformMatrices[positionIndex - 1][boneIndex];
            foreach (int testBoneIndex in boneInteraction)
            {
                //Again, check if there is no kinematics for the test bone, again, if none, just move on
                if (_transformMatrices[positionIndex - 1][testBoneIndex] == null ||
                _transformMatrices[positionIndex - 1][testBoneIndex].isIdentity())
                    continue;

                TransformMatrix tmFixedBone = _transformMatrices[positionIndex - 1][testBoneIndex];
                //so fix the current bone, and move our test bone to that position....yes?
                tmRelMotions[testBoneIndex] = tmFixedBone.Inverse() * tmBone;
            }
            return tmRelMotions;

        }

        private int[] createColormap(CTmri[] mri, int boneIndex) { return createColormap(mri, boneIndex, 0); }
        private int[] createColormap(CTmri[] mri, int boneIndex, int positionIndex)
        {
            double dDist = Double.MaxValue;
            DateTime t2 = DateTime.Now;
            float[,] pts = _colorBones[boneIndex].getVertices();
            Console.WriteLine("Getting vertices {0}: {1}", boneIndex, ((TimeSpan)(DateTime.Now - t2)));
            int numVertices = pts.GetLength(0);
            double[] dDistances = new double[Wrist.NumBones - 1];

            int[] colors = new int[numVertices];
            int[] interaction = Wrist.BoneInteractionIndex[boneIndex]; //load  bone interactions

            TransformMatrix[] tmRelMotions = calculateRelativeMotionForDistanceMaps(boneIndex, positionIndex, interaction);

            //for each vertex           
            for (int i = 0; i < numVertices; i++)
            {
                int m = 0;
                //for (int j = 0; j < Wrist.NumBones; j++)
                foreach(int j in interaction) //only use the bones that we have specified interact
                {
                    if (j == boneIndex) continue;
                    if (mri[j] == null) continue; //skip missing scans

                    double x = pts[i, 0];
                    double y = pts[i, 1];
                    double z = pts[i, 2];

                    //check if we need to move for non neutral position
                    if (positionIndex != 0)
                    {
                        //skip missing kinematic info
                        if (tmRelMotions[j] == null)
                            continue;

                        //lets move the bone getting colored, into the space of the other bone...
                        double[] p0 = new double[] { x, y, z };
                        double[] p1 = tmRelMotions[j] * p0;
                        x = p1[0];
                        y = p1[1];
                        z = p1[2];
                    }

                    double dX = (x - mri[j].CoordinateOffset[0]) / mri[j].voxelSizeX;
                    double dY = (y - mri[j].CoordinateOffset[1]) / mri[j].voxelSizeY;
                    double dZ = (z - mri[j].CoordinateOffset[2]) / mri[j].voxelSizeZ;

                    const double xBound = 96.9; //get the boundaries of the distance cube
                    const double yBound = 96.9; //
                    const double zBound = 96.9; //

                    ////////////////////////////////////////////////////////
                    //is surface point picked inside of the cube?

                    if (dX >= 3.1 && dX <= xBound && dY >= 3.1
                        && dY <= yBound && dZ >= 3.1 && dZ <= zBound)
                    {
                        dDist = mri[j].sample_s_InterpCubit(dX, dY, dZ);
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

                //double sat;
                uint GB;

                // a parameter could be used instead of plain 3
                if (dDist < 0 || dDist > 3)
                {
                    //sat = 0;
                    GB = 255; //make us white :)
                }
                else
                {
                    //sat = (1 - (dDist / 3));
                    GB = (uint)(dDist * 255.0/3.0);
                }


                /* convert to packed RGB color....how?
                 * packed color for Coin3D/inventor is 0xRRGGBBAA
                 * So take our GB values (should be from 0-255 or 8 bits), and move from
                 * Lest significant position (0x000000XX) to the G and B position, then
                 * combine with a bitwise OR. (0x00XX0000 | 0x0000XX00), which gives us
                 * the calculated value in both the G & B slots, and 0x00 in R & A.
                 * So we then ahve 0x00GGBB00, we can then bitwise OR with 0xFF0000FF, 
                 * since we want both R and Alpha to be at 255. Then we are set :)
                 */
                if (GB > 255 || GB < 0)
                    Console.WriteLine("lbha");

                int packedColor = (int)((GB << 16) | (GB << 8) | (uint)0xFF0000FF);
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

                //do we need to check if we are the fixed bone....?
                if (i == fixedBoneIndex)
                    continue;

                TransformMatrix tmCurrentBone = _transformMatrices[positionIndex - 1][i];
                relMotions[i] = tmFixedBone.Inverse() * tmCurrentBone;
            }
            return relMotions;
        }

        private void animateChangeInPosition()
        {
            //setup animations....
            if (_animationController != null)
                _animationController.Stop(); //ugly, but should work
            _animationController = new AnimationController();
            int numFrames = Math.Max((int)(_FPS * _animateDuration), 1); //want at least one frame

            HelicalTransform[] htRelMotions = new HelicalTransform[_bones.Length]; //rel motion from last to current

            TransformMatrix[] lastRelMotion = calculateRelativeMotionFromNeutral(_lastPositionIndex, _lastFixedBoneIndex); //rel motion from neutral to last
            TransformMatrix[] currentRelMotion = calculateRelativeMotionFromNeutral(_currentPositionIndex, _fixedBoneIndex); //rel motion from neutral to current

            //loop for each bone, and setup
            for (int i = 0; i < _bones.Length; i++)
            {
                //skip missing bones
                if (_bones[i] == null) continue;
                                
                //skip if no transforms at all, then the bone is not changing places...
                if (lastRelMotion[i] == null && currentRelMotion[i] == null)
                    continue;

                //if there is no transform for the current position, but there is a reverse
                if (currentRelMotion[i] == null)
                    currentRelMotion[i] = new TransformMatrix(); //set to identity....

                //if the last position was null, then just use the current position. This should happen when moving from neutral
                if (lastRelMotion[i] == null)
                {
                    htRelMotions[i] = currentRelMotion[i].ToHelical();
                }
                else
                {
                    //so we should have both transforms now... though one can be the identity.... hm....
                    TransformMatrix relLastToCurrent = currentRelMotion[i] * lastRelMotion[i].Inverse();
                    htRelMotions[i] = relLastToCurrent.ToHelical();
                }
            }

            //clear the coloring scheme, its not really calculated yet
            clearDistanceMapsForAllBones();

            _animationController.setupAnimationForLinearInterpolation(_bones, htRelMotions, lastRelMotion, numFrames);
            _animationController.LoopAnimation = false;
            _animationController.FPS = _FPS;
            _animationController.Start();
            //TODO: add color information back in at the end....how?
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
            hideBonesWithNoKinematicsForPosition(_currentPositionIndex);
            if (_animatePositionChanges)
            {
                animateChangeInPosition();
            }
            else
            {
                //load in the color maps, if they already exist
                if (hasDistanceMapsForPosition(_currentPositionIndex))
                    loadDistanceMapsForCurrentPosition();
                else
                    clearDistanceMapsForAllBones(); //we don't have color yet
                
                //first remove the old transforms, if they exist
                removeCurrentTransforms();

                //save to current positions, then apply
                TransformMatrix[] transforms = calculateRelativeMotionFromNeutral(_currentPositionIndex, _fixedBoneIndex);
                for (int i = 0; i < _bones.Length; i++)
                {
                    //skip missing bones, or bones without motion
                    if (_bones[i] == null || transforms[i] == null)
                        continue;

                    _bones[i].addTransform(transforms[i].ToTransform());
                }

            }

            //now that we are done, lets save the last positions
            _lastFixedBoneIndex = _fixedBoneIndex;
            _lastPositionIndex = _currentPositionIndex;
        }

        void _control_SelectedSeriesChanged(object sender, SelectedSeriesChangedEventArgs e)
        {
            if (_currentPositionIndex == e.SelectedIndex)
                return;

            _currentPositionIndex = e.SelectedIndex;

            setTransformsForCurrentPositionAndFixedBone();
        }

        void _control_FixedBoneChanged(object sender, FixedBoneChangeEventArgs e)
        {
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
