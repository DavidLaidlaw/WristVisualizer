using System;
using System.Collections.Generic;
using System.Text;

namespace libWrist
{
    /// <summary>
    /// Primarily static class used for calcuating the posture of various wrist positions.
    /// Code is based on callPosturesJC.m from TheCollective code.
    /// </summary>
    public class PostureCalculator
    {
        public struct Posture
        {
            public double FE;
            public double RU;
        }

        private struct CartesianCoordinate
        {
            public double az;
            public double elev;
            public double r;
        }

        private static int[] PROJ_PLANES = { 0, 2, 1 };
        private static int[] PROJ_PLANES_SIGN = { 1, 1, -1 };
        private static int POS_AXIS = 0;


        private static CartesianCoordinate cart2spherical(double x, double y, double z)
        {
            CartesianCoordinate coordinate = new CartesianCoordinate();
            double hypotxy = Math.Sqrt(x * x + y * y);
            coordinate.r = Math.Sqrt(hypotxy * hypotxy + z * z);
            coordinate.elev = Math.Atan2(z, hypotxy);
            coordinate.az = Math.Atan2(y, x);
            return coordinate;
        }

        public static void test()
        {
            TransformRT[] inertiaRTs = DatParser.parseInertiaFileToRT(@"P:\Data\Functional_Wrist\E01424\S15R\inertia15R.dat");
            TransformRT[] ACS_RT = DatParser.parseACSFileToRT(@"P:\Data\Functional_Wrist\E01424\S15R\AnatCoordSys.dat");
            TransformRT[] motion = DatParser.parseMotionFileToRT(@"P:\Data\Functional_Wrist\E01424\S02R\Motion15R02R.dat");
            TransformMatrix inertia = new TransformMatrix(inertiaRTs[8]);
            TransformMatrix ACS = new TransformMatrix(ACS_RT[0]);
            Posture p = CalculatePosture(ACS, inertia);
            Posture p2 = CalculatePosture(ACS, inertia, new TransformMatrix(motion[0]), new TransformMatrix(motion[8]));
        }

        public static Posture CalculatePosture(TransformMatrix ACS, TransformMatrix Inertia)
        {
            return CalculatePosture(ACS, Inertia, new TransformMatrix());
        }

        public static Posture CalculatePosture(TransformMatrix ACS, TransformMatrix Inertia, TransformMatrix MotionRelativeBone, TransformMatrix MotionTestBone)
        {
            TransformMatrix relativeMotion = MotionRelativeBone.Inverse() * MotionTestBone;
            return CalculatePosture(ACS, Inertia, relativeMotion);
        }

        public static Posture CalculatePosture(TransformMatrix ACS, TransformMatrix Inertia, TransformMatrix BoneRelMotion)
        {
            Posture post = new Posture();

            TransformMatrix AnatIner = ACS.Inverse() * BoneRelMotion * Inertia;

            double[] myInertia = { 0, 0, 0 };
            for (int i = 0; i < 3; i++)
                myInertia[i] = AnatIner.Array[i][POS_AXIS]; //select which axis to use....

            double x = PROJ_PLANES_SIGN[0] * myInertia[PROJ_PLANES[0]];
            double y = PROJ_PLANES_SIGN[1] * myInertia[PROJ_PLANES[1]];
            double z = PROJ_PLANES_SIGN[2] * myInertia[PROJ_PLANES[2]];
            CartesianCoordinate coord = cart2spherical(x, y, z);
            double theta = coord.az;
            double phi = coord.elev;

            //if (PROJ_PLANES == {1, 3, -2}...
            theta = (theta / Math.Abs(theta)) * Math.PI - theta;
            //endif

            post.FE = theta * 180 / Math.PI;
            post.RU = phi * 180 / Math.PI;
            return post;
        }
    }
}
