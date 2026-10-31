using System;
using System.Collections.Generic;
using System.Reflection;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Buffer;
using Game.Server.GameObjects;
using log4net;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils;

public class PlayerExtra
{
	public class FlopCardInfo
	{
		public int TemplateID { get; set; }

		public int Count { get; set; }
	}

	private static Random rand = new Random();

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected object m_lock = new object();

	protected GamePlayer m_player;

	private bool m_saveToDb;

	public const int STRENGTH_ENCHANCE = 1;

	public const int PET_REFRESH = 2;

	public const int BEAD_MASTER = 3;

	public const int HELP_STRAW = 4;

	public const int DUNGEON_HERO = 5;

	public const int TASK_SPIRIT = 6;

	public const int TIME_DEITY = 7;

	public const int PET_GRANT = 10;

	public const int PET_STAR = 11;

	public const int HORSE_LIGHT = 12;

	public const int HORSE_ANGER = 13;

	private int[] kingBlessBuffData = new int[7] { 1, 2, 3, 4, 5, 6, 7 };

	private int[] deedBuffData = new int[4] { 10, 11, 12, 13 };

	public const int COUPLE_NUM = 20;

	public const int DUNGEON_NUM = 4;

	public const int PROPS_NUM = 5;

	private UsersExtraInfo m_info;

	private SysUsersDetailInfo m_userInfo;

	private Dictionary<int, int> m_kingBlessInfo;

	private List<EventAwardInfo> m_searchGoodItems;

	private List<FlopCardInfo> m_flopCard;

	private QuestInfo[] m_buriedQuests;

	private Dictionary<int, int> m_deedInfo;

	private List<EventAwardInfo> m_magpieBridgeItems;

	private int[] m_prizeStatus;

	public UserPvePowerBuffInfo PvePowerBuff;

	public int MapId = 1;

	public int takeCardLimit = 3;

	private string[] TanabataActiveEventNum = GameProperties.TanabataActiveEventNum.Split('|');

	private string[] FinalReward = GameProperties.TanabataActiveDestinationAWardID.Split('|');

	public GamePlayer Player => m_player;

	public UsersExtraInfo Info
	{
		get
		{
			return m_info;
		}
		set
		{
			m_info = value;
		}
	}

	public SysUsersDetailInfo UserInfo
	{
		get
		{
			return m_userInfo;
		}
		set
		{
			m_userInfo = value;
		}
	}

	public Dictionary<int, int> KingBlessInfo
	{
		get
		{
			return m_kingBlessInfo;
		}
		set
		{
			m_kingBlessInfo = value;
		}
	}

	public List<EventAwardInfo> SearchGoodItems
	{
		get
		{
			return m_searchGoodItems;
		}
		set
		{
			m_searchGoodItems = value;
		}
	}

	public Dictionary<int, int> DeedInfo
	{
		get
		{
			return m_deedInfo;
		}
		set
		{
			m_deedInfo = value;
		}
	}

	public List<EventAwardInfo> MagpieBridgeItems
	{
		get
		{
			return m_magpieBridgeItems;
		}
		set
		{
			m_magpieBridgeItems = value;
		}
	}

	public int[] PrizeStatus
	{
		get
		{
			return m_prizeStatus;
		}
		set
		{
			m_prizeStatus = value;
		}
	}

	public PlayerExtra(GamePlayer player, bool saveTodb)
	{
		m_player = player;
		m_kingBlessInfo = new Dictionary<int, int>();
		m_deedInfo = new Dictionary<int, int>();
		m_searchGoodItems = new List<EventAwardInfo>();
		m_magpieBridgeItems = new List<EventAwardInfo>();
		m_flopCard = new List<FlopCardInfo>();
		m_saveToDb = saveTodb;
		m_buriedQuests = null;
		m_prizeStatus = new int[7];
		PvePowerBuff = null;
	}

