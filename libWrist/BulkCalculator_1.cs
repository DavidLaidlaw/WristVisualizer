using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.ComponentModel;

namespace libWrist
{
    public class BulkCalculator
    {
        Queue<Queue<DistanceCalculationJob>> _masterQueue;
        private Thread[] _workerThreads;
        private BackgroundWorkerStatusForm _bgStatusForm;
        private BackgroundWorker _bgWorker;

        public enum DistanceCalculationType
        {
            VetrexDistances,
            ColorMap,
            Contours
        }

        public struct DistanceCalculationJob
        {
            public FullJoint FullJoint;
            public DistanceCalculationType JobType;
            public Bone PrimaryBone;
            public Bone[] IneractionBones;
            public double ColorMapMaxDistance;
            public double[] ContourDistances;
            public System.Drawing.Color[] ContourColors;
            public int PositionIndex;

            //special information needed for interpolated data
            public int[] AnimationOrder;
            public int AnimationNumFramesPerStep;
            public Bone FixedBone;
        }

        public void ProcessMasterQueue(Queue<Queue<DistanceCalculationJob>> masterQueue)
        {
            if (masterQueue == null) return;
            _masterQueue = masterQueue;
            int totalNumberJobs = 0;
            Queue<Queue<DistanceCalculationJob>>.Enumerator e = masterQueue.GetEnumerator();
            while (e.MoveNext())
                totalNumberJobs += e.Current.Count;

            //check if there is actually work to do
            if (totalNumberJobs == 0)
                return;

            //setup gui
            _bgStatusForm = new BackgroundWorkerStatusForm(totalNumberJobs);

            //setup background worker
            _bgWorker = new BackgroundWorker();
            _bgWorker.WorkerReportsProgress = true;
            _bgWorker.WorkerSupportsCancellation = false;
            _bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_bgWorker_RunWorkerCompleted);
            _bgWorker.ProgressChanged += new ProgressChangedEventHandler(_bgWorker_ProgressChanged);
            _bgWorker.DoWork += new DoWorkEventHandler(_bgWorker_DoWork);

            //start background worker
            _bgWorker.RunWorkerAsync();
            _bgStatusForm.ShowDialog(); //show in dialog mode, we will close it elsewhere
        }

        void _bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //assume that we have at least one
            FullJoint fullJoint = _masterQueue.Peek().Peek().FullJoint;
            fullJoint.ReadInDistanceFields();

            //Loop through each queue to run
            Queue<Queue<DistanceCalculationJob>>.Enumerator enumerator = _masterQueue.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Queue<DistanceCalculationJob> currentQueue = enumerator.Current;

                int numThreads = Math.Min(System.Environment.ProcessorCount, currentQueue.Count);
                //numThreads = 1;

                //start all of the worker threads
                _workerThreads = new Thread[numThreads];
                for (int i = 0; i < numThreads; i++)
                {
                    _workerThreads[i] = new Thread(workThreadWork);
                    _workerThreads[i].Start(currentQueue);
                }
                //wait for worker threads to finish
                foreach (Thread curThread in _workerThreads)
                    curThread.Join();

            }
        }

        void _bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _bgStatusForm.UnSafeIncrimentCompletedParts();
        }

        void _bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //remove callbacks
            _bgWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(_bgWorker_RunWorkerCompleted);
            _bgWorker.ProgressChanged -= new ProgressChangedEventHandler(_bgWorker_ProgressChanged);
            _bgWorker.DoWork -= new DoWorkEventHandler(_bgWorker_DoWork);

            //close status window
            _bgStatusForm.Close();

            //cleanup
            _bgStatusForm = null;
            _bgWorker = null;
        }

        private void workThreadWork(object queue)
        {
            Queue<DistanceCalculationJob> workQueue = (Queue<DistanceCalculationJob>)queue;
            DistanceCalculationJob currentJob;
            bool done = false;
            while (!done)
            {
                //first get the job
                lock (workQueue)
                {
                    if (workQueue.Count > 0)
                        currentJob = (DistanceCalculationJob)workQueue.Dequeue();
                    else
                    {
                        done = true;
                        continue;
                    }
                }

                if (currentJob.AnimationOrder == null) //standard job
                {
                    switch (currentJob.JobType)
                    {
                        case DistanceCalculationType.VetrexDistances:
                            currentJob.PrimaryBone.CalculateAndSaveDistanceMapForPosition(currentJob.PositionIndex, currentJob.IneractionBones);
                            break;
                        case DistanceCalculationType.ColorMap:
                            currentJob.PrimaryBone.CalculateAndSaveColorDistanceMapForPosition(currentJob.PositionIndex, currentJob.ColorMapMaxDistance);
                            break;
                        case DistanceCalculationType.Contours:
                            currentJob.PrimaryBone.CalculateAndSaveContourForPosition(currentJob.PositionIndex, currentJob.ContourDistances, currentJob.ContourColors);
                            break;
                    }
                }
                else //animation job
                {
                    switch (currentJob.JobType)
                    {
                        case DistanceCalculationType.VetrexDistances:
                            currentJob.PrimaryBone.CalculateAndSaveDistanceMapForAnimation(currentJob.AnimationOrder, currentJob.AnimationNumFramesPerStep, 
                                currentJob.PositionIndex, currentJob.IneractionBones, currentJob.FixedBone);
                            break;
                        case DistanceCalculationType.ColorMap:
                            currentJob.PrimaryBone.CalculateAndSaveColorDistanceMapForAnimation(currentJob.PositionIndex, currentJob.ColorMapMaxDistance);
                            break;
                        case DistanceCalculationType.Contours:
                            currentJob.PrimaryBone.CalculateAndSaveContourForAnimation(currentJob.PositionIndex, currentJob.ContourDistances, currentJob.ContourColors);
                            break;
                    }
                }

                //done with the job, lets report in our progress
                _bgWorker.ReportProgress(1);
            }
        }
    }
}
