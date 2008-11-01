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
        }

        private void LoadIVFile()
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
            get { return _inertiaMatrix != null; }
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

    }
}
