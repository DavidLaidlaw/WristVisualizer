using System;
using System.Collections.Generic;
using System.Text;
using libWrist;
using libCoin3D;

namespace WristVizualizer
{    
    class FullWristController
    {
        private Separator[] _bones;
        private Separator[] _inertias;
        private string[] _bnames = { "rad", "uln", "sca", "lun", "trq", "pis", "tpd", "tpm", "cap", "ham", "mc1", "mc2", "mc3", "mc4", "mc5" };
        private Wrist _wrist;
        private Transform[][] _transforms;
        private int _currentPositionIndex;
        private int _fixedBoneIndex;

        public FullWristController()
        {
        }
    }
}
