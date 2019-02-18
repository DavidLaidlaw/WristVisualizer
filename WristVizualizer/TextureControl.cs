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

        //IF ADDING NEW ADJUSTMENTS to the volume or any of the bones, can call the pre existing event handler
        //public delegate void DisplayVolumeCheckboxHandler();
        //public event DisplayVolumeCheckboxHandler DisplayVolumeCheckboxChanged;

        private bool localEditingInProgress;
  

       



        public TextureControl(string[] transforms)
        {
            InitializeComponent();

            setTransforms(transforms);
            listBoxTransforms.SelectedIndex = (listBoxTransforms.Items.Count - 1);
            clearEditableTransform();
        }

        /**
         * 
         * 
         * **/
        public float[] getCurrentCenterOfRotation()
        {
            float[] centerOfRotation = new float[3];
            centerOfRotation[0] = (float)numericUpDownCenterX.Value;
            centerOfRotation[1] = (float)numericUpDownCenterY.Value;
            centerOfRotation[2] = (float)numericUpDownCenterZ.Value;
            return centerOfRotation;
        }

        public float[] getCurrentRotation()
        {
            float[] Rotation = new float[3];
            Rotation[0] = (float)numericUpDownRotX.Value;
            Rotation[1] = (float)numericUpDownRotY.Value;
            Rotation[2] = (float)numericUpDownRotZ.Value;
            return Rotation;
        }

        public float[] getCurrentTranslation()
        {
            float[] Translation = new float[3];
            Translation[0] = (float)numericUpDownTransX.Value;
            Translation[1] = (float)numericUpDownTransY.Value;
            Translation[2] = (float)numericUpDownTransZ.Value;
            return Translation;
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

        /// <summary>
        /// ////////////////////
        /// </summary>
        /// <returns></returns>
        public libWrist.TransformMatrix setNewEditedTransform(float x, float y, float z,float q0, float q1,float q2, float q3)
        {
            double[] centerRotation = new double[3];
            double[] rotationAngles = new double[3];
            double[] translation = new double[3];

            centerRotation[0] = (double)numericUpDownCenterX.Value;
            centerRotation[1] = (double)numericUpDownCenterY.Value;
            centerRotation[2] = (double)numericUpDownCenterZ.Value;

            //use a method specially designed for getting roll, pitch, and yaw
            float[] quat = new float[4];
            quat[0] = q0;
            quat[1] = q1;
            quat[2] = q2;
            quat[3] = q3;
            float[] euler=new float[3];

            quat2euler(quat,euler);
            double roll = euler[2];
            double pitch = euler[1];
            double yaw = euler[0];

            //keep small number
            if (roll > 3.1416) roll = -Math.PI + (roll - Math.PI);
            if (roll < -3.1416) roll = Math.PI + (roll - (-1) * Math.PI);

            if (pitch > 3.1416) pitch = -Math.PI + (pitch - Math.PI);
            if (pitch < -3.1416) pitch = Math.PI + (pitch - (-1) * 3.1416F);

            if (yaw > 3.1416) yaw = -Math.PI + (yaw - Math.PI);
            if (yaw < -3.1416) yaw = Math.PI + (yaw - (-1) * Math.PI);

            //test
            numericUpDownRotX.Value = (decimal)yaw;
            numericUpDownRotY.Value = (decimal)pitch;
            numericUpDownRotZ.Value = (decimal)roll;

            rotationAngles[0] = (double)numericUpDownRotX.Value;
            rotationAngles[1] = (double)numericUpDownRotY.Value;
            rotationAngles[2] = (double)numericUpDownRotZ.Value;

            x = (float)numericUpDownTransX.Value + x;
            y = (float)numericUpDownTransY.Value + y;
            z = (float)numericUpDownTransZ.Value + z;

            if (x > 350) x = 350;
            if (x < -350) x = -350;
            if (y > 350) y = 350;
            if (y < -350) y = -350;
            if (z > 350) z = 350;
            if (z < -350) z = -350;

            numericUpDownTransX.Value = (decimal)x;
            numericUpDownTransY.Value = (decimal)y;
            numericUpDownTransZ.Value = (decimal)z;

            translation[0] = (double)numericUpDownTransX.Value;
            translation[1] = (double)numericUpDownTransY.Value;
            translation[2] = (double)numericUpDownTransZ.Value;


            libWrist.TransformMatrix rot = new libWrist.TransformMatrix();
            libWrist.TransformMatrix t = new libWrist.TransformMatrix();
            

            rot.rotateAboutCenter(rotationAngles, centerRotation);
            //rotations go in order Z Y X


            t.setTranslation(translation);
            return t * rot;
        }
        /// <summary>
        /// ////////////////////////
        /// </summary>
        /// <param name="enabled"></param>
        /// 

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



        internal bool isEnabled()
        {
            return numericUpDownCenterX.Enabled;
            //throw new NotImplementedException();
        }




public void matrix2euler(float[] mat, float[] euler)
{
  // adapted from flipcode FAQ
  // http://www.flipcode.com/documents/matrfaq.html#Q37
  euler[1] = (float)Math.Asin(mat[2]);
  float C = (float)Math.Cos( euler[1] );

  float tx, ty;

  if (Math.Abs(C) > 0.005f) { // Gimball lock?
    tx = mat[10] / C;
    ty = -mat[6] / C;

    euler[0] = (float)Math.Atan2(ty, tx);

    tx = mat[0] / C;
    ty = -mat[1] / C;

    euler[2] = (float)Math.Atan2(ty, tx);
  }
  else {
    euler[0] = 0.0f;
    tx = mat[5];
    ty = mat[4];
    euler[2] = (float)Math.Atan2(ty, tx);
  }
}

public void quat2matrix(float[] quat, float[] matrix)
{
  // adapted from Coin
  float x = quat[0];
  float y = quat[1];
  float z = quat[2];
  float w = quat[3];

  matrix[0] = w*w + x*x - y*y - z*z;
  matrix[4] = 2.0f*x*y + 2.0f*w*z;
  matrix[8] = 2.0f*x*z - 2.0f*w*y;
  matrix[12] = 0.0f;

  matrix[1] = 2.0f*x*y-2.0f*w*z;
  matrix[5] = w*w - x*x + y*y - z*z;
  matrix[9] = 2.0f*y*z + 2.0f*w*x;
  matrix[13] = 0.0f;

  matrix[2] = 2.0f*x*z + 2.0f*w*y;
  matrix[6] = 2.0f*y*z - 2.0f*w*x;
  matrix[10] = w*w - x*x - y*y + z*z;
  matrix[14] = 0.0f;

  matrix[3] = 0.0f;
  matrix[7] = 0.0f;
  matrix[11] = 0.0f;
  matrix[15] = w*w + x*x + y*y + z*z;
}

public void quat2euler(float[] quat, float[] euler)
{
  float[] m=new float[16];
  quat2matrix(quat, m);
  matrix2euler(m, euler);
}

float to_angle(float rad)
{
  float angle = rad * 180.0f / (float)(Math.PI);
  return angle;
}

public void print_euler(float[] quat)
{
  float[] euler=new float[3];
  quat2euler(quat, euler);
  Console.WriteLine("rot:"+
          to_angle(euler[0])+", "+
          to_angle(euler[1]) + ", " +
          to_angle(euler[2])+"\n");
}



//determines whether or not the volume rendering of the CT data is visible
private void checkBox1_CheckedChanged(object sender, EventArgs e)
{
    //disable or enable volume
    //DisplayVolumeCheckboxChanged();

    if (EditableTransformChanged != null)
        EditableTransformChanged(this, new EventArgs());
}

public bool getDisplayVolumeChecked()
{
    return checkBox1.Checked;
}


//controls number of proxies geometries to texture
private void numericUpDown1_ValueChanged(object sender, EventArgs e)
{

    if (EditableTransformChanged != null)
        EditableTransformChanged(this, new EventArgs());
}

public int getNumSlices()
{
    return (int) numberOfSlices.Value;
}

//changes the opacity of the displayed volume render
private void numericUpDown1_ValueChanged_1(object sender, EventArgs e)

{

    if (EditableTransformChanged != null)
        EditableTransformChanged(this, new EventArgs());

}

public float getOpacity()
{
    return (float)OpacityBox.Value;
}

private void showManipulator_CheckedChanged(object sender, EventArgs e)
{
    if (EditableTransformChanged != null)
        EditableTransformChanged(this, new EventArgs());
}

public bool getshowManipulatorChecked()
{
    return showManipulator.Checked;
}



internal void disableVolumeItems()
{
    checkBox1.Enabled=(false);
    OpacityBox.Enabled=(false);
    numberOfSlices.Enabled=(false);
}
    }
}
