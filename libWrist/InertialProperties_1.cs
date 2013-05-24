using System;
using System.Collections.Generic;
using System.Text;
using DotNetMatrix;

namespace libWrist
{
    public class InertialProperties
    {
        private float[][] _pts;
        private int[][] _connections;

        private double _volume;
        private double[] _centroid;
        private GeneralMatrix _eigenvectors;
        private GeneralMatrix _eigenvalues;

        public InertialProperties(float[][] pts, int[][] connections)
        {
            _pts = pts;
            _connections = connections;
            CalculateVolumeCentroid();
            CalculateEigenvalueDecomposition();
        }

        #region Public Interface
        public double Volume
        {
            get { return _volume; }
        }

        public double[] Centroid
        {
            get { return _centroid; }
        }

        public GeneralMatrix EigenVectors
        {
            get { return _eigenvectors; }
        }

        public GeneralMatrix EigenValues
        {
            get { return _eigenvalues; }
        }

        #endregion

        #region Internal Calculations
        private void CalculateVolumeCentroid()
        {
            double[] i = new double[3];
            double[] j = new double[3];
            double[] k = new double[3];
            double[] u = new double[3];

            int[] munc = new int[3];
            munc[0] = 0;
            munc[1] = 0;
            munc[2] = 0;
            double[] absu = new double[3];
            int wxyz = 0, wxy = 0, wxz = 0, wyz = 0;

            double[] ii = new double[3];
            double[] jj = new double[3];
            double[] kk = new double[3];

            double[] t_vol = new double[3];
            double[] vol = new double[3];
            vol[0] = 0;
            vol[1] = 0;
            vol[2] = 0;

            double[] func_sum = new double[3];
            func_sum[0] = 0;
            func_sum[1] = 0;
            func_sum[2] = 0;

            double surfaceArea = 0;

            //loop through all the triangles
            for (int count = 0; count < _connections.Length; count++)
            {
                //get the 3 points of the triangle
                float[] p1 = _pts[_connections[count][0]];
                float[] p2 = _pts[_connections[count][1]];
                float[] p3 = _pts[_connections[count][2]];

                //calculate the i, j, k vectors
                i[0] = p2[0] - p1[0]; j[0] = p2[1] - p1[1]; k[0] = p2[2] - p1[2];
                i[1] = p3[0] - p1[0]; j[1] = p3[1] - p1[1]; k[1] = p3[2] - p1[2];
                i[2] = p3[0] - p2[0]; j[2] = p3[1] - p2[1]; k[2] = p3[2] - p2[2];

                //cross product between two vectors, to determine normal vector
                u[0] = j[0] * k[1] - k[0] * j[1];
                u[1] = k[0] * i[1] - i[0] * k[1];
                u[2] = i[0] * j[1] - j[0] * i[1];

                //Normalize vector to 1 
                double norm = Math.Sqrt(u[0] * u[0] + u[1] * u[1] + u[2] * u[2]);
                if (norm != 0.0)
                {
                    u[0] = u[0] / norm;
                    u[1] = u[1] / norm;
                    u[2] = u[2] / norm;
                }
                else
                {
                    u[0] = 0.0;
                    u[1] = 0.0;
                    u[2] = 0.0;
                }

                //determine max unit normal component...
                absu[0] = Math.Abs(u[0]);
                absu[1] = Math.Abs(u[1]);
                absu[2] = Math.Abs(u[2]);

                //check for cases
                if (absu[0] > absu[1] && absu[0] > absu[2])
                    munc[0]++; //mun = int?
                else if (absu[1] > absu[0] && absu[1] > absu[2])
                    munc[1]++;
                else if (absu[2] > absu[1] && absu[2] > absu[0])
                    munc[2]++;
                else if (absu[0] == absu[1] && absu[1] == absu[2])
                    wxyz++;
                else if (absu[0] == absu[1] && absu[0] > absu[2])
                    wxy++;
                else if (absu[0] == absu[2] && absu[0] > absu[1])
                    wxz++;
                else if (absu[1] == absu[2] && absu[2] > absu[0])
                    wyz++;
                else
                    throw new ArgumentException("Unknown condition");

                //This is reduced to ...

                //area of a triangle...
                double a = Math.Sqrt(i[1] * i[1] + j[1] * j[1] + k[1] * k[1]);
                double b = Math.Sqrt(i[0] * i[0] + j[0] * j[0] + k[0] * k[0]);
                double c = Math.Sqrt(i[2] * i[2] + j[2] * j[2] + k[2] * k[2]);
                double s = 0.5 * (a + b + c);
                double area = Math.Sqrt(Math.Abs(s * (s - a) * (s - b) * (s - c)));
                //patches(count,1) = area
                //
                surfaceArea += area;

                //volume elements ... 
                double zavg = (p1[2] + p2[2] + p3[2]) / 3.0;
                double yavg = (p1[1] + p2[1] + p3[1]) / 3.0;
                double xavg = (p1[0] + p2[0] + p3[0]) / 3.0;

                //calculate volume of current triangle
                t_vol[0] = area * u[0] * xavg;
                t_vol[1] = area * u[1] * yavg;
                t_vol[2] = area * u[2] * zavg;

                //add volume of current triangle to running total
                vol[0] += t_vol[0];
                vol[1] += t_vol[1];
                vol[2] += t_vol[2];

                //sum of function for centroid calculation
                func_sum[0] += t_vol[0] * xavg;
                func_sum[1] += t_vol[1] * yavg;
                func_sum[2] += t_vol[2] * zavg;
            }
            double[] kxyz = new double[3];
            kxyz[0] = (munc[0] + wxyz / 3.0 + ((wxy + wxz) / 2.0)) / _connections.Length;
            kxyz[1] = (munc[1] + wxyz / 3.0 + ((wxy + wyz) / 2.0)) / _connections.Length;
            kxyz[2] = (munc[2] + wxyz / 3.0 + ((wxz + wyz) / 2.0)) / _connections.Length;

            _volume = kxyz[0] * vol[0] + kxyz[1] * vol[1] + kxyz[2] * vol[2];
            _volume = Math.Abs(_volume);

            func_sum[0] /= 2;
            func_sum[1] /= 2;
            func_sum[2] /= 2;

            _centroid = new double[3];
            _centroid[0] = func_sum[0] / _volume;
            _centroid[1] = func_sum[1] / _volume;
            _centroid[2] = func_sum[2] / _volume;
        }

