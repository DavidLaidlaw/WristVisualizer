using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.IO;
using System.Threading;
using libCoin3D;


namespace libWrist
{
    public class DistanceMaps
    {
        Queue<Queue<DistanceMaps.DistanceCalculationJob>> _masterQueue;
        private Thread[] _workerThreads;
        private BackgroundWorkerStatusForm _bgStatusForm;
        private BackgroundWorker _bgWorker;

        public DistanceMaps()
        {           
            
        }

        /// <summary>
        /// Check an array of Bone objects and return an array of indices to the primary array 
        /// indicating which are valid bones with valid distance fields
        /// </summary>
        /// <param name="testBones"></param>
        /// <returns></returns>
        public static int[] GetIndexesOfValidBones(Bone[] testBones)
        {
            List<int> indices = new List<int>(testBones.Length);
            for (int i = 0; i < testBones.Length; i++)
            {
                if (testBones[i].IsValidBone && testBones[i].HasDistanceField)
                    indices.Add(i);
            }
            return indices.ToArray();
        }

        /// <summary>
        /// Calculates the distanceMaps for a given bone using the precalculated distance fields.
        /// If a distance field is unavailable, it is assumed to be infinately far away...
        /// </summary>
        /// <param name="boneIndex"></param>
        /// <param name="positionIndex"></param>
        /// <returns></returns>
        public static double[] createDistanceMap(Bone referenceBone, Bone[] testBones, TransformMatrix[] tmRelMotions)
        {
            if (testBones.Length != tmRelMotions.Length)
                throw new ArgumentException("Must pass the same number of transforms as test bones");

            float[,] pts = referenceBone.GetVertices();
            int numVertices = pts.GetLength(0);

            double[] distances = new double[numVertices];
            bool isNeutralPosition = (tmRelMotions[0] == null); //if we have no transforms, then its the neutral position
            int[] validTestBoneIndices = GetIndexesOfValidBones(testBones);

            //for each vertex           
            for (int i = 0; i < numVertices; i++)
            {
                distances[i] = Double.MaxValue; //set this vertex to the default
                foreach(int j in validTestBoneIndices) //only use the bones that we have specified interactions for and are valid
                {
                    double x = pts[i, 0];
                    double y = pts[i, 1];
                    double z = pts[i, 2];

                    //check if we need to move for non neutral position
                    if (!isNeutralPosition)
                    {
                        //lets move the bone getting colored, into the space of the other bone...
                        double[] p0 = new double[] { x, y, z };
                        double[] p1 = tmRelMotions[j] * p0;
                        x = p1[0];
                        y = p1[1];
                        z = p1[2];
                    }

                    double dX = (x - testBones[j].DistanceField.CoordinateOffset[0]) / testBones[j].DistanceField.voxelSizeX;
                    double dY = (y - testBones[j].DistanceField.CoordinateOffset[1]) / testBones[j].DistanceField.voxelSizeY;
                    double dZ = (z - testBones[j].DistanceField.CoordinateOffset[2]) / testBones[j].DistanceField.voxelSizeZ;

                    const double xBound = 96.9; //get the boundaries of the distance cube
                    const double yBound = 96.9; //
                    const double zBound = 96.9; //

                    ////////////////////////////////////////////////////////
                    //is surface point picked inside of the cube?

                    if (dX >= 3.1 && dX <= xBound && dY >= 3.1
                        && dY <= yBound && dZ >= 3.1 && dZ <= zBound)
                    {
                        double localDist = testBones[j].DistanceField.sample_s_InterpCubit(dX, dY, dZ);
                        if (localDist < distances[i]) //check if this is smaller, if so save it
                            distances[i] = localDist;
                    }
                }
            }
            return distances;
        }

