using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;

namespace libWrist
{
	/// <summary>
	/// Summary description for CTavw.
	/// </summary>
	public class CTavw : CT
	{
		private short[] _data;


        public CTavw(string filename)
		{
			//
			// TODO: Add constructor logic here
			//
			readFile(filename);
            _bitmaps = new Bitmap[1][];
		}


		private void readFile(string filename) 
		{			
			FileInfo info = new FileInfo(filename);
			long length = info.Length;

            using (FileStream reader = info.OpenRead())
            {
                readHeader(reader);
                long dataSize = 0;
                if (_format == Formats.Sign16 || _format == Formats.USign16) dataSize = _height * _width * _depth * 2;
                else if (_format == Formats.Sign8 || _format == Formats.USign8) dataSize = _height * _width * _depth;
                reader.Position = length - dataSize; //set to after header;
                _data = new short[_height * _width * _depth];
                readData(reader, _data);
            }

			findInensityScale(_data);
		}



		private void readData(Stream stream, short[] data)
		{
			BinaryReader r = new BinaryReader(stream);
			switch (_format) 
			{
				case Formats.Sign16:
					for (int i=0; i<_depth; i++)
						for (int j=_height-1; j>=0; j--)  //special, so we can flip about y
							for (int k=0; k<_width; k++)
								data[i*_width*_height + j*_width + k]=r.ReadInt16();	
					break;
				case Formats.USign16:
					for (int i=0; i<_depth; i++)
						for (int j=_height-1; j>=0; j--)  //special, so we can flip about y
							for (int k=0; k<_width; k++)
								data[i*_width*_height + j*_width + k]=(short)r.ReadUInt16();
					break;
				case Formats.Sign8:
					for (int i=0; i<_depth; i++)
						for (int j=_height-1; j>=0; j--)  //special, so we can flip about y
							for (int k=0; k<_width; k++)
								data[i*_width*_height + j*_width + k]=(short)r.ReadSByte();
					break;
				case Formats.USign8:
					for (int i=0; i<_depth; i++)
						for (int j=_height-1; j>=0; j--)  //special, so we can flip about y
							for (int k=0; k<_width; k++)
								data[i*_width*_height + j*_width + k]=(short)r.ReadByte();
					break;
			}

			
		}

		private void findInensityScale(short[] data) 
		{
			int min=0, max=0;
			for (int i=0; i<data.Length; i++) 
			{
				if (data[i]<min) min=data[i];
				if (data[i]>max) max=data[i];
			}

            _imageAutoOffset = new int[1];
            _imageAutoScale = new double[1];

            _imageAutoOffset[0] = min;
            _imageAutoScale[0] = 255.0 / (max - min);
		}


		private void readHeader(Stream stream) 
		{
			char[] header = new char[40001];
			byte[] headerByte = new byte[40001];
			int res = Read(stream,headerByte);
			Encoding.ASCII.GetChars(headerByte,0,res,header,0);
			string head = new string(header);

			
			//get voxel
            Regex subject = new Regex(@"VoxelWidth=([.]|[0-9][0-9]*[.]*[0-9]+)");
            Match m = subject.Match(head);
            if (!m.Success) throw new Exception("Can't find VoxelWidth info");
            _voxelSizeX = Double.Parse(m.Groups[1].ToString());

			subject = new Regex(@"VoxelHeight=([.]|[0-9][0-9]*[.]*[0-9]+)");
			m = subject.Match(head);
			if (!m.Success) throw new Exception("Can't find VoxelHeight info");
			_voxelSizeY= Double.Parse(m.Groups[1].ToString());
            
            subject = new Regex(@"SliceLocation0001=(-?[0-9]+[.]?[0-9]*)");
			m = subject.Match(head);
			if (!m.Success) throw new Exception("Can't find slice location info");
			double loc1 = Double.Parse(m.Groups[1].ToString());

            subject = new Regex(@"SliceLocation0002=(-?[0-9]+[.]?[0-9]*)");
            m = subject.Match(head);
            if (m.Success)
            {
                double loc2 = Double.Parse(m.Groups[1].ToString());
                _voxelSizeZ = Math.Abs(loc1 - loc2);
            }
            else
            {
                //fallback on the slice thickness value....
                subject = new Regex(@"VoxelDepth=([0-9\.]+)");
                m = subject.Match(head);
                if (!m.Success) throw new Exception("Can't find any slice thickness info");
                _voxelSizeZ = Double.Parse(m.Groups[1].ToString());
            }

			//get width
			subject = new Regex(@"Width=([0-9]+)");
			m = subject.Match(head);
			if (!m.Success) throw new Exception("Can't find Width info");
			_width = Int32.Parse(m.Groups[1].ToString());

			//get height
			subject = new Regex(@"Height=([0-9]+)");
			m = subject.Match(head);
			if (!m.Success) throw new Exception("Can't find Width info");
			_height = Int32.Parse(m.Groups[1].ToString());

			//get depth
			subject = new Regex(@"Depth=([0-9]+)");
			m = subject.Match(head);
			if (!m.Success) throw new Exception("Can't find Width info");
			_depth = Int32.Parse(m.Groups[1].ToString());

			//get format
			subject = new Regex(@"DataType=([a-zA-Z_]+)");
			m = subject.Match(head);
			if (!m.Success) throw new Exception("Can't find Width info");
			switch (m.Groups[1].ToString()) 
			{
				case "AVW_SIGNED_SHORT":
					_format = Formats.Sign16;
					break;
				case "AVW_UNSIGNED_SHORT":
					_format = Formats.USign16;
					break;
				case "AVW_SIGNED_CHAR":
					_format = Formats.Sign8;
					break;
				case "AVW_UNSIGNED_CHAR":
					_format = Formats.USign8;
					break;
				default:
					throw new Exception("Unknown format type: "+m.Groups[1].ToString());
			}

		}