	public virtual void LoadFromDatabase()
	{
		if (!m_saveToDb)
		{
			return;
		}
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		if (GlobalConstants.SubSysUserActice)
		{
			lock (m_lock)
			{
				m_userInfo = playerBussiness.GetSingleSysUsersDetail(Player.PlayerCharacter.ID);
				if (m_userInfo == null)
				{
					m_userInfo = new SysUsersDetailInfo();
				}
			}
		}
		UsersExtraInfo singleUsersExtra = playerBussiness.GetSingleUsersExtra(Player.PlayerCharacter.ID);
		if (singleUsersExtra == null)
		{
			lock (m_lock)
			{
				m_info = CreateUsersExtra(Player.PlayerCharacter.ID);
			}
		}
		else
		{
			lock (m_lock)
			{
				m_info = singleUsersExtra;
				if (string.IsNullOrEmpty(Info.CatchInsectGetPrize))
				{
					m_info.CatchInsectGetPrize = "0-0-0-0-0-0-0";
				}
			}
			SetupKingBlessFromData();
			SetupDeedFromData();
			Player.AvatarBag.CurrentModelIndex = m_info.CurentDressModel;
			Player.AvatarBag.StringToDressModelArr(m_info.DressModelArr);
			string[] array = Info.CatchInsectGetPrize.Split('-');
			int num = 0;
			lock (m_lock)
			{
				string[] array2 = array;
				foreach (string s in array2)
				{
					if (num < array.Length)
					{
						m_prizeStatus[num] = int.Parse(s);
					}
					num++;
				}
			}
		}
		PvePowerBuff = JsonConvert.DeserializeObject<UserPvePowerBuffInfo>((m_info.pvePowerBuffData == null) ? "" : m_info.pvePowerBuffData);
		if (PvePowerBuff == null || (DateTime.Now >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0) && PvePowerBuff.refreshDate < new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0)))
		{
			CreatePvePowerBuff();
		}
	}

	public virtual void SaveToDatabase()
	{
		if (!m_saveToDb)
		{
			return;
		}
		lock (m_lock)
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			if (Info != null && Info.IsDirty)
			{
				ConvertDeed();
				ConvertKingBless();
				ConvertSearchGoodItems();
				ConvertMagpieBridgeItems();
				ConvertCatchInsectGetPrize();
				Info.pvePowerBuffData = JsonConvert.SerializeObject(PvePowerBuff);
				Info.DressModelArr = Player.AvatarBag.DressModelArrToString();
				Info.CurentDressModel = Player.AvatarBag.CurrentModelIndex;
				if (Info.ID > 0)
				{
					playerBussiness.UpdateUsersExtra(Info);
				}
				else
				{
					playerBussiness.AddUsersExtra(Info);
				}
			}
			if (GlobalConstants.SubSysUserActice && UserInfo != null && Info.IsDirty)
			{
				if (UserInfo.UserID > 0)
				{
					playerBussiness.UpdateSysUsersDetail(UserInfo);
				}
				else
				{
					playerBussiness.AddSysUsersDetail(UserInfo, Player.PlayerCharacter.ID);
				}
			}
		}
	}

	public void CreatePvePowerBuff()
	{
		PvePowerBuff = new UserPvePowerBuffInfo();
		ResetPvePowerBuff();
		RefershPvePowerData();
	}

	public void ResetPvePowerBuff()
	{
		PvePowerBuff.refreshCount = 1;
		PvePowerBuff.refreshDate = DateTime.Now;
		PvePowerBuff.getBuffCount = 0;
		PvePowerBuff.getBuffDate = DateTime.Now.AddYears(-1);
		ResetPvePowerData();
	}

	public void RefershPvePowerData()
	{
		PvePowerBuff.ListIndexBuff = SelectPvePowerBuffRandomPlayers();
		PvePowerBuff.getBuffIndex = -1;
	}

	public void ResetPvePowerData()
	{
		PvePowerBuff.getBuffIndex = -1;
		PvePowerBuff.getBuffAtk = 0;
		PvePowerBuff.getBuffDef = 0;
		PvePowerBuff.getBuffAgl = 0;
		PvePowerBuff.getBuffLuck = 0;
		PvePowerBuff.getBuffDmg = 0;
		PvePowerBuff.getBuffAr = 0;
		PvePowerBuff.getBuffHp = 0;
		PvePowerBuff.getBuffMAtk = 0;
		PvePowerBuff.getBuffMDef = 0;
	}

	private List<UserPvePowerBuffTempInfo> SelectPvePowerBuffRandomPlayers()
	{
		List<UserPvePowerBuffTempInfo> list = new List<UserPvePowerBuffTempInfo>();
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			PlayerInfo[] randomPlayers = playerBussiness.GetRandomPlayers();
			PlayerInfo[] array = randomPlayers;
			foreach (PlayerInfo playerInfo in array)
			{
				UserPvePowerBuffTempInfo userPvePowerBuffTempInfo = new UserPvePowerBuffTempInfo();
				userPvePowerBuffTempInfo.ID = playerInfo.ID;
				userPvePowerBuffTempInfo.Attack = playerInfo.Attack;
				userPvePowerBuffTempInfo.Defence = playerInfo.Defence;
				userPvePowerBuffTempInfo.Agility = playerInfo.Agility;
				userPvePowerBuffTempInfo.Luck = playerInfo.Luck;
				userPvePowerBuffTempInfo.Damage = playerInfo.BaseAttack;
				userPvePowerBuffTempInfo.Guard = playerInfo.BaseDefence;
				userPvePowerBuffTempInfo.HP = playerInfo.hp;
				userPvePowerBuffTempInfo.MagicAttack = playerInfo.MagicAttack;
				userPvePowerBuffTempInfo.MagicDefence = playerInfo.MagicDefence;
				userPvePowerBuffTempInfo.Grade = playerInfo.Grade;
				userPvePowerBuffTempInfo.FightPower = playerInfo.FightPower;
				userPvePowerBuffTempInfo.NickName = playerInfo.NickName;
				userPvePowerBuffTempInfo.Sex = (playerInfo.Sex ? 1 : 0);
				userPvePowerBuffTempInfo.Style = playerInfo.Style;
				userPvePowerBuffTempInfo.Colors = playerInfo.Colors;
				userPvePowerBuffTempInfo.Skin = playerInfo.Skin;
				userPvePowerBuffTempInfo.Hide = playerInfo.Hide;
				CheckInvalidStyle(userPvePowerBuffTempInfo);
				list.Add(userPvePowerBuffTempInfo);
			}
		}
		return list;
	}

	public void CheckInvalidStyle(UserPvePowerBuffTempInfo user)
	{
		string[] array = user.Style.Split(',');
		string[] array2 = user.Style.Split(',');
		for (int i = 0; i < array2.Length; i++)
		{
			string value = array2[i].Split('|')[0];
			if (!string.IsNullOrEmpty(value) && ItemMgr.FindItemTemplate(Convert.ToInt32(value)) == null)
			{
				array[i] = "";
			}
		}
		user.Style = string.Join(",", array);
	}

	public void ConvertCatchInsectGetPrize()
	{
		try
		{
			string text = "";
			int[] prizeStatus = PrizeStatus;
			foreach (int num in prizeStatus)
			{
				text = text + num + "-";
			}
			lock (m_lock)
			{
				m_info.CatchInsectGetPrize = text.Substring(0, text.Length - 1);
			}
		}
		catch (Exception ex)
		{
			log.Error(ex.ToString());
		}
	}

	public UsersExtraInfo CreateUsersExtra(int UserID)
	{
		UsersExtraInfo usersExtraInfo = new UsersExtraInfo();
		usersExtraInfo.UserID = UserID;
		usersExtraInfo.starlevel = 5;
		usersExtraInfo.nowPosition = 0;
		usersExtraInfo.FreeCount = GameProperties.SearchGoodsFreeCount;
		usersExtraInfo.SearchGoodItems = "";
		usersExtraInfo.FreeAddAutionCount = 0;
		usersExtraInfo.FreeSendMailCount = 0;
		usersExtraInfo.FreePresentGoodsCount = 0;
		usersExtraInfo.KingBlessInfo = "";
		usersExtraInfo.KingBlessEnddate = DateTime.Now;
		usersExtraInfo.KingBlessIndex = 0;
		usersExtraInfo.MissionEnergy = GameProperties.MaxMissionEnergy;
		usersExtraInfo.buyEnergyCount = 1;
		usersExtraInfo.DressModelArr = ",,";
		usersExtraInfo.CurentDressModel = 0;
		usersExtraInfo.ScoreMagicstone = 0;
		usersExtraInfo.DeedInfo = "";
		usersExtraInfo.DeedEnddate = DateTime.Now;
		usersExtraInfo.DeedIndex = 0;
		usersExtraInfo.Score = 0;
		usersExtraInfo.SummerScore = 0;
		usersExtraInfo.CatchInsectGetPrize = "0-0-0-0-0-0-0";
		usersExtraInfo.PrizeStatus = 0;
		usersExtraInfo.CakeStatus = false;
		usersExtraInfo.NormalFightNum = 1;
		usersExtraInfo.HardFightNum = 1;
		usersExtraInfo.IsDoubleScore = false;
		usersExtraInfo.MagpieBridgeItems = "";
		usersExtraInfo.NowPositionMB = 0;
		usersExtraInfo.LastNum = 5;
		usersExtraInfo.MagpieNum = 0;
		usersExtraInfo.coupleNum = 0;
		usersExtraInfo.dungeonNum = 0;
		usersExtraInfo.propsNum = 0;
		usersExtraInfo.silverTime = DateTime.Now;
		usersExtraInfo.silverCount = 10;
		usersExtraInfo.goldTime = DateTime.Now.AddDays(-4.0);
		usersExtraInfo.dungeonCount = "";
		usersExtraInfo.loginDeviceDailyAward = false;
		usersExtraInfo.loginDeviceDownAward = false;
		usersExtraInfo.pvePowerBuffData = "";
		usersExtraInfo.FreeTimeMark = DateTime.Now;
		usersExtraInfo.FreeCountMark = GameProperties.EngraveVaultsFreeTimes;
		usersExtraInfo.Coin = 0;
		usersExtraInfo.SummonCount1 = 10;
		usersExtraInfo.SummonCount2 = 50;
		return usersExtraInfo;
	}

	public void ResetUsersExtra()
	{
		if (m_info == null)
		{
			return;
		}
		SetupKingBless(isFriend: false, Info.KingBlessIndex);
		SetupDeed();
		lock (m_lock)
		{
			m_info.starlevel = 5;
			m_info.nowPosition = 0;
			m_info.FreeCount = GameProperties.SearchGoodsFreeCount;
			m_info.FreeAddAutionCount = 0;
			m_info.FreeSendMailCount = 0;
			m_info.FreePresentGoodsCount = 0;
			m_info.buyEnergyCount = 1;
			m_info.coupleNum = 0;
			m_info.dungeonNum = 0;
			m_info.propsNum = 0;
			if (Info.KingBlessEnddate.Date < DateTime.Now.Date)
			{
				m_info.KingBlessIndex = 0;
			}
			if (Info.DeedEnddate.Date < DateTime.Now.Date)
			{
				m_info.DeedIndex = 0;
			}
			m_info.silverCount = 10;
			m_info.MissionEnergy += GameProperties.MissionEnergyAdd;
			if (Info.MissionEnergy > GameProperties.MaxMissionEnergy)
			{
				m_info.MissionEnergy = GameProperties.MaxMissionEnergy;
			}
			m_info.EnergyTotal = 0;
			m_info.dungeonCount = "";
			m_info.loginDeviceDailyAward = false;
			m_info.FreeCountMark = GameProperties.EngraveVaultsFreeTimes;
			m_info.FreeTimeMark = DateTime.Now;
			ResetPvePowerBuff();
		}
		CreateSaveLifeBuff();
		CreateSearchGoodItems();
	}

	public bool CanRingExp(int type)
	{
		if (Info == null)
		{
			return false;
		}
		return type switch
		{
			-1 => true, 
			1 => Info.dungeonNum < 4, 
			2 => Info.propsNum < 5, 
			_ => Info.coupleNum < 20, 
		};
	}

	public void UpdateLoveScoreLimit(int type)
	{
		if (Info == null)
		{
			return;
		}
		switch (type)
		{
		case 1:
			if (Info.dungeonNum < 4)
			{
				Info.dungeonNum++;
			}
			break;
		case 2:
			if (Info.propsNum < 5)
			{
				Info.propsNum++;
			}
			break;
		default:
			if (Info.coupleNum < 20)
			{
				Info.coupleNum++;
			}
			break;
		}
	}

	public void LoadBuriedQuests()
	{
		if (m_buriedQuests == null)
		{
			m_buriedQuests = QuestMgr.GetAllBuriedQuest();
		}
	}

	public string SetupKingBless(bool isFriend, int kingBlessIndex)
	{
		string result = "";
		int value = 100;
		int value2 = 1;
		int value3 = 3;
		int value4 = 1;
		int value5 = 1;
		int value6 = 0;
		int value7 = 1;
		switch (kingBlessIndex)
		{
		case 2:
			value2 = 2;
			value3 = 4;
			break;
		case 3:
			value = 200;
			value2 = 3;
			value3 = 6;
			value4 = 2;
			value5 = 2;
			break;
		}
		if (isFriend)
		{
			result = "STRENGTH_ENCHANCE," + value;
			result = result + "|PET_REFRESH," + value2;
			result = result + "|BEAD_MASTER," + value3;
			result = result + "|HELP_STRAW," + value4;
			result = result + "|DUNGEON_HERO," + value5;
			result = result + "|TASK_SPIRIT," + value6;
			return result + "|TIME_DEITY," + value7;
		}
		m_kingBlessInfo.Clear();
		int[] array = kingBlessBuffData;
		foreach (int num in array)
		{
			switch (num)
			{
			case 1:
				m_kingBlessInfo.Add(num, value);
				break;
			case 2:
				m_kingBlessInfo.Add(num, value2);
				break;
			case 3:
				m_kingBlessInfo.Add(num, value3);
				break;
			case 4:
				m_kingBlessInfo.Add(num, value4);
				break;
			case 5:
				m_kingBlessInfo.Add(num, value5);
				break;
			case 6:
				m_kingBlessInfo.Add(num, value6);
				break;
			case 7:
				m_kingBlessInfo.Add(num, value7);
				break;
			}
		}
		ConvertKingBless();
		return result;
	}

	public void SetupKingBlessFromData()
	{
		if (string.IsNullOrEmpty(m_info.KingBlessInfo))
		{
			return;
		}
		m_kingBlessInfo.Clear();
		string[] array = m_info.KingBlessInfo.Split('|');
		string[] array2 = array;
		foreach (string text in array2)
		{
			int value = int.Parse(text.Split(',')[1]);
			switch (text.Split(',')[0])
			{
			case "STRENGTH_ENCHANCE":
				m_kingBlessInfo.Add(1, value);
				break;
			case "PET_REFRESH":
				m_kingBlessInfo.Add(2, value);
				break;
			case "BEAD_MASTER":
				m_kingBlessInfo.Add(3, value);
				break;
			case "HELP_STRAW":
				m_kingBlessInfo.Add(4, value);
				break;
			case "DUNGEON_HERO":
				m_kingBlessInfo.Add(5, value);
				break;
			case "TASK_SPIRIT":
				m_kingBlessInfo.Add(6, value);
				break;
			case "TIME_DEITY":
				m_kingBlessInfo.Add(7, value);
				break;
			}
		}
	}

	public void ConvertKingBless()
	{
		if (KingBlessInfo.Count == 0)
		{
			return;
		}
		string text = "";
		using (Dictionary<int, int>.KeyCollection.Enumerator enumerator = KingBlessInfo.Keys.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				switch (enumerator.Current)
				{
				case 1:
					text = text + "STRENGTH_ENCHANCE," + KingBlessInfo[1];
					break;
				case 2:
					text = text + "|PET_REFRESH," + KingBlessInfo[2];
					break;
				case 3:
					text = text + "|BEAD_MASTER," + KingBlessInfo[3];
					break;
				case 4:
					text = text + "|HELP_STRAW," + KingBlessInfo[4];
					break;
				case 5:
					text = text + "|DUNGEON_HERO," + KingBlessInfo[5];
					break;
				case 6:
					text = text + "|TASK_SPIRIT," + KingBlessInfo[6];
					break;
				case 7:
					text = text + "|TIME_DEITY," + KingBlessInfo[7];
					break;
				}
			}
		}
		lock (m_lock)
		{
			m_info.KingBlessInfo = text;
		}
	}

	public bool UseKingBless(int key)
	{
		if (m_info.KingBlessEnddate.Date < DateTime.Now)
		{
			return false;
		}
		if (KingBlessInfo.ContainsKey(key) && KingBlessInfo[key] > 0)
		{
			m_kingBlessInfo[key]--;
			Player.Out.SendKingBlessUpdateBuffData(Player.PlayerCharacter.ID, key, KingBlessInfo[key]);
			return true;
		}
		return false;
	}

	public void KingBlessStrengthEnchance(bool isUp)
	{
		if (isUp && KingBlessInfo.ContainsKey(1))
		{
			Player.PlayerCharacter.StrengthEnchance = KingBlessInfo[1];
		}
		else
		{
			Player.PlayerCharacter.StrengthEnchance = 0;
		}
	}

	public bool UseDeed(int key)
	{
		if (m_info.DeedEnddate.Date < DateTime.Now)
		{
			return false;
		}
		if (DeedInfo.ContainsKey(key) && DeedInfo[key] > 0)
		{
			m_deedInfo[key]--;
			Player.Out.SendDeedUpdateBuffData(Player.PlayerCharacter.ID, key, DeedInfo[key]);
			return true;
		}
		return false;
	}

	public void ConvertDeed()
	{
		if (DeedInfo.Count == 0)
		{
			return;
		}
		string text = "";
		using (Dictionary<int, int>.KeyCollection.Enumerator enumerator = DeedInfo.Keys.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				switch (enumerator.Current)
				{
				case 10:
					text = text + "PET_GRANT," + DeedInfo[10];
					break;
				case 11:
					text = text + "|PET_STAR," + DeedInfo[11];
					break;
				case 12:
					text = text + "|HORSE_LIGHT," + DeedInfo[12];
					break;
				case 13:
					text = text + "|HORSE_ANGER," + DeedInfo[13];
					break;
				}
			}
		}
		lock (m_lock)
		{
			m_info.DeedInfo = text;
		}
	}

	public string SetupDeed()
	{
		string result = "";
		int value = 3;
		int value2 = 2;
		int value3 = 2;
		int value4 = 3;
		m_deedInfo.Clear();
		int[] array = deedBuffData;
		foreach (int num in array)
		{
			switch (num)
			{
			case 10:
				m_deedInfo.Add(num, value);
				break;
			case 11:
				m_deedInfo.Add(num, value2);
				break;
			case 12:
				m_deedInfo.Add(num, value3);
				break;
			case 13:
				m_deedInfo.Add(num, value4);
				break;
			}
			ConvertDeed();
		}
		return result;
	}

	public void SetupDeedFromData()
	{
		if (string.IsNullOrEmpty(Info.DeedInfo))
		{
			return;
		}
		m_deedInfo.Clear();
		string[] array = Info.DeedInfo.Split('|');
		string[] array2 = array;
		foreach (string text in array2)
		{
			int value = int.Parse(text.Split(',')[1]);
			switch (text.Split(',')[0])
			{
			case "PET_GRANT":
				m_deedInfo.Add(10, value);
				break;
			case "PET_STAR":
				m_deedInfo.Add(11, value);
				break;
			case "HORSE_LIGHT":
				m_deedInfo.Add(12, value);
				break;
			case "HORSE_ANGER":
				m_deedInfo.Add(13, value);
				break;
			}
		}
	}

	public List<EventAwardInfo> CreateSearchGoodsAward()
	{
		SearchGoodsTempInfo searchGoodsTempInfo = SearchGoodsMgr.GetSearchGoodsTempInfo(m_info.starlevel);
		Dictionary<int, EventAwardInfo> dictionary = new Dictionary<int, EventAwardInfo>();
		string text = ((searchGoodsTempInfo == null) ? "10,2,3,1" : searchGoodsTempInfo.ExtractNumber);
		int[] array = new int[5] { -1, -2, -3, -4, -7 };
		ThreadSafeRandom.ShufferStatic(array);
		int count = int.Parse(text.Split(',')[0]);
		int num = int.Parse(text.Split(',')[1]);
		int num2 = int.Parse(text.Split(',')[3]);
		int num3 = int.Parse(text.Split(',')[2]);
		int total = GetTotal(text);
		List<int> list = CreateRandomPos();
		List<EventAwardInfo> list2 = new List<EventAwardInfo>();
		int num4 = 0;
		EventAwardInfo[] array2 = EventAwardMgr.CreateSearchGoodsAward(count);
		for (int i = 0; i < array2.Length; i++)
		{
			num4 = list[i];
			EventAwardInfo eventAwardInfo = array2[i];
			eventAwardInfo.Position = num4;
			list2.Add(eventAwardInfo);
			list.Remove(num4);
		}
		for (int j = 0; j < num; j++)
		{
			num4 = list[j];
			list2.Add(GetSpecialTem(-5, num4));
			list.Remove(num4);
		}
		for (int k = 0; k < num3; k++)
		{
			num4 = list[k];
			int num5 = rand.Next(array.Length);
			list2.Add(GetSpecialTem(array[num5], num4));
			list.Remove(num4);
		}
		for (int l = 0; l < num2; l++)
		{
			num4 = list[l];
			list2.Add(GetSpecialTem(-6, num4));
			list.Remove(num4);
		}
		return list2;
	}

	public EventAwardInfo GetSpecialTem(int type, int pos)
	{
		EventAwardInfo eventAwardInfo = new EventAwardInfo();
		eventAwardInfo.TemplateID = type;
		eventAwardInfo.Position = pos;
		eventAwardInfo.Count = 1;
		return eventAwardInfo;
	}

	public int GetTotal(string number)
	{
		string[] array = number.Split(',');
		int num = 0;
		string[] array2 = array;
		foreach (string s in array2)
		{
			num += int.Parse(s);
		}
		return num;
	}

	public List<int> CreateRandomPos()
	{
		List<int> list = new List<int>();
		for (int i = 1; i < 35; i++)
		{
			list.Add(i);
		}
		ThreadSafeRandom.ShufferListStatic(list);
		return list;
	}

	public void CreateSearchGoodItems()
	{
		m_searchGoodItems.Clear();
		List<EventAwardInfo> list = CreateSearchGoodsAward();
		lock (m_lock)
		{
			foreach (EventAwardInfo item in list)
			{
				m_searchGoodItems.Add(item);
			}
		}
	}

	public void GetSearchGoodItemsDb()
	{
		if (string.IsNullOrEmpty(m_info.SearchGoodItems) || m_info.starlevel == 1)
		{
			CreateSearchGoodItems();
			return;
		}
		string[] array = m_info.SearchGoodItems.Split('|');
		string[] array2 = array;
		foreach (string text in array2)
		{
			EventAwardInfo eventAwardInfo = new EventAwardInfo();
			eventAwardInfo.TemplateID = int.Parse(text.Split(',')[0]);
			eventAwardInfo.Position = int.Parse(text.Split(',')[1]);
			eventAwardInfo.Count = int.Parse(text.Split(',')[2]);
			m_searchGoodItems.Add(eventAwardInfo);
		}
	}

	public void ConvertSearchGoodItems()
	{
		if (SearchGoodItems.Count <= 0)
		{
			return;
		}
		string text = "";
		foreach (EventAwardInfo searchGoodItem in SearchGoodItems)
		{
			text += $"{searchGoodItem.TemplateID},{searchGoodItem.Position},{searchGoodItem.Count}|";
		}
		text = text.Substring(0, text.Length - 1);
		lock (m_lock)
		{
			m_info.SearchGoodItems = text;
		}
	}

	public void RollDiceCallBack(bool isRemindRollBind)
	{
		int num = rand.Next(1, 6);
		bool flag = false;
		if (Info.FreeCount > 0)
		{
			Info.FreeCount--;
			flag = true;
		}
		else if (Player.MoneyDirect(GameProperties.SearchGoodsPayMoney))
		{
			flag = true;
		}
		if (flag)
		{
			m_info.nowPosition += num;
			if (m_info.nowPosition > 35)
			{
				m_info.nowPosition = 35;
			}
			GSPacketIn gSPacketIn = new GSPacketIn(98);
			gSPacketIn.WriteByte(17);
			gSPacketIn.WriteInt(m_info.FreeCount);
			if (GameProperties.VERSION >= 7600)
			{
				gSPacketIn.WriteInt(7);
			}
			gSPacketIn.WriteInt(num);
			gSPacketIn.WriteInt(m_info.nowPosition);
			Player.SendTCP(gSPacketIn);
		}
		EventAwardInfo awardByPos = GetAwardByPos();
		if (awardByPos != null)
		{
			switch (awardByPos.TemplateID)
			{
			case -1:
				m_info.nowPosition = 0;
				PlayNowPosition(awardByPos);
				break;
			case -2:
				m_info.nowPosition--;
				PlayNowPosition(awardByPos);
				break;
			case -3:
				m_info.nowPosition++;
				PlayNowPosition(awardByPos);
				break;
			case -4:
				m_info.nowPosition = 35;
				PlayNowPosition(awardByPos);
				break;
			default:
				UpdateGoodItems(awardByPos);
				break;
			case 0:
				break;
			}
		}
		GetEndAward();
	}

	public void RollDiceCallBack()
	{
		EventAwardInfo awardByPos = GetAwardByPos();
		if (awardByPos != null)
		{
			switch (awardByPos.TemplateID)
			{
			case -1:
				m_info.nowPosition = 0;
				PlayNowPosition(awardByPos);
				break;
			case -2:
				m_info.nowPosition--;
				PlayNowPosition(awardByPos);
				break;
			case -3:
				m_info.nowPosition++;
				PlayNowPosition(awardByPos);
				break;
			case -4:
				m_info.nowPosition = 35;
				PlayNowPosition(awardByPos);
				break;
			default:
				UpdateGoodItems(awardByPos);
				break;
			case 0:
				break;
			}
		}
		GetEndAward();
	}

	public void GetEndAward()
	{
		if (m_info.nowPosition == 35)
		{
			SearchGoodsTempInfo searchGoodsTempInfo = SearchGoodsMgr.GetSearchGoodsTempInfo(m_info.starlevel);
			if (searchGoodsTempInfo != null)
			{
				AddGoods(searchGoodsTempInfo.DestinationReward, 1);
			}
		}
	}

	public void PlayNowPosition(EventAwardInfo good)
	{
		if (good.TemplateID == -2 || good.TemplateID == -3)
		{
			UpdateGoodItems(good);
			RollDiceCallBack();
		}
		GSPacketIn gSPacketIn = new GSPacketIn(98);
		gSPacketIn.WriteByte(25);
		gSPacketIn.WriteInt(m_info.nowPosition);
		Player.SendTCP(gSPacketIn);
	}

	private void AddGoods(int goodId, int count)
	{
		if (goodId > 0)
		{
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(goodId);
			if (itemTemplateInfo != null)
			{
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 105);
				itemInfo.IsBinds = true;
				itemInfo.Count = count;
				Player.AddTemplate(itemInfo, itemInfo.Template.BagType, count, eGameView.OtherTypeGet, "Bản đồ kho báu");
			}
		}
	}

	private EventAwardInfo GetAwardByPos()
	{
		lock (m_lock)
		{
			foreach (EventAwardInfo searchGoodItem in m_searchGoodItems)
			{
				if (searchGoodItem.Position == m_info.nowPosition)
				{
					return searchGoodItem;
				}
			}
		}
		return null;
	}

	private void AddBuriedQuest()
	{
		if (m_buriedQuests != null && m_buriedQuests.Length != 0)
		{
			int num = rand.Next(m_buriedQuests.Length);
			QuestInfo info = m_buriedQuests[num];
			Player.QuestInventory.AddQuest(info, out var _);
		}
	}

	private void CreateTakeCard()
	{
		m_flopCard.Clear();
		for (int i = 0; i < 5; i++)
		{
			int count = rand.Next(5, 75);
			FlopCardInfo flopCardInfo = new FlopCardInfo();
			flopCardInfo.Count = count;
			flopCardInfo.TemplateID = 11680;
			m_flopCard.Add(flopCardInfo);
		}
		takeCardLimit = 3;
		ThreadSafeRandom.ShufferListStatic(m_flopCard);
		GSPacketIn gSPacketIn = new GSPacketIn(98);
		gSPacketIn.WriteByte(24);
		gSPacketIn.WriteInt(takeCardLimit);
		gSPacketIn.WriteInt(m_flopCard.Count);
		foreach (FlopCardInfo item in m_flopCard)
		{
			gSPacketIn.WriteInt(item.TemplateID);
			gSPacketIn.WriteInt(item.Count);
		}
		Player.SendTCP(gSPacketIn);
	}

	public void TakeCard(bool UseMoney)
	{
		FlopCardInfo flopCardInfo = null;
		string[] array = GameProperties.SearchGoodsTakeCardMoney.Split('|');
		if (takeCardLimit > 0)
		{
			if (Player.MoneyDirect(int.Parse(array[3 - takeCardLimit])))
			{
				ThreadSafeRandom.ShufferListStatic(m_flopCard);
				flopCardInfo = m_flopCard[takeCardLimit];
				AddGoods(flopCardInfo.TemplateID, flopCardInfo.Count);
				takeCardLimit--;
				if (flopCardInfo != null)
				{
					GSPacketIn gSPacketIn = new GSPacketIn(98);
					gSPacketIn.WriteByte(32);
					gSPacketIn.WriteInt(takeCardLimit);
					gSPacketIn.WriteInt(flopCardInfo.TemplateID);
					gSPacketIn.WriteInt(flopCardInfo.Count);
					Player.SendTCP(gSPacketIn);
				}
			}
		}
		else
		{
			Player.SendMessage(LanguageMgr.GetTranslation("PlayerExtra.TakeCard"));
		}
	}

	public void UpdateGoodItems(EventAwardInfo good)
	{
		if (good.TemplateID == -5)
		{
			CreateTakeCard();
		}
		if (good.TemplateID == -6)
		{
			AddBuriedQuest();
		}
		AddGoods(good.TemplateID, good.Count);
		for (int i = 0; i < m_searchGoodItems.Count; i++)
		{
			if (m_searchGoodItems[i].Position == good.Position)
			{
				m_searchGoodItems[i].TemplateID = 0;
				break;
			}
		}
		GSPacketIn gSPacketIn = new GSPacketIn(98);
		gSPacketIn.WriteByte(23);
		gSPacketIn.WriteInt((good.TemplateID >= 0) ? good.TemplateID : 0);
		Player.SendTCP(gSPacketIn);
	}

	public void CreateSaveLifeBuff()
	{
		if (m_player.PlayerCharacter.VIPLevel >= 4 && !m_player.PlayerCharacter.IsVIPExpire())
		{
			BufferList.CreateSaveLifeBuffer(3)?.Start(Player);
		}
		else
		{
			BufferList.CreateSaveLifeBuffer(0)?.Start(Player);
		}
	}

	public void MagpieBridgeRollDiceCallBack(bool isRemindRollBind)
	{
		int num = rand.Next(1, 6);
		bool flag = false;
		if (Info.FreeCount > 0)
		{
			Info.FreeCount--;
			flag = true;
		}
		else if (Player.MoneyDirect(GameProperties.SearchGoodsPayMoney))
		{
			flag = true;
		}
		if (flag)
		{
			m_info.NowPositionMB += num;
			if (m_info.NowPositionMB > 35)
			{
				m_info.NowPositionMB = 35;
			}
			GSPacketIn gSPacketIn = new GSPacketIn(276);
			gSPacketIn.WriteByte(2);
			gSPacketIn.WriteInt(m_info.LastNum);
			gSPacketIn.WriteInt(num);
			gSPacketIn.WriteInt(m_info.NowPositionMB);
			Player.SendTCP(gSPacketIn);
		}
		EventAwardInfo magpieBridgeAwardByPos = GetMagpieBridgeAwardByPos();
		if (magpieBridgeAwardByPos != null)
		{
			switch (magpieBridgeAwardByPos.TemplateID)
			{
			case 0:
				break;
			case -1:
				m_info.NowPositionMB = 0;
				MagpieBridgePlayNowPosition(magpieBridgeAwardByPos);
				break;
			case -2:
				m_info.NowPositionMB--;
				MagpieBridgePlayNowPosition(magpieBridgeAwardByPos);
				break;
			case -3:
				m_info.NowPositionMB++;
				MagpieBridgePlayNowPosition(magpieBridgeAwardByPos);
				break;
			case -4:
				m_info.NowPositionMB = 35;
				MagpieBridgePlayNowPosition(magpieBridgeAwardByPos);
				break;
			default:
				UpdateMagpieBridgeGoodItems(magpieBridgeAwardByPos);
				break;
			}
		}
	}

	public void MagpieBridgePlayNowPosition(EventAwardInfo good)
	{
		if (good.TemplateID == -2 || good.TemplateID == -3)
		{
			UpdateMagpieBridgeGoodItems(good);
			MagpieBridgeRollDiceCallBack();
		}
		GSPacketIn gSPacketIn = new GSPacketIn(276);
		gSPacketIn.WriteByte(7);
		gSPacketIn.WriteInt(m_info.NowPositionMB);
		Player.SendTCP(gSPacketIn);
	}

	public void MagpieBridgeRollDiceCallBack()
	{
		EventAwardInfo awardByPos = GetAwardByPos();
		if (awardByPos != null)
		{
			switch (awardByPos.TemplateID)
			{
			case 0:
				break;
			case -1:
				m_info.NowPositionMB = 0;
				MagpieBridgePlayNowPosition(awardByPos);
				break;
			case -2:
				m_info.NowPositionMB--;
				MagpieBridgePlayNowPosition(awardByPos);
				break;
			case -3:
				m_info.NowPositionMB++;
				MagpieBridgePlayNowPosition(awardByPos);
				break;
			case -4:
				m_info.NowPositionMB = 35;
				MagpieBridgePlayNowPosition(awardByPos);
				break;
			default:
				UpdateMagpieBridgeGoodItems(awardByPos);
				break;
			}
		}
	}

	public void UpdateMagpieBridgeGoodItems(EventAwardInfo good)
	{
		if (Info.NowPositionMB == 35)
		{
			AddGoods(good.TemplateID, 1);
			m_info.MagpieNum += 2;
		}
		else
		{
			if (good.TemplateID == -6)
			{
				m_info.MagpieNum++;
				UpdateMagpieNum();
			}
			AddGoods(good.TemplateID, good.Count);
			for (int i = 0; i < m_magpieBridgeItems.Count; i++)
			{
				if (m_magpieBridgeItems[i].Position == good.Position)
				{
					m_magpieBridgeItems[i].TemplateID = 0;
					break;
				}
			}
			GSPacketIn gSPacketIn = new GSPacketIn(276);
			gSPacketIn.WriteByte(6);
			gSPacketIn.WriteInt((good.TemplateID >= 0) ? good.TemplateID : 0);
			Player.SendTCP(gSPacketIn);
		}
		if (Info.MagpieNum >= 10)
		{
			m_info.NowPositionMB = 35;
			GSPacketIn gSPacketIn2 = new GSPacketIn(276);
			gSPacketIn2.WriteByte(7);
			gSPacketIn2.WriteInt(m_info.NowPositionMB);
			Player.SendTCP(gSPacketIn2);
			AddGoods(1120155, 1);
			GSPacketIn gSPacketIn3 = new GSPacketIn(276);
			gSPacketIn3.WriteByte(12);
			Player.SendTCP(gSPacketIn3);
			Info.MagpieNum = 0;
		}
	}

	public void UpdateMagpieNum()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(276);
		gSPacketIn.WriteByte(10);
		gSPacketIn.WriteInt(Info.MagpieNum);
		Player.SendTCP(gSPacketIn);
	}

	private EventAwardInfo GetMagpieBridgeAwardByPos()
	{
		lock (m_lock)
		{
			foreach (EventAwardInfo magpieBridgeItem in m_magpieBridgeItems)
			{
				if (magpieBridgeItem.Position == m_info.NowPositionMB)
				{
					return magpieBridgeItem;
				}
			}
		}
		return null;
	}

	public List<EventAwardInfo> CreateMagpieBridgeAward()
	{
		Dictionary<int, EventAwardInfo> dictionary = new Dictionary<int, EventAwardInfo>();
		int[] array = new int[5] { -1, -2, -3, -4, -7 };
		ThreadSafeRandom.ShufferStatic(array);
		int num = rand.Next(TanabataActiveEventNum.Length - 1);
		string[] array2 = TanabataActiveEventNum[num].Split(',');
		int count = int.Parse(array2[0]);
		int num2 = int.Parse(array2[1]);
		int num3 = int.Parse(array2[2]);
		List<int> list = CreateRandomPos();
		List<EventAwardInfo> list2 = new List<EventAwardInfo>();
		int num4 = 0;
		EventAwardInfo[] array3 = EventAwardMgr.CreateMagpieBridgeAward(count);
		for (int i = 0; i < array3.Length; i++)
		{
			num4 = list[i];
			EventAwardInfo eventAwardInfo = array3[i];
			eventAwardInfo.Position = num4;
			list2.Add(eventAwardInfo);
			list.Remove(num4);
		}
		for (int j = 0; j < num2; j++)
		{
			num4 = list[j];
			int num5 = rand.Next(array.Length);
			list2.Add(GetSpecialTem(array[num5], num4));
			list.Remove(num4);
		}
		for (int k = 0; k < num3; k++)
		{
			num4 = list[k];
			list2.Add(GetSpecialTem(-6, num4));
			list.Remove(num4);
		}
		num4 = rand.Next(FinalReward.Length - 1);
		list2.Add(GetSpecialTem(int.Parse(FinalReward[num4]), 35));
		return list2;
	}

	public void CreateMagpieBridgeItems(bool Refresh)
	{
		if (Refresh)
		{
			m_magpieBridgeItems.Clear();
			m_info.NowPositionMB = 0;
		}
		List<EventAwardInfo> list = CreateMagpieBridgeAward();
		lock (m_lock)
		{
			foreach (EventAwardInfo item in list)
			{
				m_magpieBridgeItems.Add(item);
			}
		}
	}

	public void GetMagpieBridgeItemsDb()
	{
		m_magpieBridgeItems.Clear();
		if (string.IsNullOrEmpty(m_info.MagpieBridgeItems))
		{
			CreateMagpieBridgeItems(Refresh: false);
			return;
		}
		string[] array = m_info.MagpieBridgeItems.Split('|');
		string[] array2 = array;
		foreach (string text in array2)
		{
			EventAwardInfo eventAwardInfo = new EventAwardInfo();
			eventAwardInfo.TemplateID = int.Parse(text.Split(',')[0]);
			eventAwardInfo.Position = int.Parse(text.Split(',')[1]);
			eventAwardInfo.Count = int.Parse(text.Split(',')[2]);
			m_magpieBridgeItems.Add(eventAwardInfo);
		}
	}

	public void ConvertMagpieBridgeItems()
	{
		if (MagpieBridgeItems.Count <= 0)
		{
			return;
		}
		string text = "";
		foreach (EventAwardInfo magpieBridgeItem in MagpieBridgeItems)
		{
			text += $"{magpieBridgeItem.TemplateID},{magpieBridgeItem.Position},{magpieBridgeItem.Count}|";
		}
		text = text.Substring(0, text.Length - 1);
		lock (m_lock)
		{
			m_info.MagpieBridgeItems = text;
		}
	}

	public GSPacketIn SendMagpieBridgeEnter()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(276, Player.PlayerId);
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteInt(MapId);
		gSPacketIn.WriteInt(Info.NowPositionMB);
		gSPacketIn.WriteInt(Info.LastNum);
		gSPacketIn.WriteInt(Info.MagpieNum);
		gSPacketIn.WriteInt(MagpieBridgeItems.Count);
		foreach (EventAwardInfo magpieBridgeItem in MagpieBridgeItems)
		{
			gSPacketIn.WriteInt(magpieBridgeItem.Position);
			gSPacketIn.WriteInt(magpieBridgeItem.TemplateID);
		}
		Player.SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public Dictionary<int, int> GetDungeonCount()
	{
		Dictionary<int, int> dictionary = JsonConvert.DeserializeObject<Dictionary<int, int>>(m_info.dungeonCount);
		if (dictionary == null)
		{
			dictionary = new Dictionary<int, int>();
		}
		return dictionary;
	}

	public void UpdateDungeonCount(int mapId)
	{
		Dictionary<int, int> dungeonCount = GetDungeonCount();
		if (dungeonCount.ContainsKey(mapId))
		{
			dungeonCount[mapId]++;
		}
		else
		{
			dungeonCount.Add(mapId, 1);
		}
		m_info.dungeonCount = JsonConvert.SerializeObject(dungeonCount);
	}
}
