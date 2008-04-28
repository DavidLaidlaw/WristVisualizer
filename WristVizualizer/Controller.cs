using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using libCoin3D;

namespace WristVizualizer
{
    abstract class Controller
    {
        public virtual UserControl Control
        {
            get { return null; }
        }

        public abstract Separator Root
        {
            get;
        }

        public virtual void CleanUp()
        {
        }
    }
}
