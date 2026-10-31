using System.IO;

public class ByteArray
{
	private MemoryStream _memoryStream;

	private DataOutput _dataOutput;

	private DataInput _dataInput;

	private ObjectEncoding _objectEncoding;

	public uint Length => (uint)_memoryStream.Length;

	public uint Position
	{
		get
		{
			return (uint)_memoryStream.Position;
		}
		set
		{
			_memoryStream.Position = value;
		}
	}

	public uint BytesAvailable => Length - Position;

	public ObjectEncoding ObjectEncoding
	{
		get
		{
			return _objectEncoding;
		}
		set
		{
			_objectEncoding = value;
			_dataInput.ObjectEncoding = value;
			_dataOutput.ObjectEncoding = value;
		}
	}

	public MemoryStream MemoryStream => _memoryStream;

	public ByteArray()
	{
		_memoryStream = new MemoryStream();
		_dataOutput = new DataOutput(_memoryStream);
		_dataInput = new DataInput(_memoryStream);
		_objectEncoding = ObjectEncoding.AMF3;
	}

	public ByteArray(MemoryStream ms)
	{
		_memoryStream = ms;
		_dataOutput = new DataOutput(_memoryStream);
		_dataInput = new DataInput(_memoryStream);
		_objectEncoding = ObjectEncoding.AMF3;
	}

	public ByteArray(byte[] buffer)
	{
		_memoryStream = new MemoryStream();
		_memoryStream.Write(buffer, 0, buffer.Length);
		_memoryStream.Position = 0L;
		_dataOutput = new DataOutput(_memoryStream);
		_dataInput = new DataInput(_memoryStream);
		_objectEncoding = ObjectEncoding.AMF3;
	}

	public void dispose()
	{
		if (_dataOutput != null)
		{
			_dataOutput.dispose();
		}
		if (_dataInput != null)
		{
			_dataInput.dispose();
		}
		if (_memoryStream != null)
		{
			_memoryStream.Close();
			_memoryStream.Dispose();
		}
		_dataOutput = null;
		_dataInput = null;
		_memoryStream = null;
	}

	public byte[] GetBuffer()
	{
		return _memoryStream.GetBuffer();
	}

	public byte[] ToArray()
	{
		return _memoryStream.ToArray();
	}

	public bool ReadBoolean()
	{
		return _dataInput.ReadBoolean();
	}

	public byte ReadByte()
	{
		return _dataInput.ReadByte();
	}

	public void ReadBytes(byte[] bytes, uint offset, uint length)
	{
		_dataInput.ReadBytes(bytes, offset, length);
	}

	public void ReadBytes(ByteArray bytes, uint offset, uint length)
	{
		uint position = bytes.Position;
		int num = (int)((length != 0) ? length : BytesAvailable);
		for (int i = 0; i < num; i++)
		{
			bytes._memoryStream.Position = i + offset;
			bytes._memoryStream.WriteByte(ReadByte());
		}
		bytes.Position = position;
	}

	public void ReadBytes(ByteArray bytes)
	{
		ReadBytes(bytes, 0u, 0u);
	}

	public double ReadDouble()
	{
		return _dataInput.ReadDouble();
	}

	public float ReadFloat()
	{
		return _dataInput.ReadFloat();
	}

	public int ReadInt()
	{
		return _dataInput.ReadInt();
	}

	public object ReadObject()
	{
		return _dataInput.ReadObject();
	}

	public short ReadShort()
	{
		return _dataInput.ReadShort();
	}

	public byte ReadUnsignedByte()
	{
		return _dataInput.ReadUnsignedByte();
	}

	public uint ReadUnsignedInt()
	{
		return _dataInput.ReadUnsignedInt();
	}

	public ushort ReadUnsignedShort()
	{
		return _dataInput.ReadUnsignedShort();
	}

	public string ReadUTF()
	{
		return _dataInput.ReadString();
	}

	public string ReadUTFBytes(uint length)
	{
		return _dataInput.ReadUTFBytes(length);
	}

	public void WriteBoolean(bool value)
	{
		_dataOutput.WriteBoolean(value);
	}

	public void WriteByte(byte value)
	{
		_dataOutput.WriteByte(value);
	}

	public void WriteBytes(byte[] bytes, int offset = 0, int length = 0)
	{
		_dataOutput.WriteBytes(bytes, offset, length);
	}

	public void WriteDouble(double value)
	{
		_dataOutput.WriteDouble(value);
	}

	public void WriteFloat(float value)
	{
		_dataOutput.WriteFloat(value);
	}

	public void WriteInt(int value)
	{
		_dataOutput.WriteInt(value);
	}

	public void WriteObject(object value)
	{
		_dataOutput.WriteObject(value);
	}

	public void WriteShort(short value)
	{
		_dataOutput.WriteShort(value);
	}

	public void WriteUnsignedInt(uint value)
	{
		_dataOutput.WriteUnsignedInt(value);
	}

	public void WriteUTF(string value)
	{
		_dataOutput.WriteUTF(value);
	}

	public void WriteUTFBytes(string value)
	{
		_dataOutput.WriteUTFBytes(value);
	}

	public void Compress()
	{
		Compress("zlib");
	}

	public void Deflate()
	{
		Compress("deflate");
	}

	public void Compress(string algorithm)
	{
	}

	public void Inflate()
	{
		Uncompress("deflate");
	}

	public void Uncompress()
	{
		Uncompress("zlib");
	}

	public void Uncompress(string algorithm)
	{
	}

	public override string ToString()
	{
		return "";
	}
}
