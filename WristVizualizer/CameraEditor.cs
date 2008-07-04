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
    public partial class CameraEditor : Form
    {
        private Camera _camera;
        public CameraEditor(Camera camera)
        {
            InitializeComponent();
            _camera = camera;
            showCameraParameters(camera);
        }


        private void showCameraParameters(Camera camera)
        {
            const string numberFormat = "F02";
            float[] pos = camera.getPosition();
            float[] orient = camera.getOrientation();
            float focalDistance = camera.FocalDistance;

            textBoxPosX.Text = pos[0].ToString(numberFormat);
            textBoxPosY.Text = pos[1].ToString(numberFormat);
            textBoxPosZ.Text = pos[2].ToString(numberFormat);

            textBoxOrientX.Text = orient[0].ToString(numberFormat);
            textBoxOrientY.Text = orient[1].ToString(numberFormat);
            textBoxOrientZ.Text = orient[2].ToString(numberFormat);
            textBoxOrientRadians.Text = orient[3].ToString(numberFormat);

            textBoxFocalDistance.Text = focalDistance.ToString(numberFormat);
        }

        private void saveCameraParametersFromForm(Camera outputCamera)
        {
            float posX = float.Parse(textBoxPosX.Text);
            float posY = float.Parse(textBoxPosY.Text);
            float posZ = float.Parse(textBoxPosZ.Text);

            float orientX = float.Parse(textBoxOrientX.Text);
            float orientY = float.Parse(textBoxOrientY.Text);
            float orientZ = float.Parse(textBoxOrientZ.Text);
            float orientRadians = float.Parse(textBoxOrientRadians.Text);

            float focalDistance = float.Parse(textBoxFocalDistance.Text);

            outputCamera.setPosition(new float[] { posX, posY, posZ });
            outputCamera.setOrientation(new float[] { orientX, orientY, orientZ, orientRadians });
            outputCamera.FocalDistance = focalDistance;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            saveCameraParametersFromForm(_camera);
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        
    }
}