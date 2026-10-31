using System;
using System.Text;

namespace Game.Base
{
	public class PacketIn
	{
		protected byte[] m_buffer;

		protected int m_length;

		protected int m_offset;

		public volatile bool IsSended = true;

		public volatile int MSended = 0;

		public volatile int PacketNum = 0;

		public byte[] Buffer => m_buffer;

		public int Length => m_length;

		public int Offset
		{
			get
			{
				return m_offset;
			}
			set
			{
				m_offset = value;
			}
		}

		public int DataLeft => m_length - m_offset;

		public PacketIn(byte[] buf, int len)
		{
			m_buffer = buf;
			m_length = len;
			m_offset = 0;
		}

		public void Skip(int num)
		{
			m_offset += num;
		}

		public virtual bool ReadBoolean()
		{
			return m_buffer[m_offset++] != 0;
		}

		public virtual byte ReadByte()
		{
			return m_buffer[m_offset++];
		}

		public virtual short ReadShort()
		{
			byte v = ReadByte();
			byte v2 = ReadByte();
			return Marshal.ConvertToInt16(v, v2);
		}

		public virtual short ReadShortLowEndian()
		{
			byte v = ReadByte();
			byte v2 = ReadByte();
			return Marshal.ConvertToInt16(v2, v);
		}

		public virtual int ReadInt()
		{
			byte v = ReadByte();
			byte v2 = ReadByte();
			byte v3 = ReadByte();
			byte v4 = ReadByte();
			return Marshal.ConvertToInt32(v, v2, v3, v4);
		}

		public virtual uint ReadUInt()
		{
			byte v = ReadByte();
			byte v2 = ReadByte();
			byte v3 = ReadByte();
			byte v4 = ReadByte();
			return Marshal.ConvertToUInt32(v, v2, v3, v4);
		}

		public virtual long ReadLong()
		{
			int num = ReadInt();
			long num2 = ReadUnsignedInt();
			int num3 = 1;
			if (num < 0)
			{
				num3 = -1;
			}
			return (long)((double)num3 * (Math.Abs((double)num * Math.Pow(2.0, 32.0)) + (double)num2));
		}

		public virtual long ReadUnsignedInt()
		{
			return ReadInt() & 0xFFFFFFFFu;
		}

		public virtual float ReadFloat()
		{
			byte[] array = new byte[4];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = ReadByte();
			}
			return BitConverter.ToSingle(array, 0);
		}

