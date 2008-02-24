using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using libCoin3D;

namespace WristVizualizer
{
    public partial class MaterialEditor : Form
    {
        private int _startR;
        private int _startG;
        private int _startB;

        private Color _startColor;

        private Material _material;

        public MaterialEditor(int r, int g, int b)
        {
            InitializeComponent();

            _startR = r;
            _startG = g;
            _startB = b;

            trackBarRed.Value = r;
            trackBarGreen.Value = g;
            trackBarBlue.Value = b;

            updateColor();
        }

        public MaterialEditor(Material material)
        {
            _material = material;

            int packedC = material.getColor();
            _startColor = unpackColor(packedC);

            trackBarRed.Value = _startColor.R;
            trackBarGreen.Value = _startColor.G;
            trackBarBlue.Value = _startColor.B;

            updateColor();
        }

        private void updateColor()
        {
            Color c = Color.FromArgb(trackBarRed.Value, trackBarGreen.Value, trackBarBlue.Value);
            panelColorSample.BackColor = c;
        }

        private void trackBarRed_Scroll(object sender, EventArgs e)
        {
            updateColor();
        }

        private void trackBarGreen_Scroll(object sender, EventArgs e)
        {
            updateColor();
        }

        private void trackBarBlue_Scroll(object sender, EventArgs e)
        {
            updateColor();
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            ColorDialog cg = new ColorDialog();
            cg.Color = panelColorSample.BackColor;

            cg.FullOpen = true;
            if (cg.ShowDialog() == DialogResult.Cancel)
                return;

            //else, update the colors here
            panelColorSample.BackColor = cg.Color;
            trackBarRed.Value = (int)cg.Color.R;
            trackBarGreen.Value = (int)cg.Color.G;
            trackBarBlue.Value = (int)cg.Color.B;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            //TODO: Update color in scene
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            //TODO: Reset color to original
            this.Close();
        }

        public void materialDeselected()
        {
            //get us out of here
            //TODO: Save or not to save. Rather, restore or not restore
            this.Close();
        }

        #region utils

        static private Color unpackColor(int packedColor)
        {
            packedColor = (packedColor >> 8);
            return Color.FromArgb(packedColor);
        }

        static public int unpackColorR(int packedColor)
        {
            return unpackColor(packedColor).R;
        }
        static public int unpackColorG(int packedColor)
        {
            return unpackColor(packedColor).G;
        }
        static public int unpackColorB(int packedColor)
        {
            return unpackColor(packedColor).B;
        }
        #endregion
    }
}