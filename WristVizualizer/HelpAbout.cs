using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WristVizualizer
{
    public partial class HelpAbout : Form
    {
        public HelpAbout()
        {
            InitializeComponent();
            labelVersion.Text = String.Format("Version {0}", Application.ProductVersion);
        }

        private void linkLabelEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:" + linkLabelEmail.Text);
        }

    }
}