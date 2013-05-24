using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WristVizualizer
{
    public partial class WristPanelLayoutControl : UserControl
    {
        public WristPanelLayoutControl()
        {
            InitializeComponent();
        }

        public void addControl(Control control)
        {
            int index = tableLayoutPanel1.Controls.Count;
            tableLayoutPanel1.Controls.Add(control, 0, index);
        }

        public void removeControl(Control control)
        {
            tableLayoutPanel1.Controls.Remove(control);
        }
    }
}
