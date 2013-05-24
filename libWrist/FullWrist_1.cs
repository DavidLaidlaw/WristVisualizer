using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using libCoin3D;

namespace libWrist
{
    public class FullWrist : FullJoint
    {
        private WristFilesystem _wrist;

        public FullWrist(WristFilesystem wrist)
        {
            _wrist = wrist;
            _fixedBoneIndex = (int)WristFilesystem.BIndex.RAD;
            _currentPositionIndex = 0;
            _bones = new Bone[WristFilesystem.NumBones];
            _showColorMap = true;
            _showContours = true;
            _contourDistances = new double[0];
            _BoneInteractionIndex = WristFilesystem.BoneInteractionIndex;
        }


        public bool ShowContours
        {
            get { return _showContours; }
            set { _showContours = value; }
        }

        public bool ShowColorMap
        {
            get { return _showColorMap; }
            set { _showColorMap = value; }
        }


        public void LoadSelectBonesAndDistancesForBatchMode(int[] testbones, int[] refbones)
        {
            _root = new Separator();
            for (int i = 0; i < WristFilesystem.NumBones; i++)
                _bones[i] = new Bone(_wrist.bpaths[i], _wrist.DistanceFieldPaths[i], i, _wrist.motionFiles.Length + 1);
            foreach (int bIndex in testbones)
            {
                _bones[bIndex].LoadIVFile();
                //for the test bones, we need the distance fields
                _bones[bIndex].TryReadDistanceField();
            }
            foreach (int bIndex in refbones)
            {
                _bones[bIndex].LoadIVFile();
            }
            LoadKinematicTransforms();
            LoadInertiaData();
        }

        public override void LoadFullJoint()
        {
            _root = new Separator();
            for (int i = 0; i < WristFilesystem.NumBones; i++)
            {
                _bones[i] = new Bone(_wrist.bpaths[i], _wrist.DistanceFieldPaths[i], i, _wrist.motionFiles.Length + 1);
                _bones[i].LoadIVFile();
                if (_bones[i].IsValidBone)
                    _root.addChild(_bones[i].BoneSeparator);
            }

            LoadKinematicTransforms();
            LoadInertiaData();
        }


        private void LoadKinematicTransforms()
        {
            int numPos = _wrist.motionFiles.Length;
            _numberPositions = numPos + 1; //add 1 for the neutral position
            for (int i = 0; i < numPos; i++)
            {
                TransformMatrix[] transforms = DatParser.parseMotionFileToTransformMatrix(_wrist.motionFiles[i]);
                for (int j = 0; j < WristFilesystem.NumBones; j++)
                    _bones[j].SetTransformation(transforms[j], i + 1); //offset position index by 1, 0 is neutral now!
            }
        }

        private void LoadInertiaData()
        {
            if (File.Exists(_wrist.inertiaFile))
            {                
                    TransformRT[] inert = DatParser.parseInertiaFileToRT(_wrist.inertiaFile);
                    for (int i = 0; i < WristFilesystem.NumBones; i++) //skip the long bones
                    {
                        _bones[i].InertiaMatrix = new TransformMatrix(inert[i]);
                    }
            }

            //now try and load special inertia data
            LoadInertiaData_SingleBone(_wrist.acsFile, (int)WristFilesystem.BIndex.RAD);
            LoadInertiaData_SingleBone(_wrist.acsFile_uln, (int)WristFilesystem.BIndex.ULN);
        }

        private void LoadInertiaData_SingleBone(string filename, int boneIndex)
        {
            if (!File.Exists(filename))
                return;

            TransformRT[] acs = DatParser.parseACSFileToRT(filename);
            _bones[boneIndex].InertiaMatrix = new TransformMatrix(acs[0]);
        }

        public void SetToAnimationFrame(int frameNumber)
        {
            for (int i = 0; i < WristFilesystem.NumBones; i++)
                _bones[i].SetAnimationFrame(frameNumber);
        }

        //public void SetupWristDistancesForAnimation(int fixedBoneIndex, int[] animationOrder, int numFrames, double colorMapDistance, double[] cDistances, System.Drawing.Color[] colors)
        //{
        //    Queue<Queue<BulkCalculator.DistanceCalculationJob>> q;
        //    q = CreateDistanceMapMasterAnimationQueue(animationOrder, numFrames, _bones[fixedBoneIndex], colorMapDistance, cDistances, colors);
        //    if (q == null)
        //        return;
        //    //go compute this!
        //    _calculator.ProcessMasterQueue(q);
        //}

        public void SetupWristForAnimation(int fixedBoneIndex, int[] animationOrder, int numFrames)
        {
            HideColorMapAndContoursTemporarily(); //hide for now

            for (int i = 0; i < WristFilesystem.NumBones; i++)
            {
                //skip invalid bones
                if (!_bones[i].IsValidBone) continue;

                Switch animationSwitch = AnimationCreator.CreateAnimationSwitch(_bones[i], _bones[fixedBoneIndex], animationOrder, numFrames);
                Switch animationHamSwitch = AnimationCreator.CreateHAMSwitch(_bones[i], _bones[fixedBoneIndex], animationOrder, numFrames);
                _bones[i].SetToPosition(0, _bones[i]); //remove the current saved transform for regular mode, dirty hack
                _bones[i].SetupForAnimation(animationSwitch, animationHamSwitch);
            }
        }

        public void EndAnimation()
        {
            for (int i = 0; i < WristFilesystem.NumBones; i++)
                _bones[i].EndAnimation();
            SetToPositionAndFixedBone(_currentPositionIndex, _fixedBoneIndex);
        }

    }
}
