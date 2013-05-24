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
    public static class DistanceMaps
    {
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

        private static Separator createContourCentroidSphere(Contour contour, int contourIndex)
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

            Contour cont1 = new Contour(colors, cDistances);

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

        public static Contour CreateContourSingleBoneSinglePositionTargetingArea(Bone referenceBone, double[] distanceMap, double targetArea, double tolerance, int iterations)
        {
            float[,] points = referenceBone.GetVertices();
            int[,] conn = referenceBone.GetFaceSetIndices();

            System.Drawing.Color[] colors = new System.Drawing.Color[] { System.Drawing.Color.White };

            /* Setup my starting bounds for our target answer. We are done with
             * this loop when the difference between these bounds is either less 
             * then the tolerance, or we hit the max number of iterations
             */
            double minBounds = 0; 
            double maxBounds = 3;
            while (iterations-- > 0)
            {
                double currentDistanceTrial = (maxBounds + minBounds) / 2; //try the middle
                double[] cDistances = new double[] {currentDistanceTrial};
                Contour c = createContourSingleBoneSinglePosition(points, conn, distanceMap, cDistances, colors);
                if (c.Areas[0] < targetArea) //if our area was too small, then are distance is too small
                    minBounds = currentDistanceTrial;
                else if (c.Areas[0] > targetArea) // area is too large => distance is too large
                    maxBounds = currentDistanceTrial;
                else //we hit the nail on the head, not possible, but oh well
                    return c;

                //TODO: Should I be able to check for an area within a limit?
                //check if we are done.
                System.Diagnostics.Debug.Assert(maxBounds > minBounds);
                if (maxBounds - minBounds < tolerance)
                {
                    return c;
                }
            }
            return null; //if we reach here, we maxed our iterations, so return null....I think?
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

    }
}
