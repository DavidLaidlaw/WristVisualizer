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
        private int _totalParts;
        private int _currentPart;



        public BackgroundWorkerStatusForm(int TotalNumberParts)
        {
            InitializeComponent();

            this.progressBar.Value = 0;
            _currentPart = 0;
            _totalParts = TotalNumberParts;
        }
        
        public void UnSafeProgressUpdate(double percent)
        {
            this.progressBar.Value = Math.Min((int)percent, 100);
        }

        public void UnSafeIncrimentCompletedParts()
        {
            _currentPart++;
            UnSafeProgressUpdate(100.0 * _currentPart / _totalParts);
        }

    }
}