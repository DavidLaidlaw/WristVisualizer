using System;
using System.Collections.Generic;
using System.Text;
using libCoin3D;

namespace libWrist
{
    public static class AnimationCreator
    {
        public static Switch CreateAnimationSwitch(Bone bone, Bone fixedBone, int[] animationOrder, int numFrames)
        {
            Switch sw = new Switch();
            sw.reference();

            //add starting position, each animation does the animation and the ending frame, not the starting one :)
            TransformMatrix startPosition = bone.CalculateRelativeMotionFromNeutral(animationOrder[0], fixedBone);
            sw.addChild(startPosition.ToTransform());
            for (int i = 0; i < animationOrder.Length - 1; i++) //not the last animation, there need start and end
            {
                TransformMatrix[] tmTransforms = bone.CalculateInterpolatedMotion(animationOrder[i], animationOrder[i + 1], fixedBone, numFrames);
                foreach (TransformMatrix tform in tmTransforms)
                {
                    sw.addChild(tform.ToTransform());
                }
            }
            sw.unrefNoDelete();
            return sw;
        }

        public static Switch CreateHAMSwitch(Bone bone, Bone fixedBone, int[] animationOrder, int numFrames)
        {
            Switch sw = new Switch();
            sw.reference();

            //add starting HAM (none shown), each animation does the animation and the ending frame, not the starting one :)
            Separator nullSep = new Separator();
            sw.addChild(nullSep);
            for (int i = 0; i < animationOrder.Length - 1; i++) //not the last animation, there need start and end
            {
                TransformMatrix tmTransform = bone.CalculateRelativeMotion(animationOrder[i], animationOrder[i + 1], fixedBone);
                HelicalTransform tform = tmTransform.ToHelical();
                if (bone.HasInertia)
                {
                    double[] cent = { bone.InertiaMatrix.GetElement(0, 3), bone.InertiaMatrix.GetElement(1, 3), bone.InertiaMatrix.GetElement(2, 3) };
                    tform.AdjustQToLocateHamNearCentroid(cent);
                }
                HamAxis axis = new HamAxis(tform.N[0], tform.N[1], tform.N[2], tform.Q[0], tform.Q[1], tform.Q[2]);
                for (int j = 0; j < numFrames - 1; j++) //do one less then num frames, no HAM shown for the final position
                {
                    sw.addChild(axis);
                }
                sw.addChild(nullSep); //add the empty at the end :)
            }
            sw.unrefNoDelete();
            return sw;
        }

    }
}