        /// <summary>
        /// Can be used to create the distance map for a single reference bone, against a single 
        /// test distance field.
        /// </summary>
        /// <param name="testDistanceField">Distance field of the test bone.</param>
        /// <param name="referenceBone">The bone file of the reference bone. The distance map should be drawn on this bone</param>
        /// <param name="relativeMotion">The relative transform of to move the reference bone into the distance field for the test bone</param>
        /// <returns>an array of distance values (in mm) for each vertex of the referenceBone</returns>
        public static double[] createDistanceMap(CTmri testDistanceField, ColoredBone referenceBone, TransformMatrix relativeMotion)
        {
            /* Yes this is bad, its basically just cut and paste code from above, 
             * but I need these both to be pretty optimized, and it was hard to remove
             * the loops from above, or I would have really had to force this
             */

            CTmri mri = testDistanceField;
            float[,] pts = referenceBone.getVertices();
            int numVertices = pts.GetLength(0);

            double[] distances = new double[numVertices];

            bool isNeutral = relativeMotion.isIdentity();

            //for each vertex           
            for (int i = 0; i < numVertices; i++)
            {
                distances[i] = Double.MaxValue; //set this vertex to the default

                double x = pts[i, 0];
                double y = pts[i, 1];
                double z = pts[i, 2];

                //check if we need to move for non neutral position
                if (!isNeutral)
                {
                    //lets move the bone getting colored, into the space of the other bone...
                    double[] p0 = new double[] { x, y, z };
                    double[] p1 = relativeMotion * p0;
                    x = p1[0];
                    y = p1[1];
                    z = p1[2];
                }

                double dX = (x - mri.CoordinateOffset[0]) / mri.voxelSizeX;
                double dY = (y - mri.CoordinateOffset[1]) / mri.voxelSizeY;
                double dZ = (z - mri.CoordinateOffset[2]) / mri.voxelSizeZ;

                const double xBound = 96.9; //get the boundaries of the distance cube
                const double yBound = 96.9; //
                const double zBound = 96.9; //

                ////////////////////////////////////////////////////////
                //is surface point picked inside of the cube?

                if (dX >= 3.1 && dX <= xBound && dY >= 3.1
                    && dY <= yBound && dZ >= 3.1 && dZ <= zBound)
                {
                    distances[i] = mri.sample_s_InterpCubit(dX, dY, dZ);
                }
            }
            return distances;
        }

        public static int[] createColormap(double[] distances, double maxColorDistance)
        {
            int numVertices = distances.Length;
            int[] colors = new int[numVertices];

            for (int i = 0; i < numVertices; i++)
            {
                UInt32 packedColor;

                //first check if there is collission
                if (distances[i] < 0)
                {
                    //color collision in blue
                    packedColor = 0X0000FFFF;
                }
                // a parameter could be used instead of plain 3
                else if (distances[i] > maxColorDistance)  //check if we are too far away
                {
                    //make us white
                    packedColor = 0xFFFFFFFF;
                }
                else
                {
                    /* convert to packed RGB color....how?
                     * packed color for Coin3D/inventor is 0xRRGGBBAA
                     * So take our GB values (should be from 0-255 or 8 bits), and move from
                     * Lest significant position (0x000000XX) to the G and B position, then
                     * combine with a bitwise OR. (0x00XX0000 | 0x0000XX00), which gives us
                     * the calculated value in both the G & B slots, and 0x00 in R & A.
                     * So we then ahve 0x00GGBB00, we can then bitwise OR with 0xFF0000FF, 
                     * since we want both R and Alpha to be at 255. Then we are set :)
                     */
                    uint GB = (uint)(distances[i] * 255.0 / maxColorDistance);
                    packedColor = (GB << 16) | (GB << 8) | 0xFF0000FF;
                }
                colors[i] = (int)packedColor;
            }
            return colors;
        }

        private Separator createContourCentroidSphere(Contour contour, int contourIndex)
        {
            Sphere s = new Sphere(0.3f);
            Separator spher1 = new Separator();
            Material m = new Material();
            Transform t = new Transform();

            spher1.addNode(t);
            spher1.addNode(m);
            spher1.addNode(s);
            t.setTranslation(contour.Centroids[contourIndex][0], contour.Centroids[contourIndex][1], contour.Centroids[contourIndex][2]);
            m.setColor(0, 1, 0);
            return spher1;
        }

        //[Obsolete("Don't f'ing use!")]
        //public void createContourShit()
        //{
        //    double[] cDistances = new double[] { 1.0, 2.0 };
        //    for (int i = 0; i < Wrist.NumBones; i++)
        //    {
        //        Contour cont1 = createContourSingleBoneSinglePosition(i, 0, cDistances);
        //        _colorBones[i].setAndReplaceContour(cont1);
        //        for (int j=0; j<cDistances.Length; j++)
        //        {
        //            double d = cDistances[j];
        //            Console.WriteLine("Bone {0}, contour {1}mm: Area={2}, Centroid=({3}, {4}, {5})",
        //                i, d, cont1.Areas[j], cont1.Centroids[j][0], cont1.Centroids[j][1], cont1.Centroids[j][2]);
        //        }
        //    }
        //}


