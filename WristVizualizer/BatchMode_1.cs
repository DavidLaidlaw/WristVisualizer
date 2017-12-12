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
        private WristFilesystem _wrist;
        private WristFilesystem.Sides _side;

        private int _refBoneIndex;
        private int _testBoneIndex;
        private int[] _positionList;

        private double[] _cDistances;
        private double[] _cAreas;

        public BatchMode(CommandLineOptions options)
        {
            //Lets setup the batch mode....
            try
            {
                _options = options;
                ReadInJobFromOption();

                //load the wrist and files
                _fullWrist.LoadSelectBonesAndDistancesForBatchMode(new int[] { _testBoneIndex }, new int[] { _refBoneIndex }); //only load selective bones..

                //process parts
                if (_cDistances != null)
                    CalculateAndSaveContours();
                if (_cAreas != null)
                    CalcualteAndSaveContourByArea();
                
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
            _side = WristFilesystem.GetSideFromString(_options.SideString);
            _wrist = new WristFilesystem(WristFilesystem.findRadius(_options.Subject, _side));
            _fullWrist = new FullWrist(_wrist);

            _refBoneIndex = _options.GetReferenceBoneIndex();
            _testBoneIndex = _options.GetTestBoneIndex();
            //int fixedBoneIndex = options.GetFixedBoneIndex();
            _positionList = GetPositionIndexes(_wrist, _options.GetPositionNames());
            _cDistances = _options.GetCoutourDistances();
            _cAreas = _options.GetTargetContourAreas();
        }

        private void CalcualteAndSaveContourByArea()
        {
            if (_cAreas == null) return;
            if (_options.SaveAreaDirectory == null) return; //nothing to do if we don't save

            Contour[][] contours = ProcessTargetAreaSingleThread();
            for (int i = 0; i < _positionList.Length; i++)
            {
                if (_options.SaveAreaDirectory != null) //save area information if needed
                {
                    string fname = CreateTargetAreaOutputFilename(_positionList[i]);
                    SaveContourAreaAndCentroidToFile(contours[i], fname);
                }
                if (_options.SaveContourFileDirectory != null) //save contour stack file if needed
                {
                    SaveContourToStackFile(contours[i], _positionList[i]);
                }
            }
        }

        private void CalculateAndSaveContours()
        {
            if (_cDistances == null) return;

            //Perform the actuall calculations
            if (_options.MultiThread)
                ProcessContoursMultiThreaded(_fullWrist, _refBoneIndex, _testBoneIndex, _positionList);
            else
                ProcessContoursSingleThread(_fullWrist, _refBoneIndex, _testBoneIndex, _positionList);

            //save the output as needed
            for (int i = 0; i < _positionList.Length; i++)
            {
                Contour c = _fullWrist.Bones[_refBoneIndex].GetContourForPosition(_positionList[i]);
                if (_options.SaveAreaDirectory != null) //save area information if needed
                {
                    string fname = CreateAreaOutputFilename(_positionList[i]);
                    SaveContourAreaAndCentroidToFile(c, fname);
                }
                if (_options.SaveContourFileDirectory != null) //save contour stack file if needed
                {
                    SaveContourToStackFile(c, _positionList[i]);
                }
            }
        }

        private string CreateTargetAreaOutputFilename(int posIndex)
        {
            const string format = "%SUBJECT%_%REFBONE%_%TESTBONE%%POSITION%_ContourTargeted.dat";
            string fname = createFilenameHelper(format, posIndex);

            return Path.Combine(GetAreaOutputDirectory(posIndex), fname);
        }

        private string CreateAreaOutputFilename(int posIndex)
        {
            const string format = "%SUBJECT%_%REFBONE%_%TESTBONE%%POSITION%_ContourMaster.dat";
            string fname = createFilenameHelper(format, posIndex);

            return Path.Combine(GetAreaOutputDirectory(posIndex), fname);
        }

        private string CreateContourOutputFilename(int posIndex, double distance)
        {
            const string format = "%SUBJECT%_%REFBONE%_%TESTBONE%%POSITION%_%DISTANCE%Contour.stack";
            string fname = createFilenameHelper(format, posIndex, distance);

            return Path.Combine(GetAreaOutputDirectory(posIndex), fname);
        }

        private string CreateContourIVFilename(int posIndex, double distance)
        {
            const string format = "%SUBJECT%_%REFBONE%_%TESTBONE%%POSITION%_%DISTANCE%Contour.iv";
            string fname = createFilenameHelper(format, posIndex, distance);

            return Path.Combine(GetAreaOutputDirectory(posIndex), fname);
        }

        private string createFilenameHelper(string formatString, int posIndex)
        {
            return createFilenameHelper(formatString, posIndex, 0);
        }
        private string createFilenameHelper(string formatString, int posIndex, double distance)
        {
            string pos = (posIndex == 0) ? _wrist.neutralSeries : _wrist.series[posIndex - 1];
            string refBname = WristFilesystem.ShortBoneNames[_refBoneIndex];
            string testBname = WristFilesystem.ShortBoneNames[_testBoneIndex];

            string output = formatString;
            output = output.Replace("%SUBJECT%", _wrist.subject);
            output = output.Replace("%REFBONE%", refBname);
            output = output.Replace("%TESTBONE%", testBname);
            output = output.Replace("%POSITION%", pos.Substring(1));
            output = output.Replace("%DISTANCE%", distance.ToString("0.000"));
            return output;
        }

        private string GetAreaOutputDirectory(int posIndex)
        {
            if (_options.SaveAreaDirectory.Trim().Length > 0) //if an output directory was specified, use that
                return _options.SaveAreaDirectory;

            //otherwise, specify our own default location
            string seriesPath = (posIndex == 0) ? _wrist.NeutralSeriesPath : _wrist.getSeriesPath(posIndex - 1);
            string distanceFolder = Path.Combine(seriesPath, "Distances");
            if (!Directory.Exists(distanceFolder))
                Directory.CreateDirectory(distanceFolder);
            return distanceFolder;
        }

        private string GetContourOutputDirectory(int posIndex)
        {
            if (_options.SaveContourFileDirectory.Trim().Length > 0) //if an output directory was specified, use that
                return _options.SaveContourFileDirectory;

            //otherwise, specify our own default location
            string seriesPath = (posIndex == 0) ? _wrist.NeutralSeriesPath : _wrist.getSeriesPath(posIndex - 1);
            string distanceFolder = Path.Combine(seriesPath, "Distances");
            if (!Directory.Exists(distanceFolder))
                Directory.CreateDirectory(distanceFolder);
            return distanceFolder;
        }        

        private static void SaveContourAreaAndCentroidToFile(Contour contour, string filename)
        {
            SaveContourAreaAndCentroidToFile(new Contour[] { contour }, filename);
        }
        private static void SaveContourAreaAndCentroidToFile(Contour[] contours, string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename, false))
            {
                foreach (Contour contour in contours)
                {
                    for (int i = 0; i < contour.ContourDistances.Length; i++)
                    {
                        writer.WriteLine("{0},{1},{2},{3},{4}", contour.ContourDistances[i], contour.Areas[i], contour.Centroids[i][0], contour.Centroids[i][1], contour.Centroids[i][2]);
                    }
                }
            }
        }


        private void SaveContourToStackFile(Contour[] contours, int posIndex)
        {
            foreach (Contour c in contours)
                SaveContourToStackFile(c, posIndex);
        }
        private void SaveContourToStackFile(Contour contour, int posIndex)
        {
            //loop for each contour
            for (int i = 0; i < contour.ContourDistances.Length; i++)
            {
                //Get contour distance
                double distance = contour.ContourDistances[i];
                //Get filename
                string filename = CreateContourIVFilename(posIndex, distance);
                //save to contour....
                string ivText = contour.getContourNodeGraph(i);
                using (StreamWriter writer = new StreamWriter(filename, false))
                {
                    writer.Write(ivText);
                }
            }
        }
        


        private Contour[][] ProcessTargetAreaSingleThread()
        {
            Bone refBone = _fullWrist.Bones[_refBoneIndex];
            Bone testBone = _fullWrist.Bones[_testBoneIndex];
            Contour[][] allContours = new Contour[_positionList.Length][];
            for (int i = 0; i < _positionList.Length; i++)
            {
                int pos = _positionList[i];
                refBone.CalculateAndSaveDistanceMapForPosition(pos, new Bone[] { testBone });
                allContours[i] = refBone.CalculateContourForPositionTargetingAreas(pos, _cAreas, _options.Tolerance, _options.IterationLimit);
            }
            return allContours;
        }

        private void ProcessContoursSingleThread(FullWrist fullWrist, int refBoneIndex, int testBoneIndex, int[] posList)
        {
            Bone refBone = fullWrist.Bones[refBoneIndex];
            Bone testBone = fullWrist.Bones[testBoneIndex];
            for (int i = 0; i < posList.Length; i++)
            {
                int pos = posList[i];
                refBone.CalculateAndSaveDistanceMapForPosition(pos, new Bone[] { testBone });
                refBone.CalculateAndSaveContourForPosition(pos, _cDistances, GetWhiteColors(_cDistances.Length));
            }
        }

        private void ProcessContoursMultiThreaded(FullWrist fullWrist, int refBoneIndex, int testBoneIndex, int[] posList)
        {
            Queue<Queue<BulkCalculator.DistanceCalculationJob>> q;
            q = BuildMasterQueue(fullWrist, refBoneIndex, testBoneIndex, posList, _cDistances);
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
                job.FullJoint = fullWrist;
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
                job.FullJoint = fullWrist;
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

        private static int[] GetPositionIndexes(WristFilesystem wrist, string[] positionNames)
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
