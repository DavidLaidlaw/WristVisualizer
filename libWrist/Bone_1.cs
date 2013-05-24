using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using libCoin3D;

namespace libWrist
{
    public class Bone
    {
        private string _ivFilename;
        private string _distanceFieldFilename;

        private string _longName;
        private string _shortName;
        private int _boneIndex;

        private bool _hamVisible; //for state reasons
        private bool _isManuallyHidden; 

        //Data objects
        private TransformMatrix _inertiaMatrix;
        private TransformMatrix[] _transformMatrices;

        //DistanceField Data objects
        private CTmri _distanceField;
        private double[][] _computedDistances;
        private int[][] _computedColorMaps;
        private Contour[] _computedContours;

        private double[][] _animationComputedDistances;
        private int[][] _animationComputedColorMaps;
        private Contour[] _animationComputedContours;

        //Coin3D objects
        private Separator _bone;
        private ColoredBone _coloredBone;
        private Separator _inertiaSeparator;
        private Switch _animationSwitch;
        private Switch _animationHamSwitch;

        private Material _boneMaterial;

        public Bone(string ivFilename, string distanceFieldFilename, int boneIndex) : this(ivFilename, distanceFieldFilename, boneIndex, 0) { }
        public Bone(string ivFilename, string distanceFieldFilename, int boneIndex, int numSeries)
        {
            _boneIndex = boneIndex;
            _longName = WristFilesystem.LongBoneNames[boneIndex];
            _shortName = WristFilesystem.ShortBoneNames[boneIndex];
            _ivFilename = ivFilename;
            _distanceFieldFilename = distanceFieldFilename;
            _hamVisible = false;
            _isManuallyHidden = false;

            InitializeDataStructures(numSeries);
        }

        public void InitializeDataStructures(int numSeries)
        {
            _transformMatrices = new TransformMatrix[numSeries];
            if (numSeries > 0)
                _transformMatrices[0] = new TransformMatrix(); //set the neutral position to the identity matrix :)
            _computedDistances = new double[numSeries][];
            _computedColorMaps = new int[numSeries][];
            _computedContours = new Contour[numSeries];
        }

        public void LoadIVFile()
        {
            if (!File.Exists(_ivFilename))
                return;

            Separator bone = new Separator();
            bone.makeHideable();
            try
            {
                _coloredBone = new ColoredBone(_ivFilename);
                bone.addNode(_coloredBone);
            }
            catch (System.ArgumentException)
            {
                //try and load non-standard bones here.... shit. This needs to be fixed
                //TODO: Better error handling...
                _coloredBone = null;
                bone.addFile(_ivFilename);
            }
            _bone = bone; //only save if we get this far, its possible to still be throwing an exception
        }


        public void TryReadDistanceField()
        {
            if (!Directory.Exists(_distanceFieldFilename))
                return;

            _distanceField = new CTmri(_distanceFieldFilename);
            _distanceField.loadImageData();
        }

        public TransformMatrix InertiaMatrix
        {
            get { return _inertiaMatrix; }
            set { _inertiaMatrix = value; }
        }

        public bool HasInertia
        {
            get
            {
                return _inertiaMatrix != null && _inertiaMatrix.Determinant() != 0;
            }
        }

        public TransformMatrix[] TransformMatrices
        {
            get { return _transformMatrices; }
        }

        public string IvFilename
        {
            get { return _ivFilename; }
        }
        
        public string LongName
        {
            get { return _longName; }
        }

        public string ShortName
        {
            get { return _shortName; }
        }

        public int BoneIndex
        {
            get { return _boneIndex; }
        }

        public Separator BoneSeparator
        {
            get { return _bone; }
        }

        public CTmri DistanceField
        {
            get { return _distanceField; }
        }

        public bool HasDistanceField
        {
            get { return _distanceField != null; }
        }

        public bool IsValidBone
        {
            get { return _bone != null; }
        }

        public bool IsColoredBone
        {
            get { return _coloredBone != null; }
        }

        public void SetTransformation(TransformMatrix[] transforms)
        {
            // Do I need to add more error checking to ensure that the length is correct?
            _transformMatrices = transforms;
        }

