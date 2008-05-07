using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using libWrist;
using libCoin3D;

namespace WristVizualizer
{
    class AnimationController
    {
        public event EventHandler EndOfAnimationReached;

        private Separator _root;
        private Separator[] _bones;
        private Transform[] _rootTransforms;
        private Transform[][] _boneTransforms;

        private Timer _timer;
        private decimal _FPS;
        private bool _loopAnimation;

        private int _currentFrame;
        private int _numberOfFrames;

        public AnimationController()
        {
            _timer = new Timer();
            _timer.Enabled = false;
            _timer.Tick += new EventHandler(_timer_Tick);
            _currentFrame = 0;
        }

        public void setupAnimation(Separator Root, Separator[] Bones, Transform[] RootTransforms, Transform[][] BoneTransforms)
        {
            //first save the variables
            _root = Root;
            _bones = Bones;
            _rootTransforms = RootTransforms;
            _boneTransforms = BoneTransforms;
            
            //now validate
            validateTransforms();

            _numberOfFrames = _boneTransforms.Length; //save the number of frames
        }

        private void validateTransforms()
        {
            // 1) check num Bones
            if (_bones == null || _bones.Length == 0)
                throw new ArgumentException("Can not setup animation with 0 bones!");

            if (_root == null && _rootTransforms != null)
                throw new ArgumentException("Can not have root transforms when no node was passed for root");

            if (_boneTransforms==null || _boneTransforms.Length < 2)
                throw new ArgumentException("Must pass AT LEAST 2 positions to be animated! Can't animate less then that, duh");

            if (_rootTransforms != null && _rootTransforms.Length != _boneTransforms.Length)
                throw new ArgumentException("There must be the same number of transforms for both Root and the Bones!");

            for (int i = 0; i < _boneTransforms.Length; i++)
            {
                if (_boneTransforms[i].Length != _bones.Length)
                    throw new ArgumentException(String.Format("Every frame must specify a transform for every bone. For frame {0}, found only {1} transform (expecting {2})", i, _boneTransforms[i].Length, _bones.Length));
            }
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            advanceFrame();
        }

        private void removeCurrentTransforms()
        {
            if (_root.hasTransform())
                _root.removeTransform();
            for (int i = 0; i < _bones.Length; i++)
            {
                //skip missing bones & remove the old
                if (_bones[i] != null && _bones[i].hasTransform())
                    _bones[i].removeTransform();
            }
        }

        private void advanceFrame()
        {
            _currentFrame++;
            //check if we have looped (should only happen if _loopAnimation==true)
            if (_currentFrame == _numberOfFrames)
                _currentFrame = 0; //reset to the first frame

            updateToCurrentFrame();

            //now lets check if this was the last frame & we are not looping
            if (!_loopAnimation && _currentFrame == _numberOfFrames - 1) //need -1 from end, because we want to know immediatly when its the last frame
            {
                //if here, we want to stop and announce the event
                _timer.Stop(); //top the timer, so we don't get called again
                if (EndOfAnimationReached != null) //if anyone is listening, lets announce ourselves!
                    EndOfAnimationReached(this, new EventArgs()); 
            }
        }

        private void updateToCurrentFrame()
        {
            //remove old transforms
            removeCurrentTransforms();

            //apply current to root
            if (_rootTransforms != null & _rootTransforms[_currentFrame]!=null)
                _root.addTransform(_rootTransforms[_currentFrame]);

            //apply transform to other bones
            for (int i = 0; i < _bones.Length; i++)
            {
                if (_bones[i] == null || _boneTransforms[_currentFrame][i] == null)
                    continue;

                _bones[i].addTransform(_boneTransforms[_currentFrame][i]);
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

        /// <summary>
        /// Whether the animation should loop back to the begining when its finished,
        /// or if it should stop on the last frame.
        /// </summary>
        public bool LoopAnimation
        {
            get { return _loopAnimation; }
            set { _loopAnimation = value; }
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }



        //TODO: do I need a Reset() or SetFrame(int) method?
    }
}
