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
        private Transform[][] _transforms;
        private TransformMatrix[][] _transformMatrices;
        private int _currentPositionIndex;
        private int _fixedBoneIndex;

        private Separator _root;

        private FullWristControl _wristControl;

        public FullWristController()
        {
            setupControl();
            setupControlEventListeners();
            _root = new Separator();
            _bones = new Separator[Wrist.NumBones];
            _colorBones = new ColoredBone[Wrist.NumBones];
            _inertias = new Separator[Wrist.NumBones];
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
            _transforms = new Transform[numPos][];

            for (int i = 0; i < numPos; i++)
            {
                _transformMatrices[i] = new TransformMatrix[Wrist.NumBones];
                _transforms[i] = new Transform[Wrist.NumBones];
                TransformRT[] tfm = DatParser.parseMotionFileToRT(_wrist.motionFiles[i]);
                for (int j = 0; j < Wrist.NumBones; j++)
                {
                    //TODO: Clean up this mess....
                    //_transforms[i][j] = new Transform();
                    //DatParser.addRTtoTransform(tfm[j], _transforms[i][j]);
                    _transformMatrices[i][j] = new TransformMatrix(tfm[j]);
                    _transforms[i][j] = _transformMatrices[i][j].ToTransform();
                }
            }
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
                DatParser.addRTtoTransform(DatParser.parseMotionFileToRT(_wrist.getMotionFilePath(_currentPositionIndex - 1))[_fixedBoneIndex], t);
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
                        _wristControl.hideBone(i);
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
            DatParser.addRTtoTransform(DatParser.parseMotionFileToRT(_wrist.getMotionFilePath(_currentPositionIndex - 1))[e.BoneIndex], t);
            t.invert();
            //_root.addTransform(_transforms[_currentPositionIndex-1][0]); //minus 1 to skip neutral
            _root.addTransform(t);
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
