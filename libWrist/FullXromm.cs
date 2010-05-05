using System;
using System.Collections.Generic;
using System.Text;
using libCoin3D;

namespace libWrist
{
    public class FullXromm : FullJoint
    {
        private XrommFilesystem _xrommFileSystem;

        //private bool _showContours;
        //private bool _showColorMap;

        //private double _colorMapDistance;
        //private double[] _contourDistances;
        //private System.Drawing.Color[] _contourColors;

        //private int _numberPositions;

        public FullXromm(XrommFilesystem xrommFileSystem)
        {
            _xrommFileSystem = xrommFileSystem;
            _fixedBoneIndex = 0;
            _currentPositionIndex = 0;
            _bones = new Bone[xrommFileSystem.NumBones];
            //_showColorMap = true;
            //_showContours = true;
            //_contourDistances = new double[0];
        }

        public override void LoadFullJoint()
        {
            _root = new Separator();
            for (int i = 0; i < _xrommFileSystem.NumBones; i++)
            {
                _bones[i] = new Bone(_xrommFileSystem.bpaths[i], _xrommFileSystem.DistanceFieldPaths[i], i, 50); //TODO: Fix 50!!!
                _bones[i].LoadIVFile();
                if (_bones[i].IsValidBone)
                    _root.addChild(_bones[i].BoneSeparator);
            }

            //LoadKinematicTransforms();
            //LoadInertiaData();
        }
    }
}
