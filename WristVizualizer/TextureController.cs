using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using libCoin3D;
using libWrist;

namespace WristVizualizer
{
    class TextureController : Controller
    {
        private Separator _root;
        private Separator[] _bones;
        private TransformParser _parser;
        private TextureControl _textureControl;
        private TransformParser.EuclideanTransform[] _editableTransforms;

        private Hashtable[] _transformHashtables;

        public TextureController(Separator root, Separator[] bones, TransformParser parser)
        {
            _root = root;
            _bones = bones;
            _parser = parser;

            if (parser == null)
            {
                _textureControl = null;
                return;
            }

            _transformHashtables = parser.getArrayOfTransformHashtables();
            _textureControl = new TextureControl(parser.getArrayOfAllignmentSteps());
            _editableTransforms = parser.getArrayOfOptimizedBothTransforms();
            setupListeners();

            //set the current editable transform
            _textureControl_SelectedTransformChanged();
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
        }
                

        private void removeListeners()
        {
            _textureControl.SelectedTransformChanged -= new TextureControl.SelectedTransformChangedHandler(_textureControl_SelectedTransformChanged);
            _textureControl.EditableTransformChanged -= new EventHandler(_textureControl_EditableTransformChanged);
        }

        void _textureControl_EditableTransformChanged(object sender, EventArgs e)
        {
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
        }

        void _textureControl_SelectedTransformChanged()
        {
            int newIndex = _textureControl.selectedTransformIndex;

            //setup control for current transform....
            if (String.IsNullOrEmpty(_editableTransforms[newIndex].boneName))
            {
                //...no transform available....
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
                if (_editableTransforms[newIndex].boneName ==TextureSettings.TransformBNames[i])
                    _editableTransforms[newIndex].boneIndex = i;
            }
        }

        public override Separator Root
        {
            get { return _root; }
        }

        public override System.Windows.Forms.Control Control
        {
            get { return _textureControl; }
        }
    }
}