		/// <summary>
		/// Reads data from a stream until the end is reached. The
		/// data is returned as a byte array. An IOException is
		/// thrown if any of the underlying IO calls fail.
		/// </summary>
		/// <param name="stream">The stream to read data from</param>
		/// <param name="buffer">The buffer to place read value into, will read to size of buffer or EOF</param>
		private int Read (Stream stream, byte[] buffer)
		{
   
			int read=0;
			int chunk;
			while ( (chunk = stream.Read(buffer, read, buffer.Length-read)) > 0)
			{
				read += chunk;

				//if we reached our size
				if (read==buffer.Length) return read;
				
			}
			//should be EOF if we are here
			return read;
		}

        private void copyDataFromShortToBitmaps(int echo)
        {
            if (echo >= _layers || _data == null)
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
                        byte* row = (byte*)imdata.Scan0 + ((_height - j - 1) * imdata.Stride);
                        for (int k = 0; k < _width; k++)
                        {
                            byte intensity;
                            if (_format == Formats.Sign8 || _format == Formats.USign8)
                                intensity = (byte)(_data[i * _width * _height + j * _width + k]);
                            else
                                intensity = (byte)((_data[i * _width * _height + j * _width + k] - _imageAutoOffset[echo]) * _imageAutoScale[echo]);
                            row[k * 4] = intensity;
                            row[k * 4 + 1] = intensity;
                            row[k * 4 + 2] = intensity;
                        }
                    }
                }
                im.UnlockBits(imdata);
            }
        }

        public override Byte[][] getCroppedRegionScaledToBytes(int echo)
        {
            if (echo >= _layers || _data == null)
                throw new ArgumentException("Can not get image from non existant data later");

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
                        if (_format == Formats.Sign8 || _format == Formats.USign8)
                            voxels[z][(x * sizeY) + y] = (Byte)(_data[(z + _zmin) * _width * _height + (y + _ymin) * _width + (x + _xmin)]);
                        else
                            voxels[z][(x * sizeY) + y] = (Byte)((_data[(z + _zmin) * _width * _height + (y + _ymin) * _width + (x + _xmin)] - offset) * scale); //scale to correct range
                    }
            }
            return voxels;
        }

        public override void loadBitmapData(int echo)
        {
            copyDataFromShortToBitmaps(echo);
        }

        public override void loadImageData(int layer) { } //nothing to do, we loaded to begin with :)


        /// <summary>
        /// returns the raw data, no checks, nothing
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public override ushort getVoxel(int x, int y, int z, int echo)
        {
            return (ushort)_data[z * _width * _height + y * _width + x];
        }

        public override int getVoxel_s(int x, int y, int z, int echo) { return getVoxel_as(x, y, z, echo); }
        public override int getVoxel_as(int x, int y, int z, int echo)
        {
            //if 8bit
            if (_format == Formats.Sign8 || _format == Formats.USign8) return _data[z * _width * _height + y * _width + x];

            int temp = (int)((_data[z * _width * _height + y * _width + x] - _imageAutoOffset[0]) * _imageAutoScale[0]);
            return Math.Min(temp, 255);
        }
	}
}
