using System;
using System.Collections.Generic;
using System.Reflection;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using log4net;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils;

public class PlayerMagicHouse : PlayerInventory
{
	private new static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public int FREEBOX_MAXCOUNT = 5;

	public int CHARGEBOX_MAXCOUNT = 20;

	private string[] m_juniorWeaponList;

	private string[] m_midWeaponList;

	private string[] m_seniorWeaponList;

	private string[] m_juniorAddAttribute;

	private string[] m_midAddAttribute;

	private string[] m_seniorAddAttribute;

	private string[] m_levelUpCount;

	private string[] m_openNeedMoney;

	private int[] m_equipOpenList;

	private UsersMagicHouseInfo m_magicHouse;

	private static int m_baseDepotCount = 10;

	public readonly int TITLE_JUNIOR_ID = 1010;

	public readonly int TITLE_MID_ID = 1011;

	public readonly int TITLE_SENIOR_ID = 1012;

	public UsersMagicHouseInfo Info => m_magicHouse;

	public int BaseDepotCount => m_baseDepotCount;

	public PlayerMagicHouse(GamePlayer player)
		: base(player, saveTodb: true, 100, 51, 0, autoStack: true)
	{
		m_juniorWeaponList = GameProperties.MagicRoomJuniorWeaponList.Split('|');
		m_midWeaponList = GameProperties.MagicRoomMidWeaponList.Split('|');
		m_seniorWeaponList = GameProperties.MagicRoomSeniorWeaponList.Split('|');
		m_juniorAddAttribute = GameProperties.MagicRoomJuniorAddAttribute.Split('|');
		m_midAddAttribute = GameProperties.MagicRoomMidAddAttribute.Split('|');
		m_seniorAddAttribute = GameProperties.MagicRoomSeniorAddAttribute.Split('|');
		m_levelUpCount = GameProperties.MagicRoomLevelUpCount.Split('|');
		m_openNeedMoney = GameProperties.MagicRoomOpenNeedMoney.Split('|');
		m_equipOpenList = new int[9];
	}

	public override void LoadFromDatabase()
	{
		BeginChanges();
		try
		{
			base.LoadFromDatabase();
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			UsersMagicHouseInfo usersMagicHouseInfo = playerBussiness.GetAllUsersMagicHouseByID(base.Player.PlayerId);
			if (usersMagicHouseInfo == null)
			{
				usersMagicHouseInfo = new UsersMagicHouseInfo();
				usersMagicHouseInfo.UserID = base.Player.PlayerId;
				usersMagicHouseInfo.isOpen = true;
				usersMagicHouseInfo.isMagicRoomShow = true;
				usersMagicHouseInfo.magicJuniorLv = 0;
				usersMagicHouseInfo.magicJuniorExp = 0;
				usersMagicHouseInfo.magicMidLv = 0;
				usersMagicHouseInfo.magicMidExp = 0;
				usersMagicHouseInfo.magicSeniorLv = 0;
				usersMagicHouseInfo.magicSeniorExp = 0;
				usersMagicHouseInfo.freeGetCount = 0;
				usersMagicHouseInfo.freeGetTime = DateTime.Now;
				usersMagicHouseInfo.chargeGetCount = 0;
				usersMagicHouseInfo.chargeGetTime = DateTime.Now;
				usersMagicHouseInfo.depotCount = m_baseDepotCount;
				usersMagicHouseInfo.activityWeapons = SerializeActivityWeapons();
			}
			lock (m_lock)
			{
				m_magicHouse = usersMagicHouseInfo;
			}
		}
		finally
		{
			DeserializeActivityWeapons();
			CommitChanges();
		}
	}

	public override void SaveToDatabase()
	{
		base.SaveToDatabase();
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		if (m_magicHouse != null && m_magicHouse.IsDirty)
		{
			m_magicHouse.activityWeapons = SerializeActivityWeapons();
			if (m_magicHouse.ID == 0)
			{
				playerBussiness.AddUsersMagicHouse(m_magicHouse);
			}
			else
			{
				playerBussiness.UpdateUsersMagicHouse(m_magicHouse);
			}
		}
	}

	public void DeserializeActivityWeapons()
	{
		try
		{
			if (Info == null || string.IsNullOrEmpty(Info.activityWeapons))
			{
				return;
			}
			int[] array = JsonConvert.DeserializeObject<int[]>(Info.activityWeapons);
			if (array.Length == m_equipOpenList.Length)
			{
				for (int i = 0; i < m_equipOpenList.Length; i++)
				{
					m_equipOpenList[i] = array[i];
				}
			}
		}
		catch (Exception exception)
		{
			log.Error("DeserializeMagicHouse fail: ", exception);
		}
	}

