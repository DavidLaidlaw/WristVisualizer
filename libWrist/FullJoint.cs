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

    }
}
