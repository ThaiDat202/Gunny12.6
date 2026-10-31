using System.Reflection;
using log4net;

namespace Game.Base.Packets
{
	public class GSPacketIn : PacketIn
	{
		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public const ushort HDR_SIZE = 20;

		public const short HEADER = 29099;

		protected short m_code;

		protected int m_cliendId;

		protected int m_parameter1;

		protected int m_parameter2;

		public short Code
		{
			get
			{
				return m_code;
			}
			set
			{
				m_code = value;
			}
		}

		public int ClientID
		{
			get
			{
				return m_cliendId;
			}
			set
			{
				m_cliendId = value;
			}
		}

		public int Parameter1
		{
			get
			{
				return m_parameter1;
			}
			set
			{
				m_parameter1 = value;
			}
		}

		public int Parameter2
		{
			get
			{
				return m_parameter2;
			}
			set
			{
				m_parameter2 = value;
			}
		}

		public GSPacketIn(byte[] buf, int size)
			: base(buf, size)
		{
		}

		public GSPacketIn(short code)
			: this(code, 0, 8192)
		{
		}

		public GSPacketIn(short code, int clientId)
			: this(code, clientId, 8192)
		{
		}

		public GSPacketIn(short code, int clientId, int size)
			: base(new byte[size], 20)
		{
			m_code = code;
			m_cliendId = clientId;
			m_offset = 20;
		}

		public void ReadHeader()
		{
			ReadShort();
			m_length = ReadShort();
			ReadShort();
			m_code = ReadShort();
			m_cliendId = ReadInt();
			m_parameter1 = ReadInt();
			m_parameter2 = ReadInt();
		}

		public void WriteHeader()
		{
			lock (this)
			{
				int offset = m_offset;
				m_offset = 0;
				WriteShort(29099);
				WriteShort((short)m_length);
				m_offset = 6;
				WriteShort(m_code);
				WriteInt(m_cliendId);
				WriteInt(m_parameter1);
				WriteInt(m_parameter2);
				m_offset = 4;
				WriteShort(CheckSum());
				m_offset = offset;
			}
		}

		public short CheckSum()
		{
			short num = 119;
			int num2 = 6;
			while (num2 < m_length)
			{
				num = (short)(num + m_buffer[num2++]);
			}
			return (short)(num & 0x7F7F);
		}

		public void WritePacket(GSPacketIn pkg)
		{
			pkg.WriteHeader();
			Write(pkg.Buffer, 0, pkg.Length);
		}

		public GSPacketIn ReadPacket()
		{
			byte[] array = ReadBytes();
			GSPacketIn gSPacketIn = new GSPacketIn(array, array.Length);
			gSPacketIn.ReadHeader();
			return gSPacketIn;
		}

		public void Compress()
		{
			byte[] array = Marshal.Compress(m_buffer, 20, base.Length - 20);
			m_offset = 20;
			Write(array);
			m_length = array.Length + 20;
		}

		public void UnCompress()
		{
			m_offset = 20;
			byte[] array = Marshal.Uncompress(ReadBytes());
			m_offset = 20;
			Write(array);
			m_length = array.Length + 20;
			m_offset = 20;
		}

		public void ClearContext()
		{
			m_offset = 20;
			m_length = 20;
		}

		public void ClearOffset()
		{
			m_offset = 20;
		}

		public GSPacketIn Clone()
		{
			GSPacketIn gSPacketIn = new GSPacketIn(m_buffer, m_length);
			gSPacketIn.ReadHeader();
			gSPacketIn.Offset = m_length;
			return gSPacketIn;
		}
	}
}
