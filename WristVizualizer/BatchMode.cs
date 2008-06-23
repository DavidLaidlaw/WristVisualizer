using System;
using System.Collections.Generic;
using System.Text;
using libWrist;
using libCoin3D;

namespace WristVizualizer
{
    class BatchMode
    {
        public BatchMode(CommandLineOptions options)
        {
            switch (options.getMode())
            {
                case CommandLineOptions.BatchModes.ContourData:
                    processContourData(options.subject, options.test_position, options.output, 0,0,0);
                    break;
                default:
                    throw new WristVizualizerException("Unknown batch mode encountered: " + options.getMode().ToString());
            }
        }

        private void processContourData(string subject, string test_position, string output, int referenceBoneIndex, int testBoneIndex, double contourDistance)
        {
            //ReferenceBone is the bone that we are generating the contour map on...

            //TODO: Need a try block in case of error....
            Wrist.Sides side = Wrist.getSideFromSeries(test_position);
            string radiusPath = Wrist.findRadius(subject, side);
            Wrist w = new Wrist(radiusPath);
            
            //first read in the reference bone
            ColoredBone refBone = new ColoredBone(w.bpaths[referenceBoneIndex]);

            //load the motion file
            int seriesIndex = w.getSeriesIndexFromName(test_position);
            TransformMatrix relativeMotion;
            if (seriesIndex == 0) //neutral
            {
                relativeMotion = new TransformMatrix(); //set to identity for neutral
            }
            else //non neutral
            {
                TransformMatrix[] transforms = DatParser.parseMotionFileToTransformMatrix(w.motionFiles[seriesIndex - 1]); //offset -1 to skip neutral
                //this is the transform that moves the reference bone into the distance field of the test bone
                relativeMotion = transforms[testBoneIndex].Inverse() * transforms[referenceBoneIndex];
            }

            //load in the distance field for the test bone
            CTmri testField = new CTmri(w.DistanceFieldPaths[testBoneIndex]);

            //generate the distance field
            double[] distanceMap = DistanceMaps.createDistanceMap(testField, refBone, relativeMotion);

            //go and generate the contours....yes?
        }
    }
}
