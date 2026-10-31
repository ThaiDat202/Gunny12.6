using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class DataOutput
{
	private MemoryStream _amfWriter;

	private ObjectEncoding _objectEncoding;

	private Dictionary<object, int> _amf0ObjectReferences;

	private Dictionary<object, int> _objectReferences;

	private Dictionary<object, int> _stringReferences;

	private Dictionary<object, int> _classDefinitionReferences;

	private bool _useLegacyCollection = true;

	private bool _useLegacyThrowable = true;

	public ObjectEncoding ObjectEncoding
	{
		get
		{
			return _objectEncoding;
		}
		set
		{
			_objectEncoding = value;
		}
	}

	public bool UseLegacyCollection
	{
		get
		{
			return _useLegacyCollection;
		}
		set
		{
			_useLegacyCollection = value;
		}
	}

	public bool UseLegacyThrowable
	{
		get
		{
			return _useLegacyThrowable;
		}
		set
		{
			_useLegacyThrowable = value;
		}
	}

	public DataOutput(MemoryStream amfWriter)
	{
		_amfWriter = amfWriter;
		_objectEncoding = ObjectEncoding.AMF3;
		_amf0ObjectReferences = new Dictionary<object, int>(5);
		_objectReferences = new Dictionary<object, int>(5);
		_stringReferences = new Dictionary<object, int>(5);
		_classDefinitionReferences = new Dictionary<object, int>();
	}

	public void dispose()
	{
		if (_amf0ObjectReferences != null)
		{
			_amf0ObjectReferences.Clear();
		}
		if (_objectReferences != null)
		{
			_objectReferences.Clear();
		}
		if (_stringReferences != null)
		{
			_stringReferences.Clear();
		}
		if (_classDefinitionReferences != null)
		{
			_classDefinitionReferences.Clear();
		}
		_amf0ObjectReferences = null;
		_objectReferences = null;
		_stringReferences = null;
		_classDefinitionReferences = null;
		_amfWriter = null;
	}

	public void WriteBoolean(bool value)
	{
		WriteByte((byte)(value ? 3 : 2));
	}

	public void WriteByte(byte value)
	{
		_amfWriter.WriteByte(value);
	}

	public void WriteBytes(byte[] bytes, int offset, int length)
	{
		for (int i = offset; i < offset + length; i++)
		{
			_amfWriter.WriteByte(bytes[i]);
		}
	}

	public void WriteDouble(double value)
	{
		byte[] bytes = BitConverter.GetBytes(value);
		WriteBigEndian(bytes);
	}

	private void WriteBigEndian(byte[] bytes)
	{
		if (bytes != null)
		{
			for (int num = bytes.Length - 1; num >= 0; num--)
			{
				WriteByte(bytes[num]);
			}
		}
	}

	public void WriteFloat(float value)
	{
		byte[] bytes = BitConverter.GetBytes(value);
		WriteBigEndian(bytes);
	}

	public void WriteInt(int value)
	{
		WriteInt32(value);
	}

	public void WriteInt32(int value)
	{
		byte[] bytes = BitConverter.GetBytes(value);
		WriteBigEndian(bytes);
	}

	private void WriteAMF3IntegerData(int value)
	{
		value &= 0x1FFFFFFF;
		if (value < 128)
		{
			WriteByte((byte)value);
		}
		else if (value < 16384)
		{
			WriteByte((byte)(((uint)(value >> 7) & 0x7Fu) | 0x80u));
			WriteByte((byte)((uint)value & 0x7Fu));
		}
		else if (value < 2097152)
		{
			WriteByte((byte)(((uint)(value >> 14) & 0x7Fu) | 0x80u));
			WriteByte((byte)(((uint)(value >> 7) & 0x7Fu) | 0x80u));
			WriteByte((byte)((uint)value & 0x7Fu));
		}
		else
		{
			WriteByte((byte)(((uint)(value >> 22) & 0x7Fu) | 0x80u));
			WriteByte((byte)(((uint)(value >> 15) & 0x7Fu) | 0x80u));
			WriteByte((byte)(((uint)(value >> 8) & 0x7Fu) | 0x80u));
			WriteByte((byte)((uint)value & 0xFFu));
		}
	}

	public void WriteObject(object value)
	{
		WriteAMF3Data(value);
	}

	public void WriteShort(short value)
	{
		byte[] bytes = BitConverter.GetBytes((ushort)value);
		WriteBigEndian(bytes);
	}

	public void WriteUnsignedInt(uint value)
	{
	}

	public void WriteUTF(string value)
	{
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		int byteCount = uTF8Encoding.GetByteCount(value);
		byte[] bytes = uTF8Encoding.GetBytes(value);
		WriteShort((short)byteCount);
		if (bytes.Length != 0)
		{
			Write(bytes);
		}
	}

	public void WriteUTFBytes(string value)
	{
	}

	public void WriteAMF3Bool(bool value)
	{
		WriteByte((byte)(value ? 3 : 2));
	}

	public void WriteAMF3Data(object data)
	{
		if (data == null)
		{
			WriteAMF3Null();
			return;
		}
		Type type = data.GetType();
		if (type == typeof(string))
		{
			WriteAMF3String(data as string);
		}
		else if (type == typeof(bool))
		{
			WriteAMF3Bool((bool)data);
		}
		else if (type == typeof(char))
		{
			WriteAMF3String(new string((char)data, 1));
		}
		else if (type == typeof(byte) || type == typeof(short) || type == typeof(ushort) || type == typeof(int) || type == typeof(uint))
		{
			WriteAMF3Int(Convert.ToInt32(data));
		}
		else if (type == typeof(long) || type == typeof(ulong) || type == typeof(double) || type == typeof(decimal) || type == typeof(float))
		{
			WriteAMF3Double(Convert.ToDouble(data));
		}
		else
		{
			AMF3ObjectWriter(data);
		}
	}

	public void WriteAMF3String(string value)
	{
		WriteByte(6);
		WriteAMF3UTF(value);
	}

	public void WriteAMF3Null()
	{
		WriteByte(1);
	}

	public void WriteAMF3UTF(string value)
	{
		if (value == string.Empty)
		{
			WriteAMF3IntegerData(1);
		}
		else if (!_stringReferences.ContainsKey(value))
		{
			_stringReferences.Add(value, _stringReferences.Count);
			UTF8Encoding uTF8Encoding = new UTF8Encoding();
			int byteCount = uTF8Encoding.GetByteCount(value);
			int num = byteCount;
			num <<= 1;
			num |= 1;
			WriteAMF3IntegerData(num);
			byte[] bytes = uTF8Encoding.GetBytes(value);
			if (bytes.Length != 0)
			{
				Write(bytes);
			}
		}
		else
		{
			int num2 = _stringReferences[value];
			num2 <<= 1;
			WriteAMF3IntegerData(num2);
		}
	}

	public void WriteAMF3Int(int value)
	{
		if (value >= -268435456 && value <= 268435455)
		{
			WriteByte(4);
			WriteAMF3IntegerData(value);
		}
		else
		{
			WriteAMF3Double(value);
		}
	}

	public void WriteAMF3Double(double value)
	{
		WriteByte(5);
		WriteDouble(value);
	}

	public void Write(byte[] buffer)
	{
		if (buffer != null && buffer.Length != 0)
		{
			WriteBytes(buffer, 0, buffer.Length);
		}
	}

	public void WriteAMF3Array(Array value)
	{
		if (!_objectReferences.ContainsKey(value))
		{
			_objectReferences.Add(value, _objectReferences.Count);
			int length = value.Length;
			length <<= 1;
			length |= 1;
			WriteAMF3IntegerData(length);
			WriteAMF3UTF(string.Empty);
			for (int i = 0; i < value.Length; i++)
			{
				WriteAMF3Data(value.GetValue(i));
			}
		}
		else
		{
			int num = _objectReferences[value];
			num <<= 1;
			WriteAMF3IntegerData(num);
		}
	}

	public void WriteAMF3Array(IList value)
	{
		if (!_objectReferences.ContainsKey(value))
		{
			_objectReferences.Add(value, _objectReferences.Count);
			int count = value.Count;
			count <<= 1;
			count |= 1;
			WriteAMF3IntegerData(count);
			WriteAMF3UTF(string.Empty);
			for (int i = 0; i < value.Count; i++)
			{
				WriteAMF3Data(value[i]);
			}
		}
		else
		{
			int num = _objectReferences[value];
			num <<= 1;
			WriteAMF3IntegerData(num);
		}
	}

	public void WriteAMF3AssociativeArray(IDictionary value)
	{
		if (!_objectReferences.ContainsKey(value))
		{
			_objectReferences.Add(value, _objectReferences.Count);
			WriteAMF3IntegerData(1);
			foreach (DictionaryEntry item in value)
			{
				WriteAMF3UTF(item.Key.ToString());
				WriteAMF3Data(item.Value);
			}
			WriteAMF3UTF(string.Empty);
		}
		else
		{
			int num = _objectReferences[value];
			num <<= 1;
			WriteAMF3IntegerData(num);
		}
	}

	public void AMF3ObjectWriter(object data)
	{
		if (data is IList value)
		{
			WriteByte(9);
			WriteAMF3Array(value);
		}
		else if (data is IDictionary dictionary)
		{
			if (dictionary.Count == 0)
			{
				WriteByte(10);
				WriteAMF3Object(data);
			}
			else
			{
				WriteByte(9);
				WriteAMF3AssociativeArray(dictionary);
			}
		}
	}

	public void WriteAMF3Object(object value)
	{
		if (!_objectReferences.ContainsKey(value))
		{
			_objectReferences.Add(value, _objectReferences.Count);
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			if (dictionary == null)
			{
			}
			if (_classDefinitionReferences.ContainsKey(dictionary))
			{
				int num = _classDefinitionReferences[dictionary];
				num <<= 2;
				num |= 1;
				WriteAMF3IntegerData(num);
			}
			else
			{
				_classDefinitionReferences.Add(dictionary, _classDefinitionReferences.Count);
				int count = dictionary.Count;
				count <<= 1;
				count |= 0;
				count <<= 1;
				count |= 0;
				count <<= 2;
				count |= 3;
				WriteAMF3IntegerData(count);
				WriteAMF3UTF(string.Empty);
			}
			bool flag = true;
		}
		else
		{
			int num2 = _objectReferences[value];
			num2 <<= 1;
			WriteAMF3IntegerData(num2);
		}
	}
}
