using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using libWrist;
using libCoin3D;

namespace WristVizualizer
{
    class ShortAnimationController
    {
        public event EventHandler EndOfAnimationReached;

        private FullWrist _fullWrist;
        private TransformMatrix[][] _boneTransforms;

        private Timer _timer;
        private decimal _FPS;

        private int _currentFrame;
        private int _numberOfFrames;

        public ShortAnimationController()
        {
            _timer = new Timer();
            _timer.Enabled = false;
            _timer.Tick += new EventHandler(_timer_Tick);
            _currentFrame = 0;
        }


        public void SetupAnimationForLinearInterpolation(FullWrist wrist, int startPosition, int endPosition, int startFixedBoneIndex, int endFixedBoneIndex, int numSteps)
        {
            Bone startFixedBone = wrist.Bones[startFixedBoneIndex];
            Bone endFixedBone = wrist.Bones[endFixedBoneIndex];
            if (!startFixedBone.IsValidBone || !endFixedBone.IsValidBone)
                throw new ArgumentException("Cannot set fixed bone to a non-valid bone");

            _boneTransforms = new TransformMatrix[Wrist.NumBones][];
            for (int i = 0; i < Wrist.NumBones; i++)
            {
                if (!wrist.Bones[i].IsValidBone) continue;
                Bone bone = wrist.Bones[i];

                _boneTransforms[i] = bone.CalculateInterpolatedMotion(startPosition, endPosition, startFixedBone, endFixedBone, numSteps);
            }

            _fullWrist = wrist;
            _numberOfFrames = numSteps;
        }


        void _timer_Tick(object sender, EventArgs e)
        {
            advanceFrame();
        }

        private void advanceFrame()
        {
            _currentFrame++; //TODO: problem, shouldn't this happen at the end....?

            showFrame(_currentFrame);

            //now lets check if this was the last frame & we are not looping
            if (_currentFrame == _numberOfFrames - 1) //need -1 from end, because we want to know immediatly when its the last frame
            {
                //if here, we want to stop and announce the event
                _timer.Stop(); //top the timer, so we don't get called again
                if (EndOfAnimationReached != null) //if anyone is listening, lets announce ourselves!
                    EndOfAnimationReached(this, new EventArgs());               
            }
        }

        private void showFrame(int frameNum)
        {
            for (int i = 0; i < Wrist.NumBones; i++)
            {
                if (_fullWrist.Bones[i].IsValidBone)
                    _fullWrist.Bones[i].MoveToPosition(_boneTransforms[i][frameNum]);
            }
        }


        private static int convertToFPSToInerval(decimal FPS)
        {
            //need to convert from FPS -> Miliseconds/frame
            return (int)(1000 / (double)FPS);
        }

        public decimal FPS
        {
            get { return _FPS; }
            set
            {
                _FPS = value;
                _timer.Interval = convertToFPSToInerval(value);
            }
        }
        
        public void Start()
        {
            //when we start, we want the first frame to start NOW, not wait for the timer event
            _timer.Start();
            advanceFrame();
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }
}
