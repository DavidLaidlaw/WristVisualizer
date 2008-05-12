using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace libWrist
{
    /* Added DebuggerDisplay to fix problem when debugging multi-threaded processes in VS.NET 2005.
     * Fix based on this article: http://blogs.msdn.com/greggm/archive/2005/11/18/494648.aspx
     */
    [System.Diagnostics.DebuggerDisplay("BackgroundWorkerStatusForm")]
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
            int totalPercent = (int)((_currentPart * 100 + percent) / _numParts);
            _worker.ReportProgress(Math.Min(totalPercent,100));
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
                    {
                        _distance.readInAllDistanceColorMaps(this);
                        _currentPart++;
                    }
                    if (_loadContour)
                    {
                        _distance.calculateAllContours(this);
                        _currentPart++;
                    }
                    break;
                case Modes.Nothing:
                    break;
            }
        }


    }
}