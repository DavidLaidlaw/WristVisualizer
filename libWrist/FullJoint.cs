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

        protected bool _showContours;
        protected bool _showColorMap;

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

        #endregion

    }
}
