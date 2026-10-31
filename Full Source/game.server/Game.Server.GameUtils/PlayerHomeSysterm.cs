using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using log4net;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils;

public class PlayerHomeSysterm
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected object m_lock = new object();

	protected GamePlayer m_player;

	private UserHomeInfo m_data;

	private Dictionary<int, AdornInfo> m_adorns;

	private List<int> m_logTaste;

	private AdornInfo[] m_houseAdorn;

	private AdornInfo[] m_yardAdorn;

	private UserHomeInfo m_friendData;

	private AdornInfo[] m_friendHouseAdorn;

	private AdornInfo[] m_friendYardAdorn;

	private MoneyTreeInfo[] m_moneyTrees;

	private bool m_saveToDb;

	private bool m_loadFormDb;

	private int[] defaultHouseAdorn = new int[17]
	{
		3710001, 3706001, 3701002, 0, 0, 0, 3709002, 0, 0, 0,
		0, 0, 0, 0, 0, 0, 0
	};

	public int[] CategoryHouseArr = new int[13]
	{
		10, 6, 1, 11, 4, 2, 9, 12, 3, 14,
		15, 16, 17
	};

	private int[] defaultYardAdorn = new int[4] { 3705001, 0, 3707001, 0 };

	public int[] CategoryYardArr = new int[4] { 5, 13, 7, 8 };

	private List<HomeFishInfo> m_fishList = new List<HomeFishInfo>();

	private bool m_isFishing;

	public GamePlayer Player => m_player;

	public UserHomeInfo Data
	{
		get
		{
			return m_data;
		}
		set
		{
			m_data = value;
		}
	}

	public Dictionary<int, AdornInfo> Adorns
	{
		get
		{
			return m_adorns;
		}
		set
		{
			m_adorns = value;
		}
	}

	public List<int> LogTaste
	{
		get
		{
			return m_logTaste;
		}
		set
		{
			m_logTaste = value;
		}
	}

	public AdornInfo[] HouseAdorn
	{
		get
		{
			return m_houseAdorn;
		}
		set
		{
			m_houseAdorn = value;
		}
	}

	public AdornInfo[] YardAdorn
	{
		get
		{
			return m_yardAdorn;
		}
		set
		{
			m_yardAdorn = value;
		}
	}

	public UserHomeInfo FriendData
	{
		get
		{
			return m_friendData;
		}
		set
		{
			m_friendData = value;
		}
	}

	public AdornInfo[] FriendHouseAdorn
	{
		get
		{
			return m_friendHouseAdorn;
		}
		set
		{
			m_friendHouseAdorn = value;
		}
	}

	public AdornInfo[] FriendYardAdorn
	{
		get
		{
			return m_friendYardAdorn;
		}
		set
		{
			m_friendYardAdorn = value;
		}
	}

	public MoneyTreeInfo[] MoneyTrees
	{
		get
		{
			return m_moneyTrees;
		}
		set
		{
			m_moneyTrees = value;
		}
	}

	public List<HomeFishInfo> FishList
	{
		get
		{
			return m_fishList;
		}
		set
		{
			m_fishList = value;
		}
	}

	public bool IsFishing
	{
		get
		{
			return m_isFishing;
		}
		set
		{
			m_isFishing = value;
		}
	}

	public PlayerHomeSysterm(GamePlayer player, bool saveTodb)
	{
		m_player = player;
		m_saveToDb = saveTodb;
		m_houseAdorn = new AdornInfo[CategoryHouseArr.Length];
		m_yardAdorn = new AdornInfo[CategoryYardArr.Length];
		m_adorns = new Dictionary<int, AdornInfo>();
		m_logTaste = new List<int>();
		m_friendHouseAdorn = new AdornInfo[CategoryHouseArr.Length];
		m_friendYardAdorn = new AdornInfo[CategoryYardArr.Length];
		m_moneyTrees = new MoneyTreeInfo[4];
	}

	public virtual bool LoadFromDatabase(int hostId)
	{
		if (m_loadFormDb)
		{
			return true;
		}
		if (m_saveToDb)
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			try
			{
				UserHomeInfo allUserHomeByID = playerBussiness.GetAllUserHomeByID(Player.PlayerId);
				if (allUserHomeByID == null)
				{
					CreateHomeInfo(hostId);
				}
				else
				{
					lock (m_lock)
					{
						m_data = allUserHomeByID;
					}
				}
				AdornInfo[] allAdornByUserID = playerBussiness.GetAllAdornByUserID(Player.PlayerId);
				lock (m_lock)
				{
					AdornInfo[] array = allAdornByUserID;
					foreach (AdornInfo adornInfo in array)
					{
						if (!m_adorns.ContainsKey(adornInfo.TemplateId))
						{
							m_adorns.Add(adornInfo.TemplateId, adornInfo);
						}
					}
				}
			}
			catch (Exception message)
			{
				log.Error(message);
				return false;
			}
			finally
			{
				DeserializeData();
				if (Data.LastLogin.Date < DateTime.Now.Date)
				{
					ResetHomeInfo();
				}
				m_loadFormDb = true;
			}
		}
		return true;
	}

	public virtual void SaveToDatabase()
	{
		if (!m_saveToDb)
		{
			return;
		}
		SerializeData();
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		lock (m_lock)
		{
			if (m_data != null && m_data.IsDirty)
			{
				if (m_data.ID > 0)
				{
					playerBussiness.UpdateUserHome(m_data);
				}
				else
				{
					playerBussiness.AddUserHome(m_data);
				}
			}
			foreach (int key in Adorns.Keys)
			{
				if (m_adorns[key].IsDirty)
				{
					if (m_adorns[key].ID > 0)
					{
						playerBussiness.UpdateAdorn(m_adorns[key]);
					}
					else
					{
						playerBussiness.AddAdorn(m_adorns[key]);
					}
				}
			}
		}
	}

	public void SerializeData()
	{
		lock (m_lock)
		{
			if (m_data != null)
			{
				if (m_yardAdorn != null)
				{
					m_data.yardAdornInfo = JsonConvert.SerializeObject(m_yardAdorn);
				}
				if (m_houseAdorn != null)
				{
					m_data.houseAdornInfo = JsonConvert.SerializeObject(m_houseAdorn);
				}
				if (m_logTaste != null)
				{
					m_data.LogTaste = JsonConvert.SerializeObject(m_logTaste);
				}
				if (m_moneyTrees != null)
				{
					m_data.MoneyTrees = JsonConvert.SerializeObject(m_moneyTrees);
				}
			}
		}
	}

	public void DeserializeData()
	{
		lock (m_lock)
		{
			if (m_data != null)
			{
				if (!string.IsNullOrEmpty(Data.yardAdornInfo))
				{
					m_yardAdorn = JsonConvert.DeserializeObject<AdornInfo[]>(Data.yardAdornInfo);
				}
				if (!string.IsNullOrEmpty(Data.houseAdornInfo))
				{
					m_houseAdorn = JsonConvert.DeserializeObject<AdornInfo[]>(Data.houseAdornInfo);
				}
				if (!string.IsNullOrEmpty(Data.LogTaste))
				{
					m_logTaste = JsonConvert.DeserializeObject<List<int>>(Data.LogTaste);
				}
				if (!string.IsNullOrEmpty(Data.MoneyTrees))
				{
					m_moneyTrees = JsonConvert.DeserializeObject<MoneyTreeInfo[]>(Data.MoneyTrees);
				}
				else
				{
					CreateMoneyTreeInfo();
				}
			}
		}
	}

	public void CreateMoneyTreeInfo()
	{
		string[] array = GameProperties.WealthTreeConfigList.Split('|');
		lock (m_lock)
		{
			for (int i = 0; i < MoneyTrees.Length; i++)
			{
				string[] array2 = array[i].Split(',');
				int num = int.Parse(array2[2]);
				MoneyTreeInfo moneyTreeInfo = new MoneyTreeInfo();
				moneyTreeInfo.ItemID = int.Parse(array2[0]);
				moneyTreeInfo.positon = i;
				moneyTreeInfo.timeGrown = DateTime.Now.AddMinutes(int.Parse(array2[1]));
				moneyTreeInfo.priceSpeedUp = num + num * moneyTreeInfo.PickDaily;
				m_moneyTrees[i] = moneyTreeInfo;
			}
		}
	}

	public MoneyTreeInfo GetMoneyTreeInfo(int pos)
	{
		if (pos < 0 || pos > m_moneyTrees.Length)
		{
			return null;
		}
		return m_moneyTrees[pos];
	}

	public void SendMoneyTreeInfo()
	{
		if (Data == null)
		{
			LoadFromDatabase(Player.PlayerId);
		}
		if (m_loadFormDb)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(296);
			gSPacketIn.WriteByte(1);
			gSPacketIn.WriteInt(Data.NumRedPkgRemain);
			for (int i = 0; i < 4; i++)
			{
				gSPacketIn.WriteDateTime(MoneyTrees[i].timeGrown);
				gSPacketIn.WriteInt(MoneyTrees[i].priceSpeedUp);
			}
			Player.SendTCP(gSPacketIn);
		}
	}

	public void CreateHomeInfo(int hostId)
	{
		lock (m_lock)
		{
			m_data = new UserHomeInfo();
			m_data.UserID = hostId;
			m_data.houseStage = 0;
			m_data.energyCount = 0;
			m_data.drinkCount = GameProperties.HomeBarDrinkNum;
			m_data.cdEndTime = DateTime.Now.AddMinutes(-30.0);
			m_data.pourCount = 0;
			m_data.tasteCount = 10;
			m_data.enableFight = 1;
			m_data.yardAdornInfo = "";
			m_data.houseAdornInfo = "";
			m_data.LogTaste = "";
			m_data.LastLogin = DateTime.Now;
			m_data.MoneyTrees = "";
		}
		lock (m_lock)
		{
			for (int i = 0; i < m_houseAdorn.Length; i++)
			{
				AdornInfo adornInfo = new AdornInfo();
				adornInfo.UserID = hostId;
				adornInfo.TemplateId = defaultHouseAdorn[i];
				adornInfo.Category = CategoryHouseArr[i];
				m_houseAdorn[i] = adornInfo;
				if (adornInfo.TemplateId > 0)
				{
					Adorns.Add(adornInfo.TemplateId, adornInfo);
				}
			}
		}
		lock (m_lock)
		{
			for (int j = 0; j < m_yardAdorn.Length; j++)
			{
				AdornInfo adornInfo2 = new AdornInfo();
				adornInfo2.UserID = hostId;
				adornInfo2.TemplateId = defaultYardAdorn[j];
				adornInfo2.Category = CategoryYardArr[j];
				m_yardAdorn[j] = adornInfo2;
				if (adornInfo2.TemplateId > 0)
				{
					Adorns.Add(adornInfo2.TemplateId, adornInfo2);
				}
			}
		}
	}

	public UserHomeInfo CreateFriendHomeInfo(int hostId)
	{
		lock (m_lock)
		{
			m_friendData = new UserHomeInfo();
			m_friendData.UserID = hostId;
			m_friendData.cdEndTime = DateTime.Now;
		}
		lock (m_lock)
		{
			for (int i = 0; i < m_friendHouseAdorn.Length; i++)
			{
				AdornInfo adornInfo = new AdornInfo();
				adornInfo.UserID = hostId;
				adornInfo.TemplateId = defaultHouseAdorn[i];
				adornInfo.Category = CategoryHouseArr[i];
				m_friendHouseAdorn[i] = adornInfo;
			}
		}
		lock (m_lock)
		{
			for (int j = 0; j < m_friendYardAdorn.Length; j++)
			{
				AdornInfo adornInfo2 = new AdornInfo();
				adornInfo2.UserID = hostId;
				adornInfo2.TemplateId = defaultYardAdorn[j];
				adornInfo2.Category = CategoryYardArr[j];
				m_friendYardAdorn[j] = adornInfo2;
			}
		}
		return FriendData;
	}

	public List<AdornInfo> GetAdornByType(string type)
	{
		List<AdornInfo> list = new List<AdornInfo>();
		foreach (AdornInfo value in Adorns.Values)
		{
			if (type == "yard" && CategoryYardArr.Contains(value.Category))
			{
				list.Add(value);
			}
			else if (type == "house" && CategoryHouseArr.Contains(value.Category))
			{
				list.Add(value);
			}
		}
		return list;
	}

	public void UpdateAdorn(string type)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(292);
		if (type == "house")
		{
			gSPacketIn.WriteByte(1);
			gSPacketIn.WriteBoolean(val: true);
		}
		else
		{
			gSPacketIn.WriteByte(8);
			List<AdornInfo> adornByType = GetAdornByType(type);
			gSPacketIn.WriteInt(adornByType.Count);
			foreach (AdornInfo item in adornByType)
			{
				gSPacketIn.WriteInt(item.TemplateId);
				gSPacketIn.WriteInt(item.Category);
			}
		}
		Player.SendTCP(gSPacketIn);
	}

	public void BuyAndActives(ItemTemplateInfo item)
	{
		if (Data == null)
		{
			LoadFromDatabase(Player.PlayerId);
		}
		if (!m_loadFormDb)
		{
			return;
		}
		AdornInfo adornInfo = new AdornInfo();
		adornInfo.UserID = Player.PlayerId;
		adornInfo.TemplateId = item.TemplateID;
		adornInfo.Category = item.Property2;
		AddAdorn(adornInfo);
		if (CategoryYardArr.Contains(adornInfo.Category))
		{
			lock (m_lock)
			{
				for (int i = 0; i < CategoryYardArr.Length; i++)
				{
					if (YardAdorn[i].Category == adornInfo.Category)
					{
						m_yardAdorn[i] = adornInfo;
					}
				}
			}
		}
		if (!CategoryHouseArr.Contains(adornInfo.Category))
		{
			return;
		}
		lock (m_lock)
		{
			for (int j = 0; j < CategoryHouseArr.Length; j++)
			{
				if (HouseAdorn[j].Category == adornInfo.Category)
				{
					m_houseAdorn[j] = adornInfo;
				}
			}
		}
	}

	public void AddAdorn(AdornInfo info)
	{
		if (!CheckExitAdorn(info.TemplateId))
		{
			Adorns.Add(info.TemplateId, info);
		}
	}

	public AdornInfo GetAdorn(int templateId)
	{
		lock (m_lock)
		{
			return m_adorns[templateId];
		}
	}

	public bool CheckExitAdorn(int templateId)
	{
		lock (m_lock)
		{
			if (m_adorns.ContainsKey(templateId))
			{
				return true;
			}
		}
		return false;
	}

	public void SaveYardAdorn(AdornInfo info)
	{
		lock (m_lock)
		{
			for (int i = 0; i < CategoryYardArr.Length; i++)
			{
				if (YardAdorn[i].Category == info.Category)
				{
					m_yardAdorn[i] = info;
				}
			}
		}
	}

	public void SaveAdorn(AdornInfo info)
	{
		lock (m_lock)
		{
			for (int i = 0; i < CategoryHouseArr.Length; i++)
			{
				if (HouseAdorn[i].Category == info.Category)
				{
					m_houseAdorn[i] = info;
				}
			}
		}
	}

	public bool isDrink()
	{
		int num = DateTime.Compare(Data.cdEndTime, DateTime.Now);
		return num > 0;
	}

	public void ResetHomeInfo()
	{
		lock (m_lock)
		{
			if (m_data != null)
			{
				m_data.drinkCount = GameProperties.HomeBarDrinkNum;
				m_data.pourCount = 0;
				m_data.tasteCount = 10;
				m_data.enableFight = 1;
				m_data.LastLogin = DateTime.Now;
				m_data.remainCount = 0;
				m_data.dayScore = 0;
				m_data.fishGameTime = 1;
			}
		}
		lock (m_lock)
		{
			if (m_logTaste != null)
			{
				m_logTaste.Clear();
			}
		}
		lock (m_lock)
		{
			for (int i = 0; i < MoneyTrees.Length; i++)
			{
				if (m_moneyTrees[i] != null)
				{
					m_moneyTrees[i].PickDaily = 0;
				}
			}
		}
	}

	public void SendHomeBarInfo(int playerType)
	{
		SendHomeBarInfo(playerType, -1);
	}

	public void SendHomeBarInfo(int playerType, int hostId)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(294);
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteInt(playerType);
		if (playerType == 0)
		{
			gSPacketIn.WriteInt(Data.energyCount);
			gSPacketIn.WriteInt(Data.drinkCount);
			gSPacketIn.WriteDateTime(Data.cdEndTime);
			gSPacketIn.WriteInt(Data.pourCount);
		}
		else
		{
			gSPacketIn.WriteBoolean(IsTaste(hostId));
			gSPacketIn.WriteInt(Data.tasteCount);
			gSPacketIn.WriteInt(Data.energyCount);
		}
		Player.SendTCP(gSPacketIn);
	}

	public void SendHomeBarUpdateEnergy()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(294);
		gSPacketIn.WriteByte(6);
		gSPacketIn.WriteInt(Data.energyCount);
		Player.SendTCP(gSPacketIn);
	}

	public void AddTaste(int hostId)
	{
		lock (m_lock)
		{
			if (!m_logTaste.Contains(hostId))
			{
				m_logTaste.Add(hostId);
			}
		}
	}

	public bool IsTaste(int hostId)
	{
		return LogTaste.Contains(hostId);
	}

	public void BuildFishList()
	{
		List<HomeFishInfo> allHomeFish = HomeFishMgr.GetAllHomeFish();
		int num = (int)((double)allHomeFish.Count * 4.3);
		lock (m_lock)
		{
			foreach (HomeFishInfo item in allHomeFish)
			{
				if (item.Score != 0)
				{
					int num2 = num;
					if (num2 < 1)
					{
						num2 = 1;
					}
					if (item.Score < 7)
					{
						num2 = (int)((double)num * 1.5);
					}
					for (int i = 0; i < num2; i++)
					{
						m_fishList.Add(item);
					}
					num -= item.Score;
				}
			}
			if (FishList.Count > 5)
			{
				ThreadSafeRandom.ShufferListStatic(m_fishList);
			}
		}
	}

	public void ClearFishList()
	{
		lock (m_lock)
		{
			m_fishList.Clear();
		}
	}

	public void SendBroadcast(string name)
	{
		string translation = LanguageMgr.GetTranslation("FishBroadcast.Notice", Player.PlayerCharacter.NickName, name);
		GSPacketIn packet = WorldMgr.SendSysNotice(translation);
		GameServer.Instance.LoginServer.SendPacket(packet);
	}

	public void SendUpdateFishPlayerInfo()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(285);
		gSPacketIn.WriteByte(22);
		gSPacketIn.WriteInt(Data.fishGameTime);
		gSPacketIn.WriteInt(Data.fishScore);
		gSPacketIn.WriteInt(Data.dayScore);
		gSPacketIn.WriteInt(Data.maxScore);
		gSPacketIn.WriteInt(Data.remainCount);
		Player.SendTCP(gSPacketIn);
	}
}
