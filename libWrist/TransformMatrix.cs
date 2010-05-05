using System;
using System.Collections.Generic;
using System.Text;
using DotNetMatrix;

namespace libWrist
{
    public class TransformMatrix : GeneralMatrix
    {
        public enum Axes
        {
            X, Y, Z
        }

        public TransformMatrix() : base(4, 4)
        {
            setToIdentity();
        }

        public TransformMatrix(GeneralMatrix gm) : base(4, 4)
        {
            this.SetMatrix(gm);
        }

        public TransformMatrix(HelicalTransform ham) : base(4, 4)
        {
            setToIdentity();
            //sourced from Helical_To_RT.m
            //original source: Kinzel et al., JOB 5:93-105, 1972

            double phi_r = ham.Phi * Math.PI / 180; //convert phi to radians
            double vers = 1 - Math.Cos(phi_r);
            this.Array[0][0] = ham.N[0] * ham.N[0] * vers + Math.Cos(phi_r);
            this.Array[0][1] = ham.N[0] * ham.N[1] * vers - ham.N[2] * Math.Sin(phi_r);
            this.Array[0][2] = ham.N[0] * ham.N[2] * vers + ham.N[1] * Math.Sin(phi_r);
            this.Array[1][0] = ham.N[0] * ham.N[1] * vers + ham.N[2] * Math.Sin(phi_r);
            this.Array[1][1] = ham.N[1] * ham.N[1] * vers + Math.Cos(phi_r);
            this.Array[1][2] = ham.N[1] * ham.N[2] * vers - ham.N[0] * Math.Sin(phi_r);
            this.Array[2][0] = ham.N[2] * ham.N[0] * vers - ham.N[1] * Math.Sin(phi_r);
            this.Array[2][1] = ham.N[1] * ham.N[2] * vers + ham.N[0] * Math.Sin(phi_r);
            this.Array[2][2] = ham.N[2] * ham.N[2] * vers + Math.Cos(phi_r);

            //Translation
            GeneralMatrix R = this.GetMatrix(0,2,0,2);
            GeneralMatrix q = new GeneralMatrix(ham.Q, 3);
            GeneralMatrix Rq = R*q;

            this.Array[0][3] = ham.Q[0] + (ham.N[0] * ham.Trans) - Rq.Array[0][0];
            this.Array[1][3] = ham.Q[1] + (ham.N[1] * ham.Trans) - Rq.Array[1][0];
            this.Array[2][3] = ham.Q[2] + (ham.N[2] * ham.Trans) - Rq.Array[2][0];
        }

        public TransformMatrix(TransformRT rt) : base(4, 4)
        {
            this.setToIdentity();
            this.SetMatrix(0, 2, 0, 2, rt.R);// set R
            this.SetMatrix(0, 2, 3, 3, rt.T.Transpose()); //set T, do I need to transpose first?
        }

        /// <summary>
        /// Create a transform matrix from a 3x3 rotational matrix, and a 1x3 translation vector
        /// </summary>
        /// <param name="r"></param>
        /// <param name="t"></param>
        public TransformMatrix(double[][] r, double[] t) : base(4,4)
        {
            this.setToIdentity();
            //set r values
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    this.Array[i][j] = r[i][j];
                }
            }

