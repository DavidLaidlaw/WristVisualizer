using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using libWrist;
using libCoin3D;

namespace WristVizualizer
{
    class XrommController : Controller
    {
        private XrommFilesystem _xrommFileSys;
        private FullXromm _fullXromm;

        private PosViewControl _posViewControl;

        public XrommController(string filename)
        {
            _xrommFileSys = new XrommFilesystem(filename);
            _fullXromm = new FullXromm(_xrommFileSys);
            _fullXromm.LoadFullJoint();


            //TEMP STUFF
            _posViewControl = new PosViewControl();
            _posViewControl.setupController(_fullXromm.NumberPositions, false, false);
            _posViewControl.ShowHam = false;
            _posViewControl.ShowLabels = false;
            _posViewControl.OverrideMaterial = false;
            _posViewControl.PlayButtonEnabled = true; //we are going to start stopped
            _posViewControl.StopButtonEnabled = false;
            _posViewControl.FPS = 10; //default FPS

            _posViewControl.TrackbarScroll += new PosViewControl.TrackbarScrollHandler(_posViewControl_TrackbarScroll);

        }

        void _posViewControl_TrackbarScroll()
        {
            _fullXromm.HideBonesWithNoKinematics(_posViewControl.currentFrame);
            _fullXromm.MoveToPositionAndFixedBone(_posViewControl.currentFrame, 0);
        }

        public override Separator Root
        {
            get { return _fullXromm.Root; }
        }

        public override Control Control
        {
            get { return _posViewControl; }
        }

        //private void readAllFiles()
        //{
        //    foreach (string ivFile in _xrommFileSys.bpaths)
        //        _root.addFile(ivFile);
        //}


        //public override string ApplicationTitle { get { return _firstFilename; } }
        //public override string WatchedFileFilename { get { return _firstFilename; } }
        public override string LastFileFilename { get { return _xrommFileSys.PathFirstIVFile; } }
    }
}
