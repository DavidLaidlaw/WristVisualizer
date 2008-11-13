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
        private Wrist _wrist;
        private FullWrist _fullWrist; //the full wrist we are part of

        //Data objects
        private TransformMatrix _inertiaMatrix;
        private TransformMatrix[] _transformMatrices;
        private CTmri _distanceField;
        private double[][] _computedDistances;
        private int[][] _computedColorMaps;
        private Contour[] _computedContours;

        private float[,] _cachedVertices;
        private int[,] _cachedFaceSetIndices;


        //Coin3D objects
        private Separator _bone;
        private ColoredBone _coloredBone;
        private Separator _inertiaSeparator;

        public Bone(Wrist wrist, FullWrist fullwrist, int boneIndex)
        {
            _fullWrist = fullwrist;
            _wrist = wrist;
            _boneIndex = boneIndex;
            _longName = Wrist.LongBoneNames[boneIndex];
            _shortName = Wrist.ShortBoneNames[boneIndex];
            _ivFilename = _wrist.bpaths[boneIndex];
            _distanceFieldFilename = _wrist.DistanceFieldPaths[boneIndex];
            int numSeries = _wrist.motionFiles.Length + 1; //add 1 for neutral
            _transformMatrices = new TransformMatrix[numSeries];
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

        public void ReadDistanceField()
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

        public void HideBone()
        {
            if (_coloredBone != null)
                _coloredBone.setHidden(true);
            else
                _bone.hide();
        }

        public void ShowBone()
        {
            if (_coloredBone != null)
                _coloredBone.setHidden(false);
            else
                _bone.show();
        }

        public void SetBoneVisibility(bool visible)
        {
            if (visible)
                ShowBone();
            else
                HideBone();
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
            return (!_transformMatrices[positionIndex].isIdentity());
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

        public void MoveToPosition(int positionIndex, Bone fixedBone)
        {
            //first remove any existing transform....
            if (_bone.hasTransform())
                _bone.removeTransform();

            //there is no need to move if we are the fixed bone, or this is the neutral position
            if (positionIndex == 0 || fixedBone == this)
                return;

            //now create the correct transform and apply
            TransformMatrix tm = CalculateRelativeMotionFromNeutral(positionIndex, fixedBone);
            _bone.addTransform(tm.ToTransform());
        }

        public void MoveToPosition(TransformMatrix tm)
        {
            if (_bone.hasTransform())
                _bone.removeTransform();

            if (tm.isIdentity()) return; //nothing to do in this case
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
            //quick check that we can do this.
            if (_distanceField == null || _coloredBone == null) return;

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

        public void CalculateAndSaveContourForPosition(int positionIndex, double[] cDistances, System.Drawing.Color[] colors)
        {
            if (_computedDistances[positionIndex] == null)
                throw new WristException("Raw distances (_computedDistancs) must be pre-computed before calculating ColorDistanceMap");

            Contour contour = DistanceMaps.createContourSingleBoneSinglePosition(this, _computedDistances[positionIndex], cDistances, colors);
            lock (_computedContours)
            {
                _computedContours[positionIndex] = contour;
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
    }
}