        public void SetTransformation(TransformMatrix transform, int positionIndex)
        {
            _transformMatrices[positionIndex] = transform;
        }

        public float[,] GetVertices()
        {
            if (_coloredBone == null)
                throw new WristException("Cannot get vertices from non-colored bone");

            return _coloredBone.getVertices();
        }

        public int[,] GetFaceSetIndices()
        {
            if (_coloredBone == null)
                throw new WristException("Cannot get FaceSetIndices from non-colored bone");

            return _coloredBone.getFaceSetIndices();
        }

        //public void HideBone()
        //{
        //    _isManuallyHidden = true;
        //    if (_coloredBone != null)
        //        _coloredBone.setVisibility(false);
        //    else
        //        _bone.SetVisibility(false);
        //}

        //public void ShowBone()
        //{
        //    _isManuallyHidden = false;
        //    if (_coloredBone != null)
        //        _coloredBone.setVisibility(true);
        //    else
        //        _bone.SetVisibility(true);
        //}

        private void RestoreBoneVisibility()
        {
            SetBoneVisibility(!_isManuallyHidden);
        }

        public void SetBoneVisibilityManually(bool visible)
        {
            _isManuallyHidden = !visible;
            SetBoneVisibility(visible);
        }

        private void SetBoneVisibility(bool visible)
        {
            if (_coloredBone != null)
                _coloredBone.setVisibility(visible);
            else
                _bone.SetVisibility(visible);
        }

        public void HideInertia()
        {
            //first check if we are visible. if not, we are done
            if (_inertiaSeparator == null) return;

            _bone.removeChild(_inertiaSeparator);
        }

        public void ShowInertia()
        {
            //if there is no inertia information, do nothing
            if (!this.HasInertia) return;

            //if it does not exist, generate it
            if (_inertiaSeparator == null)
                GenerateInertiaSeparator();

            _bone.addChild(_inertiaSeparator); //TODO: check if already there!
        }

        public void SetInertiaVisibility(bool visible)
        {
            if (visible)
                ShowInertia();
            else
                HideInertia();
        }

        public System.Drawing.Color GetColor()
        {
            if (_boneMaterial != null)
                return _boneMaterial.getColor();
            else
                return Material.GetDefaultColor();
        }

        public void SetColor(System.Drawing.Color color)
        {
            if (_boneMaterial == null)
            {
                _boneMaterial = new Material();
                _bone.insertNode(_boneMaterial, 0);
            }
            _boneMaterial.setColor(color);
        }

        private void GenerateInertiaSeparator() { GenerateInertiaSeparator(0); }
        private void GenerateInertiaSeparator(int arrowLength)
        {
            //if there is no inertia information, do nothing
            if (!this.HasInertia) return;

            Separator inert = new Separator();
            Transform t = _inertiaMatrix.ToTransform();
            if (arrowLength == 0)
                inert.addNode(new ACS());
            else
                inert.addNode(new ACS(45));
            inert.addTransform(t);
            _inertiaSeparator = inert;
        }

        public bool HasKinematicInformationForPosition(int positionIndex)
        {
            if (positionIndex == 0) return true; //always have for neutral... duh
            if (_transformMatrices.Length <= positionIndex) return false;
            if (_transformMatrices[positionIndex] == null) return false;
            if (_transformMatrices[positionIndex].isIdentity()) return false; //ugly, but we assume identity matrix is missing data
            return true; //got here, than we are good
        }


        public TransformMatrix CalculateRelativeMotionFromNeutral(int positionIndex, Bone fixedBone)
        {
            //check for a neutral position
            if (positionIndex == 0)
                return new TransformMatrix(); //no motion in neutral

            //check for the fixed bone being us :)
            if (this == fixedBone)
            {
                //TODO: What do I do in this case...?
                return new TransformMatrix();
            }

            TransformMatrix tmFixedBoneInverse = fixedBone._transformMatrices[positionIndex].Inverse();
            return tmFixedBoneInverse * _transformMatrices[positionIndex];
        }

