using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using libCoin3D;
using libWrist;

//using CenterballDragger;

namespace WristVizualizer
{
    class TextureController : Controller
    {
        private Separator _root;
        private Separator[] _bones;
        private TransformParser _parser;
        private TextureControl _textureControl;
        private FullWristControl _fullWristControl;
        private WristPanelLayoutControl _mainControlPanel;
        private TransformParser.EuclideanTransform[] _editableTransforms;

        private float[] currTranslation;
        private float[] currRotation;

        private Hashtable[] _transformHashtables;

        //values that are used for the centerball
        private Boolean CBenabled = true;
        float[] scaleValues;
        float[] beginningTranslation;
        CenterballDragger _centerballDragger;//the current centerball dragger
        Separator _rootSeparator;//the eparator containing the centerball
        Boolean _centerballVisible = true;

        //node for the volume render
        CallbackNode volNode;

        public TextureController(Separator root, Separator[] bones, TransformParser parser)
        {
            _root = root;
            _bones = bones;
            _parser = parser;

            currTranslation = new float[3];
            currRotation = new float[3];


            currTranslation[0] = 0; currRotation[0] = 0;
            currTranslation[1] = 0; currRotation[1] = 0;
            currTranslation[2] = 0; currRotation[2] = 0;

            if (parser == null)
            {
                _textureControl = null;
                return;
            }

            _transformHashtables = parser.getArrayOfTransformHashtables();
            _textureControl = new TextureControl(parser.getArrayOfAllignmentSteps());
            _fullWristControl = new FullWristControl();
            _fullWristControl.setupControl(WristFilesystem.LongBoneNames, false);
            _mainControlPanel = new WristPanelLayoutControl();
            _mainControlPanel.addControl(_textureControl);
            _mainControlPanel.addControl(_fullWristControl);

            _editableTransforms = parser.getArrayOfOptimizedBothTransforms();
            setupListeners();

            //set the current editable transform
            _textureControl_SelectedTransformChanged();


            /////////////////////////////////////


            float[] center = _textureControl.getCurrentCenterOfRotation();
            float[] translate = _textureControl.getCurrentTranslation();
            float[] rotation = _textureControl.getCurrentRotation();

            currTranslation[0] = translate[0]; currRotation[0] = rotation[0];
            currTranslation[1] = translate[1]; currRotation[1] = rotation[1];
            currTranslation[2] = translate[2]; currRotation[2] = rotation[2];

            _rootSeparator = new Separator();
            _rootSeparator.reference();
            root.addChild(_rootSeparator);
            _centerballVisible = true;
            Scale myScale = new Scale();

            _centerballDragger = new CenterballDragger();
            //get the center of rotation from the form elements
            center[0] += translate[0];
            center[1] += translate[1];
            center[2] += translate[2];



            _rootSeparator.addNode(myScale);
            scaleValues = new float[3];
            scaleValues[0] = scaleValues[1] = scaleValues[2] = 10;
            myScale.setScaleFactor(scaleValues[0], scaleValues[1], scaleValues[2]);

            _centerballDragger.setRotation(rotation[0], rotation[1], rotation[2]);
            _centerballDragger.setTranslation(center[0] / scaleValues[0], center[1] / scaleValues[1], center[2] / scaleValues[2]);

          
            _rootSeparator.addChild(_centerballDragger);
            //_centerballDragger.setRotation(rotation[0], rotation[1], rotation[2]);
            //_centerballDragger.setCenter(center[0] / scaleValues[0], center[1] / scaleValues[1], center[2] / scaleValues[2]);
            beginningTranslation = _textureControl.getCurrentTranslation();

            //tempFUNC myFuncObj = new tempFUNC(_textureControl_EditableTransformChangedFromCenterball);
            CenterballDragger.delFunc d = new CenterballDragger.delFunc(_textureControl_EditableTransformChangedFromCenterball);
            _centerballDragger.addCB(d);


           //the vol node neads a texture--preferably some kind of int array
            volNode = new CallbackNode();
            volNode.setUpCallBack();
            _root.addChild(volNode);
            //////////////////////////////////////

            wasRotated();
        }


        public bool wasRotated()
        {
            float[] rotation = _textureControl.getCurrentRotation();
            float[] translate = _textureControl.getCurrentTranslation();

            if(rotation[0]!=currRotation[0] || rotation[1]!=currRotation[1] ||rotation[2]!=currRotation[2] ){
                currRotation[0] = rotation[0];
                currRotation[1] = rotation[1];
                currRotation[2] = rotation[2];
                currTranslation[0] = translate[0];
                currTranslation[1] = translate[1];
                currTranslation[2] = translate[2]; 
                return true;
            }
            currRotation[0] = rotation[0];
            currRotation[1] = rotation[1];
            currRotation[2] = rotation[2];
            currTranslation[0] = translate[0]; 
            currTranslation[1] = translate[1]; 
            currTranslation[2] = translate[2]; 

            return false;
        }