            //set t
            for (int i = 0; i < 3; i++)
                this.Array[i][3] = t[i];
        }

        /// <summary>
        /// Create a transform matrix from a 4x4 rotational matrix, which was flattened into a 16x1 array
        /// </summary>
        /// <param name="a">Flattened array of 16 elements representing the 4x4</param>
        public TransformMatrix(double[] a) : this(a, 0) { }


        /// <summary>
        /// Create a transform matrix from a 4x4 rotational matrix, which was flattened into a 16x1 array
        /// </summary>
        /// <param name="r">Array containing the 16 elements representing the 4x4 matrix flattened out</param>
        /// <param name="offset">Starting index of the first value in r</param>
        public TransformMatrix(double[] a, int offset) : base(4, 4)
        {
            this.setToIdentity();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    this.Array[j][i] = a[i * 4 + j + offset];
                }
            }
        }

        public libCoin3D.Transform ToTransform()
        {
            libCoin3D.Transform transform = new libCoin3D.Transform();
            transform.setTransform(Array[0][0], Array[0][1], Array[0][2],
                Array[1][0], Array[1][1], Array[1][2],
                Array[2][0], Array[2][1], Array[2][2],
                Array[0][3], Array[1][3], Array[2][3]);
            return transform;
        }

        public HelicalTransform ToHelical()
        {
            //ported from RT_to_helical.m
            /* This procedure calculates the helical axis of the mapping
             * R (3x3) and T (1x3). Returns the angle in phi degrees, unit Ham in n (1x3), 
             * translation along HAM,and a point Q (1x3).
             * 
             * Modified 9/02 SS
             * Different formulas were used for situation where 1 or 2 components of n are zero. 
             * as described in page 454 of Panjabi's paper (using R11 and R12 instead of R13 and R33)
             * the situation for n = [0 0 0] returns NaN as of now, can add in a specific situation for 
             * that.
             */

            if (this.isIdentity())
                return new HelicalTransform();
            

            bool flag = false;
            double[] n = new double[3];

            double[][] R = this.Array; //for clarity
            double cos_phi = 0, sin_phi = 0;
            double c = (R[0][0] - 1) * (R[1][1] - 1) - R[0][1] * R[1][0]; //c value defined using rows 1 and 2 of R
            if (c != 0) //TODO: add error...?
            {
                double a, b;
                if (Math.Abs(c) < 2.5e-17) //cutoff for z=0, where c approaches zero
                {
                    c = R[1][0] * R[2][1] - (R[1][1] - 1) * R[2][0]; //c value defined using rows 1 and 2 of R
                    a = ((R[1][1] - 1) * (R[2][2] - 1) - R[2][1] * R[1][2]) / c;
                    b = (R[1][2] * R[2][0] - R[2][0] * (R[2][2] - 1)) / c;
                }
                else //z not zero
                {
                    a = (R[0][1] * R[1][2] - R[0][2] * (R[1][1] - 1)) / c;
                    b = (R[0][2] * R[1][0] - R[1][2] * (R[0][0] - 1)) / c;
                }
                n[2] = Math.Sqrt(1 / (1 + a * a + b * b));
                n[0] = a * n[2];
                n[1] = b * n[2];


                if (Math.Abs(n[2]) < 10e-15) //if y=0, n2 approaches zero, so use different equations without n2
                {
                    //in denominator
                    cos_phi = (R[0][0] - n[0] * n[0]) / (1 - n[0] * n[0]); //R11 = M11
                    sin_phi = -R[0][1] / n[2]; // R12 = M12
                }
                else
                {
                    cos_phi = (R[2][2] - n[2] * n[2]) / (1 - n[2] * n[2]); //R33 = M33
                    sin_phi = (R[0][2] - n[0] * n[2] * (1 - cos_phi)) / n[1]; //R13 = M13
                }
            }
            else // c==0
            {
                n[0] = 0;
                n[1] = 0;
                n[2] = 0;
                if (R[2][2] == 1) //index 9
                {
                    n[2] = 1;
                    cos_phi = R[0][0];
                    sin_phi = R[0][1];
                }
                else if (R[1][1] == 1) //index 5
                {
                    n[1] = 1;
                    cos_phi = R[0][0];
                    sin_phi = R[2][0];
                }
                else if (R[0][0] == 1) //index 1
                {
                    n[0] = 1;
                    cos_phi = R[1][1];
                    sin_phi = R[1][2];
                }

                //if R==eye(3)
                if (R[0][0] == 1 && R[0][1] == 0 && R[0][2] == 0 &&
                    R[1][0] == 0 && R[1][1] == 1 && R[1][2] == 0 &&
                    R[2][0] == 0 && R[2][1] == 0 && R[2][2] == 1)
                {
                    flag = true;
                }                
            }

            double phi_r = Math.Atan2(sin_phi, cos_phi);
            double phi = phi_r * 180 / Math.PI;
            //double t_ham = dot(T,n);
            double t_ham = Array[0][3] * n[0] + Array[1][3] * n[1] + Array[2][3] * n[2];

            //From Panjabi et al., 1981
            //let vector a = [0,0,0], then rA = T;
            double[] e = new double[3]; //e =T - n.*t_ham;
            e[0] = Array[0][3] - n[0]*t_ham;
            e[1] = Array[1][3] - n[1]*t_ham;
            e[2] = Array[2][3] - n[2]*t_ham;            
            if (flag) //ZERO ROTATION
            { 
                return new HelicalTransform(); //return empty, or zero
            }

            //q = e./2 + cross(n,e)./(2*tan(phi*pi/180/2));
            double[] q = new double[3];
            double[] crossNxE = new double[3];
            crossNxE[0] = n[1] * e[2] - n[2] * e[1];
            crossNxE[1] = n[2] * e[0] - n[0] * e[2];
            crossNxE[2] = n[0] * e[1] - n[1] * e[0];
            q[0] = e[0] / 2 + crossNxE[0] / (2 * Math.Tan(phi_r / 2));
            q[1] = e[1] / 2 + crossNxE[1] / (2 * Math.Tan(phi_r / 2));
            q[2] = e[2] / 2 + crossNxE[2] / (2 * Math.Tan(phi_r / 2));
            
            

            /* Direction Trapping - fix all Phi's to positive
             * this is needed for error calculations,  Mathematically
             * equivalent values w/opposite orientations cause
             * problems... (+5 deg, +5 mm, + x is equal to -5 deg, -5 mm, -x)
             * R. McGovern 9/22
             */
            if (phi < 0)
            {
                phi = -phi;
                n[0] = -n[0];
                n[1] = -n[1];
                n[2] = -n[2];
                t_ham = -t_ham;
            }
            return new HelicalTransform(phi, n, t_ham, q);
        }

        /// <summary>
        /// Returns the Rotation Matrix represented as 3 Euler angles (in degrees), in x,y,z order.
        /// </summary>
        /// <returns></returns>
        public double[] ToEuler()
        {
            double[] euler = new double[3];
            double sinThetaY = -this.Array[2][0];
            double cosThetaY = Math.Sqrt(1 - sinThetaY * sinThetaY);
            euler[0] = Math.Atan2(this.Array[2][1], Array[2][2]) * 180 / Math.PI;
            euler[1] = Math.Atan2(sinThetaY, cosThetaY) * 180 / Math.PI;
            euler[2] = Math.Atan2(this.Array[1][0], this.Array[0][0]) * 180 / Math.PI;
            return euler;
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

        #region Point Multiplication Operations

        public virtual double[] Multiply(double[] P)
        {
            if (P.Length != 3)
                throw new ArgumentException("Expecting a point with 3 doubles: x,y,z");

            double[] point = new double[3];
            point[0] = Array[0][0] * P[0] + Array[0][1] * P[1] + Array[0][2] * P[2] + Array[0][3];
            point[1] = Array[1][0] * P[0] + Array[1][1] * P[1] + Array[1][2] * P[2] + Array[1][3];
            point[2] = Array[2][0] * P[0] + Array[2][1] * P[1] + Array[2][2] * P[2] + Array[2][3];
            return point;
        }

        public static double[] operator *(TransformMatrix m, double[] p)
        {
            return m.Multiply(p);
        }
        #endregion

        #region Overloaded Methods Redefined to return TransformMatrix

        /// <summary>Linear algebraic matrix multiplication, A * B</summary>
        /// <param name="B">   another matrix
        /// </param>
        /// <returns>     Matrix product, A * B
        /// </returns>
        /// <exception cref="System.ArgumentException">  Matrix inner dimensions must agree.
        /// </exception>
        public virtual TransformMatrix Multiply(TransformMatrix B)
        {
            return new TransformMatrix(base.Multiply(B));
        }

        /// <summary>
        /// Multiplication of matrices
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public static TransformMatrix operator *(TransformMatrix m1, TransformMatrix m2)
        {
            return m1.Multiply(m2);
        }

        /// <summary>Matrix inverse or pseudoinverse</summary>
        /// <returns>     inverse(A) if A is square, pseudoinverse otherwise.
        /// </returns>
        public new TransformMatrix Inverse()
        {
            return new TransformMatrix(base.Inverse());
        }

        public new TransformMatrix Transpose()
        {
            return new TransformMatrix(base.Transpose());
        }
        #endregion

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

            TransformMatrix rotMat;
            TransformMatrix transMat = new TransformMatrix();
            TransformMatrix transMatInverse = new TransformMatrix();
            TransformMatrix rotX = new TransformMatrix();
            TransformMatrix rotY = new TransformMatrix();
            TransformMatrix rotZ = new TransformMatrix();

            rotX.rotate(Axes.X,angles[0]);
            rotY.rotate(Axes.Y,angles[1]);
            rotZ.rotate(Axes.Z,angles[2]);

            rotMat = rotX * rotY * rotZ;
            transMat.setTranslation(center);
            transMatInverse.setTranslation(-center[0], -center[1], -center[2]);
            TransformMatrix final = transMat * rotMat * transMatInverse;
            SetMatrix(final);
        }


        public void printToConsole()
        {
            printToConsole(this);
        }

        public static void printToConsole(TransformMatrix m)
        {
            //Console.WriteLine("TM Matrix:");
            Console.WriteLine("\t{0}\t{1}\t{2}\t{3}", m.Array[0][0], m.Array[0][1], m.Array[0][2], m.Array[0][3]);
            Console.WriteLine("\t{0}\t{1}\t{2}\t{3}", m.Array[1][0], m.Array[1][1], m.Array[1][2], m.Array[1][3]);
            Console.WriteLine("\t{0}\t{1}\t{2}\t{3}", m.Array[2][0], m.Array[2][1], m.Array[2][2], m.Array[2][3]);
            Console.WriteLine("\t{0}\t{1}\t{2}\t{3}", m.Array[3][0], m.Array[3][1], m.Array[3][2], m.Array[3][3]);
        }

        public bool isIdentity()
        {
            return this.isEqual(new TransformMatrix());
        }
        public bool isIdentity(double tolerance)
        {
            return this.isEqual(new TransformMatrix(), tolerance); //check if equal to new matrix, which defaults to identity
        }

        /// <summary>
        /// Compares two TransformMatrices to see if they are equal
        /// </summary>
        /// <param name="B">Matrix to test against</param>
        /// <returns></returns>
        public bool isEqual(TransformMatrix B) { return isEqual(B, 0.0001f); }

        /// <summary>
        /// Compares two TransformMatrices to see if they are equal
        /// </summary>
        /// <param name="B">Matrix to test against</param>
        /// <param name="tolerance">Precision to use for determining equality</param>
        /// <returns></returns>
        public bool isEqual(TransformMatrix B, double tolerance)
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    if (Math.Abs(Array[i][j] - B.Array[i][j]) > tolerance)
                        return false;
                }
            return true;
        }

    }
}
