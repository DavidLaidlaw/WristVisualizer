using System;
using System.Collections.Generic;
using System.Text;
using libCoin3D;

namespace libWrist
{
    public abstract class FullJoint
    {
        protected Bone[] _bones;
        //private WristFilesystem _wrist;
        protected Separator _root;

        protected int _fixedBoneIndex;
        protected int _currentPositionIndex;
        protected int _numberPositions;

        protected bool _showContours;
        protected bool _showColorMap;

        protected double _colorMapDistance;
        protected double[] _contourDistances;
        protected System.Drawing.Color[] _contourColors;

        protected int[][] _BoneInteractionIndex = null;

        public abstract void LoadFullJoint();


        #region Public Properties

        public Separator Root
        {
            get { return _root; }
        }

        public Bone[] Bones
        {
            get { return _bones; }
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

        #endregion

        #region Common Code

        public virtual void MoveToPositionAndFixedBone(int positionIndex, int fixedBoneIndex)
        {
            //quick checks here
            Bone fixedBone = _bones[fixedBoneIndex];
            if (fixedBone == null || !fixedBone.IsValidBone)
                throw new WristException(String.Format("Attempting to set fixed bone to a non-valid bone (bone index: {0})", fixedBoneIndex));

            _currentPositionIndex = positionIndex;
            _fixedBoneIndex = fixedBoneIndex;

            for (int i = 0; i < _bones.Length; i++)
            {
                if (!_bones[i].IsValidBone) continue; //skip missing bones 

                _bones[i].MoveToPosition(_currentPositionIndex, _bones[_fixedBoneIndex]);
            }

            UpdateColorsAndContoursForCurrentPosition();
            HideBonesWithNoKinematics(); //yes?
        }


        public virtual void HideBonesWithNoKinematics()
        {
            HideBonesWithNoKinematics(_currentPositionIndex);
        }

        public virtual void HideBonesWithNoKinematics(int positionIndex)
        {
            for (int i = 0; i < _bones.Length; i++)
            {
                if (!_bones[i].HasKinematicInformationForPosition(positionIndex))
                    _bones[i].HideBone();
            }
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
            for (int i = 0; i < _bones.Length; i++)
                _bones[i].SetContourForPositionIfCalculated(_currentPositionIndex);
        }

        public void HideContours()
        {
            _showContours = false;
            for (int i = 0; i < _bones.Length; i++)
                _bones[i].RemoveContour();
        }

        public void ShowColorMapIfCalculated()
        {
            _showColorMap = true;
            for (int i = 0; i < _bones.Length; i++)
                _bones[i].SetColorMapForPositionIfCalculated(_currentPositionIndex);
        }

        public void HideColorMap()
        {
            _showColorMap = false;
            for (int i = 0; i < _bones.Length; i++)
                _bones[i].RemoveColorMap();
        }

        public void HideColorMapAndContoursTemporarily()
        {
            for (int i = 0; i < _bones.Length; i++)
            {
                _bones[i].RemoveColorMap();
                _bones[i].RemoveContour();
            }
        }

        public void ReadInDistanceFields()
        {
            for (int i = 0; i < _bones.Length; i++)
            {
                if (!_bones[i].IsValidBone) continue;

                if (!_bones[i].HasDistanceField) //skip if already loaded
                    _bones[i].TryReadDistanceField();
            }
        }

        #endregion

        #region Distance Map Job Creation

        private bool ContourDistancesDiffer(double[] cDistances)
        {
            if (_contourDistances.Length != cDistances.Length) return true;
            for (int i = 0; i < _contourDistances.Length; i++)
                if (_contourDistances[i] != cDistances[i]) return true;

            return false;
        }

        public Queue<Queue<BulkCalculator.DistanceCalculationJob>> CreateDistanceMapMasterAnimationQueue(int[] animationOrder, int numFramesPerStep, Bone fixedBone, double colorMapDistance, double[] cDistances, System.Drawing.Color[] colors)
        {
            //first check if we are doing anything
            if (colorMapDistance <= 0 && cDistances.Length == 0)
                return null;

            Queue<Queue<BulkCalculator.DistanceCalculationJob>> masterQueue = new Queue<Queue<BulkCalculator.DistanceCalculationJob>>(2);
            int totalNumPositions = (animationOrder.Length - 1) * numFramesPerStep + 1;

            //okay, doing something, compute the distances at each vertex
            masterQueue.Enqueue(CreateVertexDistanceQueue(totalNumPositions, animationOrder, numFramesPerStep, fixedBone));
            if (cDistances.Length > 0)
                masterQueue.Enqueue(CreateContourQueue(cDistances, colors, totalNumPositions, animationOrder, numFramesPerStep));
            if (colorMapDistance > 0) //check that we have this job
                masterQueue.Enqueue(CreateColorMapQueue(colorMapDistance, totalNumPositions, animationOrder, numFramesPerStep));

            return masterQueue;
        }

        public Queue<Queue<BulkCalculator.DistanceCalculationJob>> CreateDistanceMapMasterQueue(double colorMapDistance, double[] cDistances, System.Drawing.Color[] colors)
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

        private Queue<BulkCalculator.DistanceCalculationJob> CreateVertexDistanceQueue()
        {
            return CreateVertexDistanceQueue(_numberPositions, null, 0, null); //pass in nothing!
        }
        private Queue<BulkCalculator.DistanceCalculationJob> CreateVertexDistanceQueue(int totalNumberPositions, int[] animationOrder, int numFramesPerStep, Bone fixedBone)
        {
            Queue<BulkCalculator.DistanceCalculationJob> q = new Queue<BulkCalculator.DistanceCalculationJob>();
            //need to create this for each position and every bone...
            for (int boneIndex = 0; boneIndex < WristFilesystem.NumBones; boneIndex++)
            {
                Bone referenceBone = _bones[boneIndex];
                //we can only do this for valid color bones, so lets check that this is the case, if not, continue
                if (!referenceBone.IsValidBone || !referenceBone.IsColoredBone) continue;

                Bone[] interactionBones = GetBonesThatInteractWithBone(boneIndex);
                for (int i = 0; i < totalNumberPositions; i++)
                {
                    //check that we need to do this (can only skip when not in animation mode)
                    if (animationOrder == null && referenceBone.HasVertexDistancesForPosition(i)) continue;

                    BulkCalculator.DistanceCalculationJob job = new BulkCalculator.DistanceCalculationJob();
                    job.JobType = BulkCalculator.DistanceCalculationType.VetrexDistances;
                    job.FullJoint = this;
                    job.PrimaryBone = referenceBone;
                    job.IneractionBones = interactionBones;
                    job.PositionIndex = i;
                    job.AnimationOrder = animationOrder;
                    job.AnimationNumFramesPerStep = numFramesPerStep;
                    job.FixedBone = fixedBone;
                    q.Enqueue(job);
                }
            }
            return q;
        }

        private Queue<BulkCalculator.DistanceCalculationJob> CreateColorMapQueue(double colorMapDistance)
        {
            return CreateColorMapQueue(colorMapDistance, _numberPositions, null, 0);
        }
        private Queue<BulkCalculator.DistanceCalculationJob> CreateColorMapQueue(double colorMapDistance, int totalNumberPositions, int[] animationOrder, int numFramesPerStep)
        {
            Queue<BulkCalculator.DistanceCalculationJob> q = new Queue<BulkCalculator.DistanceCalculationJob>();
            //need to create this for each position and every bone...
            for (int boneIndex = 0; boneIndex < WristFilesystem.NumBones; boneIndex++)
            {
                Bone referenceBone = _bones[boneIndex];
                //we can only do this for valid color bones, so lets check that this is the case, if not, continue
                if (!referenceBone.IsValidBone || !referenceBone.IsColoredBone) continue;

                for (int i = 0; i < totalNumberPositions; i++)
                {
                    BulkCalculator.DistanceCalculationJob job = new BulkCalculator.DistanceCalculationJob();
                    job.JobType = BulkCalculator.DistanceCalculationType.ColorMap;
                    job.FullJoint = this;
                    job.PrimaryBone = referenceBone;
                    job.PositionIndex = i;
                    job.ColorMapMaxDistance = colorMapDistance;
                    job.AnimationOrder = animationOrder;
                    job.AnimationNumFramesPerStep = numFramesPerStep;
                    q.Enqueue(job);
                }
            }
            return q;
        }

        private Queue<BulkCalculator.DistanceCalculationJob> CreateContourQueue(double[] cDistances, System.Drawing.Color[] colors)
        {
            return CreateContourQueue(cDistances, colors, _numberPositions, null, 0);
        }
        private Queue<BulkCalculator.DistanceCalculationJob> CreateContourQueue(double[] cDistances, System.Drawing.Color[] colors, int totalNumberPositions, int[] animationOrder, int numFramesPerStep)
        {
            Queue<BulkCalculator.DistanceCalculationJob> q = new Queue<BulkCalculator.DistanceCalculationJob>();
            //need to create this for each position and every bone...
            for (int boneIndex = 0; boneIndex < WristFilesystem.NumBones; boneIndex++)
            {
                Bone referenceBone = _bones[boneIndex];
                //we can only do this for valid color bones, so lets check that this is the case, if not, continue
                if (!referenceBone.IsValidBone || !referenceBone.IsColoredBone) continue;

                for (int i = 0; i < totalNumberPositions; i++)
                {
                    BulkCalculator.DistanceCalculationJob job = new BulkCalculator.DistanceCalculationJob();
                    job.JobType = BulkCalculator.DistanceCalculationType.Contours;
                    job.FullJoint = this;
                    job.PrimaryBone = referenceBone;
                    job.PositionIndex = i;
                    job.ContourDistances = cDistances;
                    job.ContourColors = colors;
                    job.AnimationOrder = animationOrder;
                    job.AnimationNumFramesPerStep = numFramesPerStep;
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
            //check if we know what the bone-bone interaction matrix should look like
            if (_BoneInteractionIndex != null)
            {
                //if we know it, we can use that interaction map
                Bone[] interactionBones = new Bone[_BoneInteractionIndex[testBoneIndex].Length];
                for (int i = 0; i < interactionBones.Length; i++)
                    interactionBones[i] = _bones[_BoneInteractionIndex[testBoneIndex][i]];
                return interactionBones;
            }
            else
            {
                //if the interaction is not known, then we need to test against every other bone (which sucks)
                List<Bone> tempBones = new List<Bone>(_bones);
                tempBones.RemoveAt(testBoneIndex); //remove the specified bone
                return tempBones.ToArray();
            }
        }

        #endregion

    }
}
