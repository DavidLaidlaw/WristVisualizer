using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WristVizualizer
{
    public partial class SideSelector : Form
    {
        public enum SideResult
        {
            LEFT,
            RIGHT,
            CANCEL
        }

        private SideResult _result;

        public SideSelector()
        {
            _result = SideResult.CANCEL;
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (radioLeft.Checked)
                _result = SideResult.LEFT;
            else if (radioRight.Checked)
                _result = SideResult.RIGHT;
            else
                _result = SideResult.CANCEL;

            this.Close();
        }

        public SideResult results
        {
            get { return _result; }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            _result = SideResult.CANCEL;
            this.Close();
        }
    }
}