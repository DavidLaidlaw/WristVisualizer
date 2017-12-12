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
	public class CTmri : CT
	{
		private const string RESOLUTION = "resolution";
		private const string VSIZE = "vsize";
        private const string PARAMETERS = "parameters";
        private const string COORD_OFFSET = "cooroffset.dat";
        
        private string _mriDirectory;

        /* values set by looking at the min and max values for each echo
         * And then scaling that data to range from 0-255 automatically
         */
        private ushort[] _minIntensity, _maxIntensity;



        private double _scaleIntensity;
        private double _offsetIntensity;
        private int _signedIntensity;

        private double[] _coordOffset;

		private ushort[][] _data;


        public CTmri(string mriDirectory)
		{
			_format = Formats.USign16;  //default value for now. According to dhl, that is the only format that MRI images come in....
            _mriDirectory = mriDirectory;

			readVoxelSize(mriDirectory);
			readDimensions(mriDirectory);
            readParametersFile(mriDirectory);
            readCoordinateOffsetFile(mriDirectory);

            //setup the data storage locations
            _data = new ushort[_layers][];
            _bitmaps = new Bitmap[_layers][];
            _minIntensity = new ushort[_layers];
            _maxIntensity = new ushort[_layers];
            _imageAutoOffset = new int[_layers];
            _imageAutoScale = new double[_layers];
		}

        public override void loadBitmapData(int echo)
        {
            //this should only work for now when there are NO crop values set
            if (_xmin != 0 || _xmax != 0 || _ymin != 0 || _ymax != 0)
                throw new NotImplementedException("Can not yet load Bitmaps for cropped images....sorry");

            //because we want to scale the data, we first need to get the min/max, etc.
            loadImageData(echo);
            copyDataFromShortToBitmaps(echo);

        }

        public override void loadImageData() { loadImageData(0); }
        public override void loadImageData(int layer)
        {
            readDataToShort(_mriDirectory, layer);
            calculateOffsetAndScaleFromMinMax(layer);
        }

        private void calculateOffsetAndScaleFromMinMax(int echo)
        {
            _imageAutoOffset[echo] = _minIntensity[echo];
            _imageAutoScale[echo] = 255.0 / ((double)_maxIntensity[echo] - _minIntensity[echo]);
        }

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

        private void readCoordinateOffsetFile(string mriDirectory)
        {
            string coordFile = Path.Combine(mriDirectory, COORD_OFFSET);
            _coordOffset = new double[3];
            if (!File.Exists(coordFile))
            {
                //no coordoffset.dat File, so assume offset of 0
                for (int i = 0; i < 3; i++)
                    _coordOffset[i] = 0;
                return;
            }

            using (StreamReader r = new StreamReader(coordFile))
            {
                string line = r.ReadLine();
                string[] parts = line.Split(new char[]{' ','\t'});
                if (parts.Length < 3) throw new ArgumentException("Error: unable to determine coordinate offset of scan");
                for (int i = 0; i < 3; i++)
                    _coordOffset[i] = Double.Parse(parts[i].Trim());
                r.Close();
            }
        }

        private void readParametersFile(string mriDirectory)
        {
            string paramFile = Path.Combine(mriDirectory, PARAMETERS);
            if (!File.Exists(paramFile))
            {
                _scaleIntensity = 1;
                _offsetIntensity = 0;
                _signedIntensity = 0;
                return;
            }
            using (StreamReader r = new StreamReader(paramFile))
            {
                while (!r.EndOfStream)
                {
                    string line = r.ReadLine();
                    if (line.StartsWith("//")) 
                        continue;
                    string[] parts = line.Split(' ');
                    if (parts.Length < 2) 
                        continue;
                    switch (parts[0].ToLower().Trim())
                    {

                        case "scaleintensity":
                            _scaleIntensity = Double.Parse(parts[1]);
                            break;
                        case "offsetintensity":
                            _offsetIntensity = Double.Parse(parts[1]);
                            break;
                        case "signedIntensity":
                            _signedIntensity = Int32.Parse(parts[1]);
                            break;
                        case "mrispaceshift":
                            break;
                    }
                }
            }
        }

        private void readDataToShort(string mriDirectory) { readDataToShort(mriDirectory, 0); }
        private void readDataToShort(string mriDirectory, int echo)
        {
            _data[echo] = new ushort[_height * _width * _depth];
            _minIntensity[echo] = ushort.MaxValue;
            _maxIntensity[echo] = ushort.MinValue;

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
                    _data[echo][(i * _width * _height) + j] = ShortSwap((ushort)r.ReadUInt16());
                    if (_data[echo][(i * _width * _height) + j] < _minIntensity[echo])
                        _minIntensity[echo] = _data[echo][(i * _width * _height) + j];
                    if (_data[echo][(i * _width * _height) + j] > _maxIntensity[echo])
                        _maxIntensity[echo] = _data[echo][(i * _width * _height) + j];
                }

                r.Close();
                s.Close();
            }
            //Console.WriteLine("Done reading");
        }

        [Obsolete("This method is no longer used, use the loadBitmapData function")]
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
                    for (int j = 0; j < _height; j++) //special, so we can flip about y 
                    {
                        byte* row = (byte*)imdata.Scan0 + ((_height-j-1) * imdata.Stride);
                        for (int k = 0; k < _width; k++)
                        {
                            byte intensity = (byte)((_data[echo][i * _width * _height + j * _width + k] - _imageAutoOffset[echo]) * _imageAutoScale[echo]);
                            row[k * 4] = intensity;
                            row[k * 4 + 1] = intensity;
                            row[k * 4 + 2] = intensity;
                        }
                    }
                }
                im.UnlockBits(imdata);
            }
        }

        private ushort ShortSwap(ushort x)
        {
            int b1, b2;
            b1 = x & 255;
            b2 = (x >> 8) & 255;
            ushort d = (ushort)((b1 << 8) + b2);
            return d;
        }

        private short ShortSwap(short x)
		{
			int b1, b2;
			b1 = x & 255;
			b2 = (x >> 8) & 255;
			short d = (short)((b1 << 8) + b2);
			return d;
		}

		#region Accessors for Volume Data

        public override Byte[][] getCroppedRegionScaledToBytes(int echo)
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
            int offset = _imageAutoOffset[echo];
            double scale = _imageAutoScale[echo];
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
        public override ushort getVoxel(int x, int y, int z, int echo) 
		{
			//y = _height - 1 - y; //flip about the y axis - lets do it when we load
			return _data[echo][z*_width*_height + y*_width + x];
		}

        public override int getVoxel_s(int x, int y, int z, int echo)
		{
			//if 8bit
			if (_format==Formats.Sign8 || _format==Formats.USign8) return _data[echo][z*_width*_height + y*_width + x];

            return (int)((_data[echo][z * _width * _height + y * _width + x] * _scaleIntensity) - _offsetIntensity);
		}

        public override int getVoxel_as(int x, int y, int z, int echo)
        {
            //if 8bit
            if (_format == Formats.Sign8 || _format == Formats.USign8) return _data[echo][z * _width * _height + y * _width + x];

            int temp = (int)((_data[echo][z * _width * _height + y * _width + x] - _imageAutoOffset[echo]) * _imageAutoScale[echo]);
            return Math.Min(temp, 255);
        }

        public override short getCroppedVoxel(int x, int y, int z, int echo)
        {
            if (_data == null) return 0;
            x += _xmin;
            y += _ymin;
            z += _zmin;
            if (x < 0 || y < 0 || z < 0 || x >= _width || y >= _height || z >= _depth) 
                return 0;
            return (short)((_data[echo][z * _width * _height + y * _width + x] - _imageAutoOffset[echo]) * _imageAutoScale[echo]); //scale to correct range
        }

		#endregion

        #region Advanced Methods from libmri
        public double sample_s_InterpCubit(double x, double y, double z)
        {
            return sample_InterpCubit(x, y, z) * _scaleIntensity + _offsetIntensity;
        }
        public double sample_InterpCubit(double x, double y, double z)
        {
            //int d = 0;
            double back = 0;
            //double inside = 0;

            int xi, yi, zi;
            double xf, yf, zf;
            double[] xx = new double[3];
            double[] tc = new double[3];
            double[] wx = new double[4]; //w* == weight for interpolation
            double[] wy = new double[4];
            double[] wz = new double[4];

            //transform x, y, and z into object coordinate space            
            xx[0] = x; xx[1] = y; xx[2] = z;
            //labToBody(Minv, B, xx, tc); //I don't think this does anything....?
            for (int i = 0; i < 3; i++)
                tc[i] = xx[i];

            xi = frac(tc[0], out xf);
            yi = frac(tc[1], out yf);
            zi = frac(tc[2], out zf);

            //check if completely outside
            if (xi <= -3 || xi >= (_width + 1) ||
                yi <= -3 || yi >= (_height + 1) ||
                zi <= -3 || zi >= (_depth + 1))
            {
                return back; //return 0
            }

            for (int i = -1; i < 3; i++)
            {
                wx[i + 1] = cube_filt(i - xf);
                wy[i + 1] = cube_filt(i - yf);
                wz[i + 1] = cube_filt(i - zf);
            }

            double temp = 0;

            //dat = &data[(xi-1) + (yi-1)*xres + (zi-1)*xyres + d*xyzres];

            if (xi <= 0 || xi >= (_width - 3) ||
                yi <= 0 || yi >= (_height - 3) ||
                zi <= 0 || zi >= (_depth - 3))
            {			// partially outside
                //for (int k = -1; k < 3; k++, dat += xyres - 4 * yres)
                for (int k = -1; k < 3; k++)
                {
                    int tk = zi + k;
                    double wk = wz[k + 1];
                    bool kout = (tk < 0 || tk >= _depth); //check if z outside the box
                    //for (j = -1; j < 3; j++, dat += xres - 4)
                    for (int j = -1; j < 3; j++)
                    {
                        int tj = yi + j;
                        double wj = wy[j + 1];
                        double wjk = wj * wk;
                        bool jout = (tj < 0 || tj >= _height); //check if y is outside the box
                        for (int i = -1; i < 3; i++)
                        {
                            int ti = xi + i;
                            double w = wx[i + 1] * wjk;
                            bool iout = (ti < 0 || ti >= _width); //check if x is outside the box
                            if (!iout && !jout && !kout)
                            {
                                temp += getVoxel(ti, tj, tk, 0) * w;
                            }
                        }
                    }
                }
            }
            else
            {			// completely inside
                //for (k = -1; k < 3; k++, dat += xyres - 4 * xres)
                for (int k = -1; k < 3; k++)
                {
                    int tk = zi + k;
                    double wk = wz[k + 1];
                    //for (j = -1; j < 3; j++, dat += xres - 4)
                    for (int j = -1; j < 3; j++)
                    {
                        int tj = yi + j;
                        double wj = wy[j + 1];
                        double wjk = wj * wk;
                        for (int i = -1; i < 3; i++)
                        {
                            int ti = xi + i;
                            double w = wx[i + 1] * wjk;
                            //temp += *dat++ * w;
                            temp += getVoxel(ti, tj, tk, 0) * w;
                        }
                    }
                }
            }
            return temp;
        }

        private static double cube_filt(double x)
        {
            double retval = -1;
            if (x <= -2) retval = 0.0;
            else if (x <= -1)
            {
                x += 2;
                retval = (1 / 6d) * x * x * x;
            }
            else if (x <= 0)
            {
                x += 1;
                retval = (1 / 6d) * (1 + (3 + (3 - 3 * x) * x) * x);
            }
            else if (x <= 1)
            {
                retval = (1 / 6d) * (4 + (-6 + 3 * x) * x * x);
            }
            else if (x <= 2)
            {
                x -= 1;
                retval = (1 / 6d) * (1 + (-3 + (3 - x) * x) * x);
            }
            else retval = 0.0;
            if (retval < 0) return 0;
            else return retval;
        }

        private static int frac(double d, out double f)
        {
            int i = (int)Math.Floor(d);
            f = d - i;
            return i;
        }
        #endregion

        #region Static Accessors

        public double[] CoordinateOffset
        {
            get { return _coordOffset; }
        }

        public override int Cropped_SizeX
        {
            get
            {
                if (_xmax == 0 && _xmin == 0) //If crop value not set, I don't think I need to check them all...
                    return _width;
                else
                    return _xmax - _xmin + 1;
            }
        }

        public override int Cropped_SizeY
        {
            get
            {
                if (_xmax == 0 && _xmin == 0) //If crop value not set, I don't think I need to check them all...
                    return _height;
                else
                    return _ymax - _ymin + 1;
            }
        }

        public override int Cropped_SizeZ
        {
            get
            {
                if (_xmax == 0 && _xmin == 0) //If crop value not set, I don't think I need to check them all...
                    return _depth;
                else
                    return _zmax - _zmin + 1;
            }
        }

        public override int Layers
        {
            get { return _layers; }
        }

		#endregion
	}
}
