using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using libCoin3D;

namespace WristVizualizer
{
    abstract class Controller
    {
        public virtual Control Control
        {
            get { return _control; }
        }

        public abstract Separator Root
        {
            get;
        }

        public virtual void CleanUp()
        {
        }

        protected Control _control = null;
    }
}
