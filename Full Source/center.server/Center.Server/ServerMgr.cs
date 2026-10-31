using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bussiness;
using log4net;
using SqlDataProvider.Data;

namespace Center.Server
{
	public class ServerMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private static Dictionary<int, ServerInfo> _list = new Dictionary<int, ServerInfo>();

		private static Dictionary<int, ZoneLoginMgr> _logins = new Dictionary<int, ZoneLoginMgr>();

		private static object _syncStop = new object();

		public static ServerInfo[] Servers => _list.Values.ToArray();

		public static bool Start()
		{
			try
			{
				using (ServiceBussiness serviceBussiness = new ServiceBussiness())
				{
					ServerInfo[] serverList = serviceBussiness.GetServerList();
					foreach (ServerInfo serverInfo in serverList)
					{
						serverInfo.State = 1;
						serverInfo.Online = 0;
						_list.Add(serverInfo.ID, serverInfo);
						if (!_logins.ContainsKey(serverInfo.AreaId))
						{
							ZoneLoginMgr value = new ZoneLoginMgr();
							_logins.Add(serverInfo.AreaId, value);
						}
					}
				}
				log.Info("Load server list from db.");
				return true;
			}
			catch (Exception arg)
			{
				log.ErrorFormat("Load server list from db failed:{0}", arg);
				return false;
			}
		}

		public static Player GetPlayer(int userId, int zoneId)
		{
			return GetZoneLoginByAreaID(zoneId)?.GetPlayer(userId);
		}

		public static ZoneLoginMgr GetZoneLoginByAreaID(int areaId)
		{
			return _logins.ContainsKey(areaId) ? _logins[areaId] : null;
		}

		public static ZoneLoginMgr[] GetAllZoneLogin()
		{
			return _logins.Values.ToArray();
		}

		public static bool ReLoadServerList()
		{
			try
			{
				using (ServiceBussiness serviceBussiness = new ServiceBussiness())
				{
					lock (_syncStop)
					{
						ServerInfo[] serverList = serviceBussiness.GetServerList();
						foreach (ServerInfo serverInfo in serverList)
						{
							if (_list.ContainsKey(serverInfo.ID))
							{
								_list[serverInfo.ID].IP = serverInfo.IP;
								_list[serverInfo.ID].Name = serverInfo.Name;
								_list[serverInfo.ID].Port = serverInfo.Port;
								_list[serverInfo.ID].Room = serverInfo.Room;
								_list[serverInfo.ID].Total = serverInfo.Total;
								_list[serverInfo.ID].MustLevel = serverInfo.MustLevel;
								_list[serverInfo.ID].LowestLevel = serverInfo.LowestLevel;
								_list[serverInfo.ID].Online = serverInfo.Online;
								_list[serverInfo.ID].State = serverInfo.State;
								_list[serverInfo.ID].AreaId = serverInfo.AreaId;
							}
							else
							{
								serverInfo.State = 1;
								serverInfo.Online = 0;
								_list.Add(serverInfo.ID, serverInfo);
							}
							if (!_logins.ContainsKey(serverInfo.AreaId))
							{
								ZoneLoginMgr value = new ZoneLoginMgr();
								_logins.Add(serverInfo.AreaId, value);
							}
						}
					}
				}
				log.Info("ReLoad server list from db.");
				return true;
			}
			catch (Exception arg)
			{
				log.ErrorFormat("ReLoad server list from db failed:{0}", arg);
				return false;
			}
		}

		public static ServerInfo GetServerInfo(int id)
		{
			return _list.ContainsKey(id) ? _list[id] : null;
		}

		public static int GetState(int count, int total)
		{
			if (count >= total)
			{
				return 5;
			}
			return ((double)count > (double)total * 0.5) ? 4 : 2;
		}

		public static void SaveToDatabase()
		{
			try
			{
				using ServiceBussiness serviceBussiness = new ServiceBussiness();
				foreach (ServerInfo value in _list.Values)
				{
					if (value.State != 1)
					{
						value.State = GetState(value.Online, value.Total);
					}
					serviceBussiness.UpdateService(value);
				}
			}
			catch (Exception exception)
			{
				log.Error("Save server state", exception);
			}
		}
	}
}