	public string SerializeActivityWeapons()
	{
		return JsonConvert.SerializeObject(m_equipOpenList);
	}

	public int[] JuniorWeaponList(int slot)
	{
		List<int> list = new List<int>();
		string[] array = m_juniorWeaponList[slot].Split(',');
		string[] array2 = array;
		foreach (string s in array2)
		{
			list.Add(int.Parse(s));
		}
		return list.ToArray();
	}

	public int[] MidWeaponList(int slot)
	{
		List<int> list = new List<int>();
		string[] array = m_midWeaponList[slot].Split(',');
		string[] array2 = array;
		foreach (string s in array2)
		{
			list.Add(int.Parse(s));
		}
		return list.ToArray();
	}

	public int[] SeniorWeaponList(int slot)
	{
		List<int> list = new List<int>();
		string[] array = m_seniorWeaponList[slot].Split(',');
		string[] array2 = array;
		foreach (string s in array2)
		{
			list.Add(int.Parse(s));
		}
		return list.ToArray();
	}

	public int[] LevelUpExp()
	{
		List<int> list = new List<int>();
		string[] levelUpCount = m_levelUpCount;
		foreach (string s in levelUpCount)
		{
			list.Add(int.Parse(s));
		}
		return list.ToArray();
	}

	public int[] OpenNeedMoney()
	{
		List<int> list = new List<int>();
		string[] openNeedMoney = m_openNeedMoney;
		foreach (string s in openNeedMoney)
		{
			list.Add(int.Parse(s));
		}
		return list.ToArray();
	}

	public void OpenMagicLib(int templateId, int type, int pos)
	{
		lock (m_lock)
		{
			m_equipOpenList[(type - 1) * 3 + pos] = templateId;
		}
		if (EquipActive(type))
		{
			switch (type)
			{
			case 1:
				base.Player.Rank.AddNewRank(TITLE_JUNIOR_ID);
				break;
			case 2:
				base.Player.Rank.AddNewRank(TITLE_MID_ID);
				break;
			case 3:
				base.Player.Rank.AddNewRank(TITLE_SENIOR_ID);
				break;
			}
			base.Player.Rank.SendUserRanks();
			base.Player.EquipBag.UpdatePlayerProperties();
		}
	}

	public bool EquipActive(int type)
	{
		if (type == 1 && m_equipOpenList[0] != 0 && m_equipOpenList[1] != 0 && m_equipOpenList[2] != 0)
		{
			base.Player.Rank.AddNewRank(TITLE_JUNIOR_ID);
			return true;
		}
		if (type == 2 && m_equipOpenList[3] != 0 && m_equipOpenList[4] != 0 && m_equipOpenList[5] != 0)
		{
			base.Player.Rank.AddNewRank(TITLE_MID_ID);
			return true;
		}
		if (type == 3 && m_equipOpenList[6] != 0 && m_equipOpenList[7] != 0 && m_equipOpenList[8] != 0)
		{
			base.Player.Rank.AddNewRank(TITLE_SENIOR_ID);
			return true;
		}
		return false;
	}

	public void UpdateEnhanceProperties(int baseAttack, int baseDefence, ref int enhanceAtt, ref int enhanceDef, ref int attack, ref int defence, ref int agility, ref int lucky, ref int critBonus)
	{
		int attack2 = 0;
		int defence2 = 0;
		int crit = 0;
		if (EquipActive(1))
		{
			NewTitleInfo newTitleInfo = NewTitleMgr.FindNewTitle(TITLE_JUNIOR_ID);
			if (newTitleInfo != null)
			{
				attack += newTitleInfo.Att;
				defence += newTitleInfo.Def;
				agility += newTitleInfo.Agi;
				lucky += newTitleInfo.Luck;
			}
			UpdateAddAttribute(1, ref attack2, ref defence2, ref crit);
		}
		if (EquipActive(2))
		{
			NewTitleInfo newTitleInfo2 = NewTitleMgr.FindNewTitle(TITLE_MID_ID);
			if (newTitleInfo2 != null)
			{
				attack += newTitleInfo2.Att;
				defence += newTitleInfo2.Def;
				agility += newTitleInfo2.Agi;
				lucky += newTitleInfo2.Luck;
			}
			UpdateAddAttribute(2, ref attack2, ref defence2, ref crit);
		}
		if (EquipActive(3))
		{
			NewTitleInfo newTitleInfo3 = NewTitleMgr.FindNewTitle(TITLE_SENIOR_ID);
			if (newTitleInfo3 != null)
			{
				attack += newTitleInfo3.Att;
				defence += newTitleInfo3.Def;
				agility += newTitleInfo3.Agi;
				lucky += newTitleInfo3.Luck;
			}
			UpdateAddAttribute(3, ref attack2, ref defence2, ref crit);
		}
		enhanceAtt = baseAttack * attack2 / 100;
		enhanceDef = baseDefence * defence2 / 100;
		critBonus = crit;
	}

