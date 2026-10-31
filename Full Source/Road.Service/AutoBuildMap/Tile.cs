using System;
using System.Drawing;
using System.IO;

namespace AutoBuildMap;

public class Tile
{
	private byte[] _data;

	private int _width;

	private int _height;

	private Rectangle _rect;

	private int _bw = 0;

	private int _bh = 0;

	private bool _digable;

	public Rectangle Bound => _rect;

	public byte[] Data => _data;

	public int Width => _width;

	public int Height => _height;

	public Tile(byte[] data, int width, int height, bool digable)
	{
		_data = data;
		_width = width;
		_height = height;
		_digable = digable;
		_bw = _width / 8 + 1;
		_bh = _height;
		_rect = new Rectangle(0, 0, _width, _height);
		GC.AddMemoryPressure(data.Length);
	}

	public Tile(Bitmap bitmap, bool digable)
	{
		_width = bitmap.Width;
		_height = bitmap.Height;
		_bw = _width / 8 + 1;
		_bh = _height;
		_data = new byte[_bw * _bh];
		_digable = digable;
		for (int i = 0; i < bitmap.Height; i++)
		{
			for (int j = 0; j < bitmap.Width; j++)
			{
				byte b = ((bitmap.GetPixel(j, i).A > 100) ? ((byte)1) : ((byte)0));
				_data[i * _bw + j / 8] |= (byte)(b << 7 - j % 8);
			}
		}
		_rect = new Rectangle(0, 0, _width, _height);
		GC.AddMemoryPressure(_data.Length);
	}

	public Tile(string file, bool digable)
	{
		FileStream input = File.Open(file, FileMode.Open);
		BinaryReader binaryReader = new BinaryReader(input);
		_width = binaryReader.ReadInt32();
		_height = binaryReader.ReadInt32();
		_bw = _width / 8 + 1;
		_bh = _height;
		_data = binaryReader.ReadBytes(_bw * _bh);
		_digable = digable;
		_rect = new Rectangle(0, 0, _width, _height);
		binaryReader.Close();
		GC.AddMemoryPressure(_data.Length);
	}

	public void Dig(int cx, int cy, Tile surface, Tile border)
	{
		if (_digable && surface != null)
		{
			int x = cx - surface.Width / 2;
			int y = cy - surface.Height / 2;
			Remove(x, y, surface);
			if (border != null)
			{
				x = cx - border.Width / 2;
				y = cy - border.Height / 2;
				Add(x, y, surface);
			}
		}
	}

	protected void Add(int x, int y, Tile tile)
	{
	}

	protected void Remove(int x, int y, Tile tile)
	{
		byte[] data = tile._data;
		Rectangle bound = tile.Bound;
		bound.Offset(x, y);
		bound.Intersect(_rect);
		if (bound.Width == 0 || bound.Height == 0)
		{
			return;
		}
		bound.Offset(-x, -y);
		int num = bound.X / 8;
		int num2 = (bound.X + x) / 8;
		int y2 = bound.Y;
		int num3 = bound.Width / 8 + 1;
		int height = bound.Height;
		if (bound.X == 0)
		{
			if (num3 + num2 < _bw)
			{
				num3++;
				num3 = ((num3 > tile._bw) ? tile._bw : num3);
			}
			int num4 = (bound.X + x) % 8;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			int num9 = 0;
			int num10 = 0;
			for (int i = 0; i < height; i++)
			{
				num7 = 0;
				num8 = 0;
				for (int j = 0; j < num3; j++)
				{
					num5 = (i + y + y2) * _bw + j + num2;
					num6 = (i + y2) * tile._bw + j + num;
					num9 = data[num6];
					num7 = num9 >> num4;
					num10 = _data[num5];
					num10 &= ~(num10 & num7);
					if (num8 != 0)
					{
						num10 &= ~(num10 & num8);
					}
					_data[num5] = (byte)num10;
					num8 = num9 << 8 - num4;
				}
			}
			return;
		}
		int num11 = bound.X % 8;
		int num12 = 0;
		int num13 = 0;
		int num14 = 0;
		int num15 = 0;
		int num16 = 0;
		int num17 = 0;
		for (int k = 0; k < height; k++)
		{
			num14 = 0;
			num15 = 0;
			for (int l = 0; l < num3; l++)
			{
				num12 = (k + y + y2) * _bw + l + num2;
				num13 = (k + y2) * tile._bw + l + num;
				num16 = data[num13];
				num15 = num16 << num11;
				if (l < num3 - 1)
				{
					num16 = data[num13 + 1];
					num14 = num16 >> 8 - num11;
				}
				else
				{
					num14 = 0;
				}
				num17 = _data[num12];
				num17 &= ~(num17 & num15);
				if (num14 != 0)
				{
					num17 &= ~(num17 & num14);
				}
				_data[num12] = (byte)num17;
			}
		}
	}

	public bool IsEmpty(int x, int y)
	{
		if (x >= 0 && x < _width && y >= 0 && y < _height)
		{
			byte b = (byte)(1 << 7 - x % 8);
			return (_data[y * _bw + x / 8] & b) == 0;
		}
		return true;
	}

	public bool IsYLineEmtpy(int x, int y, int h)
	{
		if (x >= 0 && x < _width)
		{
			y = ((y >= 0) ? y : 0);
			h = ((y + h > _height) ? (_height - y) : h);
			for (int i = 0; i < h; i++)
			{
				if (!IsEmpty(x, y + i))
				{
					return false;
				}
			}
			return true;
		}
		return true;
	}

	public bool IsRectangleEmptyQuick(Rectangle rect)
	{
		rect.Intersect(_rect);
		if (IsEmpty(rect.Right, rect.Bottom) && IsEmpty(rect.Left, rect.Bottom) && IsEmpty(rect.Right, rect.Top) && IsEmpty(rect.Left, rect.Top))
		{
			return true;
		}
		return false;
	}

	public Point FindNotEmptyPoint(int x, int y, int h)
	{
		if (x >= 0 && x < _width)
		{
			y = ((y >= 0) ? y : 0);
			h = ((y + h > _height) ? (_height - y) : h);
			for (int i = 0; i < h; i++)
			{
				if (!IsEmpty(x, y + i))
				{
					return new Point(x, y + i);
				}
			}
			return new Point(-1, -1);
		}
		return new Point(-1, -1);
	}

	public Bitmap ToBitmap()
	{
		Bitmap bitmap = new Bitmap(_width, _height);
		for (int i = 0; i < _height; i++)
		{
			for (int j = 0; j < _width; j++)
			{
				if (IsEmpty(j, i))
				{
					bitmap.SetPixel(j, i, Color.FromArgb(0, 0, 0, 0));
				}
				else
				{
					bitmap.SetPixel(j, i, Color.FromArgb(255, 0, 0, 0));
				}
			}
		}
		return bitmap;
	}

	public Tile Clone()
	{
		return new Tile(_data.Clone() as byte[], _width, _height, _digable);
	}

	public void BitmapToTile(string file, bool digable)
	{
		BinaryWriter binaryWriter = new BinaryWriter(File.Open(file, FileMode.Create));
		binaryWriter.Write(_width);
		binaryWriter.Write(_height);
		binaryWriter.Write(_data);
		binaryWriter.Close();
	}
}
