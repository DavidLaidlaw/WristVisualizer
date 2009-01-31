using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace libWrist
{
    public abstract class CT
    {
        protected int _height = 0;
        protected int _width = 0;
        protected int _depth = 0;
        protected int _layers = 1;
        protected double _voxelSizeX;
        protected double _voxelSizeY;
        protected double _voxelSizeZ;

        protected int[] _imageAutoOffset;
        protected double[] _imageAutoScale;

        
        //crop settings
        protected int _xmin, _xmax, _ymin, _ymax, _zmin, _zmax;

        protected Formats _format;
        public enum Formats { Sign16, USign16, Sign8, USign8 };

        protected Bitmap[][] _bitmaps;

        public virtual void loadBitmapDataAllLayers()
        {
            for (int i = 0; i < _layers; i++)
                loadBitmapData(i);
        }

        public virtual void loadBitmapData() { loadBitmapData(0); }
        public abstract void loadBitmapData(int echo);


        public virtual void loadImageData() { loadImageData(0); }
        public abstract void loadImageData(int layer);

        public virtual void deleteFrames()
        {
            if (_bitmaps == null) return;
            for (int i = 0; i < _bitmaps.Length; i++)
            {
                if (_bitmaps[i] == null) continue;
                for (int j = 0; j < _bitmaps[i].Length; j++)
                {
                    if (_bitmaps[i][j] != null)
                        _bitmaps[i][j].Dispose();
                }
                _bitmaps[i] = null;
            }
            _bitmaps = null;
        }
        

        public virtual Byte[][] getCroppedRegionScaledToBytes() { return getCroppedRegionScaledToBytes(0); }
        public abstract Byte[][] getCroppedRegionScaledToBytes(int echo);

        public abstract ushort getVoxel(int x, int y, int z, int echo);
        public abstract int getVoxel_s(int x, int y, int z, int echo);
        public abstract int getVoxel_as(int x, int y, int z, int echo);

        public virtual short getCroppedVoxel(int x, int y, int z, int echo)
        {
            x += _xmin;
            y += _ymin;
            z += _zmin;
            if (x < 0 || y < 0 || z < 0 || x >= _width || y >= _height || z >= _depth)
                return 0;
            return (short)getVoxel_as(x, y, z, echo);
        }

        public virtual Bitmap getFrame(int frame) { return getFrame(frame, 0); }
        public virtual Bitmap getFrame(int frame, int echo)
        {
            return _bitmaps[echo][frame];
        }

        public static CT SmartLoad(string path)
        {
            if (Directory.Exists(path))
                return new CTmri(path);

            if (File.Exists(path))
                return new CTavw(path);

            throw new ArgumentException(String.Format("File does not exist: '{0}'", path));
        }

        public virtual void setCrop(int xmin, int xmax, int ymin, int ymax, int zmin, int zmax)
        {
            //check for an already cropped image
            if ((_width <= xmax - xmin + 1) && (_height <= ymax - ymin + 1) && (_depth <= zmax - zmin + 1))
                return; //then return, we don't actually want to crop

            _xmin = xmin;
            _xmax = xmax;
            _ymin = ymin;
            _ymax = ymax;
            _zmin = zmin;
            _zmax = zmax; 
        }

        #region Static Accessors

        public virtual int Cropped_SizeX
        {
            get
            {
                if (_xmax == 0 && _xmin == 0) //If crop value not set, I don't think I need to check them all...
                    return _width;
                else
                    return _xmax - _xmin + 1;
            }
        }

        public virtual int Cropped_SizeY
        {
            get
            {
                if (_xmax == 0 && _xmin == 0) //If crop value not set, I don't think I need to check them all...
                    return _height;
                else
                    return _ymax - _ymin + 1;
            }
        }

        public virtual int Cropped_SizeZ
        {
            get
            {
                if (_xmax == 0 && _xmin == 0) //If crop value not set, I don't think I need to check them all...
                    return _depth;
                else
                    return _zmax - _zmin + 1;
            }
        }

        public virtual int width
        {
            get
            {
                return _width;
            }
        }

        public virtual int height
        {
            get
            {
                return _height;
            }
        }

        public virtual int depth
        {
            get
            {
                return _depth;
            }
        }

        public virtual int Layers
        {
            get { return _layers; }
        }

        public virtual Formats fileFormat
        {
            get
            {
                return _format;
            }
        }        

        public virtual double voxelSizeX
        {
            get { return _voxelSizeX; }
        }

        public virtual double voxelSizeY
        {
            get { return _voxelSizeY; }
        }

        public virtual double voxelSizeZ
        {
            get { return _voxelSizeZ; }
        }
        #endregion
    }
}
