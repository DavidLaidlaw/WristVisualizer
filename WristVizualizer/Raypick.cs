using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using libCoin3D;

namespace WristVizualizer
{
    public class Raypick
    {
        private ExaminerViewer _viewer;

        public Raypick(ExaminerViewer viewer)
        {
            _viewer = viewer;

            _viewer.setRaypick();  //active raypicking event handler
            _viewer.OnRaypick += new RaypickEventHandler(_viewer_OnRaypick);
        }

        public void _viewer_OnRaypick(int x, int y)
        {
            //MessageBox.Show(String.Format("Clicked: ({0},{1})", x, y));
            Console.WriteLine(String.Format("Clicked: ({0},{1})", x, y));
        }

        public void stopRaypicking()
        {
            _viewer.OnRaypick -= new RaypickEventHandler(_viewer_OnRaypick);
            _viewer.resetRaypick();
        }

    }
}