        public void resetCenterball()
        {
            if (_centerballDragger != null && _textureControl.isEnabled())//&& not disabled
            {
                //makevisible
                if (!_centerballVisible)
                {
                    _centerballVisible = true;
                    _root.addChild(_rootSeparator);
                }

                _rootSeparator.removeChild(_centerballDragger);

                _centerballDragger = null;
                _centerballDragger = new CenterballDragger();
                _rootSeparator.addChild(_centerballDragger);

                float[] center = new float[3];
                center = _textureControl.getCurrentCenterOfRotation();
                float[] translate = new float[3];
                translate = _textureControl.getCurrentTranslation();
                center[0] += translate[0];
                center[1] += translate[1];
                center[2] += translate[2];
                float[] rotation = new float[3];
                rotation = _textureControl.getCurrentRotation();

                _centerballDragger.setRotation(rotation[0], rotation[1], rotation[2]);
                _centerballDragger.setTranslation(center[0] / scaleValues[0], center[1] / scaleValues[1], center[2] / scaleValues[2]);

                beginningTranslation = _textureControl.getCurrentTranslation();

                tempFUNC myFuncObj = new tempFUNC(_textureControl_EditableTransformChangedFromCenterball);
                CenterballDragger.delFunc d = new CenterballDragger.delFunc(_textureControl_EditableTransformChangedFromCenterball);
                _centerballDragger.addCB(d);
            }
            else if (_centerballDragger != null && !_textureControl.isEnabled())//&& disabled
            {
                //hide
                if (_centerballVisible)
                {
                    _root.removeChild(_rootSeparator);
                    _centerballVisible = false;
                }

            }
            
            wasRotated();
        }



        public override void CleanUp()
        {
            if (_textureControl != null)
                removeListeners();
        }

        private void setupListeners()
        {
            _textureControl.SelectedTransformChanged += new TextureControl.SelectedTransformChangedHandler(_textureControl_SelectedTransformChanged);
            _textureControl.EditableTransformChanged += new EventHandler(_textureControl_EditableTransformChanged);
            _fullWristControl.BoneHideChanged += new BoneHideChangedHandler(_fullWristControl_BoneHideChanged);

            //setup up listeners for manipulators
        }

        private void removeListeners()
        {
            _textureControl.SelectedTransformChanged -= new TextureControl.SelectedTransformChangedHandler(_textureControl_SelectedTransformChanged);
            _textureControl.EditableTransformChanged -= new EventHandler(_textureControl_EditableTransformChanged);
            _fullWristControl.BoneHideChanged -= new BoneHideChangedHandler(_fullWristControl_BoneHideChanged);
            //remove listeners for manipulators


        }



        void _fullWristControl_BoneHideChanged(object sender, BoneHideChangeEventArgs e)
        {
            if (_bones[e.BoneIndex] == null) return;

            if (e.BoneHidden)
            {
                _bones[e.BoneIndex].reference();
                _root.removeChild(_bones[e.BoneIndex]);
            }
            else
            {
                _root.addChild(_bones[e.BoneIndex]);
                _bones[e.BoneIndex].unref();
            }
        }


        void _textureControl_EditableTransformChanged(object sender, EventArgs e)
        {

            if (CBenabled)
            {
                CBenabled = false;

                int currentIndex = _textureControl.selectedTransformIndex;
                int boneIndex = _editableTransforms[currentIndex].boneIndex;

                //create new transform... needed edited part, allong with the part that leads up to it from the original parsed file
                TransformMatrix tm = _textureControl.getEditedTransform() * _editableTransforms[currentIndex].StartingTransform;
                Transform tfrm = tm.ToTransform();

                //remove the old one first
                if (_bones[boneIndex].hasTransform())
                    _bones[boneIndex].removeTransform();

                //add edited
                _bones[boneIndex].addTransform(tfrm);


                ////call the manipulator method
                //float[] rotation = _textureControl.getCurrentRotation();
                //float[] translation = _textureControl.getCurrentTranslation();
                //float[] center = _textureControl.getCurrentCenterOfRotation();
                ////translation[0] += center[0];
                ////translation[1] += center[1];
                ////translation[2] += center[2];

                //float xr,yr,zr,xt,yt,zt;
                //xr=rotation[0] - currRotation[0];yr=rotation[1] - currRotation[1];zr= rotation[2] - currRotation[2];
                //xt = currTranslation[0] - translation[0]; yt = currTranslation[1] - translation[1]; zt = currTranslation[2] - translation[2];
              
                //xt = translation[0]; yt = translation[1]; zt = translation[2];
                //xt /= scaleValues[0]; yt /= scaleValues[1]; zt /= scaleValues[2];

                //bool doTranslate = !wasRotated();
                //_centerballDragger.memberCallbackFromText((translation[0] - beginningTranslation[0]) / scaleValues[0], (translation[1] - beginningTranslation[1]) / scaleValues[1], (translation[2] - beginningTranslation[2]) / scaleValues[2], xr, yr, zr, doTranslate);
                //thisone//_centerballDragger.memberCallbackFromText((translation[0] + center[0]) / scaleValues[0], (translation[1] + center[1]) / scaleValues[1], (translation[2] + center[2]) / scaleValues[2], xr, yr, zr, doTranslate);
                //_centerballDragger.memberCallbackFromText(xt,yt,zt, xr,yr,zr);


                resetCenterball();
                //next thing-after each text box thing, create a new centerball, give it the above calculated locaiton for a new center
                //extract the old quaternions and put them into the new centerball, and apply the change in orientation to the centerball

                wasRotated();

                CBenabled = true;
            }
        }


