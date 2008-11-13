using System;
using System.Collections.Generic;
using System.Text;
using libCoin3D;

namespace libWrist
{
    public class AnimationCreator
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


        //private static Switch createSwitchSingleBone(int boneIndex, int fixedBoneIndex, TransformMatrix[][] transforms, int[] animationOrder, int numFrames)
        //{
        //    Switch sw = new Switch();
        //    sw.reference();

        //    //add starting position, each animation does the animation and the ending frame, not the starting one :)
        //    TransformMatrix startPosition = calculateRelativeMotionFromNeutral(boneIndex, fixedBoneIndex, transforms, animationOrder[0]);
        //    sw.addChild(startPosition.ToTransform());
        //    for (int i = 0; i < animationOrder.Length - 1; i++) //not the last animation, there need start and end
        //    {
        //        Transform[] tforms = createSingleAnimation(boneIndex, fixedBoneIndex, transforms, animationOrder[i], animationOrder[i + 1], numFrames);
        //        foreach (Transform tform in tforms)
        //        {
        //            sw.addChild(tform);
        //        }
        //    }
        //    sw.unrefNoDelete();
        //    return sw;
        //}

        //private static Transform[] createSingleAnimation(int boneIndex, int fixedBoneIndex, TransformMatrix[][] transforms, int startPosition, int endPosition, int numFrames)
        //{
        //    Transform[] finalTransforms = new Transform[numFrames];

        //    TransformMatrix startRelTransform = calculateRelativeMotionFromNeutral(boneIndex, fixedBoneIndex, transforms, startPosition);
        //    TransformMatrix endRelTransform = calculateRelativeMotionFromNeutral(boneIndex, fixedBoneIndex, transforms, endPosition);

        //    //this motion, relative to the radius
        //    TransformMatrix relMotion = endRelTransform * startRelTransform.Inverse();
        //    HelicalTransform relMotionHT = relMotion.ToHelical();
        //    HelicalTransform[] htTransforms = relMotionHT.LinearlyInterpolateMotion(numFrames);
        //    for (int i = 0; i < numFrames; i++)
        //    {
        //        TransformMatrix finalMotion = htTransforms[i].ToTransformMatrix() * startRelTransform;
        //        finalTransforms[i] = finalMotion.ToTransform();
        //    }

        //    return finalTransforms;
        //}

        //[Obsolete("DON'T USE")]
        //private static TransformMatrix calculateRelativeMotionFromNeutral(int boneIndex, int fixedBoneIndex, TransformMatrix[][] transforms, int positionIndex)
        //{
        //    if (positionIndex == 0) //the first position, also no change :)
        //        return new TransformMatrix();

        //    TransformMatrix tmFixedBone = transforms[positionIndex - 1][fixedBoneIndex];
        //    TransformMatrix tmCurrentBone = transforms[positionIndex - 1][boneIndex];

        //    return tmFixedBone.Inverse() * tmCurrentBone;
        //}


        //public static Switch[] CreateHAMSwitches(int fixedBoneIndex, Separator[] bones, TransformMatrix[][] transforms, TransformMatrix[] inertias, int[] animationOrder, int numFrames)
        //{
        //    Switch[] switches = new Switch[bones.Length];
        //    //loop through each bone, skip the first one (radius, we set that to be the fixed bone, yay!)
        //    for (int i = 0; i < bones.Length; i++)
        //    {
        //        if (bones[i] == null || i==fixedBoneIndex)
        //            continue; //do nothing if the bone does not exist :)

        //        //now need to loop through this this bone
        //        switches[i] = createHAMSwitchSingleBone(i,fixedBoneIndex, transforms, inertias, animationOrder, numFrames);
        //    }
        //    return switches;
        //}

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

        //private static Switch createHAMSwitchSingleBone(int boneIndex, int fixedBoneIndex, TransformMatrix[][] transforms, TransformMatrix[] inertias, int[] animationOrder, int numFrames)
        //{
        //    Switch sw = new Switch();
        //    sw.reference();

        //    //add starting HAM (none shown), each animation does the animation and the ending frame, not the starting one :)
        //    Separator nullSep = new Separator();
        //    sw.addChild(nullSep);
        //    for (int i = 0; i < animationOrder.Length - 1; i++) //not the last animation, there need start and end
        //    {
        //        HelicalTransform tform = createSingleHAM(boneIndex,fixedBoneIndex, transforms, animationOrder[i], animationOrder[i + 1]);
        //        if (inertias != null && inertias[boneIndex] != null)
        //        {
        //            double[] cent = { inertias[boneIndex].GetElement(0, 3), inertias[boneIndex].GetElement(1, 3), inertias[boneIndex].GetElement(2, 3) };
        //            tform.AdjustQToLocateHamNearCentroid(cent);
        //        }
        //        HamAxis axis = new HamAxis(tform.N[0], tform.N[1], tform.N[2], tform.Q[0], tform.Q[1], tform.Q[2]);
        //        for (int j = 0; j < numFrames - 1; j++) //do one less then num frames, no HAM shown for the final position
        //        {
        //            sw.addChild(axis);
        //        }
        //        sw.addChild(nullSep); //add the empty at the end :)
        //    }
        //    sw.unrefNoDelete();
        //    return sw;
        //}

        //private static HelicalTransform createSingleHAM(int boneIndex, int fixedBoneIndex, TransformMatrix[][] transforms, int startPosition, int endPosition)
        //{
        //    TransformMatrix startRelTransform = calculateRelativeMotionFromNeutral(boneIndex,fixedBoneIndex, transforms, startPosition);
        //    TransformMatrix endRelTransform = calculateRelativeMotionFromNeutral(boneIndex,fixedBoneIndex, transforms, endPosition);

        //    //this motion, relative to the radius
        //    TransformMatrix relMotion = endRelTransform * startRelTransform.Inverse();
        //    return relMotion.ToHelical();
        //}
    }
}
