using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using libWrist;
using libCoin3D;
using AviFile;

namespace WristVizualizer
{
    class PosViewController : Controller
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
        private PosViewControl _control;

        Timer _timer;

        public PosViewController(string posViewFilename, ExaminerViewer viewer)
        {
            _posFileName = posViewFilename;
            _viewer = viewer;

            _control = new PosViewControl();

            //do the hardwork and read everything
            loadPosView(posViewFilename);

            //setup the control
            _control.setupController(_numPositions, _reader.ShowHams, _reader.HasLables);
            _control.ShowHam = _reader.ShowHams;
            _control.ShowLabels = _reader.HasLables;
            _control.OverrideMaterial = _reader.SetColor;
            _control.PlayButtonEnabled = false; //we are going to start playing now
            _control.FPS = 10; //default FPS
            setupEventListeners();

            //setup the timer
            _timer = new Timer();
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Enabled = false;

            _viewer.setSceneGraph(_root);
            _viewer.viewAll(); //move camera so the whole scene can be viewed
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            advanceOneFrame();
        }

        #region Interfaces
        public override Separator Root
        {
            get { return _root; }
        }

        public override UserControl Control
        {
            get { return _control; }
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
        #endregion

        private void setTimeToFPS(decimal FPS)
        {
            //need to convert from FPS -> Miliseconds/frame
            _timer.Interval = (int)(1000 / (double)FPS);
        }

        public void saveToMovie()
        {
            //save starting state & stop playback
            bool startPlaying = _timer.Enabled;
            int startFrame = _currentFrame;
            _timer.Enabled = false;
            //TODO: Disable buttons....

            //CurrentFrame = 0;
            //_viewer.saveToJPEG(@"C:\Temp\testMovie\test1.jpg");
            //System.Drawing.Image im = _viewer.getImage();
            //im.Save(@"C:\Temp\testMovie\test2.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            ////debug.
            //return;

            //show save dialogue
            MovieExportOptions dialog = new MovieExportOptions(_posFileName, _control.FPS);
            dialog.ShowDialog();

            //okay, now lets figure out what we are doing here
            switch (dialog.results)
            {
                case MovieExportOptions.SaveType.CANCEL:
                    //nothing to do, we were canceled
                    break;
                case MovieExportOptions.SaveType.IMAGES:
                    //save images
                    string outputDir = dialog.OutputFileName;
                    //save it to a movie
                    for (int i = 0; i < NumPositions; i++)
                    {
                        CurrentFrame = i;  //change to current frame
                        string fname = Path.Combine(outputDir, String.Format("outfile{0:d3}.jpg", i));
                        _viewer.saveToJPEG(fname);
                    }
                    break;
                case MovieExportOptions.SaveType.MOVIE:
                    //save movie
                    try
                    {
                        AviManager aviManager = new AviManager(dialog.OutputFileName, false);
                        CurrentFrame = 0; //set to first frame, so we can grab it.
                        System.Drawing.Bitmap frame = (System.Drawing.Bitmap)_viewer.getImage();
                        VideoStream vStream = aviManager.AddVideoStream(dialog.MovieCompress, (double)dialog.MovieFPS, frame);
                        for (int i = 1; i < NumPositions; i++) //start from frame 1, frame 0 was added when we began
                        {
                            CurrentFrame = i;  //change to current frame
                            vStream.AddFrame((System.Drawing.Bitmap)_viewer.getImage());
                        }
                        aviManager.Close();  //close out and save
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error saving to movie file.\n\n" + ex.Message);
                    }
                    break;
            }

            //wrap us up, resume frame and playing status
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
            if (_control.ShowHam) //only update hams if we are showing them
                for (int i = 0; i < _hamsSwitch.Length; i++)
                    _hamsSwitch[i].whichChild(_currentFrame);
            if (_control.ShowLabels) //only update label if we are showing
                _labels.whichChild(_currentFrame);

            if (_control.currentFrame != _currentFrame)
                _control.currentFrame = _currentFrame;
        }
        #endregion

        public new void CleanUp()
        {
            _timer.Enabled = false;
            removeCallBacks();
        }

        private void setupEventListeners()
        {
            _control.TrackbarScroll += new PosViewControl.TrackbarScrollHandler(_control_TrackbarScroll);
            _control.PlayClicked += new PosViewControl.PlayClickedHandler(_control_PlayClicked);
            _control.StopClicked += new PosViewControl.StopClickedHandler(_control_StopClicked);
            _control.FPSChanged += new PosViewControl.FPSChangedHandler(_control_FPSChanged);
            _control.ShowHamClicked += new PosViewControl.ShowHamClickedHandler(_control_ShowHamClicked);
            _control.ShowPositionLabelClicked += new PosViewControl.ShowPositionLabelClickedHandler(_control_ShowPositionLabelClicked);
            _control.OverrideMaterialClicked += new PosViewControl.OverrideMaterialClickedHandler(_control_OverrideMaterialClicked);
        }

        void _control_OverrideMaterialClicked()
        {
            if (_control.OverrideMaterial)
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

        void _control_ShowPositionLabelClicked()
        {
            if (_control.ShowLabels)
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

        void _control_ShowHamClicked()
        {
            if (_control.ShowHam)
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

        void _control_FPSChanged()
        {
            setTimeToFPS(_control.FPS);
        }

        void _control_StopClicked()
        {
            _control.PlayButtonEnabled = true;
            _control.StopButtonEnabled = false;
            _control.TrackBarEnabled = true;

            _timer.Enabled = false;
        }

        void _control_PlayClicked()
        {
            _control.PlayButtonEnabled = false;
            _control.StopButtonEnabled = true;
            _control.TrackBarEnabled = false; 
            _timer.Enabled = true;
        }

        void _control_TrackbarScroll()
        {
            CurrentFrame = _control.currentFrame;
        }

        private void removeCallBacks()
        {
            _control.TrackbarScroll -= new PosViewControl.TrackbarScrollHandler(_control_TrackbarScroll);
            _control.PlayClicked -= new PosViewControl.PlayClickedHandler(_control_PlayClicked);
            _control.StopClicked -= new PosViewControl.StopClickedHandler(_control_StopClicked);
            _control.FPSChanged -= new PosViewControl.FPSChangedHandler(_control_FPSChanged);
            _control.ShowHamClicked -= new PosViewControl.ShowHamClickedHandler(_control_ShowHamClicked);
            _control.ShowPositionLabelClicked -= new PosViewControl.ShowPositionLabelClickedHandler(_control_ShowPositionLabelClicked);
            _control.OverrideMaterialClicked -= new PosViewControl.OverrideMaterialClickedHandler(_control_OverrideMaterialClicked);
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

            s.reference();
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
            s.unrefNoDelete();
            return s;
        }

        private Switch setupPosViewBone(PosViewReader pos, int boneIndex)
        {
            Switch s = new Switch();
            s.reference();
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
            s.unrefNoDelete();
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
            catch
            {
                throw;
            }
        }
    }
}
