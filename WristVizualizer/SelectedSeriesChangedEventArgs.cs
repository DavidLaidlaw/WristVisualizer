using System;
using System.Collections.Generic;
using System.Text;

namespace WristVizualizer
{
    public class SelectedSeriesChangedEventArgs : EventArgs
    {
        private int _selectedIndex;
        public SelectedSeriesChangedEventArgs(int selectedIndex)
        {
            _selectedIndex = selectedIndex;
        }

        public int SelectedIndex
        {
            get { return _selectedIndex; }
        }
    }
}
