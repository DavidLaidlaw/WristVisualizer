using System;
using System.Collections.Generic;
using System.Text;
using DotNetMatrix;

namespace libWrist
{
    public class TM : GeneralMatrix
    {
        public TM() : base(4, 4)
        {
            setToIdentity();
        }

        public TM(GeneralMatrix gm) : base(4, 4)
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    this.Array[i][j] = gm.Array[i][j];
        }

        /// <summary>
        /// sets the current matrix to the identity matrix
        /// </summary>
        private void setToIdentity()
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    this.Array[i][j] = (i == j ? 1.0 : 0.0);
        }

        public void setTranslation(GeneralMatrix gm)
        {
            setTranslation(gm.Array[0]);
        }
        public void setTranslation(double[] t)
        {
            if (t.Length != 3)
                throw new ArgumentException("Tranlsation must have 3 values");
            for (int i = 0; i < 3; i++)
                this.Array[3][i] = t[i];
        }

        /// <summary>
        /// Sets the rotation matrix to be a rotation about a given axis
        /// </summary>
        /// <param name="axis">A value from 0-2 indicating what axis to rotate about. (0=X axis, 1=Y, 2=Z)</param>
        /// <param name="angle">Amount to rotate in radians</param>
        public void rotate(int axis, double angle)
        {
            double s = Math.Sin(angle);
            double c = Math.Cos(angle);
            setToIdentity();
            switch (axis)
            {
                case 0: Array[1][1] = c; Array[1][2] = -s; Array[2][1] = s; Array[2][2] = c; break;
                case 1: Array[0][0] = c; Array[0][2] = s; Array[2][0] = -s; Array[2][2] = c; break;
                case 2: Array[0][0] = c; Array[0][1] = -s; Array[1][0] = s; Array[1][1] = c; break;
                default:
                    throw new ArgumentException("Invalid axis to rotate about, must be 0, 1, or 2");
            }
        }

        /// <summary>
        /// Set Transform to be a rotation matrix a given amount around an
        /// arbitrary axis
        /// 
        /// see CG AvDam pg 227 (mind wrong sign in M[0][1] in old edition) or quat Watt
        /// </summary>
        /// <param name="axis">axis to rotate about</param>
        /// <param name="angle">Amount to rotate in radians</param>
        public void rotateAxis(double[] axis, double angle)
        {
            if (axis.Length != 3)
                throw new ArgumentException("Axis must have 3 values");

            double s = Math.Sin(angle);
            double c = Math.Cos(angle);
            setToIdentity();
            double nx = axis[0]; 
            double ny = axis[1]; 
            double nz = axis[2];
            double nx2 = nx * nx;
            double ny2 = ny * ny;
            double nz2 = nz * nz;

            Array[0][0] = nx2 + c * (1 - nx2);
            Array[0][1] = nx * ny * (1 - c) - nz * s;
            Array[0][2] = nx * nz * (1 - c) + ny * s;

            Array[1][0] = nx * ny * (1 - c) + nz * s;
            Array[1][1] = ny2 + c * (1 - ny2);
            Array[1][2] = ny * nz * (1 - c) - nx * s;

            Array[2][0] = nz * nx * (1 - c) - ny * s;
            Array[2][1] = ny * nz * (1 - c) + nx * s;
            Array[2][2] = nz2 + c * (1 - nz2);
        }

        public void rotate_around_com(double[] angles, double[] con)
        {
            //int i;
            //TransfMatr rotMat, retMatr;
            //TransfMatr rotX, rotY, rotZ;
            //TransfMatr trans_com, trans_com_inverse;

            //rotX.rotate(0, angles[0]);
            //rotY.rotate(1, angles[1]);
            //rotZ.rotate(2, angles[2]);

            //rotMat = rotX * rotY * rotZ;
            //cout << "Rotation x,y,z" << endl;
            //rotMat.print();

            //trans_com.identity();
            //trans_com_inverse.identity();

            //trans_com.set_translate(com);
            //for (i = 0; i < 3; i++)
            //    com[i] = -com[i];
            //trans_com_inverse.set_translate(com);
            //cout << "Transl com " << endl;
            //trans_com.print();
            //cout << "Transl com inverse " << endl;
            //trans_com_inverse.print();
            //retMatr = trans_com * rotMat * trans_com_inverse;

            //return retMatr;

        }
    }
}
