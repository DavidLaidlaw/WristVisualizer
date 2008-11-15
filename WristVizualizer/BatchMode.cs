using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using libWrist;
using libCoin3D;

namespace WristVizualizer
{
    class BatchMode
    {
        public BatchMode(CommandLineOptions options)
        {
            //Lets setup the batch mode....
            try
            {
                ConvertOptionsToFullWrist(options);
            }
            catch (Exception ex)
            {
                libWrist.ExceptionHandling.HandledExceptionManager.ShowDialog("Error processing batch mode.\n"+ex.Message, "Batch mode did not succeed", "", ex);
                System.Environment.Exit(1);
            }
        }


        private static void ConvertOptionsToFullWrist(CommandLineOptions options)
        {
            PreliminaryValidation(options);
            Wrist.Sides side = Wrist.GetSideFromString(options.SideString);
            Wrist wrist = new Wrist(Wrist.findRadius(options.Subject, side));
            FullWrist fullWrist = new FullWrist(wrist);

            int referenceBoneIndex = options.GetReferenceBoneIndex();
            int testBoneIndex = options.GetTestBoneIndex();
            //int fixedBoneIndex = options.GetFixedBoneIndex();
            int[] positionList = GetPositionIndexes(wrist, options.GetPositionNames());
            double[] cDistances = options.GetCoutourDistances();

            fullWrist.LoadSelectBonesAndDistancesForBatchMode(new int[] { testBoneIndex }, new int[] { referenceBoneIndex }); //only load selective bones..


            if (options.MultiThread)
                ProcessMultiThreaded(fullWrist, referenceBoneIndex, testBoneIndex, positionList, cDistances);
            else
                ProcessSingleThread(fullWrist, referenceBoneIndex, testBoneIndex, positionList, cDistances);
            

            SaveDataCentroidArea(options, fullWrist);
        }

        private static void SaveDataCentroidArea(CommandLineOptions options, FullWrist fullWrist)
        {
            int refBoneIndex = options.GetReferenceBoneIndex();
            int testBoneIndex = options.GetTestBoneIndex();
            int[] positionList = GetPositionIndexes(fullWrist.Wrist, options.GetPositionNames());

            for (int i = 0; i < positionList.Length; i++)
            {
                Contour c = fullWrist.Bones[refBoneIndex].GetContourForPosition(positionList[i]);
                string line = CreateContourResultsLine(c);
                if (options.SaveCentroidAreaDefault)
                {
                    string fname = CreateOutputFilename(fullWrist, refBoneIndex, testBoneIndex, positionList[i]);
                    writeToFile(fname, line);
                }
                if (options.AppendMasterAreaCentroidFname != null)
                {
                    string fname = options.AppendMasterAreaCentroidFname;
                    appendToFile(fname, line);
                }
            }
        }

        private static string CreateOutputFilename(FullWrist fullWrist, int refBoneIndex, int testBoneIndex, int posIndex)
        {
            Wrist w = fullWrist.Wrist;
            string pos = (posIndex == 0) ? w.neutralSeries : w.series[posIndex - 1];
            string refBname = Wrist.ShortBoneNames[refBoneIndex];
            string testBname = Wrist.ShortBoneNames[testBoneIndex];
            string fname = String.Format("{0}_{1}{2}_ContourMaster.dat", refBname, testBname, pos.Substring(1));
            string seriesPath = (posIndex == 0) ? w.NeutralSeriesPath : w.getSeriesPath(posIndex - 1);
            string distanceFolder = Path.Combine(seriesPath, "Distances");
            if (!Directory.Exists(distanceFolder))
                Directory.CreateDirectory(distanceFolder);
            return Path.Combine(distanceFolder, fname);
        }

