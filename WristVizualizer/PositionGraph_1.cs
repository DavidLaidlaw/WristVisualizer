using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using libWrist;

namespace WristVizualizer
{
    public partial class PositionGraph : UserControl
    {
        public event SelectedSeriesChangedHandler SelectedSeriesChanged;

        private const int MAX_FE = 90;
        private const int MAX_RU = 40;
        private const int DOT_SIZE = 4;

        private double _FE_conversion;
        private double _RU_conversion;

        private bool _showPS = true; //if we have information to show the PS angle

        private double[][] _positionsFE;
        private double[] _positionsPS;

        private Bitmap _baseImage;

        public PositionGraph()
        {
            InitializeComponent();

            _FE_conversion = (double)pictureBoxGraph.Height / (MAX_FE * 2);
            _RU_conversion = (double)pictureBoxGraph.Width / (MAX_RU * 2);

            _positionsFE = new double[0][];

            createGraph(); //create an empty graph
        }

        public void SetPositions(PostureCalculator.Posture[] positionsFE, PostureCalculator.PronationSupination[] positionsPS)
        {
            _positionsFE = new double[positionsFE.Length][];
            for (int i = 0; i < positionsFE.Length; i++)            
                _positionsFE[i] = new double[] { positionsFE[i].FE, positionsFE[i].RU };
            createGraph();            

            //check if we have PS information
            if (positionsPS == null)
            {
                HidePS();
                return;
            }
            
            ShowPS();
            _positionsPS = new double[positionsPS.Length];
            for (int i = 0; i < positionsPS.Length; i++)
                _positionsPS[i] = positionsPS[i].PronationAngle;
        }

        public void HidePS()
        {
            _showPS = false;
            textBoxPS.Clear();
        }

        public void ShowPS()
        {
            _showPS = true;
        }

        private void createGraph()
        {
            _baseImage = new Bitmap(pictureBoxGraph.Width, pictureBoxGraph.Height);
            Graphics g = Graphics.FromImage(_baseImage);
            g.FillRectangle(Brushes.White, 0, 0, _baseImage.Width, _baseImage.Height); //setup white canvas

            Pen black = new Pen(Color.Black, 1);
            //lets draw the grid
            g.DrawLine(black, 0, _baseImage.Height / 2, _baseImage.Width - 1, _baseImage.Height / 2); //horizontal line
            g.DrawLine(black, _baseImage.Width / 2, 0, _baseImage.Width / 2, _baseImage.Height - 1); //vertical line

            //draw test point
            foreach (double[] point in _positionsFE)
                drawSinglePoint(g, point);

            pictureBoxGraph.Image = _baseImage;
        }

        public void setCurrentVisisblePosture(int postureIndex)
        {
            showHighlightedPoint(postureIndex);
            showTextPostionForPosture(postureIndex);
        }

        private void showTextPostionForPosture(int postureIndex)
        {
            textBoxFE.Text = _positionsFE[postureIndex][0].ToString("0.00");
            textBoxRU.Text = _positionsFE[postureIndex][1].ToString("0.00");
            if (_showPS)
                textBoxPS.Text = _positionsPS[postureIndex].ToString("0.00");
        }

        private void drawSinglePoint(Graphics g, double[] point)
        {
            //first convert location to image coordinates
            // (0,0) is the top left pixel of the image....
            // pixels/degree for FE

            int newY = (int)((point[0] * _FE_conversion) + (pictureBoxGraph.Height / 2) - (DOT_SIZE/2));
            int newX = (int)((point[1] * _RU_conversion) + (pictureBoxGraph.Width / 2) - (DOT_SIZE / 2));

            g.FillEllipse(Brushes.Red, newX, newY, DOT_SIZE, DOT_SIZE);
        }

        private void pictureBoxGraph_MouseClick(object sender, MouseEventArgs e)
        {
            //Console.WriteLine("Mouse Click: ({0}, {1})",e.X,e.Y);

            int index = findClosestPoint(e.X, e.Y);
            setCurrentVisisblePosture(index); //update display
            //showHighlightedPoint(index); 

            //send the event
            if (SelectedSeriesChanged == null) return;
            SelectedSeriesChanged(this, new SelectedSeriesChangedEventArgs(index));
        }

        private void showHighlightedPoint(int index)
        {            
            Bitmap highlightedImage = (Bitmap)_baseImage.Clone();
            Graphics g = Graphics.FromImage(highlightedImage);
            drawCircleAroundPoint(g, _positionsFE[index]);
            pictureBoxGraph.Image = highlightedImage;            
        }

        private void drawCircleAroundPoint(Graphics g, double[] point)
        {
            //first convert location to image coordinates
            // (0,0) is the top left pixel of the image....
            // pixels/degree for FE

            int newY = (int)((point[0] * _FE_conversion) + (pictureBoxGraph.Height / 2) - (DOT_SIZE / 2));
            int newX = (int)((point[1] * _RU_conversion) + (pictureBoxGraph.Width / 2) - (DOT_SIZE / 2));

            //g.FillEllipse(Brushes.Red, newX, newY, DOT_SIZE, DOT_SIZE);
            Pen redpen = new Pen(Brushes.Red,1);
            g.DrawEllipse(redpen, newX - 2, newY - 2, DOT_SIZE + 4, DOT_SIZE + 4);
        }

        private int findClosestPoint(int imageX, int imageY)
        {
            //convert to FE & UR
            double FE = (imageY - (pictureBoxGraph.Height / 2)) / _FE_conversion;
            double RU = (imageX - (pictureBoxGraph.Width / 2)) / _RU_conversion;

            //return the index to the closest
            int closestIndex = 0;
            double minDist = Double.MaxValue;
            for (int i = 0; i < _positionsFE.Length; i++)
            {
                double dist = Math.Sqrt((_positionsFE[i][0] - FE) * (_positionsFE[i][0] - FE) + (_positionsFE[i][1] - RU) * (_positionsFE[i][1] - RU));
                if (dist < minDist)
                {
                    minDist = dist;
                    closestIndex = i;
                }
            }
            return closestIndex;
        }
    }
}