		public virtual double ReadDouble()
		{
			byte[] array = new byte[8];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = ReadByte();
			}
			return BitConverter.ToDouble(array, 0);
		}

		public virtual string ReadString()
		{
			short num = ReadShort();
			string @string = Encoding.UTF8.GetString(m_buffer, m_offset, num);
			m_offset += num;
			return @string.Replace("\0", "");
		}

		public virtual byte[] ReadBytes(int maxLen)
		{
			byte[] array = new byte[maxLen];
			Array.Copy(m_buffer, m_offset, array, 0, maxLen);
			m_offset += maxLen;
			return array;
		}

		public virtual byte[] ReadBytes()
		{
			return ReadBytes(m_length - m_offset);
		}

		public DateTime ReadDateTime()
		{
			return new DateTime(ReadShort(), ReadByte(), ReadByte(), ReadByte(), ReadByte(), ReadByte());
		}

		public virtual int CopyTo(byte[] dst, int dstOffset, int offset)
		{
			int num = ((m_length - offset < dst.Length - dstOffset) ? (m_length - offset) : (dst.Length - dstOffset));
			if (num > 0)
			{
				System.Buffer.BlockCopy(m_buffer, offset, dst, dstOffset, num);
			}
			return num;
		}

		public virtual int CopyTo(byte[] dst, int dstOffset, int offset, byte[] key, ref byte subkey)
		{
			int num = ((m_length - offset < dst.Length - dstOffset) ? (m_length - offset) : (dst.Length - dstOffset));
			if (num > 0)
			{
				byte[] array = new byte[num];
				for (int i = 0; i < num; i++)
				{
					if (i + offset > 0)
					{
						if (i > 0)
						{
							subkey = array[i - 1];
						}
						key[(i + offset) % 8] = (byte)((key[(i + offset) % 8] + subkey) ^ (i + offset));
						subkey = (byte)((m_buffer[i + offset] ^ key[(i + offset) % 8]) + subkey);
						array[i] = subkey;
					}
					else
					{
						array[0] = (byte)(m_buffer[0] ^ key[0]);
					}
				}
				System.Buffer.BlockCopy(array, 0, dst, dstOffset, num);
			}
			return num;
		}

		public virtual int CopyFrom(byte[] src, int srcOffset, int offset, int count)
		{
			if (count < m_buffer.Length && count - srcOffset < src.Length)
			{
				System.Buffer.BlockCopy(src, srcOffset, m_buffer, offset, count);
				return count;
			}
			return -1;
		}

		public virtual int[] CopyFrom(byte[] src, int srcOffset, int offset, int count, byte[] key)
		{
			int[] result = new int[count];
			for (int i = 0; i < count; i++)
			{
				m_buffer[i] = src[i];
			}
			if (count <= m_buffer.Length && count - srcOffset <= src.Length)
			{
				m_buffer[0] = (byte)(src[srcOffset] ^ key[0]);
				for (int j = 1; j < count; j++)
				{
					key[j % 8] = (byte)((key[j % 8] + src[srcOffset + j - 1]) ^ j);
					m_buffer[j] = (byte)((src[srcOffset + j] - src[srcOffset + j - 1]) ^ key[j % 8]);
				}
			}
			return result;
		}

		public virtual void WriteBoolean(bool val)
		{
			if (m_offset == m_buffer.Length)
			{
				byte[] buffer = m_buffer;
				m_buffer = new byte[m_buffer.Length * 2];
				Array.Copy(buffer, m_buffer, buffer.Length);
			}
			m_buffer[m_offset++] = (byte)(val ? 1 : 0);
			m_length = ((m_offset > m_length) ? m_offset : m_length);
		}

		public virtual void WriteByte(byte val)
		{
			if (m_offset == m_buffer.Length)
			{
				byte[] buffer = m_buffer;
				m_buffer = new byte[m_buffer.Length * 2];
				Array.Copy(buffer, m_buffer, buffer.Length);
			}
			m_buffer[m_offset++] = val;
			m_length = ((m_offset > m_length) ? m_offset : m_length);
		}

		public virtual void Write(byte[] src)
		{
			Write(src, 0, src.Length);
		}

		public virtual void Write(byte[] src, int offset, int len)
		{
			if (m_offset + len >= m_buffer.Length)
			{
				byte[] buffer = m_buffer;
				m_buffer = new byte[m_buffer.Length * 2];
				Array.Copy(buffer, m_buffer, buffer.Length);
				Write(src, offset, len);
			}
			else
			{
				Array.Copy(src, offset, m_buffer, m_offset, len);
				m_offset += len;
				m_length = ((m_offset > m_length) ? m_offset : m_length);
			}
		}

		public virtual void WriteShort(short val)
		{
			WriteByte((byte)(val >> 8));
			WriteByte((byte)((uint)val & 0xFFu));
		}

		public virtual void WriteShortLowEndian(short val)
		{
			WriteByte((byte)((uint)val & 0xFFu));
			WriteByte((byte)(val >> 8));
		}

		public virtual void WriteInt(int val)
		{
			WriteByte((byte)(val >> 24));
			WriteByte((byte)((uint)(val >> 16) & 0xFFu));
			WriteByte((byte)((val & 0xFFFF) >> 8));
			WriteByte((byte)((uint)val & 0xFFFFu & 0xFFu));
		}

		public virtual void WriteUInt(uint val)
		{
			WriteByte((byte)(val >> 24));
			WriteByte((byte)((val >> 16) & 0xFFu));
			WriteByte((byte)((val & 0xFFFF) >> 8));
			WriteByte((byte)(val & 0xFFFFu & 0xFFu));
		}

		public virtual void WriteLong(long val)
		{
			uint val2 = (uint)val;
			int val3 = (int)Math.Floor((double)val / Math.Pow(2.0, 32.0));
			WriteInt(val3);
			WriteInt((int)val2);
		}

		public virtual void WriteFloat(float val)
		{
			byte[] bytes = BitConverter.GetBytes(val);
			Write(bytes);
		}

		public virtual void WriteDouble(double val)
		{
			byte[] bytes = BitConverter.GetBytes(val);
			Write(bytes);
		}

		public virtual void Fill(byte val, int num)
		{
			for (int i = 0; i < num; i++)
			{
				WriteByte(val);
			}
		}

		public virtual void WriteString(string str)
		{
			if (!string.IsNullOrEmpty(str))
			{
				byte[] bytes = Encoding.UTF8.GetBytes(str);
				WriteShort((short)(bytes.Length + 1));
				Write(bytes, 0, bytes.Length);
				WriteByte(0);
			}
			else
			{
				WriteShort(1);
				WriteByte(0);
			}
		}

		public virtual void WriteString(string str, int maxlen)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			int num = ((bytes.Length < maxlen) ? bytes.Length : maxlen);
			WriteShort((short)num);
			Write(bytes, 0, num);
		}

		public void WriteDateTime(DateTime date)
		{
			WriteShort((short)date.Year);
			WriteByte((byte)date.Month);
			WriteByte((byte)date.Day);
			WriteByte((byte)date.Hour);
			WriteByte((byte)date.Minute);
			WriteByte((byte)date.Second);
		}
	}
}
