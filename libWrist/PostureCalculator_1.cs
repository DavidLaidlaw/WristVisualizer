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
            public double FE_Raw;
            public double RU_Raw;
        }

        public struct PronationSupination
        {
            public double Euler_x;
            public double Euler_y;
            public double Euler_z;
            public double PronationAngle;
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

        private const double NEUTRAL_FE_POSITION = -43.107;
        private const double NEUTRAL_RU_POSITION = -10.26;

        private const double NEUTRAL_PS_POSITION = 171.6424;


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

            post.FE_Raw = theta * 180 / Math.PI;
            post.RU_Raw = phi * 180 / Math.PI;

            post.FE = post.FE_Raw - NEUTRAL_FE_POSITION;
            post.RU = post.RU_Raw - NEUTRAL_RU_POSITION;
            return post;
        }

        public static PronationSupination CalculatePronationSupination(TransformMatrix RCS, TransformMatrix UCS, TransformMatrix MotionTestBone, TransformMatrix MotionRelativeBone)
        {
            TransformMatrix relativeMotion = MotionRelativeBone.Inverse() * MotionTestBone;
            return CalculatePronationSupination(RCS, UCS, relativeMotion);
        }

        public static PronationSupination CalculatePronationSupination(TransformMatrix RCS, TransformMatrix UCS)
        {
            return CalculatePronationSupination(RCS, UCS, new TransformMatrix());
        }

        public static PronationSupination CalculatePronationSupination(TransformMatrix RCS, TransformMatrix UCS, TransformMatrix BoneRelMation)
        {
            TransformMatrix relativeCSMotion = UCS.Inverse() * BoneRelMation * RCS;
            double[] euler = relativeCSMotion.ToEuler();
            PronationSupination result = new PronationSupination();
            result.Euler_x = euler[0];
            result.Euler_y = euler[1];
            result.Euler_z = euler[2];
            double pronationAngle = euler[0] - NEUTRAL_PS_POSITION; //correct for offset
            //correct for negative angles, want final wrist position to be [-180 180]
            while (pronationAngle < -180) pronationAngle += 360;
            result.PronationAngle = pronationAngle;
            return result;
        }

        public static Posture[] CalculatePosturesFE(Bone CoordinateSystemBone, Bone PositionDefiningBone)
        {
            //First check if we have enough
            if (!CoordinateSystemBone.IsValidBone || !CoordinateSystemBone.HasInertia ||
                !PositionDefiningBone.IsValidBone || !PositionDefiningBone.HasInertia)
                return null;

            //Lets create an array for the number of postures...
            int numPositions = CoordinateSystemBone.TransformMatrices.Length;
            Posture[] postures = new Posture[numPositions];
            for (int i = 0; i < numPositions; i++)
            {
                postures[i] = CalculatePosture(CoordinateSystemBone.InertiaMatrix, PositionDefiningBone.InertiaMatrix,
                    CoordinateSystemBone.TransformMatrices[i], PositionDefiningBone.TransformMatrices[i]);

                //need to make certain that only the capitate is corrected
                if (PositionDefiningBone.BoneIndex != (int)WristFilesystem.BIndex.CAP)
                {
                    postures[i].FE = postures[i].FE_Raw;
                    postures[i].RU = postures[i].RU_Raw;
                }
            }
            return postures;
        }

        public static PronationSupination[] CalculatePosturesPS(Bone radius, Bone ulna)
        {
            //First check if we have enough
            if (!radius.IsValidBone || !radius.HasInertia ||
                !ulna.IsValidBone || !ulna.HasInertia)
                return null;

            //Lets create an array for the number of postures...
            int numPositions = radius.TransformMatrices.Length;
            PronationSupination[] postures = new PronationSupination[numPositions];

            for (int i = 0; i < numPositions; i++)
            {
                postures[i] = CalculatePronationSupination(radius.InertiaMatrix, ulna.InertiaMatrix,
                    radius.TransformMatrices[i], ulna.TransformMatrices[i]);
            }
            return postures;
        }
    }
}
