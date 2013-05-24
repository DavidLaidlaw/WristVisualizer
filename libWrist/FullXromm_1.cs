using System;
using System.Collections.Generic;
using System.Text;
using libCoin3D;

namespace libWrist
{
    public class FullXromm : FullJoint
    {
        private XrommFilesystem _xrommFileSystem;
        private int[] _numPositionsPerSeries;

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

        public void SetToPositionAndFixedBoneAndTrial(int positionIndex, int fixedBoneIndex, int trial)
        {
            if (trial >= _numPositionsPerSeries.Length)
                throw new WristException("Invalid trial index number specified");

            //calculate the correct position offset
            int offset = 0;
            for (int i = 0; i < trial; i++)
                offset += _numPositionsPerSeries[i];

            SetToPositionAndFixedBone(positionIndex + offset, fixedBoneIndex);
        }

        public int[] NumPositionsPerTrial
        {
            get { return _numPositionsPerSeries; }
        }

        private void LoadKinematicTransforms()
        {
            _numPositionsPerSeries = new int[_xrommFileSystem.Trials.Length + 1];
            _numPositionsPerSeries[0] = 1; //CT Scan

            //temporarily save kinematics here, before we can pass them on
            List<TransformMatrix>[] tempTM = new List<TransformMatrix>[_xrommFileSystem.NumBones];
            for (int i = 0; i < _xrommFileSystem.NumBones; i++)
            {
                tempTM[i] = new List<TransformMatrix>();
                tempTM[i].Add(new TransformMatrix()); //add initial identity matrix for CT position
            }

            //lets loop through each Trial now and load the data
            for (int i = 0; i < _xrommFileSystem.Trials.Length; i++)
            {
                //Read in the data
                TransformMatrix[][] fm = DatParser.parseXrommKinematicFileToTransformMatrix(_xrommFileSystem.Trials[i].KinematicFilename);

                if (fm.Length == 0)
                    throw new WristException("Unable to load Trial no data, help!");

                //add data for each bone to the correct location
                for (int j = 0; j < _xrommFileSystem.NumBones; j++)
                    tempTM[j].AddRange(fm[j]);


                _numPositionsPerSeries[i + 1] = fm[0].Length;
            }

            _numberPositions = tempTM[0].Count; //the total count!

            //finally, lets move all the data into the bone structure
            for (int i = 0; i < _xrommFileSystem.NumBones; i++)
            {
                _bones[i].InitializeDataStructures(_numberPositions);
                _bones[i].SetTransformation(tempTM[i].ToArray());
            }
        }
    }
}