        private static string CreateContourResultsLine(Contour contour)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < contour.ContourDistances.Length; i++)
            {
                builder.AppendFormat("{0},{1},{2},{3},{4}", contour.ContourDistances[i], contour.Areas[i], contour.Centroids[i][0], contour.Centroids[i][1], contour.Centroids[i][2]);
                builder.AppendLine();
            }
            return builder.ToString();
        }

        private static void appendToFile(string fname, string line)
        {
            using (StreamWriter writer = new StreamWriter(fname, true))
            {
                writer.Write(line);
            }
        }

        private static void writeToFile(string fname, string line)
        {
            using (StreamWriter writer = new StreamWriter(fname, false))
            {
                writer.Write(line);
            }
        }

        private static void ProcessSingleThread(FullWrist fullWrist, int refBoneIndex, int testBoneIndex, int[] posList, double[] cDistances)
        {
            Bone refBone = fullWrist.Bones[refBoneIndex];
            Bone testBone = fullWrist.Bones[testBoneIndex];
            for (int i = 0; i < posList.Length; i++)
            {
                int pos = posList[i];
                refBone.CalculateAndSaveDistanceMapForPosition(pos, new Bone[] { testBone });
                refBone.CalculateAndSaveContourForPosition(pos, cDistances, TestGetColors(cDistances.Length));
            }
        }

        private static void ProcessMultiThreaded(FullWrist fullWrist, int refBoneIndex, int testBoneIndex, int[] posList, double[] cDistances)
        {
            Queue<Queue<BulkCalculator.DistanceCalculationJob>> q;
            q = BuildMasterQueue(fullWrist, refBoneIndex, testBoneIndex, posList, cDistances);
            //go compute this!
            BulkCalculator calculator = new BulkCalculator();
            calculator.ProcessMasterQueue(q);
        }

        private static Queue<Queue<BulkCalculator.DistanceCalculationJob>> BuildMasterQueue(FullWrist fullWrist, int refBoneIndex, int testBoneIndex, int[] posList, double[] cDistances)
        {
            Queue<Queue<BulkCalculator.DistanceCalculationJob>> masterQueue = new Queue<Queue<BulkCalculator.DistanceCalculationJob>>(2);
            Queue<BulkCalculator.DistanceCalculationJob> q = new Queue<BulkCalculator.DistanceCalculationJob>();
            for (int i = 0; i < posList.Length; i++)
            {
                BulkCalculator.DistanceCalculationJob job = new BulkCalculator.DistanceCalculationJob();
                job.JobType = BulkCalculator.DistanceCalculationType.VetrexDistances;
                job.FullWrist = fullWrist;
                job.PrimaryBone = fullWrist.Bones[refBoneIndex];
                job.IneractionBones = new Bone[] { fullWrist.Bones[testBoneIndex] };
                job.PositionIndex = posList[i];
                job.AnimationOrder = null;
                q.Enqueue(job);
            }
            masterQueue.Enqueue(q);

            q = new Queue<BulkCalculator.DistanceCalculationJob>();
            for (int i = 0; i < posList.Length; i++)
            {
                BulkCalculator.DistanceCalculationJob job = new BulkCalculator.DistanceCalculationJob();
                job.JobType = BulkCalculator.DistanceCalculationType.Contours;
                job.FullWrist = fullWrist;
                job.PrimaryBone = fullWrist.Bones[refBoneIndex];
                job.ContourDistances = cDistances;
                job.ContourColors = TestGetColors(cDistances.Length);
                job.PositionIndex = posList[i];
                job.AnimationOrder = null;
                q.Enqueue(job);
            }
            masterQueue.Enqueue(q);

            return masterQueue;
        }

        private static System.Drawing.Color[] TestGetColors(int num)
        {
            System.Drawing.Color[] c = new System.Drawing.Color[num];
            for (int i = 0; i < num; i++)
                c[i] = System.Drawing.Color.White;
            return c;
        }

        private static int[] GetPositionIndexes(Wrist wrist, string[] positionNames)
        {
            int[] indices = new int[positionNames.Length];
            for (int i = 0; i < positionNames.Length; i++)
                indices[i] = wrist.getSeriesIndexFromName(positionNames[i]);
            return indices;
        }

        private static void PreliminaryValidation(CommandLineOptions options)
        {
            //lots of checks
            if (options.Subject == null)
                throw new ArgumentNullException("Subject", "No subject path specified");
            if (options.SideString == null)
                throw new ArgumentNullException("Side", "No side specified");
            if (options.TestBone == null)
                throw new ArgumentNullException("TestBone", "No test bone specified");
            if (options.ReferenceBone == null)
                throw new ArgumentNullException("RefBone", "No reference bone specified");
            if (options.PositionList == null)
                throw new ArgumentNullException("poslist", "No position list specified");
            //if (options.FixedBone == null)
            //    throw new ArgumentNullException("fixedbone", "No fixed bone specified");

            if (!Directory.Exists(options.Subject))
                throw new ArgumentException("Subject is not a valid directory");
        }


        //private static void processContourData(string subject, string test_position, string output, int referenceBoneIndex, int testBoneIndex, double contourDistance)
        //{
        //    //ReferenceBone is the bone that we are generating the contour map on...

        //    //TODO: Need a try block in case of error....
        //    Wrist.Sides side = Wrist.getSideFromSeries(test_position);
        //    string radiusPath = Wrist.findRadius(subject, side);
        //    Wrist w = new Wrist(radiusPath);
            
        //    //first read in the reference bone
        //    ColoredBone refBone = new ColoredBone(w.bpaths[referenceBoneIndex]);

        //    //load the motion file
        //    int seriesIndex = w.getSeriesIndexFromName(test_position);
        //    TransformMatrix relativeMotion;
        //    if (seriesIndex == 0) //neutral
        //    {
        //        relativeMotion = new TransformMatrix(); //set to identity for neutral
        //    }
        //    else //non neutral
        //    {
        //        TransformMatrix[] transforms = DatParser.parseMotionFileToTransformMatrix(w.motionFiles[seriesIndex - 1]); //offset -1 to skip neutral
        //        //this is the transform that moves the reference bone into the distance field of the test bone
        //        relativeMotion = transforms[testBoneIndex].Inverse() * transforms[referenceBoneIndex];
        //    }

        //    //load in the distance field for the test bone
        //    CTmri testField = new CTmri(w.DistanceFieldPaths[testBoneIndex]);
        //    testField.loadImageData();

        //    //generate the distance field
        //    double[] distanceMap = DistanceMaps.createDistanceMap(testField, refBone, relativeMotion);

        //    //generate contour
        //    Contour contour = DistanceMaps.createContourSingleBoneSinglePosition(refBone, distanceMap, contourDistance);

        //    //quickly dump the output for testing
        //    Console.WriteLine("Contour {0}mm: Area={1}, Centroid=({2}, {3}, {4})",
        //        contourDistance, contour.Areas[0], contour.Centroids[0][0], contour.Centroids[0][1], contour.Centroids[0][2]);
        //}
    }
}
