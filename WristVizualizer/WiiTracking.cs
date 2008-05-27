using System;
using System.Collections.Generic;
using System.Text;
//using System.Windows.Forms;
using System.ComponentModel;
using libCoin3D;
using WiimoteLib;

namespace WristVizualizer
{
    class WiiTracking
    {        
        private delegate void UpdateWiimoteStateDelegate(WiimoteLib.WiimoteChangedEventArgs args);

        private Wiimote _remote;
        private ExaminerViewer _viewer;
        private WristVizualizer _wristViz;

        private float _lastHeadX = 0;
        private float _lastHeadY = 0;

        public WiiTracking(WristVizualizer wristViz, ExaminerViewer viewer)
        {
            _wristViz = wristViz;
            _viewer = viewer;
        }

        public bool TryConnect()
        {
            _remote = new WiimoteLib.Wiimote();
            _remote.WiimoteChanged += new WiimoteLib.WiimoteChangedEventHandler(remote_WiimoteChanged);
            try
            {
                _remote.Connect();
                _remote.SetReportType(WiimoteLib.Wiimote.InputReport.IRAccel, true);
                _remote.SetLEDs(true, false, false, true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        void remote_WiimoteChanged(object sender, WiimoteLib.WiimoteChangedEventArgs args)
        {
            _wristViz.BeginInvoke(new UpdateWiimoteStateDelegate(UpdateWiimoteState), args);
        }

        private void UpdateWiimoteState(WiimoteLib.WiimoteChangedEventArgs args)
        {
            getWiimotePoints(args.WiimoteState);
        }




        private void getWiimotePoints(WiimoteLib.WiimoteState state)
        {
            if (state == null) return;
            int[][] points = new int[][] {new int[] {0,0,0,0},
                new int[] {0,0,0,0},
                new int[] {0,0,0,0},
                new int[] {0,0,0,0}};

            int currentPoint = 0;
            if (state.IRState.Found1)
            {
                points[currentPoint][0] = state.IRState.RawX1;
                points[currentPoint][1] = state.IRState.RawY1;
                currentPoint++;
            }
            if (state.IRState.Found2)
            {
                points[currentPoint][0] = state.IRState.RawX2;
                points[currentPoint][1] = state.IRState.RawY2;
                currentPoint++;
            }
            if (state.IRState.Found3)
            {
                points[currentPoint][0] = state.IRState.RawX3;
                points[currentPoint][1] = state.IRState.RawY4;
                currentPoint++;
            }
            if (state.IRState.Found4)
            {
                points[currentPoint][0] = state.IRState.RawX4;
                points[currentPoint][1] = state.IRState.RawY4;
                currentPoint++;
            }

            if (currentPoint < 2)
            {
                //not enough points....do something?
                //_remote.SetLEDs(false, false, false, true);

                return;
            }
            bool cameraIsAboveScreen = true;
            float dotDistanceInMM = 8.5f * 25.4f;//width of the wii sensor bar
            float screenHeightinMM = 20 * 25.4f;
            float radiansPerPixel = (float)(Math.PI / 4) / 1024.0f; //45 degree field of view with a 1024x768 camera
            float movementScaling = 1.0f;
            float cameraVerticaleAngle = 0; //begins assuming the camera is point straight forward

            float dx = points[0][0] - points[1][0];
            float dy = points[0][1] - points[1][1];
            float pointDist = (float)Math.Sqrt(dx * dx + dy * dy);

            float angle = radiansPerPixel * pointDist / 2;
            //in units of screen hieght since the box is a unit cube and box hieght is 1
            float headDist = movementScaling * (float)((dotDistanceInMM / 2) / Math.Tan(angle)) / screenHeightinMM;


            float avgX = (points[0][0] + points[1][0]) / 2.0f;
            float avgY = (points[0][1] + points[1][1]) / 2.0f;


            //should  calaculate based on distance

            float headX = (float)(movementScaling * Math.Sin(radiansPerPixel * (avgX - 512)) * headDist);

            float relativeVerticalAngle = (avgY - 384) * radiansPerPixel;//relative angle to camera axis

            float headY;
            if (cameraIsAboveScreen)
                headY = .5f + (float)(movementScaling * Math.Sin(relativeVerticalAngle + cameraVerticaleAngle) * headDist);
            else
                headY = -.5f + (float)(movementScaling * Math.Sin(relativeVerticalAngle + cameraVerticaleAngle) * headDist);
            //_remote.SetLEDs(true, false, false, true);

            float deltaX = _lastHeadX - headX;
            float deltaY = _lastHeadY - headY;

            _lastHeadX = headX;
            _lastHeadY = headY;
            //Console.WriteLine("Position: {0}, {1}", headX, headY);
            Console.WriteLine("delta: {0}, {1}", deltaX, deltaY);
            if (_viewer != null)
            {
                _viewer.Camera.rotateCameraInX(deltaY);
                _viewer.Camera.rotateCameraInY(deltaX);
            }
        }
    }
}
