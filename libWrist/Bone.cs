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

        //Data objects
        private TransformMatrix _inertiaMatrix;
        private List<TransformMatrix> _transformMatrices;
        private CTmri _distanceField;

        //Coin3D objects
        private Separator _bone;
        private ColoredBone _coloredBone;
        private Separator _inertiaSeparator;

        public Bone(Wrist wrist, int boneIndex)
        {
            _wrist = wrist;
            _boneIndex = boneIndex;
            _longName = Wrist.LongBoneNames[boneIndex];
            _shortName = Wrist.ShortBoneNames[boneIndex];
            _ivFilename = _wrist.bpaths[boneIndex];
            _distanceFieldFilename = _wrist.DistanceFieldPaths[boneIndex];
            _transformMatrices = new List<TransformMatrix>();
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

        private void ReadDistanceField()
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
            if (_transformMatrices.Count <= positionIndex) return false;
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
    }
}
