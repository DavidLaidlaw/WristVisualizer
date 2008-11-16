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
        private CommandLineOptions _options;
        private FullWrist _fullWrist;
        private Wrist _wrist;
        private Wrist.Sides _side;

        private int _refBoneIndex;
        private int _testBoneIndex;
        private int[] _positionList;

        private double[] _cDistances;

        public BatchMode(CommandLineOptions options)
        {
            //Lets setup the batch mode....
            try
            {
                _options = options;
                ReadInJobFromOption();
                LoadFilesAndCalculate();
                SaveDataCentroidArea();
            }
            catch (Exception ex)
            {
                libWrist.ExceptionHandling.HandledExceptionManager.ShowDialog("Error processing batch mode.\n"+ex.Message, "Batch mode did not succeed", "", ex);
                System.Environment.Exit(1);
            }
        }

        private void ReadInJobFromOption()
        {
            PreliminaryValidation(_options);
            _side = Wrist.GetSideFromString(_options.SideString);
            _wrist = new Wrist(Wrist.findRadius(_options.Subject, _side));
            _fullWrist = new FullWrist(_wrist);

            _refBoneIndex = _options.GetReferenceBoneIndex();
            _testBoneIndex = _options.GetTestBoneIndex();
            //int fixedBoneIndex = options.GetFixedBoneIndex();
            _positionList = GetPositionIndexes(_wrist, _options.GetPositionNames());
            _cDistances = _options.GetCoutourDistances();
        }

        private void LoadFilesAndCalculate()
        {
            _fullWrist.LoadSelectBonesAndDistancesForBatchMode(new int[] { _testBoneIndex }, new int[] { _refBoneIndex }); //only load selective bones..
            
            if (_options.MultiThread)
                ProcessMultiThreaded(_fullWrist, _refBoneIndex, _testBoneIndex, _positionList, _cDistances);
            else
                ProcessSingleThread(_fullWrist, _refBoneIndex, _testBoneIndex, _positionList, _cDistances);
        }

        private void SaveDataCentroidArea()
        {
            if (_options.SaveAreaDirectory == null) return; //nothing to do. no save specified
            for (int i = 0; i < _positionList.Length; i++)
            {
                Contour c = _fullWrist.Bones[_refBoneIndex].GetContourForPosition(_positionList[i]);
                string fname = CreateAreaOutputFilename(_positionList[i]);
                SaveContourAreaAndCentroidToFile(c, fname);
            }
        }

        private string CreateAreaOutputFilename(int posIndex)
        {
            string pos = (posIndex == 0) ? _wrist.neutralSeries : _wrist.series[posIndex - 1];
            string refBname = Wrist.ShortBoneNames[_refBoneIndex];
            string testBname = Wrist.ShortBoneNames[_testBoneIndex];
            string fname = String.Format("{0}_{1}_{2}{3}_ContourMaster.dat", _wrist.subject, refBname, testBname, pos.Substring(1));
            
            if (_options.SaveAreaDirectory.Trim().Length > 0) //if an output directory was specified, use that
                return Path.Combine(_options.SaveAreaDirectory, fname);
            
            //otherwise, specify our own default location
            string seriesPath = (posIndex == 0) ? _wrist.NeutralSeriesPath : _wrist.getSeriesPath(posIndex - 1);
            string distanceFolder = Path.Combine(seriesPath, "Distances");
            if (!Directory.Exists(distanceFolder))
                Directory.CreateDirectory(distanceFolder);
            return Path.Combine(distanceFolder, fname);
        }

        private static void SaveContourAreaAndCentroidToFile(Contour contour, string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename, false))
            {
                for (int i = 0; i < contour.ContourDistances.Length; i++)
                {
                    writer.WriteLine("{0},{1},{2},{3},{4}", contour.ContourDistances[i], contour.Areas[i], contour.Centroids[i][0], contour.Centroids[i][1], contour.Centroids[i][2]);
                }
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
                refBone.CalculateAndSaveContourForPosition(pos, cDistances, GetWhiteColors(cDistances.Length));
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
                job.ContourColors = GetWhiteColors(cDistances.Length);
                job.PositionIndex = posList[i];
                job.AnimationOrder = null;
                q.Enqueue(job);
            }
            masterQueue.Enqueue(q);

            return masterQueue;
        }

        private static System.Drawing.Color[] GetWhiteColors(int num)
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
    }
}
