using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using libWrist;
using libCoin3D;

namespace WristVizualizer
{
    class PosViewController
    {
        ExaminerViewer _viewer;
        Switch[] _bonesSwitch;
        Switch[] _hamsSwitch;
        Separator[] _bones;
        Material[] _boneMaterials;
        Switch _labels;

        Separator _root;
        int _numPositions;
        int _currentFrame;

        PosViewReader _reader;
        string _posFileName;

        //controls
        TrackBar trackBarPosViewCurrentFrame;
        Button buttonPosViewPlay;
        Button buttonPosViewStop;
        NumericUpDown numericUpDownPosViewFPS;
        CheckBox checkBoxShowHams;
        CheckBox checkBoxOverrideMaterial;
        CheckBox checkBoxShowLabels;

        Timer _timer;

        public PosViewController(string posViewFilename, ExaminerViewer viewer)
        {
            _posFileName = posViewFilename;
            _viewer = viewer;
            loadPosView(posViewFilename);
            _timer = new Timer();
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Enabled = false;

            _viewer.setSceneGraph(_root);
            _viewer.viewAll();
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
                buttonPosViewPlay.Enabled = true;
                buttonPosViewPlay.Click += new EventHandler(buttonPosViewPlay_Click);
            }
        }
        public Button Control_StopButton
        {
            set
            {
                buttonPosViewStop = value;
                buttonPosViewStop.Enabled = false;
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
        public CheckBox Control_ShowHAMS
        {
            set
            {
                checkBoxShowHams = value;
                checkBoxShowHams.Enabled = _reader.ShowHams;
                checkBoxShowHams.Checked = _reader.ShowHams;
                checkBoxShowHams.CheckedChanged += new EventHandler(checkBoxShowHams_CheckedChanged);
            }
        }
        public CheckBox Control_OverrideMaterial
        {
            set
            {
                checkBoxOverrideMaterial = value;
                checkBoxOverrideMaterial.Enabled = true;
                checkBoxOverrideMaterial.Checked = _reader.SetColor;
                checkBoxOverrideMaterial.CheckedChanged += new EventHandler(checkBoxOverrideMaterial_CheckedChanged);
            }
        }
        public CheckBox Control_ShowLabels
        {
            set
            {
                checkBoxShowLabels = value;
                checkBoxShowLabels.Enabled = _reader.HasLables;
                checkBoxShowLabels.Checked = _reader.HasLables;
                checkBoxShowLabels.CheckedChanged += new EventHandler(checkBoxShowLabels_CheckedChanged);
            }
        }
        #endregion

        void checkBoxShowLabels_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxShowLabels.Checked)
            {
                //then we need to show them
                _labels.whichChild(_currentFrame);
            }
            else
            {
                //lets hide it
                _labels.hideAll();
            }
        }