        /// <summary>
        /// Will create a contour based on a distance field for the given reference bone.
        /// </summary>
        /// <param name="referenceBone">Bone on which to draw the contour</param>
        /// <param name="distanceMap">Pre-calculated distance map for the referenceBone. Distance for each vertex in order</param>
        /// <param name="cDistance">Contour Distance (mm)</param>
        /// <returns></returns>
        public static Contour createContourSingleBoneSinglePosition(ColoredBone referenceBone, double[] distanceMap, double cDistance)
        {
            /* Yes, its bad to repeat code, but I needed this pretty fast.
             */
            double[] dist = distanceMap;

            float[,] points = referenceBone.getVertices();
            int[,] conn = referenceBone.getFaceSetIndices();

            Contour cont1 = new Contour(1); //only a single distance for this contour
            //cont1.Color = colors[0];

            double[] cDistances = new double[] { cDistance }; //create new array with single value, needed for helper function later

            int numTrian = conn.GetLength(0);
            for (int i = 0; i < numTrian; i++)
            {
                //check if all the points are out, if so, skip it
                if (dist[conn[i, 0]] > cDistance &&
                    dist[conn[i, 1]] > cDistance &&
                    dist[conn[i, 2]] > cDistance)
                    continue;

                double[] triDist = { dist[conn[i, 0]], dist[conn[i, 1]], dist[conn[i, 2]] };
                float[][] triPts = { 
                    new float[] {points[conn[i, 0], 0], points[conn[i, 0], 1], points[conn[i, 0], 2]},
                    new float[] {points[conn[i, 1], 0], points[conn[i, 1], 1], points[conn[i, 1], 2]},
                    new float[] {points[conn[i, 2], 0], points[conn[i, 2], 1], points[conn[i, 2], 2]}
                };

                contourSingleTriangle(triDist, triPts, cont1, cDistances);
            }
            return cont1;
        }

        public static Contour createContourSingleBoneSinglePosition(Bone referenceBone, double[] distanceMap, double[] cDistances, System.Drawing.Color[] colors)
        {
            float[,] points = referenceBone.GetVertices();
            int[,] conn = referenceBone.GetFaceSetIndices();
            return DistanceMaps.createContourSingleBoneSinglePosition(points, conn, distanceMap, cDistances, colors);
        }

        private static Contour createContourSingleBoneSinglePosition(float[,] points, int[,] conn, double[] distanceMap, double[] cDistances, System.Drawing.Color[] colors)
        {
            if (cDistances.Length != colors.Length)
                throw new ArgumentException("Number of colors must be the same as the number of contours being created");

            double[] dist = distanceMap;

            Contour cont1 = new Contour(colors);

            double maxContourDistance = -1;
            foreach (double cDist in cDistances)
                maxContourDistance = Math.Max(maxContourDistance, cDist);

            int numTrian = conn.GetLength(0);
            for (int i = 0; i < numTrian; i++)
            {
                //check if all the points are out, if so, skip it
                if (dist[conn[i, 0]] > maxContourDistance &&
                    dist[conn[i, 1]] > maxContourDistance &&
                    dist[conn[i, 2]] > maxContourDistance)
                    continue;

                double[] triDist = { dist[conn[i, 0]], dist[conn[i, 1]], dist[conn[i, 2]] };
                float[][] triPts = { 
                    new float[] {points[conn[i, 0], 0], points[conn[i, 0], 1], points[conn[i, 0], 2]},
                    new float[] {points[conn[i, 1], 0], points[conn[i, 1], 1], points[conn[i, 1], 2]},
                    new float[] {points[conn[i, 2], 0], points[conn[i, 2], 1], points[conn[i, 2], 2]}
                };

                contourSingleTriangle(triDist, triPts, cont1, cDistances);
            }
            return cont1;
        }

        #region Contour Helper Functions
        private static double distanceBetweenPoints(float[] p0, float[] p1)
        {
            double x = p0[0] - p1[0];
            double y = p0[1] - p1[1];
            double z = p0[2] - p1[2];
            return Math.Sqrt(x * x + y * y + z * z);
        }

        private static double calculateTriangleArea(float[][] vertices)
        {
            return calculateTriangleArea(vertices[0], vertices[1], vertices[2]);
        }
        private static double calculateTriangleArea(float[] vertex0, float[] vertex1, float[] vertex2)
        {
            double a = distanceBetweenPoints(vertex0, vertex1);
            double b = distanceBetweenPoints(vertex1, vertex2);
            double c = distanceBetweenPoints(vertex2, vertex0);
            double s = 0.5 * (a + b + c);
            double s1 = s - a;
            double s2 = s - b;
            double s3 = s - c;
            return Math.Sqrt(s * s1 * s2 * s3);
        }

        private static double[] calculateCentroid(float[][] vertices)
        {
            double x = 0, y = 0, z = 0;
            for (int i = 0; i < vertices.Length; i++)
            {
                x += vertices[i][0];
                y += vertices[i][1];
                z += vertices[i][2];
            }
            x = x / vertices.Length;
            y = y / vertices.Length;
            z = z / vertices.Length;
            return new double[] { x, y, z };
        }

