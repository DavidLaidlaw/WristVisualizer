using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
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

        private const int NUM_BONES = 4; //TODO: should be 10 :)

        private int _numPositions;

        private Separator _root;
        private Separator[] _boneSeparators;
        private ColoredBone[] _bones;
        private Switch[] _transformsSwitch;
        private Switch[] _inverseTransformsSwitch;
        private int[][][] _colorData;

        private FullWristControl _wristControl;
        private AnimationControl _animationControl;

        //save state information
        private Switch _currentRelativeBoneInverseSwitch;

        public DistvController()
        {
            _root = new Separator();

            setupDistv();
            setupControl();
        }

        private void setupControl()
        {
            _wristControl = new FullWristControl();
            _wristControl.setupControl(Wrist.LongBoneNames, false);
            //disable long bones
            for (int i = 10; i < Wrist.NumBones; i++)
                _wristControl.disableBone(i);

            WristPanelLayoutControl p = new WristPanelLayoutControl();
            p.addControl(_wristControl);

            _animationControl = new AnimationControl();
            _animationControl.setupController(_numPositions);
            p.addControl(_animationControl);

            //save control
            _control = p;

            setupEventListeners();
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
            _transformsSwitch = new Switch[NUM_BONES];
            _inverseTransformsSwitch = new Switch[NUM_BONES];
            _colorData = new int[NUM_BONES][][];

            for (int i = 0; i < NUM_BONES; i++)
            {
                string bonePath = Path.Combine(DISTV_ROOT, String.Format(BONE_FILE_PATTERN, Wrist.ShortBoneNames[i]));
                string transformPath = Path.Combine(DISTV_ROOT, String.Format(RT_FILE_PATTERN, Wrist.ShortBoneNames[i]));
                string colorPath = Path.Combine(DISTV_ROOT, String.Format(COLOR_FILE_PATTERN, Wrist.ShortBoneNames[i]));
                _boneSeparators[i] = new Separator();
                _bones[i] = new ColoredBone(bonePath);

                _transformsSwitch[i] = new Switch();
                _inverseTransformsSwitch[i] = new Switch();
                TransformRT[] tfrm = DatParser.parseRTFileWithHeaderToRT(transformPath);
                for (int j = 0; j < tfrm.Length; j++)
                {
                    Transform t1 = new Transform();
                    DatParser.addRTtoTransform(tfrm[j], t1);
                    _transformsSwitch[i].addChild(t1);

                    //now the inverse to allow us to fix another bone
                    Transform t2 = new Transform();
                    DatParser.addRTtoTransform(tfrm[j], t2);
                    t2.invert();
                    _inverseTransformsSwitch[i].addChild(t2);
                }
                _numPositions = tfrm.Length; //TODO: Check that all are the same...

                _colorData[i] = DatParser.parseDistvColorFile(colorPath, tfrm.Length, _bones[i].getNumberVertices());

                //TODO: Add swich in with transforms....
                _boneSeparators[i].addNode(_transformsSwitch[i]);
                _boneSeparators[i].addNode(_bones[i]);
                _root.addChild(_boneSeparators[i]);
            }

            setAllColorMaps(0);
        }

        private void updateFrame(int frame)
        {
            for (int i = 0; i < _bones.Length; i++)
                _transformsSwitch[i].whichChild(frame);
            setAllColorMaps(frame);
            if (_currentRelativeBoneInverseSwitch != null)
                _currentRelativeBoneInverseSwitch.whichChild(frame);
        }

        private void setAllColorMaps(int positionIndex)
        {
            for (int i = 0; i < _bones.Length; i++)
            {
                _bones[i].setColorMap(_colorData[i][positionIndex]);
            }
        }

        private void setupEventListeners()
        {
            _wristControl.BoneHideChanged += new BoneHideChangedHandler(_wristControl_BoneHideChanged);
            _wristControl.FixedBoneChanged += new FixedBoneChangedHandler(_wristControl_FixedBoneChanged);
            _animationControl.PlayClicked += new AnimationControl.PlayClickedHandler(_animationControl_PlayClicked);
            _animationControl.StopClicked += new AnimationControl.StopClickedHandler(_animationControl_StopClicked);
            _animationControl.FPSChanged += new AnimationControl.FPSChangedHandler(_animationControl_FPSChanged);
            _animationControl.TrackbarScroll += new AnimationControl.TrackbarScrollHandler(_animationControl_TrackbarScroll);
        }

        void _animationControl_TrackbarScroll()
        {
            int frame = _animationControl.currentFrame;
            updateFrame(frame);
        }

        void _animationControl_FPSChanged()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void _animationControl_StopClicked()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void _animationControl_PlayClicked()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void _wristControl_FixedBoneChanged(object sender, FixedBoneChangeEventArgs e)
        {
            //check for existing inverse swtich
            if (_currentRelativeBoneInverseSwitch != null)
            {
                _root.removeChild(_currentRelativeBoneInverseSwitch);
            }

            //if this is the 0 index bone, then we don't do anything :)
            if (e.BoneIndex == 0)
                return;

            //now lets add in the new one.
            _root.insertNode(_inverseTransformsSwitch[e.BoneIndex],0);
            _currentRelativeBoneInverseSwitch = _inverseTransformsSwitch[e.BoneIndex];
            int currentFrame = _animationControl.currentFrame;
            _currentRelativeBoneInverseSwitch.whichChild(currentFrame);
        }

        void _wristControl_BoneHideChanged(object sender, BoneHideChangeEventArgs e)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
