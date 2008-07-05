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

        }


        /// <summary>
        /// Checks that we were passed a valid camera, then calls ShowDialog
        /// </summary>
        public DialogResult CheckAndShowDialog()
        {
            //quick check that this is an othrographic camera, else we don't work
            if (!_camera.IsOrthographic)
            {
                string msg = "Non Othrogrpahic Camera found in scene.\nCan only edit othographic cameras. Sorry.";
                MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return DialogResult.Abort;
            }

            showCameraParameters(_camera);
            return this.ShowDialog();
        }


        private void showCameraParameters(Camera camera)
        {
            const string numberFormat = "F02";
            float[] pos = camera.getPosition();
            float[] orient = camera.getOrientation();
            float focalDistance = camera.FocalDistance;
            float nearDistance = camera.NearDistance;
            float farDistance = camera.FarDistance;
            float height  = camera.Height;

            textBoxPosX.Text = pos[0].ToString(numberFormat);
            textBoxPosY.Text = pos[1].ToString(numberFormat);
            textBoxPosZ.Text = pos[2].ToString(numberFormat);

            textBoxOrientX.Text = orient[0].ToString(numberFormat);
            textBoxOrientY.Text = orient[1].ToString(numberFormat);
            textBoxOrientZ.Text = orient[2].ToString(numberFormat);
            textBoxOrientRadians.Text = orient[3].ToString(numberFormat);

            textBoxFocalDistance.Text = focalDistance.ToString(numberFormat);
            textBoxNearDistance.Text = nearDistance.ToString(numberFormat);
            textBoxFarDistance.Text = farDistance.ToString(numberFormat);
            textBoxHeight.Text = height.ToString(numberFormat);
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
            float nearDistance = float.Parse(textBoxNearDistance.Text);
            float farDistance = float.Parse(textBoxFarDistance.Text);
            float height = float.Parse(textBoxHeight.Text);

            outputCamera.setPosition(new float[] { posX, posY, posZ });
            outputCamera.setOrientation(new float[] { orientX, orientY, orientZ, orientRadians });
            outputCamera.FocalDistance = focalDistance;
            outputCamera.NearDistance = nearDistance;
            outputCamera.FarDistance = farDistance;
            outputCamera.Height = height;
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

        private void buttonSaveClipboard_Click(object sender, EventArgs e)
        {
            string graph = _camera.getNodeGraph();
            Clipboard.SetText(graph);
        }

        private void buttonLoadClipboard_Click(object sender, EventArgs e)
        {
            string graph = Clipboard.GetText();
            if (String.IsNullOrEmpty(graph))
            {
                string msg = "No camera found in Clipboard";
                MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Camera c = new Camera(graph);
                showCameraParameters(c);
            }
            catch (ArgumentException ex)
            {
                string msg = String.Format("Error loading camera.\n{0}",ex.Message);
                MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }



        
    }
}