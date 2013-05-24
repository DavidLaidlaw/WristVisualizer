using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using libWrist;
using libCoin3D;

namespace WristVizualizer
{
    abstract class Controller
    {
        public virtual Control Control
        {
            get { return _control; }
        }

        public abstract Separator Root
        {
            get;
        }

        protected Control _control = null;

        public virtual string ApplicationTitle { get { return null; } }
        public virtual string WatchedFileFilename { get { return null; } }
        public virtual string LastFileFilename { get { return null; } }

        // region where we define if the class has certain features
        // By default, they don't, unless the subclass overrides it :)
        public virtual bool CanImportObject { get { return false; } }
        public virtual bool CanCalculateDistanceMap { get { return false; } }
        public virtual bool CanEditBoneColors { get { return false; } }
        public virtual bool CanSaveToMovie { get { return false; } }

        public virtual bool CanViewSource { get { return false; } }

        public virtual bool CanCreateComplexAnimations { get { return false; } }
        public virtual bool CanAnimatePositionTransforms { get { return false; } }

        public virtual bool CanShowMetacarpalInertias { get { return false; } }
        public virtual bool CanShowCarpalInertias { get { return false; } }
        public virtual bool CanShowACS { get { return false; } }

        public virtual bool CanChangeReferenceBone { get { return false; } }


        // Empty functions that by default do nothing, but can
        // Be overridden in an inheriting class to have functionality
        public virtual void CleanUp() {}
        public virtual void saveToMovie() {}
        public virtual void calculateDistanceMapsToolClickedHandler() { }
        public virtual void EditBoneColorsShowDialog() { }

        public virtual void setInertiaVisibilityCarpalBones(bool visible) { }
        public virtual void setInertiaVisibilityMetacarpalBones(bool visible) { }
        public virtual void setACSVisibility(bool visible) { }

        public virtual void changeReferenceBoneByIndex(int referenceBoneIndex) { }

        public virtual void setPositionTransitionAnimationRate(int FPS, double animationDuration) { }
        public virtual bool AnimatePositionTransitions { set { } }

        public virtual DialogResult createComplexAnimationMovie() { return DialogResult.Cancel; }
        public virtual void EndFullAnimation() { }

        public virtual void ImportFilesToScene(string[] filenames) { }

        public enum Types
        {
            Sceneviewer,
            PosView,
            FullWrist,
            Xromm
        }

        public static Types GetTypeOfControllerForFile(string[] filenames)
        {
            //Check if this is a full wrist and if we should load as such
            if (WristFilesystem.isRadius(filenames))
            {
                string msg = "It looks like you are trying to open a radius.\n\nDo you wish to load the entire wrist?";
                if (DialogResult.Yes == MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    return Types.FullWrist;
            }

            if (XrommFilesystem.IsXrommFile(filenames)) //then we are in XROMM mode
            {
                string msg = "It looks like you are trying to open an XROMM model.\n\nDo you wish to load the entire model?";
                if (DialogResult.Yes == MessageBox.Show(msg, Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    return Types.Xromm;
            }

            if (PosViewController.IsPosViewFile(filenames))
            {
                return Types.PosView;
            }

            return Types.Sceneviewer;
        }
        
    }
}
