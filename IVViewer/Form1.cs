using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using libCoin3D;

namespace IVViewer
{
    public partial class Form1 : Form
    {
        private ExaminerViewer _viewer;

        public Form1(string[] args)
        {
            InitializeComponent();

            new libCoin3D.Coin3DBase();
            _viewer = new ExaminerViewer((int)panel1.Handle);
            libCoin3D.Separator s = new libCoin3D.Separator();
            foreach (string file in args)
                s.addFile(file);

            //libCoin3D.ACS acs = new libCoin3D.ACS(40);
            //s.addNode(acs);
            _viewer.setSceneGraph(s);
            if (args.Length >= 1)
                this.Text = "IVViewer - " + args[0];

        }




    }
}