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
        private Color _startColor;
        private float _startTransparency;

        private Material _material;
        private ExaminerViewer _viewer;
        private bool _hadMaterial;

        public MaterialEditor(ExaminerViewer viewer)
        {
            //lets get info on the selected node!
            _viewer = viewer;
            _material = _viewer.getSelectedMaterial();
            if (_material == null)
            {
                _hadMaterial = false;
                //keep searching
                _material = _viewer.createMaterialForSelected();
                if (_material == null)
                {
                    //if still null, this is an error!
                    this.Close();
                    return;
                }
            }
            else
                _hadMaterial = true;

            //setup the form at this point, we have a material
            InitializeComponent();


            int packedC = _material.getColor();
            _startColor = unpackColor(packedC);
            _startTransparency = _material.getTransparency();

            trackBarRed.Value = _startColor.R;
            trackBarGreen.Value = _startColor.G;
            trackBarBlue.Value = _startColor.B;

            trackBarTransparency.Value = (int)(_startTransparency*100f);

            updateColorSample();
        }

        private void updateColorSample()
        {
            Color c = Color.FromArgb(trackBarRed.Value, trackBarGreen.Value, trackBarBlue.Value);
            panelColorSample.BackColor = c;
            
            if (checkBoxLiveUpdate.Checked)
                updateMaterialColor();
        }

        private void updateMaterialColor()
        {
            Color c = Color.FromArgb(trackBarRed.Value, trackBarGreen.Value, trackBarBlue.Value);
            _material.setColor(c.R / 255f, c.G / 255f, c.B / 255f);
        }
        private void updateMaterialTransparency()
        {
            _material.setTransparency(trackBarTransparency.Value/100f);
        }
        #region Trackup Updates
        private void trackBarRed_Scroll(object sender, EventArgs e)
        {
            updateColorSample();
        }

        private void trackBarGreen_Scroll(object sender, EventArgs e)
        {
            updateColorSample();
        }

        private void trackBarBlue_Scroll(object sender, EventArgs e)
        {
            updateColorSample();
        }

        private void trackBarTransparency_Scroll(object sender, EventArgs e)
        {
            if (checkBoxLiveUpdate.Checked)
                updateMaterialTransparency();
        }
        #endregion

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
            //all we need to do, is ensure that we have updated everything, and close
            updateMaterialColor();
            updateMaterialTransparency();
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (_hadMaterial)
            {
                //then we need to re-apply the original material settings
                Color c = _startColor;
                _material.setColor(c.R / 255f, c.G / 255f, c.B / 255f);
                _material.setTransparency(_startTransparency);
            }
            else
            {
                //we need to remove the material from the scene
                _viewer.removeMaterialFromScene(_material);
            }
            this.Close();
        }

        public void materialDeselected()
        {
            //get us out of here, leave the scene "as-is"
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

        private void checkBoxLiveUpdate_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxLiveUpdate.Checked)
            {
                updateMaterialColor();
                updateMaterialTransparency();
            }
        }

        
    }
}