        public TransformMatrix CalculateRelativeMotion(int startPositionIndex, int endPositionIndex, Bone fixedBone)
        {
            TransformMatrix startRelTransform = this.CalculateRelativeMotionFromNeutral(startPositionIndex, fixedBone);
            TransformMatrix endRelTransform = this.CalculateRelativeMotionFromNeutral(endPositionIndex, fixedBone);

            return endRelTransform * startRelTransform.Inverse();
        }

        public TransformMatrix[] CalculateInterpolatedMotion(int startPositionIndex, int endPositionIndex, Bone fixedBone, int numSteps)
        {
            return CalculateInterpolatedMotion(startPositionIndex, endPositionIndex, fixedBone, fixedBone, numSteps);
        }
        public TransformMatrix[] CalculateInterpolatedMotion(int startPositionIndex, int endPositionIndex, Bone startFixedBone, Bone endFixedBone, int numSteps)
        {
            TransformMatrix[] finalTransforms = new TransformMatrix[numSteps];
            TransformMatrix startTransform = this.CalculateRelativeMotionFromNeutral(startPositionIndex, startFixedBone);
            TransformMatrix endTransform = this.CalculateRelativeMotionFromNeutral(endPositionIndex, endFixedBone);
            
            TransformMatrix relMotion = endTransform * startTransform.Inverse();
            HelicalTransform relMotionHT = relMotion.ToHelical();

            HelicalTransform[] htTransforms = relMotionHT.LinearlyInterpolateMotion(numSteps);

            for (int i = 0; i < numSteps; i++)
                finalTransforms[i] = htTransforms[i].ToTransformMatrix() * startTransform;

            return finalTransforms;
        }

        public void SetToPosition(int positionIndex, Bone fixedBone)
        {
            //first check if we have kinematics for ourselves and the fixedBone in the
            //specified position, if not; we will hide ourselves but not change our position
            if (!HasKinematicInformationForPosition(positionIndex) ||
                !fixedBone.HasKinematicInformationForPosition(positionIndex))
            {
                //we have no kinematic data, so hide and get out
                SetBoneVisibility(false);
                return;
            }

            //Calculate the correct transform and apply it
            // This relative motion calculation will correctly return an Identity matrix 
            // if there is no motion, which will be handled correctly later
            SetToPosition(CalculateRelativeMotionFromNeutral(positionIndex, fixedBone));
            RestoreBoneVisibility(); //make sure we are not hidden if we souldn't be
        }

        public void SetToPosition(TransformMatrix tm)
        {
            if (_bone.hasTransform())
                _bone.removeTransform();

            if (tm.isIdentity()) 
                return; //nothing to do in this case
            else
                _bone.addTransform(tm.ToTransform());
        }

        public void RemoveContour()
        {
            if (_coloredBone != null)
                _coloredBone.removeContour();
        }

        public void SetContour(Contour contour)
        {
            if (_coloredBone != null)
                _coloredBone.setAndReplaceContour(contour);
        }

        public void SetContourForPositionIfCalculated(int positionIndex)
        {
            if (_computedContours[positionIndex] != null)
                SetContour(_computedContours[positionIndex]);
            else
                RemoveContour();
        }

        public bool HasContourForPosition(int positionIndex)
        {
            return (_computedContours[positionIndex] != null);
        }

        public Contour GetContourForPosition(int positionIndex)
        {
            return _computedContours[positionIndex];
        }

        public void RemoveColorMap()
        {
            if (_coloredBone != null)
                _coloredBone.clearColorMap();
        }

        public void SetColorMap(int[] colormap)
        {
            if (_coloredBone != null)
                _coloredBone.setColorMap(colormap);
        }

        public void SetColorMapForPositionIfCalculated(int positionIndex)
        {
            if (_computedColorMaps[positionIndex] != null)
                SetColorMap(_computedColorMaps[positionIndex]);
            else
                RemoveColorMap();
        }

        public bool HasColorMapForPosition(int positionIndex)
        {
            return (_computedColorMaps[positionIndex] != null);
        }

        public bool HasVertexDistancesForPosition(int positionIndex)
        {
            return (_computedDistances[positionIndex] != null);
        }

