using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using libCoin3D;

namespace libWrist
{
    public class FullWrist
    {
        private List<Bone> _bones;
        private Wrist _wrist;
        private Separator _root;

        public FullWrist(Wrist wrist)
        {
            _wrist = wrist;
            _bones = new List<Bone>(Wrist.NumBones);
        }

        public void LoadFullWrist()
        {
            _root = new Separator();
            for (int i = 0; i < Wrist.NumBones; i++)
            {
                _bones[i] = new Bone(_wrist, i);
                _bones[i].LoadIVFile();
            }

            LoadKinematicTransforms();

        }

        private void LoadKinematicTransforms()
        {
            int numPos = _wrist.motionFiles.Length;
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
            LoadInertiaData_Special(_wrist.acsFile, 0);
            LoadInertiaData_Special(_wrist.acsFile_uln, 1);
        }

        private void LoadInertiaData_Special(string filename, int boneIndex)
        {
            if (!File.Exists(filename))
                return;

            TransformRT[] acs = DatParser.parseACSFileToRT(filename);
            _bones[boneIndex].InertiaMatrix = new TransformMatrix(acs[0]);
        }

    }
}
