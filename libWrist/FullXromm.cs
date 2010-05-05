using System;
using System.Collections.Generic;
using System.Text;
using libCoin3D;

namespace libWrist
{
    public class FullXromm : FullJoint
    {
        private XrommFilesystem _xrommFileSystem;

        public FullXromm(XrommFilesystem xrommFileSystem)
        {
            _xrommFileSystem = xrommFileSystem;
            _fixedBoneIndex = 0;
            _currentPositionIndex = 0;
            _bones = new Bone[xrommFileSystem.NumBones];
            _showColorMap = true;
            _showContours = true;
            _contourDistances = new double[0];
        }

        public override void LoadFullJoint()
        {
            _root = new Separator();
            for (int i = 0; i < _xrommFileSystem.NumBones; i++)
            {
                _bones[i] = new Bone(_xrommFileSystem.bpaths[i], _xrommFileSystem.DistanceFieldPaths[i], i);
                _bones[i].LoadIVFile();
                if (_bones[i].IsValidBone)
                    _root.addChild(_bones[i].BoneSeparator);
            }

            LoadKinematicTransforms();
            //LoadInertiaData();
        }

        private void LoadKinematicTransforms()
        {
            //hardcoded to Trial 0
            TransformMatrix[][] fm = DatParser.parseXrommKinematicFileToTransformMatrix(_xrommFileSystem.Trials[0].KinematicFilename);

            if (fm.Length == 0)
                return; //what do I do!?

            _numberPositions = fm[0].Length + 1;

            for (int i = 0; i < _xrommFileSystem.NumBones; i++)
            {
                _bones[i].InitializeDataStructures(_numberPositions);
                for (int j = 0; j < fm[i].Length; j++)
                    _bones[i].SetTransformation(fm[i][j], j + 1); //ofset by 1 to skip 0!
            }
        }
    }
}