        public void CalculateEigenvalueDecomposition()
        {
            double[] i = new double[3];
            double[] j = new double[3];
            double[] k = new double[3];
            double[] u = new double[3];

            double[] t_vol = new double[3];
            double[] vol = new double[3];
            vol[0] = 0;
            vol[1] = 0;
            vol[2] = 0;

            double[] func_sum = new double[3];
            func_sum[0] = 0;
            func_sum[1] = 0;
            func_sum[2] = 0;

            double[] func_sum_inertia = new double[3];
            func_sum_inertia[0] = 0;
            func_sum_inertia[1] = 0;
            func_sum_inertia[2] = 0;

            double func_sum_xy = 0;
            double func_sum_xz = 0;
            double func_sum_yz = 0;

            double surfaceArea = 0;

            //loop through all the triangles
            for (int count = 0; count < _connections.Length; count++)
            {
                //get the 3 points of the triangle
                double[] p1 = new double[3];
                double[] p2 = new double[3];
                double[] p3 = new double[3];

                //Fix so its relative to the centroid center of mass
                p1[0] = (double)_pts[_connections[count][0]][0] - _centroid[0]; //fix x's first
                p2[0] = (double)_pts[_connections[count][1]][0] - _centroid[0];
                p3[0] = (double)_pts[_connections[count][2]][0] - _centroid[0];

                p1[1] = (double)_pts[_connections[count][0]][1] - _centroid[1]; //fix y's 
                p2[1] = (double)_pts[_connections[count][1]][1] - _centroid[1];
                p3[1] = (double)_pts[_connections[count][2]][1] - _centroid[1];

                p1[2] = (double)_pts[_connections[count][0]][2] - _centroid[2]; //fix z's 
                p2[2] = (double)_pts[_connections[count][1]][2] - _centroid[2];
                p3[2] = (double)_pts[_connections[count][2]][2] - _centroid[2];

                //calculate the i, j, k vectors
                i[0] = p2[0] - p1[0]; j[0] = p2[1] - p1[1]; k[0] = p2[2] - p1[2];
                i[1] = p3[0] - p1[0]; j[1] = p3[1] - p1[1]; k[1] = p3[2] - p1[2];
                i[2] = p3[0] - p2[0]; j[2] = p3[1] - p2[1]; k[2] = p3[2] - p2[2];

                //cross product between two vectors, to determine normal vector
                u[0] = j[0] * k[1] - k[0] * j[1];
                u[1] = k[0] * i[1] - i[0] * k[1];
                u[2] = i[0] * j[1] - j[0] * i[1];

                //Normalize vector to 1 
                double norm = Math.Sqrt(u[0] * u[0] + u[1] * u[1] + u[2] * u[2]);
                if (norm != 0.0)
                {
                    u[0] = u[0] / norm;
                    u[1] = u[1] / norm;
                    u[2] = u[2] / norm;
                }
                else
                {
                    u[0] = 0.0;
                    u[1] = 0.0;
                    u[2] = 0.0;
                }
                
                //This is reduced to ...

                //area of a triangle...
                double a = Math.Sqrt(i[1] * i[1] + j[1] * j[1] + k[1] * k[1]);
                double b = Math.Sqrt(i[0] * i[0] + j[0] * j[0] + k[0] * k[0]);
                double c = Math.Sqrt(i[2] * i[2] + j[2] * j[2] + k[2] * k[2]);
                double s = 0.5 * (a + b + c);
                double area = Math.Sqrt(Math.Abs(s * (s - a) * (s - b) * (s - c)));
                //patches(count,1) = area
                //
                surfaceArea += area;

                //volume elements ... 
                double zavg = (p1[2] + p2[2] + p3[2]) / 3.0;
                double yavg = (p1[1] + p2[1] + p3[1]) / 3.0;
                double xavg = (p1[0] + p2[0] + p3[0]) / 3.0;

                //sum of function for centroid calculation
                func_sum[0] += t_vol[0] * xavg;
                func_sum[1] += t_vol[1] * yavg;
                func_sum[2] += t_vol[2] * zavg;

                //sum of function for inertia calculation
                func_sum_inertia[0] += area * u[0] * xavg * xavg * xavg;
                func_sum_inertia[1] += area * u[1] * yavg * yavg * yavg;
                func_sum_inertia[2] += area * u[2] * zavg * zavg * zavg;

                //sum of function for products of inertia calculation
                func_sum_xz += area * u[0] * xavg * xavg * zavg;
                func_sum_xy += area * u[1] * yavg * yavg * xavg;
                func_sum_yz += area * u[2] * zavg * zavg * yavg;
            }

            func_sum_inertia[0] /= 3;
            func_sum_inertia[1] /= 3;
            func_sum_inertia[2] /= 3;

            double Ixy = -1 * func_sum_xy / 2;
            double Ixz = -1 * func_sum_xz / 2;
            double Iyz = -1 * func_sum_yz / 2;
            double Iyx = Ixy;
            double Izx = Ixz; 
            double Izy = Iyz;

            double Ixx = func_sum_inertia[1] + func_sum_inertia[2];
            double Iyy = func_sum_inertia[0] + func_sum_inertia[2];
            double Izz = func_sum_inertia[0] + func_sum_inertia[1];

            GeneralMatrix i_CoM = new GeneralMatrix(3, 3);
            i_CoM.Array[0][0] = Ixx;
            i_CoM.Array[0][1] = Ixy;
            i_CoM.Array[0][2] = Ixz;

            i_CoM.Array[1][0] = Iyx;
            i_CoM.Array[1][1] = Iyy;
            i_CoM.Array[1][2] = Iyz;

            i_CoM.Array[2][0] = Izx;
            i_CoM.Array[2][1] = Izy;
            i_CoM.Array[2][2] = Izz;

            EigenvalueDecomposition eig = i_CoM.Eigen();
            _eigenvalues = eig.D;
            _eigenvectors = eig.GetV();
            //make sure this is a right handed matrix
            if (_eigenvectors.Determinant() < 0)
                _eigenvectors = _eigenvectors.Multiply(-1);
        }
        #endregion
    }
}
