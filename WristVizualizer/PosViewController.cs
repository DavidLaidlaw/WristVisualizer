using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using libWrist;
using libCoin3D;

namespace WristVizualizer
{
    class PosViewController
    {
        Switch[] _bones;
        Separator _root;
        int _numPositions;
        int _currentFrame;

        int _fps;

        //controls
        TrackBar trackBarPosViewCurrentFrame;
        Button buttonPosViewPlay;
        Button buttonPosViewStop;
        NumericUpDown numericUpDownPosViewFPS;

        Timer _timer;

        public PosViewController(string posViewFilename)
        {
            loadPosView(posViewFilename);
            _timer = new Timer();
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Enabled = false;
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            advanceOneFrame();
        }

        #region Interfaces
        public Separator Root
        {
            get { return _root; }
        }

        public int NumPositions
        {
            get { return _numPositions; }
        }

        public int CurrentFrame
        {
            get { return _currentFrame; }
            set
            {
                _currentFrame = value;
                updateFrame();
            }
        }

        public TrackBar Control_CurrentFrame
        {
            set
            {
                trackBarPosViewCurrentFrame = value;
                trackBarPosViewCurrentFrame.Scroll += new EventHandler(trackBarPosViewCurrentFrame_Scroll);
                trackBarPosViewCurrentFrame.Maximum = _numPositions - 1;
                trackBarPosViewCurrentFrame.Minimum = 0;
                trackBarPosViewCurrentFrame.Value = 0;
            }
        }
        public Button Control_PlayButton
        {
            set
            {
                buttonPosViewPlay = value;
                buttonPosViewPlay.Click += new EventHandler(buttonPosViewPlay_Click);
            }
        }
        public Button Control_StopButton
        {
            set
            {
                buttonPosViewStop = value;
                buttonPosViewStop.Click += new EventHandler(buttonPosViewStop_Click);
            }
        }
        public NumericUpDown Control_NumericFPS
        {
            set
            {
                numericUpDownPosViewFPS = value;
                numericUpDownPosViewFPS.Value = 10;
                numericUpDownPosViewFPS.ValueChanged += new EventHandler(numericUpDownPosViewFPS_ValueChanged);
            }
        }
        #endregion

        void trackBarPosViewCurrentFrame_Scroll(object sender, EventArgs e)
        {
            CurrentFrame = trackBarPosViewCurrentFrame.Value;
        }

        void buttonPosViewPlay_Click(object sender, EventArgs e)
        {
            buttonPosViewPlay.Enabled = false;
            buttonPosViewStop.Enabled = true;

            _timer.Enabled = true;
        }

        void buttonPosViewStop_Click(object sender, EventArgs e)
        {
            buttonPosViewPlay.Enabled = true;
            buttonPosViewStop.Enabled = false;

            _timer.Enabled = false;
        }

        void numericUpDownPosViewFPS_ValueChanged(object sender, EventArgs e)
        {
            setTimeToFPS();
        }

        private void setTimeToFPS()
        {
            //need to convert from FPS -> Miliseconds/frame
            _timer.Interval = (int)(1000 / (double)numericUpDownPosViewFPS.Value);
        }

        #region FrameControl
        public void advanceOneFrame()
        {
            _currentFrame++;
            if (_currentFrame >= _numPositions)
                _currentFrame = 0;
            updateFrame();
        }

        public void rewindOneFrame()
        {
            _currentFrame--;
            if (_currentFrame < 0)
                _currentFrame = _numPositions - 1;
            updateFrame();
        }

        private void updateFrame()
        {
            for (int i = 0; i < _bones.Length; i++)
                _bones[i].whichChild(_currentFrame);

            if (trackBarPosViewCurrentFrame.Value != _currentFrame)
                trackBarPosViewCurrentFrame.Value = _currentFrame;
        }
        #endregion

        private Switch setupPosViewBone(PosViewReader pos, int boneIndex)
        {
            Switch s = new Switch();
            Separator bone = new Separator();
            bone.addFile(pos.IvFileNames[boneIndex], false);
            Transform[] transforms = DatParser.parsePosViewRTFileToTransforms(pos.RTFileNames[boneIndex]);
            _numPositions = transforms.Length;
            for (int i = 0; i < transforms.Length; i++)
            {
                Separator sepPosition = new Separator();
                sepPosition.addNode(transforms[i]);
                sepPosition.addChild(bone);
                s.addChild(sepPosition);
            }
            s.whichChild(0); //set it to start at the first frame
            return s;
        }

        private void loadPosView(string posViewFilename)
        {
            _root = new Separator();
            _currentFrame = 0;
            try
            {
                PosViewReader reader = new PosViewReader(posViewFilename);
                _bones = new Switch[reader.NumBones];
                _root = new Separator();
                for (int i = 0; i < reader.NumBones; i++)
                {
                    _bones[i] = setupPosViewBone(reader, i);
                    _root.addNode(_bones[i]);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
