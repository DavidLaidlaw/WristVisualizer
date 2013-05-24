using System;
using System.Collections.Generic;
using System.Text;

namespace libWrist
{
    public class HelicalTransform
    {
        double _phi;
        double[] _n = new double[3];
        double _trans;
        double[] _q = new double[3];

        public HelicalTransform() : this(0, new double[] {1, 0, 0}, 0, new double[] {0, 0, 0})
        {
        }

        public HelicalTransform(double phi, double[] n, double trans, double[] q)
        {
            if (n.Length != 3)
                throw new ArgumentException("Error, n must be a unit vector of length 3");
            if (q.Length != 3)
                throw new ArgumentException("Error, q must be a point of length 3");
            _phi = phi;
            _n = n; //do I need to duplicate this array, so we are not linked....I think yes
            _trans = trans;
            _q = q;
        }

        public HelicalTransform(TransformMatrix matrix)
        {
            HelicalTransform ham = matrix.ToHelical();
            _phi = ham.Phi;
            _n = ham.N;
            _trans = ham._trans;
            _q = ham._q;
        }

        public HelicalTransform(HelicalTransform ham)
        {
            _phi = ham.Phi;
            _n = ham.N;
            _trans = ham._trans;
            _q = ham._q;
        }

        public TransformMatrix ToTransformMatrix()
        {
            return new TransformMatrix(this);
        }

        /// <summary>
        /// Will linearly interpolate the current transform into a given number of steps. Just
        /// splits up phi and T
        /// </summary>
        /// <param name="numSteps">Number of steps to divide into, must be >=1, if set to 1 gives no interpilation, just returns a copy of the current</param>
        /// <returns>Array of stepped transforms, and last == this transform, length of array == numSteps</returns>
        public HelicalTransform[] LinearlyInterpolateMotion(int numSteps)
        {
            if (numSteps < 1)
                throw new IndexOutOfRangeException("Can not interpolate to fewer then 1 step");

            HelicalTransform[] steps = new HelicalTransform[numSteps];
            for (int i = 0; i < numSteps; i++)
            {
                steps[i] = new HelicalTransform();
                steps[i]._phi = _phi * (i + 1) / numSteps;
                steps[i]._n = _n;
                steps[i]._trans = _trans * (i + 1) / numSteps;
                steps[i]._q = _q;
            }

            return steps;
        }

        public void AdjustQToLocateHamNearCentroid(double[] centroid)
        {
            //first find the index of the largest N
            int index = (Math.Abs(_n[0]) > Math.Abs(_n[1])) ? 0 : 1;
            index = (Math.Abs(_n[index]) > Math.Abs(_n[2])) ? index : 2;
            double moveFactor = (centroid[index] - _q[index]) / _n[index];
            _q[0] += _n[0] * moveFactor;
            _q[1] += _n[1] * moveFactor;
            _q[2] += _n[2] * moveFactor;
        }

        public string ToStringDirty()
        {
            return String.Format("{0} {1} {2} {3} {4} {5} {6} {7}", _phi, _n[0], _n[1], _n[2], _trans, _q[0], _q[1], _q[2]);
        }

        #region Interfaces
        public double Phi
        {
            get { return _phi; }
            set { _phi = value; }
        }

        public double[] N
        {
            get { return _n; }
            set { _n = value; }
        }
        public double Trans
        {
            get { return _trans; }
            set { _trans = value; }
        }
        public double[] Q
        {
            get { return _q; }
            set { _q = value; }
        }
        #endregion
    }
}
