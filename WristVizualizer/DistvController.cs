using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using libCoin3D;
using libWrist;

namespace WristVizualizer
{
    class DistvController : Controller
    {
        private const string DISTV_ROOT = @"P:\WORKING_OI_CODE\distv\data";

        private const string BONE_FILE_PATTERN = @"model\{0}AVG.iv";
        private const string RT_FILE_PATTERN = @"anim\{0}_AVG_anim.RT";
        private const string COLOR_FILE_PATTERN = @"color\{0}color.dat";

        private const int NUM_BONES = 10;

        private Separator _root;
        private Separator[] _boneSeparators;
        private ColoredBone[] _bones;
        private Switch[] _transforms;
        private int[][][] _colorData;

        public DistvController()
        {
            _root = new Separator();
            setupDistv();
        }

        public override Separator Root
        {
            get { return _root; }
        }

        public static Separator test()
        {
            Separator level1 = new Separator();
            ColoredBone b = new ColoredBone(@"P:\WORKING_OI_CODE\distv\data\model\scaAVG.iv");
            int numPositions = 90;
            int numVertices = b.getNumberVertices();
            int[][] colors = DatParser.parseDistvColorFile(@"P:\WORKING_OI_CODE\distv\data\color\scacolor.dat",numPositions,numVertices);
            level1.addNode(b);
            uint a = 1;
            int bb = (int)a;
            b.setColorMap((int[])colors[0]);
            return level1;
        }

        private void setupDistv()
        {
            _bones = new ColoredBone[NUM_BONES];
            _boneSeparators = new Separator[NUM_BONES];
            _transforms = new Switch[NUM_BONES];
            _colorData = new int[NUM_BONES][][];

            for (int i = 0; i < NUM_BONES; i++)
            {
                string bonePath = Path.Combine(DISTV_ROOT, String.Format(BONE_FILE_PATTERN, Wrist.ShortBoneNames[i]));
                string transformPath = Path.Combine(DISTV_ROOT, String.Format(RT_FILE_PATTERN, Wrist.ShortBoneNames[i]));
                string colorPath = Path.Combine(DISTV_ROOT, String.Format(COLOR_FILE_PATTERN, Wrist.ShortBoneNames[i]));
                _boneSeparators[i] = new Separator();
                _bones[i] = new ColoredBone(bonePath);

                _transforms[i] = new Switch();
                TransformRT[] tfrm = DatParser.parseRTFileWithHeaderToRT(transformPath);
                for (int j = 0; j < tfrm.Length; j++)
                {
                    Transform t1 = new Transform();
                    DatParser.addRTtoTransform(tfrm[j], t1);
                    _transforms[i].addChild(t1);
                }

                _colorData[i] = DatParser.parseDistvColorFile(colorPath, tfrm.Length, _bones[i].getNumberVertices());

                //TODO: Add swich in with transforms....
                _boneSeparators[i].addNode(_transforms[i]);
                _boneSeparators[i].addNode(_bones[i]);
                _root.addChild(_boneSeparators[i]);
            }

            setAllColorMaps(0);
        }

        private void setAllColorMaps(int positionIndex)
        {
            for (int i = 0; i < _bones.Length; i++)
            {
                _bones[i].setColorMap(_colorData[i][positionIndex]);
            }
        }
    }
}
