using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
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

        public DistanceMaps(Wrist wrist, TransformMatrix[][] transformMatrices, ColoredBone[] colorBones)
        {
            _wrist = wrist;
            _transformMatrices = transformMatrices;
            _colorBones = colorBones;
        }

        private void readInDistanceFieldsIfNotLoaded()
        {
            if (_distanceFields != null)
                return;

            _distanceFields = new CTmri[Wrist.NumBones];

            for (int i = 0; i < Wrist.NumBones; i++)
            {
                string basefolder = Path.Combine(Path.Combine(_wrist.subjectPath, _wrist.neutralSeries), "DistanceFields");
                string folder = String.Format("{0}{1}_mri", Wrist.ShortBoneNames[i], _wrist.neutralSeries.Substring(1, 3));
                if (Directory.Exists(Path.Combine(basefolder, folder)))
                {
                    _distanceFields[i] = new CTmri(Path.Combine(basefolder, folder));
                    _distanceFields[i].loadImageData();
                }
                else
                    _distanceFields[i] = null;
            }
        }


        private bool hasDistanceMapsForPosition(int positionIndex)
        {
            if (_calculatedColorMaps == null)
                return false;

            //only check the radius, it should be a good enough check....
            if (_calculatedColorMaps[0] == null)
                return false;

            return (_calculatedColorMaps[0][positionIndex] != null);
        }


        public void loadDistanceMapsForPositionIfCalculatedOrClear(int positionIndex)
        {
            if (hasDistanceMapsForPosition(positionIndex))
                loadDistanceMapsForPosition(positionIndex);
            else
                clearDistanceMapsForAllBones();
        }

        public void loadDistanceMapsForPosition(int positionIndex)
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
                    _calculatedColorMaps[i][positionIndex] = createColormap(_distanceFields, i, positionIndex);

                //now set that color
                _colorBones[i].setColorMap(_calculatedColorMaps[i][positionIndex]);
            }
        }

        public void clearDistanceMapsForAllBones()
        {
            for (int i = 0; i < Wrist.NumBones; i++)
            {
                if (_colorBones[i] != null)
                    _colorBones[i].clearColorMap();
            }
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

        public int[] createColormap(CTmri[] mri, int boneIndex) { return createColormap(mri, boneIndex, 0); }
        public int[] createColormap(CTmri[] mri, int boneIndex, int positionIndex)
        {
            double dDist = Double.MaxValue;
            DateTime t2 = DateTime.Now;
            float[,] pts = _colorBones[boneIndex].getVertices();
            Console.WriteLine("Getting vertices {0}: {1}", boneIndex, ((TimeSpan)(DateTime.Now - t2)));
            int numVertices = pts.GetLength(0);
            double[] dDistances = new double[Wrist.NumBones - 1];

            int[] colors = new int[numVertices];
            int[] interaction = Wrist.BoneInteractionIndex[boneIndex]; //load  bone interactions

            TransformMatrix[] tmRelMotions = calculateRelativeMotionForDistanceMaps(boneIndex, positionIndex, interaction);

            //for each vertex           
            for (int i = 0; i < numVertices; i++)
            {
                int m = 0;
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
                        dDist = mri[j].sample_s_InterpCubit(dX, dY, dZ);
                    }
                    else
                        dDist = Double.MaxValue;

                    dDistances[m] = dDist;
                    m++;
                }

                //find smallest
                double min = Double.MaxValue;
                for (int im = 0; im < m; im++)
                {
                    if (dDistances[im] < min) min = dDistances[im];
                }
                dDist = min;

                UInt32 packedColor;

                //first check if there is collission
                if (dDist < 0)
                {
                    //color collision in blue
                    packedColor = 0X0000FFFF;
                }
                // a parameter could be used instead of plain 3
                else if (dDist > 3)  //check if we are too far away
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
                    uint GB = (uint)(dDist * 255.0 / 3.0);
                    packedColor = (GB << 16) | (GB << 8) | 0xFF0000FF;
                }
                colors[i] = (int)packedColor;
            }
            return colors;
        }
    }
}