	public void UpdateAddAttribute(int type, ref int attack, ref int defence, ref int crit)
	{
		switch (type)
		{
		case 1:
		{
			for (int j = 0; j <= Info.magicJuniorLv; j++)
			{
				attack += int.Parse(m_juniorAddAttribute[j].Split(',')[0]);
				defence += int.Parse(m_juniorAddAttribute[j].Split(',')[1]);
				crit += int.Parse(m_juniorAddAttribute[j].Split(',')[2]);
			}
			break;
		}
		case 2:
		{
			for (int k = 0; k <= Info.magicMidLv; k++)
			{
				attack += int.Parse(m_midAddAttribute[k].Split(',')[0]);
				defence += int.Parse(m_midAddAttribute[k].Split(',')[1]);
				crit += int.Parse(m_midAddAttribute[k].Split(',')[2]);
			}
			break;
		}
		default:
		{
			for (int i = 0; i <= Info.magicSeniorLv; i++)
			{
				attack += int.Parse(m_seniorAddAttribute[i].Split(',')[0]);
				defence += int.Parse(m_seniorAddAttribute[i].Split(',')[1]);
				crit += int.Parse(m_seniorAddAttribute[i].Split(',')[2]);
			}
			break;
		}
		}
	}

	public void Init()
	{
		LoginMessage();
	}

	public void Reset()
	{
		if (m_magicHouse == null)
		{
			return;
		}
		lock (m_lock)
		{
			if (Info.freeGetTime.Date < DateTime.Now.Date)
			{
				m_magicHouse.freeGetCount = 0;
			}
			if (Info.chargeGetTime.Date < DateTime.Now.Date)
			{
				m_magicHouse.chargeGetCount = 0;
			}
		}
	}

	public ItemInfo[] GetAward(int count, eEventType DataId)
	{
		List<ItemInfo> list = new List<ItemInfo>();
		int num = 0;
		while (list.Count < count)
		{
			List<ItemInfo> eventAwardByType = EventAwardMgr.GetEventAwardByType(DataId);
			if (eventAwardByType.Count > 0)
			{
				ItemInfo item = eventAwardByType[0];
				list.Add(item);
			}
			num++;
		}
		return list.ToArray();
	}

	public void LoginMessage()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(84, base.Player.PlayerId);
		gSPacketIn.WriteInt(4);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteBoolean(val: true);
		int[] equipOpenList = m_equipOpenList;
		foreach (int val in equipOpenList)
		{
			gSPacketIn.WriteInt(val);
		}
		gSPacketIn.WriteInt(Info.magicJuniorLv);
		gSPacketIn.WriteInt(Info.magicJuniorExp);
		gSPacketIn.WriteInt(Info.magicMidLv);
		gSPacketIn.WriteInt(Info.magicMidExp);
		gSPacketIn.WriteInt(Info.magicSeniorLv);
		gSPacketIn.WriteInt(Info.magicSeniorExp);
		gSPacketIn.WriteInt(Info.freeGetCount);
		gSPacketIn.WriteDateTime(Info.freeGetTime);
		gSPacketIn.WriteInt(Info.chargeGetCount);
		gSPacketIn.WriteDateTime(Info.chargeGetTime);
		gSPacketIn.WriteInt(Info.depotCount);
		gSPacketIn.WriteInt(Info.essence);
		base.Player.SendTCP(gSPacketIn);
	}

	public void UpdateMessage()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(84, base.Player.PlayerId);
		gSPacketIn.WriteInt(4);
		gSPacketIn.WriteInt(2);
		gSPacketIn.WriteBoolean(val: true);
		int[] equipOpenList = m_equipOpenList;
		foreach (int val in equipOpenList)
		{
			gSPacketIn.WriteInt(val);
		}
		gSPacketIn.WriteInt(Info.magicJuniorLv);
		gSPacketIn.WriteInt(Info.magicJuniorExp);
		gSPacketIn.WriteInt(Info.magicMidLv);
		gSPacketIn.WriteInt(Info.magicMidExp);
		gSPacketIn.WriteInt(Info.magicSeniorLv);
		gSPacketIn.WriteInt(Info.magicSeniorExp);
		gSPacketIn.WriteInt(Info.freeGetCount);
		gSPacketIn.WriteDateTime(Info.freeGetTime);
		gSPacketIn.WriteInt(Info.chargeGetCount);
		gSPacketIn.WriteDateTime(Info.chargeGetTime);
		gSPacketIn.WriteInt(Info.depotCount);
		gSPacketIn.WriteInt(Info.essence);
		base.Player.SendTCP(gSPacketIn);
	}
}
