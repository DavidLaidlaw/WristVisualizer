using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


namespace Vizualization
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //string fname = @"L:\Data\NIH_Phase_II\Young_Male\E16689\\S05L\motion15L05L.dat";
            //string rad = @"L:\Data\NIH_Phase_II\SNU\E32266\S15R\IV.files\rad15R.iv";
            //libWrist.Wrist w = new libWrist.Wrist("test");
            //w.setupPaths(rad);
            //w.findAllSeries();
            //libWrist.DatParser.parseDatFile(fname);
            //libWrist.DatParser.parseMotionFile2(fname);

            new libCoin3D.Coin3DBase();
            libCoin3D.ExaminerViewer viewer = new libCoin3D.ExaminerViewer((int)panel1.Handle);
            libCoin3D.Separator s = new libCoin3D.Separator();
            libCoin3D.ACS acs = new libCoin3D.ACS(40);
            s.addNode(acs);
            viewer.setSceneGraph(s);
        }

        private void decoratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            decoratorToolStripMenuItem.Checked = !decoratorToolStripMenuItem.Checked;

        }
    }
}