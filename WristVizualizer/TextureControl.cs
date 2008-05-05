using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WristVizualizer
{
    public partial class TextureControl : UserControl
    {
        public delegate void SelectedTransformChangedHandler();
        public event SelectedTransformChangedHandler SelectedTransformChanged;

        public TextureControl(string[] transforms)
        {
            InitializeComponent();

            setTransforms(transforms);
            listBoxTransforms.SelectedIndex = (listBoxTransforms.Items.Count - 1);
        }

        private void setTransforms(string[] transforms)
        {
            listBoxTransforms.Items.Clear();
            listBoxTransforms.Items.AddRange(transforms);
            //foreach (string transform in transforms)
            //    listBoxTransforms.Items.Add(transform);
        }

        public int selectedTransformIndex
        {
            get { return listBoxTransforms.SelectedIndex; }
        }

        private void listBoxTransforms_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedTransformChanged != null)
                SelectedTransformChanged();
        }
    }
}
