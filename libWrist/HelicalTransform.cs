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
            _n = n;
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

        public TransformMatrix ToTransformMatrix()
        {
            return new TransformMatrix(this);
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
