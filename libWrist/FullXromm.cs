using System;
using System.Collections.Generic;
using System.Text;
using libCoin3D;

namespace libWrist
{
    public class FullXromm : FullJoint
    {
        private XrommFilesystem _xrommFileSystem;

        private int _fixedBoneIndex;
        private int _currentPositionIndex;

        private bool _showContours;
        private bool _showColorMap;

        private double _colorMapDistance;
        private double[] _contourDistances;
        private System.Drawing.Color[] _contourColors;

        private int _numberPositions;

        public FullXromm()
        { 
        }
    }
}
