using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using libCoin3D;

namespace libWrist
{
    public class FullWrist
    {
        private Bone[] _bones;
        private Wrist _wrist;
        private Separator _root;

        private int _fixedBoneIndex;
        private int _currentPositionIndex;

        private bool _showContourIfCalculated;
        private bool _showColorMapIfCalculated;

        public FullWrist(Wrist wrist)
        {
            _wrist = wrist;
            _fixedBoneIndex = (int)Wrist.BIndex.RAD;
            _currentPositionIndex = 0;
            _bones = new Bone[Wrist.NumBones];
            _showColorMapIfCalculated = true;
            _showContourIfCalculated = true;
        }

        public Separator Root
        {
            get { return _root; }
        }

        public Bone[] Bones
        {
            get { return _bones; }
        }

        public bool ShowContourIfCalculated
        {
            get { return _showContourIfCalculated; }
            set { _showContourIfCalculated = value; }
        }

        public bool ShowColorMapIfCalculated
        {
            get { return _showColorMapIfCalculated; }
            set { _showColorMapIfCalculated = value; }
        }

        public void LoadFullWrist()
        {
            _root = new Separator();
            for (int i = 0; i < Wrist.NumBones; i++)
            {
                _bones[i] = new Bone(_wrist, this, i);
                _bones[i].LoadIVFile();
                if (_bones[i].IsValidBone)
                    _root.addChild(_bones[i].BoneSeparator);
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

        public void MoveToPositionAndFixedBone(int positionIndex, int fixedBoneIndex)
        {
            //quick checks here
            Bone fixedBone = _bones[fixedBoneIndex];
            if (fixedBone == null || !fixedBone.IsValidBone)
                throw new WristException(String.Format("Attempting to set fixed bone to a non-valid bone ({0}: {1})", fixedBone.ShortName, fixedBoneIndex));

            _currentPositionIndex = positionIndex;
            _fixedBoneIndex = fixedBoneIndex;

            for (int i = 0; i < Wrist.NumBones; i++)
            {
                if (!_bones[i].IsValidBone) continue; //skip missing bones 

                _bones[i].MoveToPosition(_currentPositionIndex, _bones[_fixedBoneIndex]);

                if (_showColorMapIfCalculated)
                    _bones[i].SetColorMapForPositionIfCalculated(_currentPositionIndex);
                if (_showContourIfCalculated)
                    _bones[i].SetContourForPositionIfCalculated(_currentPositionIndex);
            }

            HideBonesWithNoKinematics(); //yes?
        }
        

        public void HideBonesWithNoKinematics()
        {
            HideBonesWithNoKinematics(_currentPositionIndex);
        }

        public void HideBonesWithNoKinematics(int positionIndex)
        {
            for (int i = 0; i < Wrist.NumBones; i++)
            {
                if (!_bones[i].HasKinematicInformationForPosition(positionIndex))
                    _bones[i].HideBone();
            }
        }

        public void TestLoadDistanceMaps()
        {
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            ReadInDistanceFields();

            for (int pos = 0; pos < 3; pos++)
            {
                for (int i = 0; i < Wrist.NumBones; i++)
                {
                    Bone[] testBones = new Bone[Wrist.BoneInteractionIndex[i].Length];
                    for (int j = 0; j < testBones.Length; j++)
                        testBones[j] = _bones[Wrist.BoneInteractionIndex[i][j]];
                    _bones[i].CalculateAndSaveDistanceMapForPosition(pos, testBones);
                }

                for (int i = 0; i < Wrist.NumBones; i++)
                {
                    _bones[i].CalculateAndSaveColorDistanceMapForPosition(pos);
                    _bones[i].CalculateAndSaveContourForPosition(pos, new double[] { 1.0, 1.5 }, new System.Drawing.Color[] { System.Drawing.Color.White, System.Drawing.Color.White });
                }
            }
            sw.Stop();
            Console.WriteLine("Time: {0}",sw.Elapsed.ToString());
        }
    }
}
