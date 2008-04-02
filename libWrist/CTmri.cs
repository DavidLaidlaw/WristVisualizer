using System;
using System.IO;
using System.Text;
//using System.Drawing;
//using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace libWrist
{
	/// <summary>
	/// Summary description for CTmri.
	/// </summary>
	public class CTmri
	{
		private const string RESOLUTION = "resolution";
		private const string VSIZE = "vsize";

        private int IMAGE_OFFSET = 0;
        private double IMAGE_SCALE = 8.1568627;

		private int _intensityOffset=0;
		private double _intensityScale=0;
		private int _height;
		private int _width;
		private int _depth;
		private Byte _layers;
		private bool _autoScale=false;
		private double _voxelSizeX;
		private double _voxelSizeY;
		private double _voxelSizeZ;
		private short[] _data;
		private Format _format;
		public enum Format { Sign16, USign16, Sign8, USign8 };
        private short _minIntensity, _maxIntensity;

        //crop settings
        private int _xmin, _xmax, _ymin, _ymax, _zmin, _zmax;


		public CTmri(string mriDirectory):this(mriDirectory,0,0) {}

		public CTmri(string mriDirectory, double voxelSize):this(mriDirectory,voxelSize,0) {}

		public CTmri(string mriDirectory, double voxelSize, double sliceThickness)
		{
			_format = Format.USign16;
			readScale(mriDirectory);
			readDimensions(mriDirectory);

			if (voxelSize>0) _voxelSizeX=_voxelSizeY=voxelSize;
			if (sliceThickness>0) _voxelSizeZ=sliceThickness;


            readDataToShort(mriDirectory);

            IMAGE_OFFSET = _minIntensity;
            IMAGE_SCALE = ((double)_maxIntensity - _minIntensity) / 255;

            //Console.WriteLine("Found: Min: {0}, Max: {1}, Offset: {2}, Scale: {3}",_minIntensity,_maxIntensity,IMAGE_OFFSET, IMAGE_SCALE);

            //readDataToBitmaps(mriDirectory);
			//findInensityScale(_data);
		}

		private void readScale(string mriDirectory)
		{
			string vsizeFile = Path.Combine(mriDirectory,VSIZE);
			if (!File.Exists(vsizeFile)) 
			{
				//no vsize File, so assume size of 1
				_voxelSizeX = _voxelSizeY = _voxelSizeZ = 1;
				return;
			}
			StreamReader r = new StreamReader(vsizeFile);
			string line = r.ReadLine();
			string[] parts = line.Split(' ');
			if (parts.Length<3) throw new ArgumentException("Error: unable to determine resolution of scan");
			_voxelSizeX = Double.Parse(parts[0].Trim());
			_voxelSizeY = Double.Parse(parts[1].Trim());
			_voxelSizeZ = Double.Parse(parts[2].Trim());
			r.Close();
		}

		private void readDimensions(string mriDirectory)
		{
			string resolutionFile = Path.Combine(mriDirectory,RESOLUTION);
			StreamReader r = new StreamReader(resolutionFile);
			string line = r.ReadLine();
			string[] parts = line.Split(' ');
			if (parts.Length<4) throw new ArgumentException("Error: unable to determine dimensions of scan");
			_width = Int32.Parse(parts[0].Trim());
			_height = Int32.Parse(parts[1].Trim());
			_depth = Int32.Parse(parts[2].Trim());
			_layers = Byte.Parse(parts[3].Trim());
			r.Close();
		}

        private void readDataToShort(string mriDirectory)
        {
            _data = new short[_height * _width * _depth];
            _minIntensity = 255;
            _maxIntensity = 0;

            for (int i = 0; i < _depth; i++)
            {
                string sliceFilename = Path.Combine(mriDirectory, "i." + ((int)i + 1).ToString("D3"));
                StreamReader s = new StreamReader(sliceFilename);
                BinaryReader r = new BinaryReader(s.BaseStream);

                //TODO: Handle different data types....
                for (int j = 0; j < _height; j++)
                {
                    for (int k = 0; k < _width; k++)
                    {
                        _data[(i * _width * _height) + (j * _width) + k] = ShortSwap((short)r.ReadUInt16());
                        //_data[(i * _width * _height) + j] = ShortSwap((short)r.ReadUInt16());
                        //save min and max
                        if (_data[(i * _width * _height) + (j * _width) + k] < _minIntensity)
                            _minIntensity = _data[(i * _width * _height) + (j * _width) + k];
                        if (_data[(i * _width * _height) + (j * _width) + k] > _maxIntensity)
                            _maxIntensity = _data[(i * _width * _height) + (j * _width) + k];
                    }
                }

                r.Close();
                s.Close();
            }
            Console.WriteLine("Done reading");
        }

        //public void deleteFrames()
        //{
        //    if (_bitmaps == null) return;
        //    for (int i = 0; i < _bitmaps.Length; i++)
        //    {
        //        if (_bitmaps[i] != null)
        //            _bitmaps[i].Dispose();
        //    }
        //    _bitmaps = null;
        //}

        private short ShortSwap(short x)
		{
			int b1, b2;
			b1 = x & 255;
			b2 = (x >> 8) & 255;
			short d = (short)((b1 << 8) + b2);
			return d;
		}

		private void findInensityScale(short[] data) 
		{
			int min=0, max=0;
			for (int i=0; i<data.Length; i++) 
			{
				if (data[i]<min) min=data[i];
				if (data[i]>max) max=data[i];
			}
			System.Console.WriteLine("Min: "+min.ToString());
			System.Console.WriteLine("Max: "+max.ToString());
			_intensityOffset=-min;
			_intensityScale = (max-min)/255.0;
		}

        public void setCrop(int xmin, int xmax, int ymin, int ymax, int zmin, int zmax)
        {
            _xmin = xmin;
            _xmax = xmax;
            _ymin = ymin;
            _ymax = ymax;
            _zmin = zmin;
            _zmax = zmax;
        }

		#region Accessors for Volume Data

		public int getVoxel_a(int x, int y, int z)
		{
			if (_autoScale) return getVoxel_s(x,y,z);
			else return getVoxel(x,y,z);
		}

		/// <summary>
		/// returns the raw data, no checks, nothing
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public int getVoxel(int x, int y, int z) 
		{
			//y = _height - 1 - y; //flip about the y axis - lets do it when we load
			return _data[z*_width*_height + y*_width + x];
		}

		public int getVoxel_s(int x, int y, int z)
		{
			//if 8bit
			if (_format==Format.Sign8 || _format==Format.USign8) return _data[z*_width*_height + y*_width + x];

			//if (x>= _width || y>=_height || z>=_depth) return 0;
			//int temp = (int)((_data[z*_width*_height + y*_width + x]+1040)/8.1568627);
			int temp = (int)((_data[z*_width*_height + y*_width + x]+_intensityOffset)/_intensityScale);
			return Math.Min(temp,255);
		}

        public short getCroppedVoxel(int x, int y, int slice)
        {
            if (_data == null) return 0;
            int temp = x;
            //x = _ydim - y - _ymin;
            //y = temp + _xmin;
            x = x + _xmin;
            y = y + _ymin;

            //x += _xmin;
            //y += _ymin;
            //y = _ydim - _ymin - y;
            slice += _zmin;
            if (x < 0 || y < 0 || slice < 0 || x >= _width || y >= _height || slice >= _depth) return 0;
            return (short)((_data[slice * _width * _height + y * _width + x] - IMAGE_OFFSET) / IMAGE_SCALE); //scale to correct range
        }

		#endregion

		#region Static Accessors

		public int width 
		{
			get 
			{
				return _width;
			}
		}

		public int height 
		{
			get 
			{
				return _height;
			}
		}

		public int depth 
		{
			get 
			{
				return _depth;
			}
		}

		public Format fileFormat 
		{
			get 
			{
				return _format;
			}
		}

		public double voxelSizeZ
		{
			get
			{
				return _voxelSizeZ;
			}
		}

        public double voxelSizeX
        {
            get { return _voxelSizeX; }
        }

        public double voxelSizeY
        {
            get { return _voxelSizeY; }
        }

		public bool autoScale
		{
			get 
			{
				return _autoScale;
			}
			set
			{
				_autoScale = value;
			}
		}

		#endregion
	}
}
