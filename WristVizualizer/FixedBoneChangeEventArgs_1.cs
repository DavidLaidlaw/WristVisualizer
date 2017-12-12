using System;
using System.Collections.Generic;
using System.Text;

namespace WristVizualizer
{
    public class FixedBoneChangeEventArgs : EventArgs
    {
        private int _boneIndex;
        public FixedBoneChangeEventArgs(int BoneIndex)
        {
            _boneIndex = BoneIndex;
        }
        public int BoneIndex
        {
            get { return _boneIndex; }
        }
    }
}
