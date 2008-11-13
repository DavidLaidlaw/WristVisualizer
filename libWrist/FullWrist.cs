using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using libCoin3D;

namespace libWrist
{
    public class FullWrist
    {
        private List<Bone> _bones;
        private Wrist _wrist;
        private Separator _root;

        private int _fixedBoneIndex;
        private int _currentPositionIndex;

        public FullWrist(Wrist wrist)
        {
            _wrist = wrist;
            _fixedBoneIndex = (int)Wrist.BIndex.RAD;
            _currentPositionIndex = 0;
            _bones = new List<Bone>(Wrist.NumBones);
        }

        public Separator Root
        {
            get { return _root; }
        }

        public void LoadFullWrist()
        {
            _root = new Separator();
            for (int i = 0; i < Wrist.NumBones; i++)
            {
                _bones[i] = new Bone(_wrist, this, i);
                _bones[i].LoadIVFile();
            }

            LoadKinematicTransforms();
            LoadInertiaData();
        }

        public void ReadInDistanceFields()
        {
            for (int i = 0; i < Wrist.NumBones; i++)
            {
                if (!_bones[i].IsValidBone) continue;

                if (!_bones[i].HasDistanceField) //skip if already loaded
                    _bones[i].ReadDistanceField();
            }
        }

        private void LoadKinematicTransforms()
        {
            int numPos = _wrist.motionFiles.Length;
            for (int i = 0; i < numPos; i++)
            {
                TransformMatrix[] transforms = DatParser.parseMotionFileToTransformMatrix(_wrist.motionFiles[i]);
                for (int j = 0; j < Wrist.NumBones; j++)
                    _bones[j].SetTransformation(transforms[j], i + 1); //offset position index by 1, 0 is neutral now!
            }
        }

        private void LoadInertiaData()
        {
            if (File.Exists(_wrist.inertiaFile))
            {                
                    TransformRT[] inert = DatParser.parseInertiaFileToRT(_wrist.inertiaFile);
                    for (int i = 0; i < Wrist.NumBones; i++) //skip the long bones
                    {
                        _bones[i].InertiaMatrix = new TransformMatrix(inert[i]);
                    }
            }

            //now try and load special inertia data
            LoadInertiaData_SingleBone(_wrist.acsFile, (int)Wrist.BIndex.RAD);
            LoadInertiaData_SingleBone(_wrist.acsFile_uln, (int)Wrist.BIndex.ULN);
        }

        private void LoadInertiaData_SingleBone(string filename, int boneIndex)
        {
            if (!File.Exists(filename))
                return;

            TransformRT[] acs = DatParser.parseACSFileToRT(filename);
            _bones[boneIndex].InertiaMatrix = new TransformMatrix(acs[0]);
        }


        private void SetCurrentPosition(int positionIndex)
        {
            _currentPositionIndex = positionIndex;
            UpdateTransformsForCurrentPositionAndFixedBone();
        }

        private void SetFixedBone(int boneIndex)
        {
            //quick checks here
            Bone fixedBone = _bones[boneIndex];
            if (fixedBone == null || !fixedBone.IsValidBone)
                throw new WristException(String.Format("Attempting to set fixed bone to a non-valid bone ({0}: {1})", fixedBone.ShortName, fixedBone.BoneIndex));

            _fixedBoneIndex = boneIndex;
            UpdateTransformsForCurrentPositionAndFixedBone();
        }

        private void UpdateTransformsForCurrentPositionAndFixedBone()
        {
            for (int i = 0; i < Wrist.NumBones; i++)
            {
                if (!_bones[i].IsValidBone) continue; //skip missing bones 

                _bones[i].MoveToPosition(_currentPositionIndex, _bones[_fixedBoneIndex]);
            }
        }
    }
}
