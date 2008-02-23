using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace WristVizualizer
{
    public class Raypick : libCoin3D.IRaypickCallback
    {
        public Raypick()
        {
        }

        public override void clicked()
        {
            MessageBox.Show("Test");
        }

        public override void clicked(int x, int y)
        {
            MessageBox.Show(String.Format("Clicked: ({0},{1})",x,y));
        }

        //private void pointIntersectionToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    Raypick r = new Raypick();
        //    if (_viewer == null) return;
        //    _viewer.setRaypick(r);
        //    _viewer.OnRaypick += new libCoin3D.RaypickEventHandler(this.test);
        //}

        //public void test(int i, double d)
        //{
        //    Console.WriteLine("boo");
        //}


    }
}
