using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using Bussiness.Protocol;
using Center.Server.Statics;
using Game.Base.Packets;
using log4net;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Center.Server
{
	public class CenterService : ICenterService
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private string login_key = ConfigurationManager.AppSettings["LoginKey"];

		private static ServiceHost host;

		public ServerData[] GetServerList(int zoneID)
		{
			ServerInfo[] servers = ServerMgr.Servers;
			List<ServerData> list = new List<ServerData>();
			ServerInfo[] array = servers;
			foreach (ServerInfo serverInfo in array)
			{
				if (serverInfo.AreaId == zoneID)
				{
					list.Add(new ServerData
					{
						Id = serverInfo.ID,
						Name = serverInfo.Name,
						Ip = serverInfo.IP,
						Port = serverInfo.Port,
						State = serverInfo.State,
						MustLevel = serverInfo.MustLevel,
						LowestLevel = serverInfo.LowestLevel,
						Online = serverInfo.Online,
						Total = serverInfo.Total
					});
				}
			}
			return list.ToArray();
		}

		public string GetServerListByStr(int zoneID)
		{
			ServerInfo[] servers = ServerMgr.Servers;
			List<ServerInfo> list = new List<ServerInfo>();
			ServerInfo[] array = servers;
			foreach (ServerInfo serverInfo in array)
			{
				if (serverInfo.AreaId == zoneID)
				{
					list.Add(serverInfo);
				}
			}
			return CompressString(JsonConvert.SerializeObject(list));
		}

		public string GetAllChat()
		{
			return CompressString(JsonConvert.SerializeObject(ChatMrg.GetAllChat()));
		}

		public string CompressString(string text)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			MemoryStream memoryStream = new MemoryStream();
			using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, leaveOpen: true))
			{
				gZipStream.Write(bytes, 0, bytes.Length);
			}
			memoryStream.Position = 0L;
			byte[] array = new byte[memoryStream.Length];
			memoryStream.Read(array, 0, array.Length);
			byte[] array2 = new byte[array.Length + 4];
			Buffer.BlockCopy(array, 0, array2, 4, array.Length);
			Buffer.BlockCopy(BitConverter.GetBytes(bytes.Length), 0, array2, 0, 4);
			return Convert.ToBase64String(array2);
		}

		public bool ChargeMoney(int userID, int zoneId, string chargeID)
		{
			ZoneLoginMgr zoneLoginByAreaID = ServerMgr.GetZoneLoginByAreaID(zoneId);
			if (zoneLoginByAreaID != null)
			{
				ServerClient serverClient = zoneLoginByAreaID.GetServerClient(userID);
				if (serverClient != null)
				{
					serverClient.SendChargeMoney(userID, chargeID);
					return true;
				}
			}
			return false;
		}

		public bool SystemNotice(string msg)
		{
			try
			{
				string[] array = msg.Split('|');
				if (array.Length != 3 || array[2] != md5(array[0] + login_key))
				{
					return false;
				}
				CenterServer.Instance.SendSystemNotice(array[0], int.Parse(array[1]));
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool Reload(string type)
		{
			try
			{
				string[] array = type.Split('|');
				return array.Length == 2 && !(array[1] != md5(array[0] + login_key)) && CenterServer.Instance.SendReload((eReloadType)Enum.Parse(typeof(eReloadType), array[0], ignoreCase: true));
			}
			catch
			{
				return false;
			}
		}

		public bool Shutdown(string type)
		{
			try
			{
				string[] array = type.Split('|');
				return array.Length == 2 && !(array[1] != md5(array[0] + login_key)) && CenterServer.Instance.SendShutdown(int.Parse(array[0]));
			}
			catch
			{
				return false;
			}
		}

		public bool LoginDevice(int zoneId, string content)
		{
			try
			{
				string[] array = content.Split('|');
				if (array.Length != 2 || array[1] != md5(array[0] + login_key))
				{
					return false;
				}
				ZoneLoginMgr zoneLoginByAreaID = ServerMgr.GetZoneLoginByAreaID(zoneId);
				if (zoneLoginByAreaID != null)
				{
					Player playerByName = zoneLoginByAreaID.GetPlayerByName(array[0]);
					if (playerByName != null)
					{
						playerByName.IsLoginDevice = true;
						return true;
					}
				}
				return false;
			}
			catch
			{
				return false;
			}
		}

		public static string md5(string data)
		{
			return BitConverter.ToString(EncryptData(data)).Replace("-", "").ToLower();
		}

		private static byte[] EncryptData(string data)
		{
			return new MD5CryptoServiceProvider().ComputeHash(new UTF8Encoding().GetBytes(data));
		}

		public bool KitoffUser(int playerID, int zoneId, string msg)
		{
			try
			{
				ZoneLoginMgr zoneLoginByAreaID = ServerMgr.GetZoneLoginByAreaID(zoneId);
				if (zoneLoginByAreaID != null)
				{
					ServerClient serverClient = zoneLoginByAreaID.GetServerClient(playerID);
					if (serverClient != null)
					{
						msg = (string.IsNullOrEmpty(msg) ? "You are kicking out by GM!" : msg);
						serverClient.SendKitoffUser(playerID, msg);
						zoneLoginByAreaID.RemovePlayer(playerID);
						return true;
					}
				}
			}
			catch
			{
			}
			return false;
		}

		public bool ReLoadServerList()
		{
			return ServerMgr.ReLoadServerList();
		}

		public bool MailNotice(int playerID, int zoneId)
		{
			try
			{
				ZoneLoginMgr zoneLoginByAreaID = ServerMgr.GetZoneLoginByAreaID(zoneId);
				if (zoneLoginByAreaID != null)
				{
					ServerClient serverClient = zoneLoginByAreaID.GetServerClient(playerID);
					if (serverClient != null)
					{
						GSPacketIn gSPacketIn = new GSPacketIn(117);
						gSPacketIn.WriteInt(playerID);
						gSPacketIn.WriteInt(1);
						serverClient.SendTCP(gSPacketIn);
						return true;
					}
				}
			}
			catch
			{
			}
			return false;
		}

		public bool AASUpdateState(bool state)
		{
			try
			{
				return CenterServer.Instance.SendAAS(state);
			}
			catch
			{
			}
			return false;
		}

		public int AASGetState()
		{
			try
			{
				return CenterServer.Instance.ASSState ? 1 : 0;
			}
			catch
			{
			}
			return 2;
		}

		public int GetPlayerState(int playerID, int zoneId)
		{
			try
			{
				ZoneLoginMgr zoneLoginByAreaID = ServerMgr.GetZoneLoginByAreaID(zoneId);
				if (zoneLoginByAreaID == null)
				{
					return 0;
				}
				return zoneLoginByAreaID.GetPlayer(playerID)?.Id ?? (-1);
			}
			catch
			{
			}
			return -2;
		}

		public int ExperienceRateUpdate(int serverId)
		{
			try
			{
				return CenterServer.Instance.RateUpdate(serverId);
			}
			catch
			{
			}
			return 2;
		}

		public int NoticeServerUpdate(int serverId, int type)
		{
			try
			{
				return CenterServer.Instance.NoticeServerUpdate(serverId, type);
			}
			catch
			{
			}
			return 2;
		}

		public bool UpdateConfigState(int type, bool state)
		{
			try
			{
				return CenterServer.Instance.SendConfigState(type, state);
			}
			catch
			{
			}
			return false;
		}

		public int GetConfigState(int type)
		{
			try
			{
				switch (type)
				{
				case 1:
					return CenterServer.Instance.ASSState ? 1 : 0;
				case 2:
					return CenterServer.Instance.DailyAwardState ? 1 : 0;
				}
			}
			catch
			{
			}
			return 2;
		}

		public bool ActivePlayer(bool isActive)
		{
			try
			{
				if (isActive)
				{
					LogMgr.AddRegCount();
					return true;
				}
			}
			catch
			{
			}
			return false;
		}

		public bool CreatePlayer(int id, string name, string password, int zoneId, bool isFirst)
		{
			try
			{
				ServerMgr.GetZoneLoginByAreaID(zoneId)?.CreatePlayer(new Player
				{
					Id = id,
					Name = name,
					Password = password,
					IsFirst = isFirst,
					ZoneId = zoneId,
					IsLoginDevice = false
				});
				return true;
			}
			catch
			{
			}
			return false;
		}

		public bool ValidateLoginAndGetID(string name, string password, int zoneId, ref int userID, ref bool isFirst)
		{
			try
			{
				ZoneLoginMgr zoneLoginByAreaID = ServerMgr.GetZoneLoginByAreaID(zoneId);
				if (zoneLoginByAreaID != null)
				{
					Player[] allPlayer = zoneLoginByAreaID.GetAllPlayer();
					if (allPlayer != null)
					{
						Player[] array = allPlayer;
						foreach (Player player in array)
						{
							if (player.Name == name && player.Password == password)
							{
								userID = player.Id;
								isFirst = player.IsFirst;
								return true;
							}
						}
					}
				}
			}
			catch
			{
			}
			return false;
		}

		public static bool Start()
		{
			try
			{
				host = new ServiceHost(typeof(CenterService));
				host.Open();
				log.Info("Center Service started!");
				return true;
			}
			catch (Exception arg)
			{
				log.ErrorFormat("Start center server failed:{0}", arg);
				return false;
			}
		}

		public static void Stop()
		{
			try
			{
				if (host != null)
				{
					host.Close();
					host = null;
				}
			}
			catch
			{
			}
		}
	}
}
