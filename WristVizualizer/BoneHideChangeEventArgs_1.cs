using System;
using System.Collections.Generic;
using System.Text;

namespace WristVizualizer
{
    public class BoneHideChangeEventArgs : EventArgs
    {
        private int _boneIndex;
        private bool _hidden;
        public BoneHideChangeEventArgs(int BoneIndex, bool hidden)
        {
            _boneIndex = BoneIndex;
            _hidden = hidden;
        }
        public int BoneIndex
        {
            get { return _boneIndex; }
        }
        public bool BoneHidden
        {
            get { return _hidden; }
        }
    }
}
