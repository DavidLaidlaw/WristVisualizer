using System;
using System.Collections.Generic;
using System.Text;
using DotNetMatrix;

namespace libWrist
{
    public class TM : GeneralMatrix
    {
        public enum Axes
        {
            X, Y, Z
        }

        public TM() : base(4, 4)
        {
            setToIdentity();
        }

        public TM(GeneralMatrix gm) : base(4, 4)
        {
            this.SetMatrix(gm);
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

        public virtual void SetMatrix(GeneralMatrix gm)
        {
            if (gm.ColumnDimension != 4 && gm.RowDimension != 4)
                throw new ArgumentException("Can only create a TM from a 4x4 General matrix");

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    this.Array[i][j] = gm.Array[i][j];
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
                this.Array[i][3] = t[i];
        }
        public void setTranslation(double x, double y, double z)
        {
            this.Array[0][3] = x;
            this.Array[1][3] = y;
            this.Array[2][3] = z;
        }

        /// <summary>
        /// Sets the rotation matrix to be a rotation about a given axis
        /// </summary>
        /// <param name="axis">Axis to rotate about.</param>
        /// <param name="angle">Amount to rotate in radians</param>
        public void rotate(Axes axis, double angle)
        {
            switch (axis)
            {
                case Axes.X: 
                    rotate(0,angle); 
                    break;
                case Axes.Y: 
                    rotate(1,angle); 
                    break;
                case Axes.Z: 
                    rotate(2,angle);
                    break;
                default:
                    throw new ArgumentException("Invalid axis to rotate about, must be 0, 1, or 2");
            }
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
        /// Sets matrix as a rotation matrix about the X axis for the amount specified
        /// </summary>
        /// <param name="angle">rotation magnitude in radians</param>
        public void rotateX(double angle)
        {
            rotate(Axes.X, angle);
        }

        /// <summary>
        /// Sets matrix as a rotation matrix about the Y axis for the amount specified
        /// </summary>
        /// <param name="angle">rotation magnitude in radians</param>
        public void rotateY(double angle)
        {
            rotate(Axes.Y, angle);
        }

        /// <summary>
        /// Sets matrix as a rotation matrix about the Z axis for the amount specified
        /// </summary>
        /// <param name="angle">rotation magnitude in radians</param>
        public void rotateZ(double angle)
        {
            rotate(Axes.Z, angle);
        }

        /// <summary>Linear algebraic matrix multiplication, A * B</summary>
        /// <param name="B">   another matrix
        /// </param>
        /// <returns>     Matrix product, A * B
        /// </returns>
        /// <exception cref="System.ArgumentException">  Matrix inner dimensions must agree.
        /// </exception>
        public virtual TM Multiply(TM B)
        {
            return new TM(base.Multiply(B));
        }

        /// <summary>
        /// Multiplication of matrices
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public static TM operator *(TM m1, TM m2)
        {
            return m1.Multiply(m2);
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

        /// <summary>
        /// Sets the rotation and transformation matrix so that it is 3 roations about
        /// the given center point. Rotation angles are applied in the order Z, Y, X
        /// </summary>
        /// <param name="angles">Amount of rotation (in radians) for rotating about the X, Y, and Z axis</param>
        /// <param name="center">The center point about which to rotate</param>
        /// <exception cref="System.ArgumentException">Invalid parameters</exception>
        public void rotateAboutCenter(double[] angles, double[] center)
        {
            if (angles.Length != 3)
                throw new ArgumentException("Must pass 3 angles");

            if (center.Length != 3)
                throw new ArgumentException("Center point must have 3 values");

            TM rotMat;
            TM transMat = new TM();
            TM transMatInverse = new TM();
            TM rotX = new TM();
            TM rotY = new TM();
            TM rotZ = new TM();

            rotX.rotate(Axes.X,angles[0]);
            rotY.rotate(Axes.Y,angles[1]);
            rotZ.rotate(Axes.Z,angles[2]);

            rotMat = rotX * rotY * rotZ;
            Console.WriteLine("Rotation x,y,z");
            printToConsole(rotMat);

            transMat.setTranslation(center);
            Console.WriteLine("Trans com");
            printToConsole(transMat);
            transMatInverse.setTranslation(-center[0], -center[1], -center[2]);
            Console.WriteLine("Trans inverse");
            printToConsole(transMatInverse);
            TM final = transMat * rotMat * transMatInverse;
            SetMatrix(final);
        }


        public void printToConsole()
        {
            printToConsole(this);
        }

        public static void printToConsole(TM m)
        {
            //Console.WriteLine("TM Matrix:");
            Console.WriteLine("\t{0}\t{1}\t{2}\t{3}", m.Array[0][0], m.Array[0][1], m.Array[0][2], m.Array[0][3]);
            Console.WriteLine("\t{0}\t{1}\t{2}\t{3}", m.Array[1][0], m.Array[1][1], m.Array[1][2], m.Array[1][3]);
            Console.WriteLine("\t{0}\t{1}\t{2}\t{3}", m.Array[2][0], m.Array[2][1], m.Array[2][2], m.Array[2][3]);
            Console.WriteLine("\t{0}\t{1}\t{2}\t{3}", m.Array[3][0], m.Array[3][1], m.Array[3][2], m.Array[3][3]);
        }
    }
}
