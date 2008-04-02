using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using libWrist;
using libCoin3D;

namespace WristVizualizer
{
    public partial class LoadTextureDialog : Form
    {
        private ExaminerViewer _viewer;
        private Separator _root;
        private Texture _texture;

        public LoadTextureDialog()
        {
            InitializeComponent();
            
        }

        private void buttonBrowseImage_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.SelectedPath = textBoxImageDirectory.Text;
            if (folder.ShowDialog() == DialogResult.Cancel)
                return;

            textBoxImageDirectory.Text = folder.SelectedPath;
        }

        private string getBoneFileName(string shortBoneName, Wrist.Sides side)
        {
            string form = @"C:\Functional\E01424\S15R\Stack.files\{0}15{1}.stack";
            return String.Format(form, shortBoneName.ToLower(), side == Wrist.Sides.LEFT ? "L" : "R");
        }

        private void run()
        {
            string image = @"C:\Functional\E01424\CTScans\E01424_15";
            string ulnaStackFile = getBoneFileName("uln", Wrist.Sides.RIGHT);
            string shit1 = @"C:\Functional\E01424\S15R\IV.files\rad15R.iv";

            //crop values
            
            CropValuesParser cvp = new CropValuesParser(@"C:\Functional\E01424\crop_values.txt");
            CropValuesParser.CropValues cv = cvp.getCropData("15R");

            CTmri mri = new CTmri(image);
            mri.setCrop(cv.MinX, cv.MaxX, cv.MinY, cv.MaxY, cv.MinZ, cv.MaxZ);

            double LO_RES_HEIGHT = mri.voxelSizeX;
            double LO_RES_WIDTH = mri.voxelSizeX;
            double RES_DEPTH = mri.voxelSizeZ;

            int sizeX = cv.SizeX;
            int sizeY = cv.SizeY;
            int sizeZ = cv.SizeZ;

            Random r = new Random();

            //lets build an array of bytes (unsigned 8bit data structure)
            Byte[][] voxels = new Byte[sizeZ][];
            for (int i = 0; i < sizeZ; i++)
            {
                voxels[i] = new Byte[sizeX * sizeY];
                for (int j = 0; j < sizeY; j++)
                    for (int k = 0; k < sizeX; k++)
                    {
                        voxels[i][(j * sizeX) + k] = (Byte)mri.getCroppedVoxel(k, j, i);
                    }
            }

            //lets load each bone
            for (int i = 0; i < TextureSettings.ShortBNames.Length; i++)
            {
                double[][] pts = DatParser.parseDatFile(getBoneFileName(TextureSettings.ShortBNames[i], Wrist.Sides.RIGHT));
                Separator bone = Texture.createPointsFileObject(pts, TextureSettings.BoneColors[i]);
                _root.addChild(bone);
            }

            _texture = new Texture(cv.Side== Wrist.Sides.LEFT ? Texture.Sides.LEFT : Texture.Sides.RIGHT, 
                cv.SizeX, cv.SizeY, cv.SizeZ, cv.VoxelX, cv.VoxelY, cv.VoxelZ);
            Separator plane1 = _texture.makeDragerAndTexture(voxels, Texture.Planes.XY_PLANE);
            Separator plane2 = _texture.makeDragerAndTexture(voxels, Texture.Planes.YZ_PLANE);
            _root.addChild(plane1);
            _root.addChild(plane2);
            _root.addChild(_texture.createKeyboardCallbackObject(0));
        }

        /// <summary>
        /// Sets up the scene
        /// </summary>
        /// <param name="viewer">The examiner viewer to display everything in</param>
        /// <returns>a reference to the new root Separator node</returns>
        public Separator setup(ExaminerViewer viewer)
        {
            _viewer = viewer;
            _root = new Separator();
            run();
            _viewer.setSceneGraph(_root);
            return _root;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}