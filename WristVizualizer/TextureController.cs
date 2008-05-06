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
            setupListeners();
        }

        public override void CleanUp()
        {
            if (_textureControl != null)
                removeListeners();
        }

        private void setupListeners()
        {
            _textureControl.SelectedTransformChanged += new TextureControl.SelectedTransformChangedHandler(_textureControl_SelectedTransformChanged);
        }

        private void removeListeners()
        {
            _textureControl.SelectedTransformChanged -= new TextureControl.SelectedTransformChangedHandler(_textureControl_SelectedTransformChanged);
        }

        void _textureControl_SelectedTransformChanged()
        {
            int newIndex = _textureControl.selectedTransformIndex;

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
                _root.addChild(_bones[i]);
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
