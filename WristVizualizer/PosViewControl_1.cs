using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WristVizualizer
{
    public partial class PosViewControl : UserControl
    {
        public delegate void PlayClickedHandler();
        public delegate void StopClickedHandler();
        public delegate void FPSChangedHandler();
        public delegate void TrackbarScrollHandler();
        public delegate void ShowHamClickedHandler();
        public delegate void OverrideMaterialClickedHandler();
        public delegate void ShowPositionLabelClickedHandler();

        public event PlayClickedHandler PlayClicked;
        public event StopClickedHandler StopClicked;
        public event FPSChangedHandler FPSChanged;
        public event TrackbarScrollHandler TrackbarScroll;
        public event ShowHamClickedHandler ShowHamClicked;
        public event OverrideMaterialClickedHandler OverrideMaterialClicked;
        public event ShowPositionLabelClickedHandler ShowPositionLabelClicked;


        public PosViewControl()
        {
            InitializeComponent();
        }

        public void setupController(int numberOfFrames, bool hasHAMAxes, bool hasLabels)
        {
            trackBarPosViewCurrentFrame.Minimum = 0;
            trackBarPosViewCurrentFrame.Maximum = numberOfFrames - 1;
            trackBarPosViewCurrentFrame.Value = 0;

            checkBoxPosViewShowAxes.Enabled = hasHAMAxes;
            checkBoxPosViewLabels.Enabled = hasLabels;

            buttonPosViewPlay.Enabled = true;
            buttonPosViewStop.Enabled = true;
        }

        #region Interfaces
        public decimal FPS
        {
            get { return numericUpDownPosViewFPS.Value; }
            set { numericUpDownPosViewFPS.Value = value; }
        }

        public int currentFrame
        {
            get { return trackBarPosViewCurrentFrame.Value; }
            set { trackBarPosViewCurrentFrame.Value = value; }
        }

        public bool ShowHam
        {
            get { return checkBoxPosViewShowAxes.Checked; }
            set { checkBoxPosViewShowAxes.Checked = value; }
        }

        public bool OverrideMaterial
        {
            get { return checkBoxPosViewOverrideMaterial.Checked; }
            set { checkBoxPosViewOverrideMaterial.Checked = value; }
        }

        public bool ShowLabels
        {
            get { return checkBoxPosViewLabels.Checked; }
            set { checkBoxPosViewLabels.Checked = value; }
        }

        public bool PlayButtonEnabled
        {
            get { return buttonPosViewPlay.Enabled; }
            set { buttonPosViewPlay.Enabled = value; }
        }

        public bool StopButtonEnabled
        {
            get { return buttonPosViewStop.Enabled; }
            set { buttonPosViewStop.Enabled = value; }
        }

        public bool TrackBarEnabled
        {
            get { return trackBarPosViewCurrentFrame.Enabled; }
            set { trackBarPosViewCurrentFrame.Enabled = value; }
        }
        #endregion

        #region Event Passing
        private void buttonPosViewPlay_Click(object sender, EventArgs e)
        {
            if (PlayClicked != null)
                PlayClicked();
        }

        private void buttonPosViewStop_Click(object sender, EventArgs e)
        {
            if (StopClicked != null)
                StopClicked();
        }

        private void trackBarPosViewCurrentFrame_Scroll(object sender, EventArgs e)
        {
            if (TrackbarScroll != null)
                TrackbarScroll();
        }

        private void numericUpDownPosViewFPS_ValueChanged(object sender, EventArgs e)
        {
            if (FPSChanged != null)
                FPSChanged();
        }

        private void checkBoxPosViewShowAxes_CheckedChanged(object sender, EventArgs e)
        {
            if (ShowHamClicked != null)
                ShowHamClicked();
        }

        private void checkBoxPosViewOverrideMaterial_CheckedChanged(object sender, EventArgs e)
        {
            if (OverrideMaterialClicked != null)
                OverrideMaterialClicked();
        }

        private void checkBoxPosViewLabels_CheckedChanged(object sender, EventArgs e)
        {
            if (ShowPositionLabelClicked != null)
                ShowPositionLabelClicked();
        }
        #endregion
    }
}
