using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
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
        
        private string _mriDirectory;

        private short[] _minIntensity, _maxIntensity;
        private int[] _imageOffset;
        private double[] _imageScale;

		private int _height;
		private int _width;
		private int _depth;
		private int _layers;
		private double _voxelSizeX;
		private double _voxelSizeY;
		private double _voxelSizeZ;
		private short[][] _data;
        private Bitmap[][] _bitmaps;
		private Formats _format;
		public enum Formats { Sign16, USign16, Sign8, USign8 };

        //crop settings
        private int _xmin, _xmax, _ymin, _ymax, _zmin, _zmax;

        public CTmri(string mriDirectory)
		{
			_format = Formats.USign16;  //default value for now. According to dhl, that is the only format that MRI images come in....
            _mriDirectory = mriDirectory;

			readVoxelSize(mriDirectory);
			readDimensions(mriDirectory);

            //setup the data storage locations
            _data = new short[_layers][];
            _bitmaps = new Bitmap[_layers][];
            _minIntensity = new short[_layers];
            _maxIntensity = new short[_layers];
            _imageOffset = new int[_layers];
            _imageScale = new double[_layers];
		}

        public void loadBitmapDataAllLayers()
        {
            for (int i = 0; i < _layers; i++)
                loadBitmapData(i);
        }

        public void loadBitmapData() { loadBitmapData(0); }
        public void loadBitmapData(int echo)
        {
            //this should only work for now when there are NO crop values set
            if (_xmin != 0 || _xmax != 0 || _ymin != 0 || _ymax != 0)
                throw new NotImplementedException("Can not yet load Bitmaps for cropped images....sorry");

            //because we want to scale the data, we first need to get the min/max, etc.
            loadImageData(echo);
            copyDataFromShortToBitmaps(echo);

        }

        public void loadImageData() { loadImageData(0); }
        public void loadImageData(int layer)
        {
            readDataToShort(_mriDirectory, layer);
            calculateOffsetAndScaleFromMinMax(layer);
        }

        private void calculateOffsetAndScaleFromMinMax(int echo)
        {
            _imageOffset[echo] = _minIntensity[echo];
            _imageScale[echo] = 255.0 / ((double)_maxIntensity[echo] - _minIntensity[echo]);
        }

        [Obsolete("Replaced by readVoxelSize(string mriDirectory) for clarity in function name")]
        private void readScale(string mriDirectory) { readVoxelSize(mriDirectory); }

		private void readVoxelSize(string mriDirectory)
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
			_layers = Int32.Parse(parts[3].Trim());
			r.Close();
		}

        private void readDataToShort(string mriDirectory) { readDataToShort(mriDirectory, 0); }
        private void readDataToShort(string mriDirectory, int echo)
        {
            _data[echo] = new short[_height * _width * _depth];
            _minIntensity[echo] = short.MaxValue;
            _maxIntensity[echo] = short.MinValue;

            int startSlice = 0;
            int endSlicePlusOne = _depth; //its a normal for loop, so its 1 more because we don't read the last one
            int startIndex = 0;
            int endIndexPlusOne = _width*_height;

            //check for cropping in Z, so we can read it faster
            if (_xmin > 0 || (_zmax > 0 && _zmax + 1 < _depth))
            {
                startSlice = _zmin;
                endSlicePlusOne = _zmax + 1;
            }
            if (_ymin > 0 || _ymax > 0)
            {
                startIndex = _width * _ymin;
                endIndexPlusOne = _width * (_ymax + 1);
            }

            for (int i = startSlice; i < endSlicePlusOne; i++)
            {
                int fileNumber = i * _layers + echo + 1;
                string sliceFilename = Path.Combine(mriDirectory, "i." + fileNumber.ToString("D3"));
                StreamReader s = new StreamReader(sliceFilename);
                BinaryReader r = new BinaryReader(s.BaseStream);
                                
                //TODO: Handle different data types....
                //want to read in sequentially!!!!

                //need to handle the case when there is some kind of header on the file, offset to then end of the file
                FileInfo info = new FileInfo(sliceFilename);
                long seekOffset = info.Length - (_width * _height * 2);

                //read faster by skipping rows that we don't need, don't know if thats width or height though....
                if (startIndex > 0)
                    seekOffset += startIndex * 2;
                
                r.BaseStream.Seek(seekOffset, SeekOrigin.Begin);

                for (int j = startIndex; j < endIndexPlusOne; j++)
                {
                    _data[echo][(i * _width * _height) + j] = ShortSwap((short)r.ReadUInt16());
                    if (_data[echo][(i * _width * _height) + j] < _minIntensity[echo])
                        _minIntensity[echo] = _data[echo][(i * _width * _height) + j];
                    if (_data[echo][(i * _width * _height) + j] > _maxIntensity[echo])
                        _maxIntensity[echo] = _data[echo][(i * _width * _height) + j];
                }

                r.Close();
                s.Close();
            }
            Console.WriteLine("Done reading");
        }

        private void readDataToBitmap(string mriDirectory, int echo)
        {
            _bitmaps[echo] = new Bitmap[_depth];            
            for (int i = 0; i < _depth; i++)
            {
                int fileNumber = i * _layers + echo + 1;
                string sliceFilename = Path.Combine(mriDirectory, "i." + fileNumber.ToString("D3"));
                StreamReader s = new StreamReader(sliceFilename);
                BinaryReader r = new BinaryReader(s.BaseStream);
                _bitmaps[echo][i] = new Bitmap(_width, _height, PixelFormat.Format32bppArgb);
                Bitmap im = _bitmaps[echo][i];
                Graphics g = Graphics.FromImage(im);
                g.FillRectangle(Brushes.Black, 0, 0, _width, _height);

                BitmapData imdata = im.LockBits(new Rectangle(0, 0, _width, _height), ImageLockMode.ReadWrite, im.PixelFormat);

                unsafe
                {
                    //for (int i=0; i<_depth; i++)
                    for (int j = _height - 1; j >= 0; j--) //special, so we can flip about y 
                    {
                        byte* row = (byte*)imdata.Scan0 + (j * imdata.Stride);
                        for (int k = 0; k < _width; k++)
                        {
                            int intensity = (int)(ShortSwap((short)r.ReadUInt16()) / 16.1569);
                            row[k * 4] = (byte)intensity;
                            row[k * 4 + 1] = (byte)intensity;
                            row[k * 4 + 2] = (byte)intensity;
                        }
                    }
                }

                r.Close();
                s.Close();
                im.UnlockBits(imdata);
            }
        }

        private void copyDataFromShortToBitmaps(int echo)
        {
            if (echo >= _layers || _data[echo] == null)
                throw new ArgumentException("Can not copy image from non existant data later");

            _bitmaps[echo] = new Bitmap[_depth];
            for (int i = 0; i < _depth; i++)
            {
                _bitmaps[echo][i] = new Bitmap(_width, _height, PixelFormat.Format32bppArgb);
                Bitmap im = _bitmaps[echo][i];
                Graphics g = Graphics.FromImage(im);
                g.FillRectangle(Brushes.Black, 0, 0, _width, _height);

                BitmapData imdata = im.LockBits(new Rectangle(0, 0, _width, _height), ImageLockMode.ReadWrite, im.PixelFormat);
                unsafe
                {
                    //for (int j = _height - 1; j >= 0; j--) //special, so we can flip about y 
                    for (int j = 0; j < _height; j++) //special, so we can flip about y 
                    {
                        byte* row = (byte*)imdata.Scan0 + ((_height-j-1) * imdata.Stride);
                        for (int k = 0; k < _width; k++)
                        {
                            byte intensity = (byte)((_data[echo][i * _width * _height + j * _width + k] - _imageOffset[echo]) * _imageScale[echo]);
                            row[k * 4] = intensity;
                            row[k * 4 + 1] = intensity;
                            row[k * 4 + 2] = intensity;
                        }
                    }
                }
                im.UnlockBits(imdata);
            }
        }

        public void deleteFrames()
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

        private short ShortSwap(short x)
		{
			int b1, b2;
			b1 = x & 255;
			b2 = (x >> 8) & 255;
			short d = (short)((b1 << 8) + b2);
			return d;
		}

        public void setCrop(int xmin, int xmax, int ymin, int ymax, int zmin, int zmax)
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

		#region Accessors for Volume Data

        public Byte[][] getCroppedRegionScaledToBytes() { return getCroppedRegionScaledToBytes(0); }
        public Byte[][] getCroppedRegionScaledToBytes(int echo)
        {
            int sizeX = _xmax - _xmin + 1;
            int sizeY = _ymax - _ymin + 1;
            int sizeZ = _zmax - _zmin + 1;
            if (_xmax == 0 && _xmin == 0) //If crop value not set, I don't think I need to check them all...
            {
                sizeX = _width;
                sizeY = _height;
                sizeZ = _depth;
            }
            int offset = _imageOffset[echo];
            double scale = _imageScale[echo];
            //lets build an array of bytes (unsigned 8bit data structure)
            Byte[][] voxels = new Byte[sizeZ][];
            for (int z = 0; z < sizeZ; z++)  // Z coordinate
            {
                voxels[z] = new Byte[sizeX * sizeY];
                for (int y = 0; y < sizeY; y++)
                    for (int x = 0; x < sizeX; x++)
                    {
                        voxels[z][(x * sizeY) + y] = (Byte)((_data[echo][(z+_zmin) * _width * _height + (y+_ymin) * _width + (x+_xmin)] - offset) * scale); //scale to correct range
                    }
            }
            return voxels;
        }

		/// <summary>
		/// returns the raw data, no checks, nothing
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		/// <returns></returns>
		public short getVoxel(int x, int y, int z, int echo) 
		{
			//y = _height - 1 - y; //flip about the y axis - lets do it when we load
			return _data[echo][z*_width*_height + y*_width + x];
		}

		public int getVoxel_s(int x, int y, int z, int echo)
		{
			//if 8bit
			if (_format==Formats.Sign8 || _format==Formats.USign8) return _data[echo][z*_width*_height + y*_width + x];

			//if (x>= _width || y>=_height || z>=_depth) return 0;
			//int temp = (int)((_data[z*_width*_height + y*_width + x]+1040)/8.1568627);
            int temp = (int)((_data[echo][z * _width * _height + y * _width + x] - _imageOffset[echo]) * _imageScale[echo]);
			return Math.Min(temp,255);
		}

        public short getCroppedVoxel(int x, int y, int z, int echo)
        {
            if (_data == null) return 0;
            x += _xmin;
            y += _ymin;
            z += _zmin;
            if (x < 0 || y < 0 || z < 0 || x >= _width || y >= _height || z >= _depth) 
                return 0;
            return (short)((_data[echo][z * _width * _height + y * _width + x] - _imageOffset[echo]) * _imageScale[echo]); //scale to correct range
        }

        public Bitmap getFrame(int frame) { return getFrame(frame, 0); }
        public Bitmap getFrame(int frame, int echo)
        {
            return _bitmaps[echo][frame];
        }

		#endregion

		#region Static Accessors

        public int Cropped_SizeX
        {
            get
            {
                if (_xmax == 0 && _xmin == 0) //If crop value not set, I don't think I need to check them all...
                    return _width;
                else
                    return _xmax - _xmin + 1;
            }
        }

        public int Cropped_SizeY
        {
            get
            {
                if (_xmax == 0 && _xmin == 0) //If crop value not set, I don't think I need to check them all...
                    return _height;
                else
                    return _ymax - _ymin + 1;
            }
        }

        public int Cropped_SizeZ
        {
            get
            {
                if (_xmax == 0 && _xmin == 0) //If crop value not set, I don't think I need to check them all...
                    return _depth;
                else
                    return _zmax - _zmin + 1;
            }
        }

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

        public int Layers
        {
            get { return _layers; }
        }

		public Formats fileFormat 
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
		#endregion
	}
}