        private static void contourSingleTriangle(double[] dist, float[][] vertices, Contour contour, double[] cDistances)
        {
            double area;
            double[] centroid;
            for (int i = 0; i < cDistances.Length; i++)
            {
                double cDist = cDistances[i];
                int[] inside = { 0, 0, 0 };
                int[] outside = { 0, 0, 0 };
                int numInside = 0;
                int numOutside = 0;

                //check if each point is inside or ouside
                for (int j = 0; j < 3; j++)
                {
                    if (dist[j] < cDist) //is inside
                        inside[numInside++] = j; //add to array and incriment
                    else
                        outside[numOutside++] = j;
                }

                //skip triangles totally outside
                if (numOutside == 3)
                    continue;

                //check triangles totally inside
                if (numInside == 3)
                {
                    area = calculateTriangleArea(vertices);
                    contour.Areas[i] += area;
                    centroid = calculateCentroid(vertices);
                    contour.CentroidSums[i][0] += area * centroid[0];
                    contour.CentroidSums[i][1] += area * centroid[1];
                    contour.CentroidSums[i][2] += area * centroid[2];

                    continue;
                }

                
                float[][] areaVertices;
                float[] newPt1, newPt2;
                //so I now have one or two vertices inside, and one or two ouside.....what to do
                if (numInside == 1)
                {
                    newPt1 = createGradientPoint(dist[inside[0]], vertices[inside[0]], dist[outside[0]], vertices[outside[0]], cDist);
                    newPt2 = createGradientPoint(dist[inside[0]], vertices[inside[0]], dist[outside[1]], vertices[outside[1]], cDist);
                    area = calculateTriangleArea(newPt1, newPt2, vertices[inside[0]]);
                    areaVertices = new float[][] { newPt1, newPt2, vertices[inside[0]] };
                }
                else //I have 2 inside....yay
                {
                    newPt1 = createGradientPoint(dist[outside[0]], vertices[outside[0]], dist[inside[0]], vertices[inside[0]], cDist);
                    newPt2 = createGradientPoint(dist[outside[0]], vertices[outside[0]], dist[inside[1]], vertices[inside[1]], cDist);
                    area = calculateTriangleArea(newPt1, vertices[inside[1]], vertices[inside[0]]);
                    area += calculateTriangleArea(newPt1, vertices[inside[1]], newPt2);
                    areaVertices = new float[][] { newPt1, newPt2, vertices[inside[0]], vertices[inside[1]] };
                }
                centroid = calculateCentroid(areaVertices);
                contour.Areas[i] += area;                
                contour.CentroidSums[i][0] += area * centroid[0];
                contour.CentroidSums[i][1] += area * centroid[1];
                contour.CentroidSums[i][2] += area * centroid[2];

                contour.addLineSegment(i, newPt1[0], newPt1[1], newPt1[2], newPt2[0], newPt2[1], newPt2[2]);
            }
        }

        private static float[] createGradientPoint(double d0, float[] v0, double d1, float[] v1, double cDist)
        {
            float[] midpoint = new float[3];

            double ratio = (d0 - cDist) / (d0 - d1); //fraction of contribution for v1 (its how far away we are)
            System.Diagnostics.Debug.Assert(ratio > 0); //quick check :)
            midpoint[0] = (float)((1 - ratio) * v0[0] + ratio * v1[0]);
            midpoint[1] = (float)((1 - ratio) * v0[1] + ratio * v1[1]);
            midpoint[2] = (float)((1 - ratio) * v0[2] + ratio * v1[2]);
            return midpoint;
        }
        #endregion

        #region Multi-Threaded Processing
        public enum DistanceCalculationType
        {
            VetrexDistances,
            ColorMap,
            Contours            
        }

        public struct DistanceCalculationJob
        {
            public FullWrist FullWrist;
            public DistanceCalculationType JobType;
            public Bone PrimaryBone;
            public Bone[] IneractionBones;
            public double ColorMapMaxDistance;
            public double[] ContourDistances;
            public System.Drawing.Color[] ContourColors;
            public int PositionIndex;
        }

        public void ProcessMasterQueue(Queue<Queue<DistanceMaps.DistanceCalculationJob>> masterQueue)
        {
            if (masterQueue == null) return;
            _masterQueue = masterQueue;
            int totalNumberJobs = 0;
            Queue<Queue<DistanceMaps.DistanceCalculationJob>>.Enumerator e = masterQueue.GetEnumerator();
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
            FullWrist fullWrist = _masterQueue.Peek().Peek().FullWrist;
            fullWrist.ReadInDistanceFields();

            //Loop through each queue to run
            Queue<Queue<DistanceMaps.DistanceCalculationJob>>.Enumerator enumerator = _masterQueue.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Queue<DistanceMaps.DistanceCalculationJob> currentQueue = enumerator.Current;

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
            Queue<DistanceMaps.DistanceCalculationJob> workQueue = (Queue<DistanceMaps.DistanceCalculationJob>)queue;
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
                    default:
                        break;  
                }

                //done with the job, lets report in our progress
                _bgWorker.ReportProgress(1);
            }
        }
        #endregion
    }
}
