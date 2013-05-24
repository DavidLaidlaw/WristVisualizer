using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WristVizualizer
{
    public partial class TextureControl : UserControl
    {
        public delegate void SelectedTransformChangedHandler();
        public event SelectedTransformChangedHandler SelectedTransformChanged;

        public event EventHandler EditableTransformChanged;

        private bool localEditingInProgress;

        public TextureControl(string[] transforms)
        {
            InitializeComponent();

            setTransforms(transforms);
            listBoxTransforms.SelectedIndex = (listBoxTransforms.Items.Count - 1);
            clearEditableTransform();
        }

        private void setTransforms(string[] transforms)
        {
            listBoxTransforms.Items.Clear();
            listBoxTransforms.Items.AddRange(transforms);
        }

        public int selectedTransformIndex
        {
            get { return listBoxTransforms.SelectedIndex; }
        }

        public void setEditableTransform(double[] center, double[] rot, double[] translation)
        {
            if (center.Length != 3 || rot.Length != 3 || translation.Length != 3)
                throw new ArgumentException("Invalid data for transform");
            localEditingInProgress = true;
            numericUpDownCenterX.Value = (decimal)center[0];
            numericUpDownCenterY.Value = (decimal)center[1];
            numericUpDownCenterZ.Value = (decimal)center[2];

            numericUpDownRotX.Value = (decimal)rot[0];
            numericUpDownRotY.Value = (decimal)rot[1];
            numericUpDownRotZ.Value = (decimal)rot[2];

            numericUpDownTransX.Value = (decimal)translation[0];
            numericUpDownTransY.Value = (decimal)translation[1];
            numericUpDownTransZ.Value = (decimal)translation[2];
            localEditingInProgress = false;
            setEditableTransformEnabledState(true);
        }

        public libWrist.TransformMatrix getEditedTransform()
        {
            double[] centerRotation = new double[3];
            double[] rotationAngles = new double[3];
            double[] translation = new double[3];

            centerRotation[0] = (double)numericUpDownCenterX.Value;
            centerRotation[1] = (double)numericUpDownCenterY.Value;
            centerRotation[2] = (double)numericUpDownCenterZ.Value;

            rotationAngles[0] = (double)numericUpDownRotX.Value;
            rotationAngles[1] = (double)numericUpDownRotY.Value;
            rotationAngles[2] = (double)numericUpDownRotZ.Value;

            translation[0] = (double)numericUpDownTransX.Value;
            translation[1] = (double)numericUpDownTransY.Value;
            translation[2] = (double)numericUpDownTransZ.Value;
            libWrist.TransformMatrix rot = new libWrist.TransformMatrix();
            libWrist.TransformMatrix t = new libWrist.TransformMatrix();
            rot.rotateAboutCenter(rotationAngles, centerRotation);
            t.setTranslation(translation);
            return t * rot;
        }

        private void setEditableTransformEnabledState(bool enabled)
        {
            numericUpDownCenterX.Enabled = enabled;
            numericUpDownCenterY.Enabled = enabled;
            numericUpDownCenterZ.Enabled = enabled;
            numericUpDownRotX.Enabled = enabled;
            numericUpDownRotY.Enabled = enabled;
            numericUpDownRotZ.Enabled = enabled;
            numericUpDownTransX.Enabled = enabled;
            numericUpDownTransY.Enabled = enabled;
            numericUpDownTransZ.Enabled = enabled;
        }

        public void clearEditableTransform()
        {
            setEditableTransformEnabledState(false);
            localEditingInProgress = true;
            numericUpDownCenterX.Value = 0;
            numericUpDownCenterY.Value = 0;
            numericUpDownCenterZ.Value = 0;

            numericUpDownRotX.Value = 0;
            numericUpDownRotY.Value = 0;
            numericUpDownRotZ.Value = 0;

            numericUpDownTransX.Value = 0;
            numericUpDownTransY.Value = 0;
            numericUpDownTransZ.Value = 0;
            localEditingInProgress = false;
        }

        private void listBoxTransforms_SelectedIndexChanged(object sender, EventArgs e)
        {            
            if (SelectedTransformChanged != null)
                SelectedTransformChanged();
        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (localEditingInProgress) return;

            if (EditableTransformChanged != null)
                EditableTransformChanged(this, new EventArgs());
        }

        private void buttonCopyToClipboard_Click(object sender, EventArgs e)
        {
            string output = String.Format(";StartRotationCenter={0} {1} {2}\n", numericUpDownCenterX.Value, numericUpDownCenterY.Value, numericUpDownCenterZ.Value);
            output += String.Format("StartRotation={0} {1} {2}\n", numericUpDownRotX.Value, numericUpDownRotY.Value, numericUpDownRotZ.Value);
            output += String.Format("StartTranslation={0} {1} {2}\n", numericUpDownTransX.Value, numericUpDownTransY.Value, numericUpDownTransZ.Value);
            Clipboard.SetText(output);
        }
    }
}
