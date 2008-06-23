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
        private TransformMatrix[][] _transformMatrices;
        private ColoredBone[] _colorBones;
        private Wrist _wrist;

        private CTmri[] _distanceFields;
        private int[][][] _calculatedColorMaps;  //stores the packed colors in 32bit INT values.
        private double[][][] _calculatedDistances;
        private Contour[][] _calculatedContours;

        private Queue _workQueueColorMaps;
        private Queue _workQueueContours;
        private Thread[] _workerThreads;
        private BackgroundWorkerStatusForm _bgStatusForm;
        private BackgroundWorker _bgWorker;

        private double[] _contourDistances;
        private System.Drawing.Color[] _contourColors;
        private double _maxColoredDistance;

        public DistanceMaps(Wrist wrist, TransformMatrix[][] transformMatrices, ColoredBone[] colorBones)
        {
            _wrist = wrist;
            _transformMatrices = transformMatrices;
            _colorBones = colorBones;

            //initialize thread-safe queues
            _workQueueColorMaps = Queue.Synchronized(new Queue());
            _workQueueContours = Queue.Synchronized(new Queue());
        }

        /// <summary>
        /// Will set the maximum distance to use for applying the color of Distance Maps.
        /// If this is different then previous, it will clear ALL cached color maps to maintain
        /// consistancy.
        /// </summary>
        /// <param name="maxDistance">maximum distance to color</param>
        public void setMaxColoredDistance(double maxDistance)
        {
            if (_maxColoredDistance == maxDistance) return; //no change

            if (_maxColoredDistance > 0) //check for existing colored distance
            {
                //if so, we need to delete the cache and clear all the contours from bones
                _calculatedContours = null;
                showDistanceColorMapsForPositionIfCalculatedOrClear(0); //could be any index, they are all empty
            }

            //now save the result
            _maxColoredDistance = maxDistance;
        }

        /// <summary>
        /// Will set the number of contours to display, and what distances to use for each contour.
        /// If this is different then previous, it will clear ALL cached contours to maintain
        /// consistancy.
        /// </summary>
        /// <param name="cDistances">Array of contour distances, one value per contour</param>
        /// <param name="colors">Array of colors for the contours, one per contour</param>
        public void setContourDistances(double[] cDistances, System.Drawing.Color[] colors)
        {
            //if it was not set, then set it and get out
            if (_contourDistances == null)
            {
                _contourDistances = cDistances;
                _contourColors = colors;
                return;
            }

            //lets check if it changed
            bool changed = false;
            if (_contourDistances.Length == cDistances.Length)
            {
                for (int i = 0; i < _contourDistances.Length; i++)
                {
                    if (_contourDistances[i] != cDistances[i])
                        changed = true;
                    if (_contourColors[i] != colors[i])
                        changed = true;
                }
            }
            else 
                changed = true;

            //if nothing is different, then we are fine
            if (!changed) return;

            //if here, then something changed, and we need to clear the cache and save the new values
            _calculatedContours = null;
            showContoursForPositionIfCalculatedOrClear(0); //could be any index, they are all empty

            _contourDistances = cDistances; //save new values
            _contourColors = colors;
        }

        public double[] ContourDistances
        {
            get { return _contourDistances; }
        }

        public double MaxColoredDistance
        {
            get { return _maxColoredDistance; }
        }

        public System.Drawing.Color[] ContourColors
        {
            get { return _contourColors; }
        }

        private void readInDistanceFieldsIfNotLoaded()
        {
            if (_distanceFields != null)
                return;

            _distanceFields = new CTmri[Wrist.NumBones];

            for (int i = 0; i < Wrist.NumBones; i++)
            {
                if (Directory.Exists(_wrist.DistanceFieldPaths[i]))
                {
                    _distanceFields[i] = new CTmri(_wrist.DistanceFieldPaths[i]);
                    _distanceFields[i].loadImageData();
                }
                else
                    _distanceFields[i] = null;
            }
        }

        public bool hasContourForBonePosition(int boneIndex, int positionIndex)
        {
            if (_calculatedContours == null)
                return false;

            if (_calculatedContours[boneIndex] == null)
                return false;

            return (_calculatedContours[boneIndex][positionIndex] != null);
        }

        private bool hasDistanceMapsForBonePosition(int boneIndex, int positionIndex)
        {
            if (_calculatedDistances == null)
                return false;

            if (_calculatedDistances[boneIndex] == null)
                return false;

            return (_calculatedDistances[boneIndex][positionIndex] != null);
        }

        public bool hasDistanceColorMapsForPosition(int positionIndex)
        {
            if (_calculatedColorMaps == null)
                return false;

            //only check the radius, it should be a good enough check....
            if (_calculatedColorMaps[0] == null)
                return false;

            return (_calculatedColorMaps[0][positionIndex] != null);
        }

        public void showContoursForPositionIfCalculatedOrClear(int positionIndex)
        {
            for (int i = 0; i < Wrist.NumBones; i++)
            {
                if (_colorBones[i] == null) continue;
                if (hasContourForBonePosition(i, positionIndex))
                    _colorBones[i].setAndReplaceContour(_calculatedContours[i][positionIndex]);
                else
                    _colorBones[i].removeContour();
            }
        }

        public void showDistanceColorMapsForPositionIfCalculatedOrClear(int positionIndex)
        {
            if (hasDistanceColorMapsForPosition(positionIndex))
                showDistanceColorMapsForPosition(positionIndex);
            else
                clearDistanceColorMapsForAllBones();
        }

        public void readInAllDistanceColorMaps()
        {
            //setup save space if it doesn't exist
            if (_calculatedColorMaps == null)
                _calculatedColorMaps = new int[Wrist.NumBones][][];

            readInDistanceFieldsIfNotLoaded();
            int numPos = _transformMatrices.Length + 1; //add one extra for neutral :)

            //try and create color scheme....
            for (int i = 0; i < Wrist.NumBones; i++)
            {
                //setup space if it doesn't exist
                if (_calculatedColorMaps[i] == null)
                    _calculatedColorMaps[i] = new int[numPos][]; 

                //now read the color map for each position index
                for (int j = 0; j < numPos; j++)
                {
                    //read in the colors if not yet loaded
                    if (_calculatedColorMaps[i][j] == null)
                        _calculatedColorMaps[i][j] = createColormap(i, j);
                }
            }
        }

        public void showDistanceColorMapsForPosition(int positionIndex)
        {
            //setup save space if it doesn't exist
            if (_calculatedColorMaps == null)
                _calculatedColorMaps = new int[Wrist.NumBones][][];

            readInDistanceFieldsIfNotLoaded();

            //try and create color scheme....
            for (int i = 0; i < Wrist.NumBones; i++)
            {
                //setup space if it doesn't exist
                if (_calculatedColorMaps[i] == null)
                    _calculatedColorMaps[i] = new int[_transformMatrices.Length + 1][]; //add one extra for neutral :)

                //read in the colors if not yet loaded
                if (_calculatedColorMaps[i][positionIndex] == null)
                    _calculatedColorMaps[i][positionIndex] = createColormap(i, positionIndex);

                //now set that color
                _colorBones[i].setColorMap(_calculatedColorMaps[i][positionIndex]);
            }
        }

        public void clearDistanceColorMapsForAllBones()
        {
            for (int i = 0; i < Wrist.NumBones; i++)
            {
                if (_colorBones[i] != null)
                    _colorBones[i].clearColorMap();
            }
        }

        public void clearContoursForAllBones()
        {
            for (int i = 0; i < Wrist.NumBones; i++)
                if (_colorBones[i] != null)
                    _colorBones[i].removeContour();
        }

        private TransformMatrix[] calculateRelativeMotionForDistanceMaps(int boneIndex, int positionIndex, int[] boneInteraction)
        {
            TransformMatrix[] tmRelMotions = new TransformMatrix[Wrist.NumBones]; //for each position
            if (positionIndex == 0) //no transforms needed for the neutral position, we are all set :)
                return tmRelMotions;

            /* Check if we are missing kinematics for the bone, if so, then we can not
             * calculate distance maps (we don't know where the bone is, so we just return all null)
             */
            if (_transformMatrices[positionIndex - 1][boneIndex] == null ||
                _transformMatrices[positionIndex - 1][boneIndex].isIdentity())
                return tmRelMotions;

            TransformMatrix tmBone = _transformMatrices[positionIndex - 1][boneIndex];
            foreach (int testBoneIndex in boneInteraction)
            {
                //Again, check if there is no kinematics for the test bone, again, if none, just move on
                if (_transformMatrices[positionIndex - 1][testBoneIndex] == null ||
                _transformMatrices[positionIndex - 1][testBoneIndex].isIdentity())
                    continue;

                TransformMatrix tmFixedBone = _transformMatrices[positionIndex - 1][testBoneIndex];
                //so fix the current bone, and move our test bone to that position....yes?
                tmRelMotions[testBoneIndex] = tmFixedBone.Inverse() * tmBone;
            }
            return tmRelMotions;

        }


        private double[] getOrCalculateDistanceMap(int boneIndex, int positionIndex)
        {
            //setup save space if it doesn't exist
            if (_calculatedDistances == null)
                _calculatedDistances = new double[Wrist.NumBones][][];

            readInDistanceFieldsIfNotLoaded();

            if (_calculatedDistances[boneIndex]==null)
                _calculatedDistances[boneIndex] = new double[_transformMatrices.Length+1][]; //add one extra for neutral
            
            if (_calculatedDistances[boneIndex][positionIndex]==null)
                _calculatedDistances[boneIndex][positionIndex] = createDistanceMap(boneIndex,positionIndex);

            return _calculatedDistances[boneIndex][positionIndex];
        }

        /// <summary>
        /// Calculates the distanceMaps for a given bone using the precalculated distance fields.
        /// If a distance field is unavailable, it is assumed to be infinately far away...
        /// </summary>
        /// <param name="boneIndex"></param>
        /// <param name="positionIndex"></param>
        /// <returns></returns>
        private double[] createDistanceMap(int boneIndex, int positionIndex)
        {
            readInDistanceFieldsIfNotLoaded();
            CTmri[] mri = _distanceFields;
            float[,] pts = _colorBones[boneIndex].getVertices();
            int numVertices = pts.GetLength(0);

            double[] distances = new double[numVertices];
            int[] interaction = Wrist.BoneInteractionIndex[boneIndex]; //load  bone interactions

            TransformMatrix[] tmRelMotions = calculateRelativeMotionForDistanceMaps(boneIndex, positionIndex, interaction);

            //for each vertex           
            for (int i = 0; i < numVertices; i++)
            {
                distances[i] = Double.MaxValue; //set this vertex to the default
                //for (int j = 0; j < Wrist.NumBones; j++)
                foreach (int j in interaction) //only use the bones that we have specified interact
                {
                    if (j == boneIndex) continue;
                    if (mri[j] == null) continue; //skip missing scans

                    double x = pts[i, 0];
                    double y = pts[i, 1];
                    double z = pts[i, 2];

                    //check if we need to move for non neutral position
                    if (positionIndex != 0)
                    {
                        //skip missing kinematic info
                        if (tmRelMotions[j] == null)
                            continue;

                        //lets move the bone getting colored, into the space of the other bone...
                        double[] p0 = new double[] { x, y, z };
                        double[] p1 = tmRelMotions[j] * p0;
                        x = p1[0];
                        y = p1[1];
                        z = p1[2];
                    }

                    double dX = (x - mri[j].CoordinateOffset[0]) / mri[j].voxelSizeX;
                    double dY = (y - mri[j].CoordinateOffset[1]) / mri[j].voxelSizeY;
                    double dZ = (z - mri[j].CoordinateOffset[2]) / mri[j].voxelSizeZ;

                    const double xBound = 96.9; //get the boundaries of the distance cube
                    const double yBound = 96.9; //
                    const double zBound = 96.9; //

                    ////////////////////////////////////////////////////////
                    //is surface point picked inside of the cube?

                    if (dX >= 3.1 && dX <= xBound && dY >= 3.1
                        && dY <= yBound && dZ >= 3.1 && dZ <= zBound)
                    {
                        double localDist = mri[j].sample_s_InterpCubit(dX, dY, dZ);
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


        private int[] createColormap(int boneIndex, int positionIndex)
        {
            double[] distances = getOrCalculateDistanceMap(boneIndex, positionIndex);
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
                else if (distances[i] > 3)  //check if we are too far away
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
                    uint GB = (uint)(distances[i] * 255.0 / 3.0);
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

        private int[] getColorMapSingleBoneSinglePosition(int boneIndex, int positionIndex)
        {
            lock (this)
            {
                if (_calculatedColorMaps == null)
                    _calculatedColorMaps = new int[Wrist.NumBones][][];

                if (_calculatedColorMaps[boneIndex] == null)
                    _calculatedColorMaps[boneIndex] = new int[_transformMatrices.Length + 1][];
            }
            if (_calculatedColorMaps[boneIndex][positionIndex] == null)
                _calculatedColorMaps[boneIndex][positionIndex] = createColormap(boneIndex, positionIndex);

            return _calculatedColorMaps[boneIndex][positionIndex];
        }

        private Contour getContourSingleBoneSinglePosition(int boneIndex, int positionIndex)
        {
            lock (this)
            {
                if (_calculatedContours == null)
                    _calculatedContours = new Contour[Wrist.NumBones][];

                if (_calculatedContours[boneIndex] == null)
                    _calculatedContours[boneIndex] = new Contour[_transformMatrices.Length + 1];
            }
            if (_calculatedContours[boneIndex][positionIndex] == null)
                _calculatedContours[boneIndex][positionIndex] = createContourSingleBoneSinglePosition(boneIndex, positionIndex, _contourDistances, _contourColors);

            return _calculatedContours[boneIndex][positionIndex];
        }

        public void showContoursForPosition(int positionIndex)
        {
            for (int i = 0; i < Wrist.NumBones; i++)
            {
                Contour cont = getContourSingleBoneSinglePosition(i, positionIndex);
                _colorBones[i].setAndReplaceContour(cont);
            }
        }

        public void calculateAllContours()
        {
            int numPos = _transformMatrices.Length + 1;
            for (int i = 0; i < Wrist.NumBones; i++)
                for (int j = 0; j < numPos; j++)
                {
                    getContourSingleBoneSinglePosition(i, j); //this function will cache them, but not show anything...
                }
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

        private Contour createContourSingleBoneSinglePosition(int boneIndex, int positionIndex, double[] cDistances, System.Drawing.Color[] colors)
        {
            double[] dist = getOrCalculateDistanceMap(boneIndex, positionIndex);
            //if distance maps are not available, just returns max distances....should we warn the user?
            float[,] points = _colorBones[boneIndex].getVertices();
            int[,] conn = _colorBones[boneIndex].getFaceSetIndices();

            Contour cont1 = new Contour(cDistances.Length);
            cont1.Color = colors[0];

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

                contour.addLineSegment(newPt1[0], newPt1[1], newPt1[2], newPt2[0], newPt2[1], newPt2[2]);
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
        private struct BonePositionInfo
        {
            public int PositionIndex;
            public int BoneIndex;
        }

        public void addToColorMapQueue(int currentPositionIndex, bool addAll, bool addCurrent)
        {
            if (addAll)
            {
                for (int i = 0; i < _transformMatrices.Length + 1; i++)
                    addToColorMapQueue(i);
            }
            else if (addCurrent)
                addToColorMapQueue(currentPositionIndex);
        }
        private void addToColorMapQueue(int[] positionIndexs)
        {
            foreach (int posIndex in positionIndexs)
                addToColorMapQueue(posIndex);
        }
        private void addToColorMapQueue(int positionIndex)
        {
            for (int i = 0; i < Wrist.NumBones; i++)
                addToColorMapQueue(positionIndex, i);
        }
        private void addToColorMapQueue(int positionIndex, int boneIndex)
        {
            //int processors = System.Environment.ProcessorCount;
            BonePositionInfo info = new BonePositionInfo();
            info.PositionIndex = positionIndex;
            info.BoneIndex = boneIndex;

            _workQueueColorMaps.Enqueue(info);
        }

        public void addToContourQueue(int currentPositionIndex, bool addAll, bool addCurrent)
        {
            if (addAll)
            {
                for (int i = 0; i < _transformMatrices.Length + 1; i++)
                    addToContourQueue(i);
            }
            else if (addCurrent)
                addToContourQueue(currentPositionIndex);
        }
        private void addToContourQueue(int[] positionIndexs)
        {
            foreach (int posIndex in positionIndexs)
                addToContourQueue(posIndex);
        }
        private void addToContourQueue(int positionIndex)
        {
            for (int i = 0; i < Wrist.NumBones; i++)
                addToContourQueue(positionIndex, i);
        }
        private void addToContourQueue(int positionIndex, int boneIndex)
        {
            BonePositionInfo info = new BonePositionInfo();
            info.PositionIndex = positionIndex;
            info.BoneIndex = boneIndex;

            _workQueueContours.Enqueue(info);
        }

        public void processAllPendingQueues()
        {
            int totalNumberJobs = _workQueueColorMaps.Count + _workQueueContours.Count;

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
            //load in the distance fields
            readInDistanceFieldsIfNotLoaded();

            //first process the ColorMap Queue
            int numThreads = Math.Min(System.Environment.ProcessorCount, _workQueueColorMaps.Count);

            //start all of the worker threads
            _workerThreads = new Thread[numThreads];
            for (int i = 0; i < numThreads; i++)
            {
                _workerThreads[i] = new Thread(workThreadWork);
                _workerThreads[i].Start(_workQueueColorMaps);
            }
            //wait for worker threads to finish
            foreach (Thread curThread in _workerThreads)
                curThread.Join();


            //okay, now lets run the Contour queue
            numThreads = Math.Min(System.Environment.ProcessorCount, _workQueueContours.Count);

            //start all of the worker threads
            _workerThreads = new Thread[numThreads];
            for (int i = 0; i < numThreads; i++)
            {
                _workerThreads[i] = new Thread(workThreadWork);
                _workerThreads[i].Start(_workQueueContours);
            }
            //wait for worker threads to finish
            foreach (Thread curThread in _workerThreads)
                curThread.Join();

            //hmm....we should be done now... so we return
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
            Queue workQueue = (Queue)queue;
            BonePositionInfo currentJob;
            bool done = false;
            while (!done)
            {
                //first get the job
                lock (workQueue)
                {
                    if (workQueue.Count > 0)
                        currentJob = (BonePositionInfo)workQueue.Dequeue();
                    else
                    {
                        done = true;
                        continue;
                    }
                }

                //now lets process this job
                if (workQueue == _workQueueColorMaps)
                {
                    getColorMapSingleBoneSinglePosition(currentJob.BoneIndex, currentJob.PositionIndex); //get and discard is fine
                }
                else if (workQueue == _workQueueContours)
                {
                    getContourSingleBoneSinglePosition(currentJob.BoneIndex, currentJob.PositionIndex); //get and discard is fine
                }

                //done with the job, lets report in our progress
                _bgWorker.ReportProgress(1);
            }
        }
        #endregion
    }
}
