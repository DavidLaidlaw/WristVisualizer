using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WristVizualizer
{
    public partial class AnimationControl : UserControl
    {
        public delegate void PlayClickedHandler();
        public delegate void StopClickedHandler();
        public delegate void FPSChangedHandler();
        public delegate void TrackbarScrollHandler();

        public event PlayClickedHandler PlayClicked;
        public event StopClickedHandler StopClicked;
        public event FPSChangedHandler FPSChanged;
        public event TrackbarScrollHandler TrackbarScroll;

        public AnimationControl()
        {
            InitializeComponent();
        }

        public void setupController(int numberOfFrames, bool hasHAMAxes, bool hasLabels)
        {
            trackBarCurrentFrame.Minimum = 0;
            trackBarCurrentFrame.Maximum = numberOfFrames - 1;
            trackBarCurrentFrame.Value = 0;

            buttonPlay.Enabled = true;
            buttonStop.Enabled = true;
        }

        #region Interfaces
        public decimal FPS
        {
            get { return numericUpDownFPS.Value; }
            set { numericUpDownFPS.Value = value; }
        }

        public int currentFrame
        {
            get { return trackBarCurrentFrame.Value; }
            set { trackBarCurrentFrame.Value = value; }
        }

        public bool PlayButtonEnabled
        {
            get { return buttonPlay.Enabled; }
            set { buttonPlay.Enabled = value; }
        }

        public bool StopButtonEnabled
        {
            get { return buttonStop.Enabled; }
            set { buttonStop.Enabled = value; }
        }

        public bool TrackBarEnabled
        {
            get { return trackBarCurrentFrame.Enabled; }
            set { trackBarCurrentFrame.Enabled = value; }
        }
        #endregion

        #region Event Passing
        private void buttonPlay_Click(object sender, EventArgs e)
        {
            if (PlayClicked != null)
                PlayClicked();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (StopClicked != null)
                StopClicked();
        }

        private void trackBarCurrentFrame_Scroll(object sender, EventArgs e)
        {
            if (TrackbarScroll != null)
                TrackbarScroll();
        }

        private void numericUpDownFPS_ValueChanged(object sender, EventArgs e)
        {
            if (FPSChanged != null)
                FPSChanged();
        }
        #endregion
    }
}
