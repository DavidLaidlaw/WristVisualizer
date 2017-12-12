using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
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
        List<Switch> _ligaments;

        Separator _root;
        int _numPositions;
        int _currentFrame;

        PosViewReader _reader;
        string _posFileName;

        //controls
        private PosViewControl _posViewControl;

        Timer _timer;

        public PosViewController(string posViewFilename, ExaminerViewer viewer)
        {
            _posFileName = posViewFilename;
            _viewer = viewer;

            _posViewControl = new PosViewControl();

            //do the hardwork and read everything
            loadPosView(posViewFilename);

            //setup the control
            _posViewControl.setupController(_numPositions, _reader.ShowHams, _reader.HasLables);
            _posViewControl.ShowHam = _reader.ShowHams;
            _posViewControl.ShowLabels = _reader.HasLables;
            _posViewControl.OverrideMaterial = _reader.SetColor;
            _posViewControl.PlayButtonEnabled = true; //we are going to start stopped
            _posViewControl.StopButtonEnabled = false;
            _posViewControl.FPS = 10; //default FPS
            setupEventListeners();

            //setup the timer
            _timer = new Timer();
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Enabled = false;  //don't start playing

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

        public override Control Control
        {
            get { return _posViewControl; }
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

        public override bool CanSaveToMovie { get { return true; } }
        public override void saveToMovie()
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
            MovieExportOptions dialog = new MovieExportOptions(_posFileName, _posViewControl.FPS);
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
                        string msg = "Error saving to movie file.\n\n" + ex.Message;
                        libWrist.ExceptionHandling.HandledExceptionManager.ShowDialog(msg, "", "", ex);
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
            if (_posViewControl.ShowHam) //only update hams if we are showing them
                for (int i = 0; i < _hamsSwitch.Length; i++)
                    _hamsSwitch[i].whichChild(_currentFrame);
            if (_posViewControl.ShowLabels) //only update label if we are showing
                _labels.whichChild(_currentFrame);
            foreach (Switch lig in _ligaments)
                lig.whichChild(_currentFrame); //update any ligaments that exist

            if (_posViewControl.currentFrame != _currentFrame)
                _posViewControl.currentFrame = _currentFrame;
        }
        #endregion

        public new void CleanUp()
        {
            _timer.Enabled = false;
            removeCallBacks();
        }

        private void setupEventListeners()
        {
            _posViewControl.TrackbarScroll += new PosViewControl.TrackbarScrollHandler(_control_TrackbarScroll);
            _posViewControl.PlayClicked += new PosViewControl.PlayClickedHandler(_control_PlayClicked);
            _posViewControl.StopClicked += new PosViewControl.StopClickedHandler(_control_StopClicked);
            _posViewControl.FPSChanged += new PosViewControl.FPSChangedHandler(_control_FPSChanged);
            _posViewControl.ShowHamClicked += new PosViewControl.ShowHamClickedHandler(_control_ShowHamClicked);
            _posViewControl.ShowPositionLabelClicked += new PosViewControl.ShowPositionLabelClickedHandler(_control_ShowPositionLabelClicked);
            _posViewControl.OverrideMaterialClicked += new PosViewControl.OverrideMaterialClickedHandler(_control_OverrideMaterialClicked);
        }

        void _control_OverrideMaterialClicked()
        {
            if (_posViewControl.OverrideMaterial)
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
            if (_posViewControl.ShowLabels)
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
            if (_posViewControl.ShowHam)
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
            setTimeToFPS(_posViewControl.FPS);
        }

        void _control_StopClicked()
        {
            _posViewControl.PlayButtonEnabled = true;
            _posViewControl.StopButtonEnabled = false;
            _posViewControl.TrackBarEnabled = true;

            _timer.Enabled = false;
        }

        void _control_PlayClicked()
        {
            _posViewControl.PlayButtonEnabled = false;
            _posViewControl.StopButtonEnabled = true;
            _posViewControl.TrackBarEnabled = false; 
            _timer.Enabled = true;
        }

        void _control_TrackbarScroll()
        {
            CurrentFrame = _posViewControl.currentFrame;
        }

        private void removeCallBacks()
        {
            _posViewControl.TrackbarScroll -= new PosViewControl.TrackbarScrollHandler(_control_TrackbarScroll);
            _posViewControl.PlayClicked -= new PosViewControl.PlayClickedHandler(_control_PlayClicked);
            _posViewControl.StopClicked -= new PosViewControl.StopClickedHandler(_control_StopClicked);
            _posViewControl.FPSChanged -= new PosViewControl.FPSChangedHandler(_control_FPSChanged);
            _posViewControl.ShowHamClicked -= new PosViewControl.ShowHamClickedHandler(_control_ShowHamClicked);
            _posViewControl.ShowPositionLabelClicked -= new PosViewControl.ShowPositionLabelClickedHandler(_control_ShowPositionLabelClicked);
            _posViewControl.OverrideMaterialClicked -= new PosViewControl.OverrideMaterialClickedHandler(_control_OverrideMaterialClicked);
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
                s.addChild(sepPosition);
                Material color = new Material();
                color.setColor(hamColors[boneIndex][0], hamColors[boneIndex][1], hamColors[boneIndex][2]);
                color.setOverride(true);
                sepPosition.addNode(color);
                HamAxis axis = new HamAxis(HAMdata[i][1], HAMdata[i][2], HAMdata[i][3], HAMdata[i][5], HAMdata[i][6], HAMdata[i][7]);
                sepPosition.addNode(axis);
                
                if (_reader.HamLength > -1) axis.SetHamLength(_reader.HamLength);
                if (_reader.HamRadius > -1) axis.SetHamRadius(_reader.HamRadius);
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
            int colorIndex = boneIndex % hamColors.Length; //prevent index exception when we have more than 15 bones. (only 15 colors defined)
            _boneMaterials[boneIndex].setColor(hamColors[colorIndex][0], hamColors[colorIndex][1], hamColors[colorIndex][2]);
            _boneMaterials[boneIndex].setOverride(true); //for this material to apply to everything below it.

            _bones[boneIndex].addFile(pos.IvFileNames[boneIndex], false);  //load bone file once, it will referenced multiple times
            if (pos.SetColor) //only insert now if the config file says to
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

        /// <summary>
        /// Fix a string which was setup using %d for sprintf in the c++ code 
        /// to use the String.Format pattern involving {0}, etc.
        /// </summary>
        /// <param name="fname"></param>
        /// <returns></returns>
        private static string reformatFiberPattern(string fname)
        {
            int index = 0;
            while (fname.IndexOf("%d") >= 0)
            {
                int pos = fname.IndexOf("%d");
                string pat = String.Format("{{{0}}}", index);
                fname = fname.Substring(0, pos) + pat + fname.Substring(pos + "%d".Length);
                index++;
            }
            return fname;
        }

        private List<Switch> setupPosViewLigaments(PosViewReader pos, int numPositions)
        {
            List<Switch> ligaments = new List<Switch>();
            if (!pos.HasLigaments) return ligaments;

            //so we actually create a switch for every ligament and ligament fiber...
            string[] ligamentNames = pos.LigamentFiberNames;
            /* we are really looping through ligaments here. Inside the loop we will
             * look for individual fibers of each ligament
             */
            foreach (string ligamentName in ligamentNames)
            {
                //first reformat the filename, so that it goes from fprint
                string ligamentPattern = Path.Combine(pos.LigamentFiberBasePath, reformatFiberPattern(ligamentName));

                //lets loop through possible fiber strings
                int fiberNumber = 1; //mike starts his counting at one
                while (File.Exists(String.Format(ligamentPattern, 1, fiberNumber)))
                {
                    Switch fiberSwitch = new Switch();
                    fiberSwitch.reference(); //prevent deletion while we work with it
                    for (int i = 0; i < numPositions; i++)
                    {
                        //since Mike uses index of 1, we need to offset the position number
                        string fname = String.Format(ligamentPattern, i+1, fiberNumber);
                        Separator streamTube = new Separator();
                        streamTube.addFile(fname);
                        fiberSwitch.addChild(streamTube);
                    }
                    fiberSwitch.whichChild(0); //start at first frame
                    fiberSwitch.unrefNoDelete();
                    ligaments.Add(fiberSwitch);

                    fiberNumber++;
                }
            }

            return ligaments;
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
                _ligaments = setupPosViewLigaments(_reader, _numPositions); //load all of the ligaments
                foreach (Switch lig in _ligaments)
                {
                    sec1.addNode(lig); //add to scenegraph
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

        public static bool IsPosViewFile(string[] filename)
        {
            return filename.Length == 1 && IsPosViewFile(filename[0]);
        }
        public static bool IsPosViewFile(string filename)
        {
            /* Path.GetExtension is now returning '.pos', but the code had existed for 'pos'. 
             * I'm not certain if an older version did not include the '.', but for now I am
             * leaving both, "just in case". Most likely I can delete the first case though
             */
            return Path.GetExtension(filename).Equals("pos", StringComparison.InvariantCultureIgnoreCase) ||
                Path.GetExtension(filename).Equals(".pos", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