        void checkBoxOverrideMaterial_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxOverrideMaterial.Checked)
            {
                //then lets insert that material!
                for (int i = 0; i < _bones.Length; i++)
                    _bones[i].insertNode(_boneMaterials[i], 0);
            }
            else
            {
                //need to go and remove all the material nodes!
                for (int i = 0; i < _bones.Length; i++)
                    _bones[i].removeChild(_boneMaterials[i]);
            }
        }

        void checkBoxShowHams_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxShowHams.Checked)
            {
                //lets show them all
                foreach (Switch ham in _hamsSwitch)
                    ham.whichChild(_currentFrame);
            }
            else
            {
                foreach (Switch ham in _hamsSwitch)
                    ham.hideAll();
            }
        }

        void trackBarPosViewCurrentFrame_Scroll(object sender, EventArgs e)
        {
            CurrentFrame = trackBarPosViewCurrentFrame.Value;
        }

        void buttonPosViewPlay_Click(object sender, EventArgs e)
        {
            buttonPosViewPlay.Enabled = false;
            buttonPosViewStop.Enabled = true;
            trackBarPosViewCurrentFrame.Enabled = false;

            _timer.Enabled = true;
        }

        void buttonPosViewStop_Click(object sender, EventArgs e)
        {
            buttonPosViewPlay.Enabled = true;
            buttonPosViewStop.Enabled = false;
            trackBarPosViewCurrentFrame.Enabled = true;

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

        public void saveToMovie()
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.SelectedPath = System.IO.Path.GetDirectoryName(_posFileName);
#if DEBUG
            fb.SelectedPath = @"C:\Temp\testMovie";
#endif
            fb.ShowNewFolderButton = true;
            fb.Description = "Choose directory to output movie frames to";
            if (DialogResult.OK != fb.ShowDialog())
                return;

            //save starting state
            bool startPlaying = _timer.Enabled;
            int startFrame = _currentFrame;
            _timer.Enabled = false;
            //TODO: Disable buttons....

            string outputDir = fb.SelectedPath;
            //save it to a movie
            for (int i = 0; i < NumPositions; i++)
            {
                CurrentFrame = i;  //change to current frame
                string fname = Path.Combine(outputDir,String.Format("outfile{0:d3}.jpg",i));
                _viewer.saveToJPEG(fname);
            }

            //wrap us up
            CurrentFrame = startFrame;
            _timer.Enabled = startPlaying;

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
            for (int i = 0; i < _bonesSwitch.Length; i++)
                _bonesSwitch[i].whichChild(_currentFrame);
            if (checkBoxShowHams.Checked) //only update hams if we are showing them
                for (int i = 0; i < _hamsSwitch.Length; i++)
                    _hamsSwitch[i].whichChild(_currentFrame);
            if (checkBoxShowLabels.Checked) //only update label if we are showing
                _labels.whichChild(_currentFrame);

            if (trackBarPosViewCurrentFrame.Value != _currentFrame)
                trackBarPosViewCurrentFrame.Value = _currentFrame;
        }
        #endregion

        public void Close()
        {
            _timer.Enabled = false;
            removeCallBacks();
        }

        private void removeCallBacks()
        {
            trackBarPosViewCurrentFrame.Scroll -= new EventHandler(trackBarPosViewCurrentFrame_Scroll);
            buttonPosViewPlay.Click -= new EventHandler(buttonPosViewPlay_Click);
            buttonPosViewStop.Click -= new EventHandler(buttonPosViewStop_Click);
            numericUpDownPosViewFPS.ValueChanged -= new EventHandler(numericUpDownPosViewFPS_ValueChanged);
            checkBoxShowHams.CheckedChanged -= new EventHandler(checkBoxShowHams_CheckedChanged);
            checkBoxOverrideMaterial.CheckedChanged -= new EventHandler(checkBoxOverrideMaterial_CheckedChanged);
            checkBoxShowLabels.CheckedChanged -= new EventHandler(checkBoxShowLabels_CheckedChanged);
        }

        private Switch setupPosViewLables(PosViewReader pos)
        {
            _labels = new Switch();
            if (!pos.HasLables)
                return _labels;

            for (int i = 0; i < pos.Labels.Length; i++)
            {
                Label2D l = new Label2D();
                l.setText(pos.Labels[i]);
                l.setLocation(-0.9f, 0.9f);
                l.setFontSize(20);
                _labels.addChild(l);
            }
            return _labels;
        }

        private Switch setupPosViewHAMs(PosViewReader pos, int boneIndex)
        {
            Switch s = new Switch();
            if (!pos.ShowHams)
                return s;

            double[][] HAMdata = DatParser.parsePosViewHAMFile(pos.HAMFileNames[boneIndex]);
            float[][] hamColors = PosViewSettings.PosViewColors;
            for (int i = 0; i < HAMdata.Length; i++)
            {
                Separator sepPosition = new Separator();
                Material color = new Material();
                color.setColor(hamColors[boneIndex][0], hamColors[boneIndex][1], hamColors[boneIndex][2]);
                color.setOverride(true);
                sepPosition.addNode(color);
                HamAxis axis = new HamAxis(HAMdata[i][1], HAMdata[i][2], HAMdata[i][3], HAMdata[i][5], HAMdata[i][6], HAMdata[i][7]);
                sepPosition.addNode(axis);
                s.addChild(sepPosition);
            }
            s.whichChild(0); //set it to start at the first frame
            return s;
        }

        private Switch setupPosViewBone(PosViewReader pos, int boneIndex)
        {
            Switch s = new Switch();
            _bones[boneIndex] = new Separator();
            //create a material for that bone!
            _boneMaterials[boneIndex] = new Material();
            float[][] hamColors = PosViewSettings.PosViewColors;
            _boneMaterials[boneIndex].setColor(hamColors[boneIndex][0], hamColors[boneIndex][1], hamColors[boneIndex][2]);
            _boneMaterials[boneIndex].setOverride(true); //for this material to apply to everything below it.

            _bones[boneIndex].addFile(pos.IvFileNames[boneIndex], false);  //load bone file once, it will referenced multiple times
            _bones[boneIndex].insertNode(_boneMaterials[boneIndex], 0);

            Transform[] transforms = DatParser.parsePosViewRTFileToTransforms(pos.RTFileNames[boneIndex]);
            _numPositions = transforms.Length;
            for (int i = 0; i < transforms.Length; i++)
            {
                Separator sepPosition = new Separator();
                sepPosition.addNode(transforms[i]);
                sepPosition.addChild(_bones[boneIndex]);
                s.addChild(sepPosition);
            }
            s.whichChild(0); //set it to start at the first frame
            return s;
        }

        private void loadPosView(string posViewFilename)
        {
            _root = new Separator();
            Separator sec1 = new Separator();
            Separator sec2 = new Separator();
            _root.addNode(sec1);
            _root.addNode(sec2);
            sec1.addNode(new Camera());
            sec2.addNode(new Camera());
            _currentFrame = 0;
            try
            {
                _reader = new PosViewReader(posViewFilename);
                _bonesSwitch = new Switch[_reader.NumBones];
                _hamsSwitch = new Switch[_reader.NumBones];
                _bones = new Separator[_reader.NumBones];
                _boneMaterials = new Material[_reader.NumBones];
                //_root = new Separator();
                for (int i = 0; i < _reader.NumBones; i++)
                {
                    _bonesSwitch[i] = setupPosViewBone(_reader, i);
                    _hamsSwitch[i] = setupPosViewHAMs(_reader, i);
                    sec1.addNode(_bonesSwitch[i]);
                    sec1.addNode(_hamsSwitch[i]);
                }
                if (_reader.HasLables)
                {
                    setupPosViewLables(_reader);
                    sec2.addNode(_labels);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
