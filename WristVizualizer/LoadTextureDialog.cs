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

        private void run()
        {
            string image = @"C:\Functional\E01424\CTScans\E01424_15";
            string side = "right";
            string ulnaStackFile = @"C:\Functional\E01424\S15R\Stack.files\uln15R.stack";
            string shit1 = @"C:\Functional\E01424\S15R\IV.files\rad15R.iv";
            string kinematicFile = @"C:\fake.shit.dat";
            //crop values
            double[][] pts = DatParser.parseDatFile(ulnaStackFile);

            Separator s = Texture.createPointsFileObject(pts);

            CTmri mri = new CTmri(image);
            mri.setCrop(0, 511, 0, 511, 0, 186);

            double LO_RES_HEIGHT = mri.voxelSizeX;
            double LO_RES_WIDTH = mri.voxelSizeX;
            double RES_DEPTH = mri.voxelSizeZ;

            int sizeX = 512;
            int sizeY = 512;
            int sizeZ = 187;

            Random r = new Random();

            //short max=0;
            //short min=255;

            //lets build an array of bytes (unsigned 8bit data structure)
            Byte[][] voxels = new Byte[sizeZ][];
            for (int i = 0; i < sizeZ; i++)
            {
                //min = 255;
                //max = -32;
                voxels[i] = new Byte[sizeX * sizeY];
                for (int j = 0; j < sizeY; j++)
                    for (int k = 0; k < sizeX; k++)
                    {
                        voxels[i][(j * sizeX) + k] = (Byte)mri.getCroppedVoxel(k, j, i);
                        //if (mri.getVoxel(k,j,i) > max)
                        //    max = (short)mri.getVoxel(k, j, i);
                        //if (voxels[i][(j * sizeX) + k] < min)
                        //    min = voxels[i][(j * sizeX) + k];
                    }
                //Console.WriteLine("For line {0}, min: {1}, max: {2}",i,min,max);
            }

                        
            Separator ulna = Texture.createPointsFileObject(pts);
            Separator rad = new Separator();
            rad.addFile(shit1);
            _root.addChild(ulna);
            _root.addChild(rad);

            _texture = new Texture();
            Separator plane1 = _texture.makeDragerAndTexture(voxels, 2);
            _root.addChild(plane1);
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