        public delegate void tempFUNC(float x, float y, float z, float q0, float q1, float q2, float q3);


        void _textureControl_EditableTransformChangedFromCenterball(float x, float y, float z, float q0, float q1, float q2, float q3)
        {
            CBenabled = false;
            //here I must also change the text boxes

            //often times the values are too small so I have deciding to limit the manipulator to movement broader than m
            float[] center = _textureControl.getCurrentCenterOfRotation();
            float[] trans = _textureControl.getCurrentTranslation();



            float m = 0.0005f;
            if (Math.Abs(x) < m)
            {
                x = 0;
            }
            if (Math.Abs(y) < m)
            {
                y = 0;
            }
            if (Math.Abs(z) < m)
            {
                z = 0;
            }

            x *= this.scaleValues[0];
            y *= this.scaleValues[1];
            z *= this.scaleValues[2];

            //Console.WriteLine("csharp delta " + x + ", " + y + ", " + z);
            //Console.Out.Flush();

            int currentIndex = _textureControl.selectedTransformIndex;
            int boneIndex = _editableTransforms[currentIndex].boneIndex;

            //create new transform... needed edited part, allong with the part that leads up to it from the original parsed file
            //the starting transform is from the original file
            TransformMatrix tm = _textureControl.setNewEditedTransform(x, y, z, q0, q1, q2, q3) * _editableTransforms[currentIndex].StartingTransform;
            Transform tfrm = tm.ToTransform();

            //remove the old one first
            if (_bones[boneIndex].hasTransform())
                _bones[boneIndex].removeTransform();

            //add edited
            _bones[boneIndex].addTransform(tfrm);

            wasRotated();
            CBenabled = true;

            //if (Math.Abs(center[0] + trans[0] + x) > 252 || Math.Abs(center[1] + trans[1] + y) > 252 || Math.Abs(center[2] + trans[2] + z) > 252)
            //{
            //    Console.WriteLine("resetting");
            //    resetCenterball();
            //    //_centerballDragger.setCBEnabled(false);
            //} 
        }




        void _textureControl_SelectedTransformChanged()
        {
            int newIndex = _textureControl.selectedTransformIndex;

            //setup control for current transform....
            if (String.IsNullOrEmpty(_editableTransforms[newIndex].boneName))
            {
                _textureControl.clearEditableTransform();
            }
            else
            {
                _textureControl.setEditableTransform(_editableTransforms[newIndex].CenterRotation, _editableTransforms[newIndex].Rotation, _editableTransforms[newIndex].Translation);
            }

            for (int i = 0; i < TextureSettings.ShortBNames.Length; i++)
            {
                //remove the old transform first
                if (_bones[i].hasTransform())
                    _bones[i].removeTransform();

                //try and load transforms
                if (_transformHashtables[newIndex] != null &&
                    _transformHashtables[newIndex].ContainsKey(TextureSettings.TransformBNames[i]))
                {
                    Transform tfrm = new Transform();
                    TransformParser.addTfmMatrixtoTransform((TransformMatrix)_transformHashtables[newIndex][TextureSettings.TransformBNames[i]], tfrm);

                    //now add the new one
                    _bones[i].addTransform(tfrm);
                }

                //check if this is the editable transform
                if (_editableTransforms[newIndex].boneName == TextureSettings.TransformBNames[i])
                    _editableTransforms[newIndex].boneIndex = i;
            }

            //reset the center of the centerball
            resetCenterball();
        }

        public override Separator Root
        {
            get { return _root; }
        }

        public override System.Windows.Forms.Control Control
        {
            get { return _mainControlPanel; }
        }



    }
}
