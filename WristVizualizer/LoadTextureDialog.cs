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
        public LoadTextureDialog()
        {
            InitializeComponent();
            run();
        }

        private void buttonBrowseImage_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.SelectedPath = textBoxImageDirectory.Text;
            if (folder.ShowDialog() == DialogResult.Cancel)
                return;

            textBoxImageDirectory.Text = folder.SelectedPath;
        }

        void compute_access_index(int code)
        {
            if (code == 0)
            {
                ACCESS_INDEX_SIGN_I = 1;
                ACCESS_INDEX_OFFSET_I = 0;
                ACCESS_INDEX_SIGN_I_X = 1;
                ACCESS_INDEX_OFFSET_I_X = 0;
            }
            if (code == 1)
            {
                ACCESS_INDEX_SIGN_I = -1;
                ACCESS_INDEX_SIGN_I_X = 1;
                ACCESS_INDEX_OFFSET_I = (MRI_X_SIZE - 1);
                ACCESS_INDEX_OFFSET_I_X = 0;
            }

        } 

        private void run()
        {
            string image = @"C:\Functional\E01424\CTScans\E01424_15";
            string side = "right";
            string ulnaStackFile = @"C:\Functional\E01424\S15R\Stack.files\uln15R.stack";
            string kinematicFile = @"C:\fake.shit.dat";
            //crop values
            double[][] pts = DatParser.parseDatFile(ulnaStackFile);

            Separator s = Texture.createPointsFileObject(pts);

            CTmri mri = new CTmri(image);

            double LO_RES_HEIGHT = mri.voxelSizeX;
            double LO_RES_WIDTH = mri.voxelSizeX;
            double RES_DEPTH = mri.voxelSizeZ;

            if (side == "left")
            {
                //compute_access_index(1);
                //makeRectangleVertices_left(); 
            }
            else if (side == "right")
            {
                //compute_access_index(0);
                //makeRectangleVertices(); 
            }

            //TODO: load slice data in as both the data_x & dataSecond_x


            double dDist;
            int slice_id;
            int vert_slice_id;
            double dDistSecond;
            //read all slices 
            //unsigned char ***all_slice_data = alloc_slice_stack( MRI_Z_SIZE, MRI_X_SIZE, MRI_Y_SIZE); 
            //unsigned char ***all_slice_dataSecond = alloc_slice_stack( MRI_Z_SIZE, MRI_X_SIZE, MRI_Y_SIZE); 
            //unsigned char ***all_slice_data_x = alloc_slice_stack( MRI_X_SIZE_vert, MRI_Y_SIZE_vert, MRI_Z_SIZE_vert); 
            //unsigned char ***all_slice_dataSecond_x = alloc_slice_stack( MRI_X_SIZE_vert, MRI_Y_SIZE_vert, MRI_Z_SIZE_vert); 

            //tmp_buf = (unsigned char*) malloc( MRI_X_SIZE * MRI_Y_SIZE * sizeof(unsigned char));

            //tmp_bufSecond = (unsigned char*) malloc( MRI_X_SIZE * MRI_Y_SIZE * sizeof(unsigned char));
            //tmp_bufThird = (unsigned char*) malloc( MRI_X_SIZE * MRI_Y_SIZE * sizeof(unsigned char));

            //tmp_buf_x = (unsigned char*) malloc( MRI_Y_SIZE_vert * MRI_Z_SIZE_vert * sizeof(unsigned char));
            //tmp_bufSecond_x = (unsigned char*) malloc( MRI_Y_SIZE_vert * MRI_Z_SIZE_vert * sizeof(unsigned char));
            //tmp_bufThird_x = (unsigned char*) malloc( MRI_Y_SIZE_vert * MRI_Z_SIZE_vert * sizeof(unsigned char));


            //double i, j;
            ////horizontal plane
            //for(slice_id = 0; slice_id < MRI_Z_SIZE; slice_id++)
            //   for( i = 0.0; i < MRI_X_SIZE; i += 1.0)
            // for( j = 0.0; j < MRI_Y_SIZE; j += 1.0) {   
            //     dDist = reader->getVoxel(i,j,slice_id);
            //     dDistSecond = reader->getVoxel(i,MRI_Y_SIZE + j,slice_id);	  
            //   // mirror on X if left
            //   if(dDist > scale_val) dDist = scale_val;
            //   if(dDistSecond > scale_val) dDistSecond = scale_val;
            //   all_slice_data[ slice_id ][ ACCESS_INDEX_SIGN_I * (int)i + ACCESS_INDEX_OFFSET_I ][(int)j] = (unsigned char) (dDist/scale_val * 255); 
            //   all_slice_dataSecond[ slice_id ][ ACCESS_INDEX_SIGN_I * (int)i + ACCESS_INDEX_OFFSET_I ][(int)j] = (unsigned char) (dDistSecond/scale_val * 255); 
            // }
            ////vertical plane
            //  for(vert_slice_id = 0; vert_slice_id < MRI_X_SIZE_vert; vert_slice_id++)
            //   for( i = 0.0; i < MRI_Y_SIZE_vert; i += LO_RES_HEIGHT)
            // for( j = 0.0; j < MRI_Z_SIZE_vert; j += 1.0) {   
            //   //got to divide by LO_RES_HEIGHT the coord on the transl axis, since
            //   //i'm advancing by 1mm everytime and not by LO_RES_HEIGHT; 
            //   //in other words X by which i translate is in real  life coords (mm), while the cube is in 521 by 512 coords
            //   // leave Z as it is
            //   // but mess up with Y which is in real life metric coords too (grows by LO_RES and not by 1 voxel)
            //     dDist = reader->getVoxel(vert_slice_id/LO_RES_HEIGHT, i/LO_RES_HEIGHT, j);
            //     dDistSecond = reader->getVoxel(vert_slice_id/LO_RES_HEIGHT, (MRI_Y_SIZE_vert + i)/LO_RES_HEIGHT, j);
            //   //get real
            //   double ps =0.0;
            //   double po =0.0;  
            //   //cubeMRI->getScaleOffset(&ps, &po);  
            //   ps = 1.;
            //   po = 0.;
            //   dDist = ps * dDist + po;
            //   dDistSecond = ps * dDistSecond + po;
            //   if(dDist > scale_val) dDist = scale_val;
            //   if(dDistSecond > scale_val) dDistSecond = scale_val;

            //   // mirror on X if left
            //   all_slice_data_x[ ACCESS_INDEX_SIGN_I_X *  vert_slice_id + ACCESS_INDEX_OFFSET_I_X  ][(int)i ][ (int)j] = (unsigned char) (dDist/scale_val * 255); 
            //   all_slice_dataSecond_x[ACCESS_INDEX_SIGN_I_X *  vert_slice_id + ACCESS_INDEX_OFFSET_I_X ][ (int)i  ][ (int)j] = (unsigned char) (dDistSecond/scale_val * 255); 
            // }
        }
    }
}