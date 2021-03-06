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
        public event EventHandler Tick;

        private Timer _animationTimer;

        public AnimationControl()
        {
            InitializeComponent();
        }

        public void setupController(int numberOfFrames)
        {
            trackBarCurrentFrame.Minimum = 0;
            trackBarCurrentFrame.Maximum = numberOfFrames - 1;
            trackBarCurrentFrame.Value = 0;

            buttonPlay.Enabled = true;
            buttonStop.Enabled = true;
        }

        public void EnableInternalTimer()
        {
            _animationTimer = new Timer();
            _animationTimer.Interval = (int)(1000 / (double)numericUpDownFPS.Value);
            _animationTimer.Tick += new EventHandler(_animationTimer_Tick);
        }

        public void StopTimer()
        {
            _animationTimer.Stop();
        }

        public void StartTimer()
        {
            _animationTimer.Start();
        }

        public void AdvanceCurrentFrameTrackbar()
        {
            int current = trackBarCurrentFrame.Value;
            current++;
            if (current > trackBarCurrentFrame.Maximum)
                current = 0;
            trackBarCurrentFrame.Value = current;
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
            set
            {
                //check for a wrapping case
                if (value > trackBarCurrentFrame.Maximum)
                    trackBarCurrentFrame.Value = 0;
                else
                    trackBarCurrentFrame.Value = value;
            }
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

        public int NumberOfFrames
        {
            get { return trackBarCurrentFrame.Maximum + 1; }
        }

        public Timer AnimationTimer
        {
            get { return _animationTimer; }
        }
        #endregion

        #region Event Passing
        private void buttonPlay_Click(object sender, EventArgs e)
        {
            if (_animationTimer != null)
            {
                _animationTimer.Start();
                buttonPlay.Enabled = false;
                buttonStop.Enabled = true;
            }

            if (PlayClicked != null)
                PlayClicked();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (_animationTimer != null)
            {
                _animationTimer.Stop();
                buttonPlay.Enabled = true;
                buttonStop.Enabled = false;
            }

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
            if (_animationTimer != null)
                _animationTimer.Interval = (int)(1000 / (double)numericUpDownFPS.Value);

            if (FPSChanged != null)
                FPSChanged();
        }

        void _animationTimer_Tick(object sender, EventArgs e)
        {
            AdvanceCurrentFrameTrackbar();
            //now let the world know that we moved
            trackBarCurrentFrame_Scroll(sender, e);
            if (Tick != null)
                Tick(this, e);
        }
        #endregion
    }
}
