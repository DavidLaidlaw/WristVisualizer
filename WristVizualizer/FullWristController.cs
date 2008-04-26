using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
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

        private Separator _root;

        private FullWristControl _control;

        public FullWristController()
        {
        }

        private void loadFullWrist(string radiusFilename)
        {
            //TODO: ShowFullWristControlBox
            //TODO: Block importing a file
            //TODO: Block viewSource
            //TODO: Enable ShowInertia
            //TODO: Enable ShowACS
            _root = new Separator();


            //First Try and load the wrist data
            _wrist = new Wrist();
            try
            {
                _wrist.setupWrist(radiusFilename);
                _transforms = DatParser.makeAllTransforms(_wrist.motionFiles, _bnames.Length);
                populateSeriesList();
            }
            catch (ArgumentException ex)
            {
                if (!hideErrorMessagesToolStripMenuItem.Checked)
                {
                    string msg = "Error loading wrist kinematics.\n\n" + ex.Message;
                    //TODO: Change to abort,retry, and find way of cancelling load
                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                for (int i = 0; i < _bnames.Length; i++)
                    _control.disableFixedBone(i);
            }

            for (int i = 0; i < _bnames.Length; i++)
            {
                string fname = _wrist.bpaths[i];
                if (File.Exists(fname))
                {
                    _bones[i] = new Separator();
                    _bones[i].addFile(fname, true);
                    _root.addChild(_bones[i]);
                }
                else
                {
                    _bones[i] = null;
                    _control.disableBone(i);
                }
            }
        }

        private void populateSeriesList()
        {
            _currentPositionIndex = 0;
            _control.clearSeriesList();
            _control.addToSeriesList(_wrist.neutralSeries);
            _control.addToSeriesList(_wrist.series);
            _control.selectedSeriesIndex = 0;
        }

        public string getTitleCaption()
        {
            return _wrist.subject + _wrist.side + " - " + _wrist.subjectPath;
        }

        public string getFilenameOfFirstFile()
        {
            return Path.Combine(_wrist.subjectPath, _wrist.subject + _wrist.side);
        }
    }
}
