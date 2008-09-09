using System;
using System.Collections.Generic;
using System.Text;
using libCoin3D;

namespace libWrist
{
    public class AnimationCreator
    {
        public AnimationCreator()
        {
        }

        public Switch[] CreateAnimationSwitches(Separator[] bones, TransformMatrix[][] transforms, int[] animationOrder, int numFrames)
        {
            return CreateAnimationSwitches(0, bones, transforms, animationOrder, numFrames);
        }
        public Switch[] CreateAnimationSwitches(int fixedBoneIndex, Separator[] bones, TransformMatrix[][] transforms, int[] animationOrder, int numFrames)
        {
            Switch[] switches = new Switch[bones.Length];
            //loop through each bone, skip the first one (radius, we set that to be the fixed bone, yay!)
            for (int i = 1; i < bones.Length; i++)
            {
                if (bones[i] == null)
                    continue; //do nothing if the bone does not exist :)

                //now need to loop through this this bone
                switches[i] = createSwitchSingleBone(i, fixedBoneIndex, transforms, animationOrder, numFrames);
            }
            return switches;
        }

        private Switch createSwitchSingleBone(int boneIndex, TransformMatrix[][] transforms, int[] animationOrder, int numFrames)
        {
            return createSwitchSingleBone(boneIndex, 0, transforms, animationOrder, numFrames);
        }

        private Switch createSwitchSingleBone(int boneIndex, int fixedBoneIndex, TransformMatrix[][] transforms, int[] animationOrder, int numFrames)
        {
            Switch sw = new Switch();
            sw.reference();

            //add starting position, each animation does the animation and the ending frame, not the starting one :)
            TransformMatrix startPosition = calculateRelativeMotionFromNeutral(boneIndex, fixedBoneIndex, transforms, animationOrder[0]);
            sw.addChild(startPosition.ToTransform());
            for (int i = 0; i < animationOrder.Length - 1; i++) //not the last animation, there need start and end
            {
                Transform[] tforms = createSingleAnimation(boneIndex, fixedBoneIndex, transforms, animationOrder[i], animationOrder[i + 1], numFrames);
                foreach (Transform tform in tforms)
                {
                    sw.addChild(tform);
                }
            }
            sw.unrefNoDelete();
            return sw;
        }

        private Transform[] createSingleAnimation(int boneIndex, TransformMatrix[][] transforms, int startPosition, int endPosition, int numFrames)
        {
            return createSingleAnimation(boneIndex, 0, transforms, startPosition, endPosition, numFrames);
        }

        private Transform[] createSingleAnimation(int boneIndex, int fixedBoneIndex, TransformMatrix[][] transforms, int startPosition, int endPosition, int numFrames)
        {
            Transform[] finalTransforms = new Transform[numFrames];

            TransformMatrix startRelTransform = calculateRelativeMotionFromNeutral(boneIndex, fixedBoneIndex, transforms, startPosition);
            TransformMatrix endRelTransform = calculateRelativeMotionFromNeutral(boneIndex, fixedBoneIndex, transforms, endPosition);

            //this motion, relative to the radius
            TransformMatrix relMotion = endRelTransform * startRelTransform.Inverse();
            HelicalTransform relMotionHT = relMotion.ToHelical();
            HelicalTransform[] htTransforms = relMotionHT.LinearlyInterpolateMotion(numFrames);
            for (int i = 0; i < numFrames; i++)
            {
                TransformMatrix finalMotion = htTransforms[i].ToTransformMatrix() * startRelTransform;
                finalTransforms[i] = finalMotion.ToTransform();
            }

            return finalTransforms;
        }

        private TransformMatrix calculateRelativeMotionFromNeutral(int boneIndex, TransformMatrix[][] transforms, int positionIndex)
        {
            return calculateRelativeMotionFromNeutral(boneIndex, 0, transforms, positionIndex); //default to fixed radius :)
        }

        private TransformMatrix calculateRelativeMotionFromNeutral(int boneIndex, int fixedBoneIndex, TransformMatrix[][] transforms, int positionIndex)
        {
            if (positionIndex == 0) //the first position, also no change :)
                return new TransformMatrix();

            TransformMatrix tmFixedBone = transforms[positionIndex - 1][fixedBoneIndex];
            TransformMatrix tmCurrentBone = transforms[positionIndex - 1][boneIndex];

            return tmFixedBone.Inverse() * tmCurrentBone;
        }


        public Switch[] CreateHAMSwitches(Separator[] bones, TransformMatrix[][] transforms, TransformMatrix[] inertias, int[] animationOrder, int numFrames)
        {
            Switch[] switches = new Switch[bones.Length];
            //loop through each bone, skip the first one (radius, we set that to be the fixed bone, yay!)
            for (int i = 1; i < bones.Length; i++)
            {
                if (bones[i] == null)
                    continue; //do nothing if the bone does not exist :)

                //now need to loop through this this bone
                switches[i] = createHAMSwitchSingleBone(i, transforms, inertias, animationOrder, numFrames);
            }
            return switches;
        }

        private Switch createHAMSwitchSingleBone(int boneIndex, TransformMatrix[][] transforms, TransformMatrix[] inertias, int[] animationOrder, int numFrames)
        {
            Switch sw = new Switch();
            sw.reference();

            //add starting HAM (none shown), each animation does the animation and the ending frame, not the starting one :)
            Separator nullSep = new Separator();
            sw.addChild(nullSep);
            for (int i = 0; i < animationOrder.Length - 1; i++) //not the last animation, there need start and end
            {
                HelicalTransform tform = createSingleHAM(boneIndex, transforms, animationOrder[i], animationOrder[i + 1]);
                if (inertias != null && inertias[boneIndex] != null)
                {
                    double[] cent = { inertias[boneIndex].GetElement(0, 3), inertias[boneIndex].GetElement(1, 3), inertias[boneIndex].GetElement(2, 3) };
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

        private HelicalTransform createSingleHAM(int boneIndex, TransformMatrix[][] transforms, int startPosition, int endPosition)
        {
            TransformMatrix startRelTransform = calculateRelativeMotionFromNeutral(boneIndex, transforms, startPosition);
            TransformMatrix endRelTransform = calculateRelativeMotionFromNeutral(boneIndex, transforms, endPosition);

            //this motion, relative to the radius
            TransformMatrix relMotion = endRelTransform * startRelTransform.Inverse();
            return relMotion.ToHelical();
        }
    }
}
