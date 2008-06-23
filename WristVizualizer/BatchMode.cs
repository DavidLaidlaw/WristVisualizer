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
            
            //first read in the two bones
            ColoredBone refBone = new ColoredBone(w.bpaths[referenceBoneIndex]);
            ColoredBone testBone = new ColoredBone(w.bpaths[testBoneIndex]);

            //load the motion file
            int seriesIndex = w.getSeriesIndexFromName(test_position);
            TransformMatrix[] transforms = DatParser.parseMotionFileToTransformMatrix(w.motionFiles[seriesIndex]);

            //load in the distance fields
            CTmri refField = new CTmri(w.DistanceFieldPaths[referenceBoneIndex]);
            CTmri testField = new CTmri(w.DistanceFieldPaths[testBoneIndex]);


        }
    }
}