        public void CalculateAndSaveDistanceMapForPosition(int positionIndex, Bone[] testBones)
        {
            //quick check that we can do this. Don't need our own distance field, just the testBones'
            if (_coloredBone == null) return;

            //calculate the relative motion for each bone. Need a transform for each testBone, that will
            //move this bone into its coordinate system
            TransformMatrix[] transforms = new TransformMatrix[testBones.Length];
            if (positionIndex != 0) //in the neutral position, we are in the correct place
            {
                for (int i = 0; i < testBones.Length; i++)
                    transforms[i] = CalculateRelativeMotionFromNeutral(positionIndex, testBones[i]);
            }

            double[] distances = DistanceMaps.createDistanceMap(this, testBones, transforms);
            lock (_computedDistances)
            {
                _computedDistances[positionIndex] = distances;
            }
        }

        public void CalculateAndSaveDistanceMapForAnimation(int[] animationOrder, int numFramesPerStep, int absoluteFrameNumber, Bone[] testBones, Bone fixedBone)
        {
            //quick check that we can do this. Don't need our own distance field, just the testBones'
            if (_coloredBone == null) return;

            //need to calculate where we are in the whole animation scheme. What two positions are we between?
            int startPositionIndex = absoluteFrameNumber / numFramesPerStep;
            int partialFrame = absoluteFrameNumber % numFramesPerStep;
            int startPosition = animationOrder[startPositionIndex];

            //calculate the relative motion for each bone. Need a transform for each testBone, that will
            //move this bone into its coordinate system
            TransformMatrix[] transforms = new TransformMatrix[testBones.Length];
            if (partialFrame == 0) //no partial frame, we are exactly where we want to be
            {
                if (startPosition != 0) //if we are at absolute neutral, no calculations needed
                {
                    //check for the non
                    for (int i = 0; i < testBones.Length; i++)
                        transforms[i] = CalculateRelativeMotionFromNeutral(startPosition, testBones[i]);
                }
            }
            else
            {
                //so we have a partial motion to deal with, first calculate the position of the current bone
                TransformMatrix tmCurrentBone = CalculateInterpolatedMotion(startPosition, animationOrder[startPositionIndex + 1], fixedBone, numFramesPerStep)[partialFrame];
                for (int i = 0; i < testBones.Length; i++)
                {
                    TransformMatrix tmTestBone = testBones[i].CalculateInterpolatedMotion(startPosition, animationOrder[startPositionIndex + 1], fixedBone, numFramesPerStep)[partialFrame];
                    transforms[i] = tmTestBone.Inverse() * tmCurrentBone;
                }
            }

            double[] distances = DistanceMaps.createDistanceMap(this, testBones, transforms);
            lock (this)
            {
                if (_animationComputedDistances == null)
                    _animationComputedDistances = new double[(animationOrder.Length-1) * numFramesPerStep + 1][];
                _animationComputedDistances[absoluteFrameNumber] = distances;
            }
        }

        public void CalculateAndSaveColorDistanceMapForPosition(int positionIndex, double maxColorDistance)
        {
            if (_computedDistances[positionIndex] == null)
                throw new WristException("Raw distances (_computedDistancs) must be pre-computed before calculating ColorDistanceMap");

            int[] colorMap = DistanceMaps.createColormap(_computedDistances[positionIndex], maxColorDistance);
            lock (_computedColorMaps)
            {
                _computedColorMaps[positionIndex] = colorMap;
            }
        }

        public void CalculateAndSaveColorDistanceMapForAnimation(int absoluteFrameNumber, double maxColorDistance)
        {
            if (_animationComputedDistances[absoluteFrameNumber] == null)
                throw new WristException("Raw distances (_animationComputedDistances) must be pre-computed before calculating ColorDistanceMap");

            int[] colorMap = DistanceMaps.createColormap(_animationComputedDistances[absoluteFrameNumber], maxColorDistance);
            lock (this)
            {
                if (_animationComputedColorMaps == null)
                    _animationComputedColorMaps = new int[_animationComputedDistances.Length][];
                _animationComputedColorMaps[absoluteFrameNumber] = colorMap;
            }
        }

