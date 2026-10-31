using System;
using System.Collections.Generic;
using System.Reflection;
using Game.Base.Packets;
using log4net;

namespace Center.Server.Managers
{
	public class EntertamentModeMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private static object m_lock = new object();

		private static object m_send = new object();

		private static bool m_isOpen = false;

		private static DateTime m_timeStart = DateTime.Now;

		private static DateTime m_timeEnd = DateTime.Now;

		private static bool m_isSendItem = false;

		private static Dictionary<int, int> m_players = new Dictionary<int, int>();

		public static void CheckOpenOrClose()
		{
			lock (m_lock)
			{
			}
		}

		public static GSPacketIn SendOpenOrClose(bool sendToAll)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(191);
			gSPacketIn.WriteBoolean(m_isOpen);
			gSPacketIn.WriteDateTime(m_timeStart);
			gSPacketIn.WriteDateTime(m_timeEnd);
			if (sendToAll)
			{
				CenterServer.Instance.SendToALL(gSPacketIn);
			}
			return gSPacketIn;
		}

		public static GSPacketIn SendGetPoint(int id, int point)
		{
			if (FindPlayer(id) == -1)
			{
				AddPlayer(id, point);
			}
			GSPacketIn gSPacketIn = new GSPacketIn(189);
			gSPacketIn.WriteInt(id);
			gSPacketIn.WriteInt(point);
			return gSPacketIn;
		}

		public static void UpdatePoint(int id, int point)
		{
			lock (m_players)
			{
				if (m_players.ContainsKey(id))
				{
					m_players[id] += point;
				}
				else
				{
					m_players.Add(id, point);
				}
			}
		}

		private static int FindPlayer(int userId)
		{
			lock (m_players)
			{
				return m_players.ContainsKey(userId) ? m_players[userId] : (-1);
			}
		}

		private static void AddPlayer(int id, int point)
		{
			lock (m_players)
			{
				if (!m_players.ContainsKey(id))
				{
					m_players.Add(id, point);
				}
			}
		}
	}
}
