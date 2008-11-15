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

        private bool _showContours;
        private bool _showColorMap;

        private double _colorMapDistance;
        private double[] _contourDistances;
        private System.Drawing.Color[] _contourColors;

        private int _numberPositions;

        public FullWrist(Wrist wrist)
        {
            _wrist = wrist;
            _fixedBoneIndex = (int)Wrist.BIndex.RAD;
            _currentPositionIndex = 0;
            _bones = new Bone[Wrist.NumBones];
            _showColorMap = true;
            _showContours = true;
            _contourDistances = new double[0];
        }

        public Separator Root
        {
            get { return _root; }
        }

        public Bone[] Bones
        {
            get { return _bones; }
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

        public double ColorMapDistance
        {
            get { return _colorMapDistance; }
        }
        public double[] ContourDistances
        {
            get { return _contourDistances; }
        }
        public System.Drawing.Color[] ContourColors
        {
            get { return _contourColors; }
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
            _numberPositions = numPos + 1; //add 1 for the neutral position
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
            }

            UpdateColorsAndContoursForCurrentPosition();
            HideBonesWithNoKinematics(); //yes?
        }

        public void UpdateColorsAndContoursForCurrentPosition()
        {
            if (_showColorMap)
                ShowColorMapIfCalculated();
            else
                HideColorMap();

            if (_showContours)
                ShowContoursIfCalculated();
            else
                HideContours();
        }

        public void ShowContoursIfCalculated()
        {
            _showContours = true;
            for (int i = 0; i < Wrist.NumBones; i++)
                _bones[i].SetContourForPositionIfCalculated(_currentPositionIndex);
        }

        public void HideContours()
        {
            _showContours = false;
            for (int i = 0; i < Wrist.NumBones; i++)
                _bones[i].RemoveContour();
        }

        public void ShowColorMapIfCalculated()
        {
            _showColorMap = true;
            for (int i = 0; i < Wrist.NumBones; i++)
                _bones[i].SetColorMapForPositionIfCalculated(_currentPositionIndex);
        }

        public void HideColorMap()
        {
            _showColorMap = false;
            for (int i = 0; i < Wrist.NumBones; i++)
                _bones[i].RemoveColorMap();
        }

        public void HideColorMapAndContoursTemporarily()
        {
            for (int i = 0; i < Wrist.NumBones; i++)
            {
                _bones[i].RemoveColorMap();
                _bones[i].RemoveContour();
            }
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

        public void SetToAnimationFrame(int frameNumber)
        {
            for (int i = 0; i < Wrist.NumBones; i++)
                _bones[i].SetAnimationFrame(frameNumber);
        }

        public void SetupWristForAnimation(int fixedBoneIndex, int[] animationOrder, int numFrames)
        {
            //TODO: Distance Maps
            HideColorMapAndContoursTemporarily(); //hide for now

            for (int i = 0; i < Wrist.NumBones; i++)
            {
                //skip invalid bones
                if (!_bones[i].IsValidBone) continue;

                Switch animationSwitch = AnimationCreator.CreateAnimationSwitch(_bones[i], _bones[fixedBoneIndex], animationOrder, numFrames);
                Switch animationHamSwitch = AnimationCreator.CreateHAMSwitch(_bones[i], _bones[fixedBoneIndex], animationOrder, numFrames);
                _bones[i].MoveToPosition(0, _bones[i]); //remove the current saved transform for regular mode, dirty hack
                _bones[i].SetupForAnimation(animationSwitch, animationHamSwitch);
            }
        }

        public void EndAnimation()
        {
            for (int i = 0; i < Wrist.NumBones; i++)
                _bones[i].EndAnimation();
            MoveToPositionAndFixedBone(_currentPositionIndex, _fixedBoneIndex);
        }

        public Queue<Queue<BulkCalculator.DistanceCalculationJob>> CreateDistanceMapJobQueue(double colorMapDistance, double[] cDistances, System.Drawing.Color[] colors)
        {
            //first check if we are doing anything
            if (colorMapDistance <= 0 && cDistances.Length == 0)
                return null;

            Queue<Queue<BulkCalculator.DistanceCalculationJob>> masterQueue = new Queue<Queue<BulkCalculator.DistanceCalculationJob>>(2);
            
            //okay, doing something, so lets make certain that we have computed the distances at each vertex
            Queue<BulkCalculator.DistanceCalculationJob> q = CreateVertexDistanceQueue();
            if (q.Count > 0)
                masterQueue.Enqueue(q);

            if (cDistances.Length > 0 && ContourDistancesDiffer(cDistances))
            {
                q = CreateContourQueue(cDistances, colors);
                masterQueue.Enqueue(q);
                _contourDistances = cDistances;
                _contourColors = colors;
            }

            if (colorMapDistance > 0 && colorMapDistance != _colorMapDistance) //check that we have this job, and its different
            {
                q = CreateColorMapQueue(colorMapDistance);
                masterQueue.Enqueue(q);
                _colorMapDistance = colorMapDistance;
            }

            return masterQueue;
        }

        private bool ContourDistancesDiffer(double[] cDistances)
        {
            if (_contourDistances.Length != cDistances.Length) return true;
            for (int i = 0; i < _contourDistances.Length; i++)            
                if (_contourDistances[i] != cDistances[i]) return true;

            return false;
        }

        private Queue<BulkCalculator.DistanceCalculationJob> CreateVertexDistanceQueue()
        {
            Queue<BulkCalculator.DistanceCalculationJob> q = new Queue<BulkCalculator.DistanceCalculationJob>();
            //need to create this for each position and every bone...
            for (int boneIndex = 0; boneIndex < Wrist.NumBones; boneIndex++)
            {
                Bone referenceBone = _bones[boneIndex];
                //we can only do this for valid color bones, so lets check that this is the case, if not, continue
                if (!referenceBone.IsValidBone || !referenceBone.IsColoredBone) continue;

                Bone[] interactionBones = GetBonesThatInteractWithBone(boneIndex);
                for (int i = 0; i < _numberPositions; i++)
                {
                    //check that we need to do this
                    if (referenceBone.HasContourForPosition(i)) continue;

                    BulkCalculator.DistanceCalculationJob job = new BulkCalculator.DistanceCalculationJob();
                    job.JobType = BulkCalculator.DistanceCalculationType.VetrexDistances;
                    job.FullWrist = this;
                    job.PrimaryBone = referenceBone;
                    job.IneractionBones = interactionBones;
                    job.PositionIndex = i;
                    q.Enqueue(job);
                }
            }
            return q;
        }

        private Queue<BulkCalculator.DistanceCalculationJob> CreateColorMapQueue(double colorMapDistance)
        {
            Queue<BulkCalculator.DistanceCalculationJob> q = new Queue<BulkCalculator.DistanceCalculationJob>();
            //need to create this for each position and every bone...
            for (int boneIndex = 0; boneIndex < Wrist.NumBones; boneIndex++)
            {
                Bone referenceBone = _bones[boneIndex];
                //we can only do this for valid color bones, so lets check that this is the case, if not, continue
                if (!referenceBone.IsValidBone || !referenceBone.IsColoredBone) continue;

                for (int i = 0; i < _numberPositions; i++)
                {
                    BulkCalculator.DistanceCalculationJob job = new BulkCalculator.DistanceCalculationJob();
                    job.JobType = BulkCalculator.DistanceCalculationType.ColorMap;
                    job.FullWrist = this;
                    job.PrimaryBone = referenceBone;
                    job.PositionIndex = i;
                    job.ColorMapMaxDistance = colorMapDistance;
                    q.Enqueue(job);
                }
            }
            return q;
        }

        private Queue<BulkCalculator.DistanceCalculationJob> CreateContourQueue(double[] cDistances, System.Drawing.Color[] colors)
        {
            Queue<BulkCalculator.DistanceCalculationJob> q = new Queue<BulkCalculator.DistanceCalculationJob>();
            //need to create this for each position and every bone...
            for (int boneIndex = 0; boneIndex < Wrist.NumBones; boneIndex++)
            {
                Bone referenceBone = _bones[boneIndex];
                //we can only do this for valid color bones, so lets check that this is the case, if not, continue
                if (!referenceBone.IsValidBone || !referenceBone.IsColoredBone) continue;

                for (int i = 0; i < _numberPositions; i++)
                {
                    BulkCalculator.DistanceCalculationJob job = new BulkCalculator.DistanceCalculationJob();
                    job.JobType = BulkCalculator.DistanceCalculationType.Contours;
                    job.FullWrist = this;
                    job.PrimaryBone = referenceBone;
                    job.PositionIndex = i;
                    job.ContourDistances = cDistances;
                    job.ContourColors = colors;
                    q.Enqueue(job);
                }
            }
            return q;
        }

        private Bone[] GetBonesThatInteractWithBone(Bone testBone)
        {
            return GetBonesThatInteractWithBone(testBone.BoneIndex);
        }
        private Bone[] GetBonesThatInteractWithBone(int testBoneIndex)
        {
            Bone[] interactionBones = new Bone[Wrist.BoneInteractionIndex[testBoneIndex].Length];
            for (int i = 0; i < interactionBones.Length; i++)
                interactionBones[i] = _bones[Wrist.BoneInteractionIndex[testBoneIndex][i]];
            return interactionBones;
        }

    }
}