        public void CalculateAndSaveContourForPosition(int positionIndex, double[] cDistances, System.Drawing.Color[] colors)
        {
            if (_computedDistances[positionIndex] == null)
                throw new WristException("Raw distances (_computedDistancs) must be pre-computed before calculating Contour");

            Contour contour = DistanceMaps.createContourSingleBoneSinglePosition(this, _computedDistances[positionIndex], cDistances, colors);
            lock (_computedContours)
            {
                _computedContours[positionIndex] = contour;
            }
        }

        public Contour[] CalculateContourForPositionTargetingAreas(int positionIndex, double[] targetArea, double tolerance, int maxNumIterations)
        {
            if (_computedDistances[positionIndex] == null)
                throw new WristException("Raw distances (_computedDistancs) must be pre-computed before calculating Contour");
            Contour[] c = new Contour[targetArea.Length];
            for (int i = 0; i < targetArea.Length; i++)
                c[i] = DistanceMaps.CreateContourSingleBoneSinglePositionTargetingArea(this, _computedDistances[positionIndex], targetArea[i], tolerance, maxNumIterations);
            return c;
        }

        public void CalculateAndSaveContourForAnimation(int absoluteFrameNumber, double[] cDistances, System.Drawing.Color[] colors)
        {
            if (_animationComputedDistances[absoluteFrameNumber] == null)
                throw new WristException("Raw distances (_computedDistancs) must be pre-computed before calculating Contour");

            Contour contour = DistanceMaps.createContourSingleBoneSinglePosition(this, _animationComputedDistances[absoluteFrameNumber], cDistances, colors);
            lock (this)
            {
                if (_animationComputedContours == null)
                    _animationComputedContours = new Contour[_animationComputedDistances.Length];
                _animationComputedContours[absoluteFrameNumber] = contour;
            }
        }

        public void ClearCachedContours()
        {
            for (int i = 0; i < _computedContours.Length; i++)
                _computedContours[i] = null;
        }

        public void ClearCachedColorMaps()
        {
            for (int i = 0; i < _computedColorMaps.Length; i++)
                _computedColorMaps[i] = null;
        }

        public void SetAnimationFrame(int frameNumber)
        {
            if (_animationSwitch != null)
                _animationSwitch.whichChild(frameNumber);

            if (_animationHamSwitch != null)
                _animationHamSwitch.whichChild(frameNumber);

            if (_animationComputedColorMaps != null)
                _coloredBone.setColorMap(_animationComputedColorMaps[frameNumber]);

            if (_animationComputedContours != null)
                _coloredBone.setAndReplaceContour(_animationComputedContours[frameNumber]);
        }

        public void SetupForAnimation(Switch animationSwitch, Switch animationHamSwitch)
        {
            //first remove the old animation switch if it exists
            RemoveAnimationSwitches();

            //now add it, if its not null
            _animationSwitch = animationSwitch;
            _animationHamSwitch = animationHamSwitch;
            if (animationSwitch != null)
                _bone.insertNode(_animationSwitch, 0);

            if (animationHamSwitch != null)
                _animationHamSwitch.reference(); //don't actually insert it, just reference it, so its ready
            
            //if ham is supposed to be visible, add it to the scene
            if (_hamVisible)
                _bone.addNode(_animationHamSwitch);
        }

        public void SetHamVisibility(bool visible)
        {
            if (_hamVisible == visible) return; //no change
            if (_animationHamSwitch == null) return; //nothing todo
            _hamVisible = visible;
            if (visible)
                _bone.addNode(_animationHamSwitch);
            else
                _bone.removeChild(_animationHamSwitch);
        }

        public void EndAnimation()
        {
            RemoveAnimationSwitches();
            _hamVisible = false;
            _animationComputedDistances = null;
            _animationComputedColorMaps = null;
            _animationComputedContours = null;
        }

        private void RemoveAnimationSwitches()
        {
            if (_animationSwitch != null)
                _bone.removeChild(_animationSwitch);
            if (_animationHamSwitch != null)
            {
                if (_hamVisible)
                    _bone.removeChild(_animationHamSwitch);
                _animationHamSwitch.unref(); //need to unref the ham switch
            }

            _animationHamSwitch = null;
            _animationSwitch = null;
        }
    }
}
