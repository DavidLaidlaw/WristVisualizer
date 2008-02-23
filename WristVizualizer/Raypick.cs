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

        public void _viewer_OnRaypick(float x, float y, float z)
        {
            Console.WriteLine(String.Format("Clicked: ({0},{1},{2})", x, y, z));
            //PointSelection p = new PointSelection();
            //p.ShowDialog();
        }

        public void stopRaypicking()
        {
            _viewer.OnRaypick -= new RaypickEventHandler(_viewer_OnRaypick);
            _viewer.resetRaypick();
        }

    }
}
