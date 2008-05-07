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

        private DateTime _startTime;

        public AnimationController()
        {
            _timer = new Timer();
            _timer.Enabled = false;
            _timer.Tick += new EventHandler(_timer_Tick);
            _currentFrame = 0;
        }

        public void setupAnimation(Separator[] Bones, Transform[][] BoneTransforms)
        {
            setupAnimation(null, Bones, null, BoneTransforms);
        }

        public void crapSetup_DELETEME(Separator[] Bones, HelicalTransform[][] BoneTransforms)
        {
            Transform[][] real = new Transform[BoneTransforms.Length][];
            for (int i = 0; i < BoneTransforms.Length; i++)
            {
                real[i] = new Transform[BoneTransforms[i].Length];
                for (int j = 0; j < BoneTransforms[i].Length; j++)
                {
                    if (BoneTransforms[i][j]!=null)
                        real[i][j] = BoneTransforms[i][j].ToTransformMatrix().ToTransform();
                }
            }
            setupAnimation(Bones, real); //call the real one!
        }

        public void setupAnimationForLinearInterpolation(Separator[] Bones, HelicalTransform[] BoneTransforms, int numSteps)
        {
            if (Bones.Length != BoneTransforms.Length)
                throw new ArgumentException("Number of bones must match the number of transforms");

            Transform[][] interpedTransforms = new Transform[Bones.Length][];
            for (int i = 0; i < Bones.Length; i++)
            {
                interpedTransforms[i] = new Transform[numSteps];

                if (BoneTransforms[i] == null)  //check if there are no transforms for this bone
                    continue; 

                //perform interpolation
                HelicalTransform[] htTransforms = BoneTransforms[i].LinearlyInterpolateMotion(numSteps);
                //save interpolated steps as Transform objects for later
                for (int j = 0; j < numSteps; j++)
                    interpedTransforms[i][j] = htTransforms[j].ToTransformMatrix().ToTransform();
            }
            setupAnimation(Bones, interpedTransforms);
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

            _numberOfFrames = _boneTransforms[0].Length; //save the number of frames
        }

        private void validateTransforms()
        {
            // 1) check num Bones
            if (_bones == null || _bones.Length == 0)
                throw new ArgumentException("Can not setup animation with 0 bones!");

            if (_root == null && _rootTransforms != null)
                throw new ArgumentException("Can not have root transforms when no node was passed for root");

            if (_boneTransforms==null || _boneTransforms[0].Length < 1) //check num frames for first bone
                throw new ArgumentException("Must pass AT LEAST 1 positions to be animated! Can't animate less then that, duh");

            if (_rootTransforms != null && _rootTransforms.Length != _boneTransforms[0].Length)
                throw new ArgumentException("There must be the same number of transforms for both Root and the Bones!");

            if (_bones.Length != _boneTransforms.Length)
                throw new ArgumentException("Must have an array of transforms for each bone!");

            //check each bone has same number of transforms as the first bone
            for (int i = 1; i < _bones.Length; i++)
            {
                if (_boneTransforms[i].Length != _boneTransforms[0].Length)
                    throw new ArgumentException(String.Format("Every bone must have the same number of transforms. For Bone {0}, found only {1} transform (expecting {2})", i, _boneTransforms[i].Length, _boneTransforms[0].Length));
            }
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            advanceFrame();
        }

        private void removeCurrentTransforms()
        {
            if (_root!=null && _root.hasTransform())
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
                //TimeSpan diff = DateTime.Now - _startTime;
                //MessageBox.Show(String.Format("Animation Took {0}ms",diff.ToString()));
            }
        }

        private void updateToCurrentFrame()
        {
            //remove old transforms
            removeCurrentTransforms();

            //apply current to root
            if (_root!=null && _rootTransforms[_currentFrame]!=null)
                _root.addTransform(_rootTransforms[_currentFrame]);

            //apply transform to other bones
            for (int i = 0; i < _bones.Length; i++)
            {
                if (_bones[i] == null || _boneTransforms[i][_currentFrame] == null)
                    continue;

                _bones[i].addTransform(_boneTransforms[i][_currentFrame]);
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
            //when we start, we want the first frame to start NOW, not wait for the timer event
            _timer.Start();
            _startTime = DateTime.Now;
            advanceFrame();
        }

        public void Stop()
        {
            _timer.Stop();
        }



        //TODO: do I need a Reset() or SetFrame(int) method?
    }
}
