using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class DataInput
{
	private MemoryStream _amfReader;

	private ObjectEncoding _objectEncoding;

	private List<object> _objectReferences;

	private List<object> _stringReferences;

	private List<AMFClassDefinition> _classDefinitions;

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

	public DataInput(MemoryStream amfReader)
	{
		_amfReader = amfReader;
		_objectEncoding = ObjectEncoding.AMF3;
		_objectReferences = new List<object>(15);
		_stringReferences = new List<object>(15);
		_classDefinitions = new List<AMFClassDefinition>(2);
	}

	public void dispose()
	{
		if (_objectReferences != null)
		{
			_objectReferences.Clear();
		}
		if (_stringReferences != null)
		{
			_stringReferences.Clear();
		}
		if (_classDefinitions != null)
		{
			_classDefinitions.Clear();
		}
		_objectReferences = null;
		_stringReferences = null;
		_classDefinitions = null;
		_amfReader = null;
	}

	public bool ReadBoolean()
	{
		return _amfReader.ReadByte() == 3;
	}

	public byte ReadByte()
	{
		return (byte)_amfReader.ReadByte();
	}

	public void ReadBytes(byte[] bytes, uint offset, uint length)
	{
		_amfReader.Read(bytes, (int)offset, (int)length);
	}

	public byte[] ReadBytes(uint c)
	{
		byte[] array = new byte[c];
		for (uint num = 0u; num < c; num++)
		{
			array[num] = ReadByte();
		}
		return array;
	}

	public double ReadDouble()
	{
		byte[] array = ReadBytes(8u);
		byte[] array2 = new byte[8];
		int num = 7;
		int num2 = 0;
		while (num >= 0)
		{
			array2[num2] = array[num];
			num--;
			num2++;
		}
		return BitConverter.ToDouble(array2, 0);
	}

	public float ReadFloat()
	{
		byte[] array = ReadBytes(4u);
		byte[] array2 = new byte[4];
		int num = 3;
		int num2 = 0;
		while (num >= 0)
		{
			array2[num2] = array[num];
			num--;
			num2++;
		}
		return BitConverter.ToSingle(array2, 0);
	}

	public int ReadInt()
	{
		return ReadInt32();
	}

	public int ReadInt32()
	{
		byte[] array = ReadBytes(4u);
		return (array[0] << 24) | (array[1] << 16) | (array[2] << 8) | array[3];
	}

	public object ReadObject()
	{
		object obj = null;
		return ReadAMF3Data();
	}

	public short ReadShort()
	{
		return ReadInt16();
	}

	public short ReadInt16()
	{
		byte[] array = ReadBytes(2u);
		return (short)((array[0] << 8) | array[1]);
	}

	public byte ReadUnsignedByte()
	{
		return 0;
	}

	public uint ReadUnsignedInt()
	{
		return 0u;
	}

	public ushort ReadUnsignedShort()
	{
		return 0;
	}

	public string ReadUTF(int length)
	{
		if (length == 0)
		{
			return string.Empty;
		}
		UTF8Encoding uTF8Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
		byte[] array = ReadBytes((uint)length);
		return uTF8Encoding.GetString(array, 0, array.Length);
	}

	public string ReadString()
	{
		int length = ReadUInt16();
		return ReadUTF(length);
	}

	public ushort ReadUInt16()
	{
		byte[] array = ReadBytes(2u);
		return (ushort)((uint)((array[0] & 0xFF) << 8) | (array[1] & 0xFFu));
	}

	public string ReadUTFBytes(uint length)
	{
		return "";
	}

	public object ReadAMF3Data()
	{
		byte typeMarker = ReadByte();
		return ReadAMF3Data(typeMarker);
	}

	public object ReadAMF3Data(byte typeMarker)
	{
		if (typeMarker == 0 || typeMarker == 1)
		{
			return null;
		}
		return typeMarker switch
		{
			2 => false, 
			3 => true, 
			4 => ReadAMF3Int(), 
			5 => ReadDouble(), 
			6 => ReadAMF3String(), 
			8 => ReadAMF3Date(), 
			9 => ReadAMF3Array(), 
			10 => ReadAMF3Object(), 
			_ => null, 
		};
	}

	public int ReadAMF3Int()
	{
		return ReadAMF3IntegerData();
	}

	public int ReadAMF3IntegerData()
	{
		int num = ReadByte();
		if (num < 128)
		{
			return num;
		}
		num = (num & 0x7F) << 7;
		int num2 = ReadByte();
		if (num2 < 128)
		{
			num |= num2;
		}
		else
		{
			num = (num | (num2 & 0x7F)) << 7;
			num2 = ReadByte();
			if (num2 < 128)
			{
				num |= num2;
			}
			else
			{
				num = (num | (num2 & 0x7F)) << 8;
				num2 = ReadByte();
				num |= num2;
			}
		}
		int num3 = 268435456;
		return -(num & num3) | num;
	}

	public string ReadAMF3String()
	{
		int num = ReadAMF3IntegerData();
		bool flag = (num & 1) != 0;
		num >>= 1;
		if (flag)
		{
			int num2 = num;
			if (num2 == 0)
			{
				return string.Empty;
			}
			string text = ReadUTF(num2);
			AddStringReference(text);
			return text;
		}
		return ReadStringReference(num);
	}

	public void AddStringReference(string str)
	{
		_stringReferences.Add(str);
	}

	public string ReadStringReference(int index)
	{
		return _stringReferences[index] as string;
	}

	public object ReadAMF3Array()
	{
		int num = ReadAMF3IntegerData();
		bool flag = (num & 1) != 0;
		num >>= 1;
		if (flag)
		{
			Dictionary<string, object> dictionary = null;
			string text = ReadAMF3String();
			object obj = null;
			while (text != null && text != string.Empty)
			{
				if (dictionary == null)
				{
					dictionary = new Dictionary<string, object>();
					AddAMF3ObjectReference(dictionary);
				}
				obj = ReadAMF3Data();
				dictionary.Add(text.ToString(), obj);
				text = ReadAMF3String();
			}
			if (dictionary == null)
			{
				object[] array = new object[num];
				byte b = 0;
				AddAMF3ObjectReference(array);
				for (int i = 0; i < num; i++)
				{
					b = ReadByte();
					obj = (array[i] = ReadAMF3Data(b));
				}
				return array;
			}
			for (int j = 0; j < num; j++)
			{
				obj = ReadAMF3Data();
				dictionary.Add(j.ToString(), obj);
			}
			return dictionary;
		}
		return ReadAMF3ObjectReference(num);
	}

	public void AddAMF3ObjectReference(object instance)
	{
		_objectReferences.Add(instance);
	}

	public object ReadAMF3ObjectReference(int index)
	{
		return _objectReferences[index];
	}

	public object ReadAMF3Object()
	{
		int num = ReadAMF3IntegerData();
		bool flag = (num & 1) != 0;
		num >>= 1;
		if (!flag)
		{
			return ReadAMF3ObjectReference(num);
		}
		AMFClassDefinition classDefinition = ReadClassDefinition(num);
		return ReadAMF3Object(classDefinition);
	}

	public object ReadAMF3Object(AMFClassDefinition classDefinition)
	{
		Dictionary<string, object> dictionary = null;
		dictionary = (string.IsNullOrEmpty(classDefinition.ClassName) ? ObjectFactory.CreateInstance() : ObjectFactory.CreateInstance(classDefinition.ClassName));
		if (dictionary == null)
		{
			dictionary = ObjectFactory.CreateInstance();
		}
		AddAMF3ObjectReference(dictionary);
		if (!classDefinition.IsExternalizable)
		{
			object obj = null;
			string text = "";
			for (int i = 0; i < classDefinition.MemberCount; i++)
			{
				text = classDefinition.Members[i].Name;
				obj = ReadAMF3Data();
				dictionary.Add(text, obj);
			}
			if (classDefinition.IsDynamic)
			{
				text = ReadAMF3String();
				while (text != null && text != string.Empty)
				{
					obj = ReadAMF3Data();
					dictionary.Add(text, obj);
					text = ReadAMF3String();
				}
			}
		}
		return dictionary;
	}

	public AMFClassDefinition ReadClassDefinition(int handle)
	{
		AMFClassDefinition aMFClassDefinition = null;
		bool flag = (handle & 1) != 0;
		handle >>= 1;
		if (flag)
		{
			string className = ReadAMF3String();
			bool externalizable = (handle & 1) != 0;
			handle >>= 1;
			bool dynamic = (handle & 1) != 0;
			handle >>= 1;
			AMFClassMember[] array = new AMFClassMember[handle];
			string text = "";
			AMFClassMember aMFClassMember = null;
			for (int i = 0; i < handle; i++)
			{
				text = ReadAMF3String();
				aMFClassMember = (array[i] = new AMFClassMember(text, 0, 64, null));
			}
			aMFClassDefinition = new AMFClassDefinition(className, array, externalizable, dynamic);
			AddClassReference(aMFClassDefinition);
		}
		else
		{
			aMFClassDefinition = ReadClassReference(handle);
		}
		return aMFClassDefinition;
	}

	private void AddClassReference(AMFClassDefinition classDefinition)
	{
		_classDefinitions.Add(classDefinition);
	}

	private AMFClassDefinition ReadClassReference(int index)
	{
		return _classDefinitions[index];
	}

	public DateTime ReadAMF3Date()
	{
		int num = ReadAMF3IntegerData();
		bool flag = (num & 1) != 0;
		num >>= 1;
		if (flag)
		{
			double value = ReadDouble();
			DateTime value2 = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(value);
			value2 = DateTime.SpecifyKind(value2, DateTimeKind.Utc);
			AddAMF3ObjectReference(value2);
			return value2;
		}
		return (DateTime)ReadAMF3ObjectReference(num);
	}
}
