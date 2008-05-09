using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace libWrist
{
    public partial class BackgroundWorkerStatusForm : Form
    {
        BackgroundWorker _worker;
        private int _numParts;
        private int _currentPart;

        private DistanceMaps _distance;
        private bool _loadColor;
        private bool _loadContour;
        private Modes _mode;


        private enum Modes
        {
            DistanceFieldCalculation,
            Nothing
        }

        public BackgroundWorkerStatusForm()
        {
            InitializeComponent();
            InitializeBackgoundWorker();
            this.progressBar.Value = 0;
            _currentPart = 0;
        }

        private void InitializeBackgoundWorker()
        {
            _worker = new BackgroundWorker();
            _worker.WorkerReportsProgress = true;
            _worker.WorkerSupportsCancellation = false;
            _worker.DoWork += new DoWorkEventHandler(_worker_DoWork);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_worker_RunWorkerCompleted);
            _worker.ProgressChanged += new ProgressChangedEventHandler(_worker_ProgressChanged);
        }

        public void processDistanceFieldCalculations(DistanceMaps distMaps, bool loadAllColorMaps, bool loadAllContours)
        {
            if (!loadAllColorMaps && !loadAllContours) //check if nothing to do....
                return;
            _mode = Modes.DistanceFieldCalculation;
            _distance = distMaps;
            _loadColor = loadAllColorMaps;
            _loadContour = loadAllContours;

            if (loadAllColorMaps && loadAllContours)
                _numParts = 2;
            else
                _numParts = 1;

            _worker.RunWorkerAsync();
            this.ShowDialog();
        }

        public void SafeProgressUpdate(double percent)
        {
            _worker.ReportProgress((int)percent);
        }

        void _worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar.Value = e.ProgressPercentage;
        }

        void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //we are all set, we should be able to close this form and be good... yes?
            this.Close();
        }

        void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            switch(_mode)
            {
                case Modes.DistanceFieldCalculation:
                    //execute this mode
                    if (_loadColor)
                        _distance.readInAllDistanceColorMaps(this);
                    if (_loadContour)
                        _distance.calculateAllContours(this);
                    break;
                case Modes.Nothing:
                    break;
            }
        }


    }
}