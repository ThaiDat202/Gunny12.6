using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.ConsortiaDomain;

public class ConsortiaDomainMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private object m_object = new object();

	private Dictionary<int, UserConsortiaDomainInfo> m_listPlayer = new Dictionary<int, UserConsortiaDomainInfo>();

	private Dictionary<int, ConsortiaDomainInfo> m_listDomain = new Dictionary<int, ConsortiaDomainInfo>();

	private readonly List<Point> DefaultSpawPoint = new List<Point>
	{
		new Point(1609, 587),
		new Point(1155, 968),
		new Point(1319, 1289),
		new Point(1570, 1428),
		new Point(1864, 776),
		new Point(1991, 1150),
		new Point(1868, 1317)
	};

	private ThreadSafeRandom m_random;

	public ConsortiaDomainMgr()
	{
		m_listPlayer = new Dictionary<int, UserConsortiaDomainInfo>();
		m_listDomain = new Dictionary<int, ConsortiaDomainInfo>();
		m_random = new ThreadSafeRandom();
		LoadAllConsortiaDomain();
	}

	private void LoadAllConsortiaDomain()
	{
		m_listDomain.Clear();
		using ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness();
		ConsortiaDomainInfo[] consortiaDomainAll = consortiaBussiness.GetConsortiaDomainAll();
		ConsortiaDomainInfo[] array = consortiaDomainAll;
		foreach (ConsortiaDomainInfo consortiaDomainInfo in array)
		{
			if (!m_listDomain.ContainsKey(consortiaDomainInfo.ConsortiaID))
			{
				m_listDomain[consortiaDomainInfo.ConsortiaID] = consortiaDomainInfo;
			}
		}
	}

	public ConsortiaDomainInfo GetConsortiaDomain(int consortiaId)
	{
		ConsortiaDomainInfo result = null;
		lock (m_listDomain)
		{
			if (m_listDomain.ContainsKey(consortiaId))
			{
				result = m_listDomain[consortiaId];
			}
		}
		return result;
	}

	public bool AddConsortiaDomain(ConsortiaDomainInfo consortia)
	{
		bool flag = false;
		lock (m_listDomain)
		{
			if (!m_listDomain.ContainsKey(consortia.ConsortiaID))
			{
				m_listDomain.Add(consortia.ConsortiaID, consortia);
				flag = true;
			}
		}
		if (flag)
		{
			using ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness();
			consortiaBussiness.AddConsortiaDomain(consortia);
		}
		return flag;
	}

	public List<UserConsortiaDomainInfo> GetAllPlayerDomain(int consortiaId)
	{
		return m_listPlayer.Values.Where((UserConsortiaDomainInfo a) => a.ConsortiaID == consortiaId).ToList();
	}

	public UserConsortiaDomainInfo GetPlayer(int userid)
	{
		UserConsortiaDomainInfo result = null;
		lock (m_listPlayer)
		{
			if (m_listPlayer.ContainsKey(userid))
			{
				result = m_listPlayer[userid];
			}
		}
		return result;
	}

	public bool AddPlayer(GamePlayer player)
	{
		bool flag = false;
		UserConsortiaDomainInfo userConsortiaDomainInfo = null;
		lock (m_listPlayer)
		{
			if (!m_listPlayer.ContainsKey(player.PlayerId))
			{
				userConsortiaDomainInfo = new UserConsortiaDomainInfo();
				userConsortiaDomainInfo.UserID = player.PlayerId;
				userConsortiaDomainInfo.ConsortiaID = player.PlayerCharacter.ConsortiaID;
				userConsortiaDomainInfo.Postion = DefaultSpawPoint[m_random.Next(DefaultSpawPoint.Count)];
				userConsortiaDomainInfo.player = player;
				m_listPlayer.Add(player.PlayerId, userConsortiaDomainInfo);
				flag = true;
			}
		}
		if (flag)
		{
			SendAddPlayer(userConsortiaDomainInfo);
		}
		return flag;
	}

	public bool RemovePlayer(GamePlayer player)
	{
		bool result = false;
		SendRemovePlayer(player);
		lock (m_listPlayer)
		{
			if (m_listPlayer.ContainsKey(player.PlayerId))
			{
				m_listPlayer.Remove(player.PlayerId);
				result = true;
			}
		}
		return result;
	}

	public void SendRemovePlayer(GamePlayer p)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(371);
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteInt(p.PlayerId);
		SendToRoom(p.PlayerCharacter.ConsortiaID, gSPacketIn);
	}

	public GSPacketIn SendOtherPlayers(List<UserConsortiaDomainInfo> list)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(371);
		gSPacketIn.WriteByte(3);
		gSPacketIn.WriteInt((list.Count > 10) ? 10 : list.Count);
		int num = 1;
		foreach (UserConsortiaDomainInfo item in list)
		{
			gSPacketIn.WriteInt(item.UserID);
			gSPacketIn.WriteString(item.player.PlayerCharacter.NickName);
			gSPacketIn.WriteInt(item.player.PlayerCharacter.VIPLevel);
			gSPacketIn.WriteInt(item.player.PlayerCharacter.typeVIP);
			gSPacketIn.WriteBoolean(item.player.PlayerCharacter.Sex);
			gSPacketIn.WriteString(item.player.PlayerCharacter.Style);
			gSPacketIn.WriteString(item.player.PlayerCharacter.Colors);
			gSPacketIn.WriteString(item.player.PlayerCharacter.Skin);
			gSPacketIn.WriteInt(item.player.PlayerCharacter.MountsType);
			gSPacketIn.WriteInt(item.player.PlayerCharacter.PetsID);
			gSPacketIn.WriteInt(item.Postion.X);
			gSPacketIn.WriteInt(item.Postion.Y);
			gSPacketIn.WriteInt(item.player.PlayerCharacter.ConsortiaID);
			gSPacketIn.WriteInt(item.player.PlayerCharacter.badgeID);
			gSPacketIn.WriteString(item.player.PlayerCharacter.ConsortiaName);
			gSPacketIn.WriteString(item.player.PlayerCharacter.Honor);
			gSPacketIn.WriteInt(item.player.PlayerCharacter.honorId);
			gSPacketIn.WriteBoolean(item.player.PlayerCharacter.isAttest);
			gSPacketIn.WriteString(item.player.PlayerCharacter.ImagePath);
			gSPacketIn.WriteBoolean(item.player.PlayerCharacter.IsShow);
			if (GameProperties.VERSION >= 10400)
			{
				gSPacketIn.WriteInt(item.player.PlayerCharacter.magicWardrobeID);
			}
			gSPacketIn.WriteBoolean(item.IsFighting);
			gSPacketIn.WriteInt(item.repairBuildId);
			if (num >= 10)
			{
				break;
			}
			num++;
		}
		return gSPacketIn;
	}

	public void SendUpdatePostionPlayer(UserConsortiaDomainInfo user, Point[] list)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(371);
		gSPacketIn.WriteByte(4);
		gSPacketIn.WriteInt(user.UserID);
		gSPacketIn.WriteInt(list.Length);
		for (int i = 0; i < list.Length; i++)
		{
			Point point = list[i];
			gSPacketIn.WriteInt(point.X);
			gSPacketIn.WriteInt(point.Y);
		}
		SendToRoom(user.ConsortiaID, gSPacketIn, user);
	}

	public void SendAddPlayer(UserConsortiaDomainInfo user)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(371);
		gSPacketIn.WriteByte(2);
		gSPacketIn.WriteInt(user.UserID);
		gSPacketIn.WriteString(user.player.PlayerCharacter.NickName);
		gSPacketIn.WriteInt(user.player.PlayerCharacter.VIPLevel);
		gSPacketIn.WriteInt(user.player.PlayerCharacter.typeVIP);
		gSPacketIn.WriteBoolean(user.player.PlayerCharacter.Sex);
		gSPacketIn.WriteString(user.player.PlayerCharacter.Style);
		gSPacketIn.WriteString(user.player.PlayerCharacter.Colors);
		gSPacketIn.WriteString(user.player.PlayerCharacter.Skin);
		gSPacketIn.WriteInt(user.player.PlayerCharacter.MountsType);
		gSPacketIn.WriteInt(user.player.PlayerCharacter.PetsID);
		gSPacketIn.WriteInt(user.Postion.X);
		gSPacketIn.WriteInt(user.Postion.Y);
		gSPacketIn.WriteInt(user.player.PlayerCharacter.ConsortiaID);
		gSPacketIn.WriteInt(user.player.PlayerCharacter.badgeID);
		gSPacketIn.WriteString(user.player.PlayerCharacter.ConsortiaName);
		gSPacketIn.WriteString(user.player.PlayerCharacter.Honor);
		gSPacketIn.WriteInt(user.player.PlayerCharacter.honorId);
		gSPacketIn.WriteBoolean(user.player.PlayerCharacter.isAttest);
		gSPacketIn.WriteString(user.player.PlayerCharacter.ImagePath);
		gSPacketIn.WriteBoolean(user.player.PlayerCharacter.IsShow);
		if (GameProperties.VERSION >= 10400)
		{
			gSPacketIn.WriteInt(user.player.PlayerCharacter.magicWardrobeID);
		}
		gSPacketIn.WriteBoolean(user.IsFighting);
		gSPacketIn.WriteInt(user.repairBuildId);
		SendToRoom(user.ConsortiaID, gSPacketIn, user);
	}

	public void SendToRoom(int ConsortiaID, GSPacketIn pkg)
	{
		SendToRoom(ConsortiaID, pkg, null);
	}

	public void SendToRoom(int ConsortiaID, GSPacketIn pkg, UserConsortiaDomainInfo ext)
	{
		List<UserConsortiaDomainInfo> allPlayerDomain = GetAllPlayerDomain(ConsortiaID);
		foreach (UserConsortiaDomainInfo item in allPlayerDomain)
		{
			if (item != null && item.player != null && (ext == null || item != ext))
			{
				item.player.SendTCP(pkg);
			}
		}
	}
}
