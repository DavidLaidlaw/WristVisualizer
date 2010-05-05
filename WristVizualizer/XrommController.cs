using System;
using System.Collections.Generic;
using System.Text;
using libWrist;
using libCoin3D;

namespace WristVizualizer
{
    class XrommController : Controller
    {
        private XrommFilesystem _xrommFileSys;
        private FullXromm _fullXromm;

        public XrommController(string filename)
        {
            _xrommFileSys = new XrommFilesystem(filename);
            _fullXromm = new FullXromm(_xrommFileSys);
            _fullXromm.LoadFullJoint();

        }

        public override Separator Root
        {
            get { return _fullXromm.Root; }
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
