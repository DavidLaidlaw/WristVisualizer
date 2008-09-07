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

        public Switch[] test(Separator[] bones, TransformMatrix[][] transforms, int[] animationOrder, int numFrames)
        {
            Switch[] switches = new Switch[bones.Length];
            //loop through each bone, skip the first one (radius, we set that to be the fixed bone, yay!)
            for (int i = 1; i < bones.Length; i++)
            {
                if (bones[i] == null)
                    continue; //do nothing if the bone does not exist :)

                //now need to loop through this this bone
                switches[i] = createSwitchSingleBone(i, transforms, animationOrder, numFrames);
            }
            return switches;
        }

        private Switch createSwitchSingleBone(int boneIndex, TransformMatrix[][] transforms, int[] animationOrder, int numFrames)
        {
            Switch sw = new Switch();
            sw.reference();

            //add starting position, each animation does the animation and the ending frame, not the starting one :)
            TransformMatrix startPosition = calculateRelativeMotionFromNeutral(boneIndex, transforms, animationOrder[0]);
            sw.addChild(startPosition.ToTransform());
            for (int i = 0; i < animationOrder.Length - 1; i++) //not the last animation, there need start and end
            {
                Transform[] tforms = createSingleAnimation(boneIndex, transforms, animationOrder[i], animationOrder[i + 1], numFrames);
                foreach (Transform tform in tforms)
                    sw.addChild(tform);
            }
            sw.unrefNoDelete();
            return sw;
        }

        private Transform[] createSingleAnimation(int boneIndex, TransformMatrix[][] transforms, int startPosition, int endPosition, int numFrames)
        {
            Transform[] finalTransforms = new Transform[numFrames];

            TransformMatrix startTransform, endTransform;
            TransformMatrix startRadTransform, endRadTransform;
            TransformMatrix startRelTransform, endRelTransform;
            if (startPosition == 0)
            { 
                //special
                startRelTransform = new TransformMatrix(); //identity
            } 
            else
            {
                startTransform = transforms[startPosition - 1][boneIndex];
                startRadTransform = transforms[startPosition - 1][0];
                startRelTransform = startRadTransform.Inverse() * startTransform;
            }
            if (endPosition == 0)
            { 
                //special
                endRelTransform = new TransformMatrix(); //identity
            } 
            else
            {
                endTransform = transforms[endPosition - 1][boneIndex];
                endRadTransform = transforms[endPosition - 1][0];
                endRelTransform = endRadTransform.Inverse() * endTransform;
            }

            //TODO: is this the same?
            //TRY 2
            startRelTransform = calculateRelativeMotionFromNeutral(boneIndex, transforms, startPosition);
            endRelTransform = calculateRelativeMotionFromNeutral(boneIndex, transforms, endPosition);

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
            if (boneIndex == 0) //relative to the Radius is always the same :)
                return new TransformMatrix();

            if (positionIndex == 0) //the first position, also no change :)
                return new TransformMatrix();

            TransformMatrix tmFixedBone = transforms[positionIndex - 1][0];
            TransformMatrix tmCurrentBone = transforms[positionIndex - 1][boneIndex];

            return tmFixedBone.Inverse() * tmCurrentBone;
        }
    }
}
