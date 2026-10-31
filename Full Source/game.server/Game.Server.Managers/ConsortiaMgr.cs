using System;
using System.Collections.Generic;
using System.Reflection;
using Bussiness;
using Game.Logic;
using Game.Logic.Phy.Object;
using Game.Server.GameObjects;
using log4net;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server.Managers;

public class ConsortiaMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<string, int> _ally;

	private static Dictionary<int, ConsortiaInfo> _consortia;

	private static Dictionary<int, ConsortiaBossConfigInfo> _consortiaBossConfigInfos;

	private static List<ConsortiaRedPackageInfo> _consortiaRedPackage;

	public static bool ReLoad()
	{
		try
		{
			Dictionary<string, int> ally = new Dictionary<string, int>();
			Dictionary<int, ConsortiaInfo> consortia = new Dictionary<int, ConsortiaInfo>();
			Dictionary<int, ConsortiaBossConfigInfo> dictionary = new Dictionary<int, ConsortiaBossConfigInfo>();
			List<ConsortiaRedPackageInfo> list = new List<ConsortiaRedPackageInfo>();
			if (Load(ally) && LoadConsortia(consortia, dictionary) && LoadRedPackage(list))
			{
				try
				{
					_ally = ally;
					_consortia = consortia;
					_consortiaBossConfigInfos = dictionary;
					_consortiaRedPackage = list;
					return true;
				}
				catch
				{
				}
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ConsortiaMgr", exception);
			}
		}
		return false;
	}

	internal static void AddBuffConsortia(GamePlayer player, ConsortiaBufferTempInfo consortiaBuffTempInfo, int consortiaID, int id, int validate)
	{
		throw new NotImplementedException();
	}

	public static bool Init()
	{
		try
		{
			_ally = new Dictionary<string, int>();
			if (!Load(_ally))
			{
				return false;
			}
			_consortia = new Dictionary<int, ConsortiaInfo>();
			_consortiaBossConfigInfos = new Dictionary<int, ConsortiaBossConfigInfo>();
			_consortiaRedPackage = new List<ConsortiaRedPackageInfo>();
			if (!LoadConsortia(_consortia, _consortiaBossConfigInfos) || !LoadRedPackage(_consortiaRedPackage))
			{
				return false;
			}
			return true;
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ConsortiaMgr", exception);
			}
			return false;
		}
	}

	private static bool Load(Dictionary<string, int> ally)
	{
		using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
		{
			ConsortiaAllyInfo[] consortiaAllyAll = consortiaBussiness.GetConsortiaAllyAll();
			ConsortiaAllyInfo[] array = consortiaAllyAll;
			foreach (ConsortiaAllyInfo consortiaAllyInfo in array)
			{
				if (consortiaAllyInfo.IsExist)
				{
					string key = ((consortiaAllyInfo.Consortia1ID >= consortiaAllyInfo.Consortia2ID) ? (consortiaAllyInfo.Consortia2ID + "&" + consortiaAllyInfo.Consortia1ID) : (consortiaAllyInfo.Consortia1ID + "&" + consortiaAllyInfo.Consortia2ID));
					if (!ally.ContainsKey(key))
					{
						ally.Add(key, consortiaAllyInfo.State);
					}
				}
			}
		}
		return true;
	}

	private static bool LoadConsortia(Dictionary<int, ConsortiaInfo> consortia, Dictionary<int, ConsortiaBossConfigInfo> consortiaBossConfig)
	{
		using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
		{
			ConsortiaInfo[] consortiaAll = consortiaBussiness.GetConsortiaAll();
			ConsortiaInfo[] array = consortiaAll;
			foreach (ConsortiaInfo consortiaInfo in array)
			{
				if (consortiaInfo.IsExist && !consortia.ContainsKey(consortiaInfo.ConsortiaID))
				{
					consortia.Add(consortiaInfo.ConsortiaID, consortiaInfo);
				}
			}
		}
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			ConsortiaBossConfigInfo[] consortiaBossConfigAll = produceBussiness.GetConsortiaBossConfigAll();
			ConsortiaBossConfigInfo[] array2 = consortiaBossConfigAll;
			foreach (ConsortiaBossConfigInfo consortiaBossConfigInfo in array2)
			{
				if (!consortiaBossConfig.ContainsKey(consortiaBossConfigInfo.BossLevel))
				{
					consortiaBossConfig.Add(consortiaBossConfigInfo.BossLevel, consortiaBossConfigInfo);
				}
			}
		}
		return true;
	}

	private static bool LoadRedPackage(List<ConsortiaRedPackageInfo> redpackage)
	{
		using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
		{
			ConsortiaRedPackageInfo[] consortiaRedPackageAll = consortiaBussiness.GetConsortiaRedPackageAll();
			ConsortiaRedPackageInfo[] array = consortiaRedPackageAll;
			foreach (ConsortiaRedPackageInfo item in array)
			{
				redpackage.Add(item);
			}
		}
		return true;
	}

	public static bool Save()
	{
		lock (_consortiaRedPackage)
		{
			using ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness();
			foreach (ConsortiaRedPackageInfo item in _consortiaRedPackage)
			{
				if (item.IsDirty)
				{
					if (item.ID <= 0)
					{
						consortiaBussiness.AddConsortiaRedPackage(item);
					}
					else
					{
						consortiaBussiness.UpdateConsortiaRedPackage(item);
					}
				}
			}
		}
		return true;
	}

	public static int UpdateConsortiaAlly(int cosortiaID1, int consortiaID2, int state)
	{
		string key = ((cosortiaID1 >= consortiaID2) ? (consortiaID2 + "&" + cosortiaID1) : (cosortiaID1 + "&" + consortiaID2));
		if (!_ally.ContainsKey(key))
		{
			_ally.Add(key, state);
		}
		else
		{
			_ally[key] = state;
		}
		return 0;
	}

	public static bool ConsortiaUpGrade(int consortiaID, int consortiaLevel)
	{
		bool result = false;
		if (_consortia.ContainsKey(consortiaID) && _consortia[consortiaID].IsExist)
		{
			_consortia[consortiaID].Level = consortiaLevel;
		}
		else
		{
			ConsortiaInfo consortiaInfo = new ConsortiaInfo();
			consortiaInfo.BuildDate = DateTime.Now;
			consortiaInfo.Level = consortiaLevel;
			consortiaInfo.IsExist = true;
			_consortia.Add(consortiaID, consortiaInfo);
		}
		return result;
	}

	public static bool ConsortiaStoreUpGrade(int consortiaID, int storeLevel)
	{
		bool result = false;
		if (_consortia.ContainsKey(consortiaID) && _consortia[consortiaID].IsExist)
		{
			_consortia[consortiaID].StoreLevel = storeLevel;
		}
		return result;
	}

	public static bool ConsortiaShopUpGrade(int consortiaID, int shopLevel)
	{
		bool result = false;
		if (_consortia.ContainsKey(consortiaID) && _consortia[consortiaID].IsExist)
		{
			_consortia[consortiaID].ShopLevel = shopLevel;
		}
		return result;
	}

	public static bool ConsortiaSmithUpGrade(int consortiaID, int smithLevel)
	{
		bool result = false;
		if (_consortia.ContainsKey(consortiaID) && _consortia[consortiaID].IsExist)
		{
			_consortia[consortiaID].SmithLevel = smithLevel;
		}
		return result;
	}

	public static bool ConsortiaSkillUpGrade(int consortiaID, int skillLevel)
	{
		bool result = false;
		if (_consortia.ContainsKey(consortiaID) && _consortia[consortiaID].IsExist)
		{
			_consortia[consortiaID].SkillLevel = skillLevel;
		}
		return result;
	}

	public static bool AddConsortia(int consortiaID)
	{
		bool result = false;
		if (!_consortia.ContainsKey(consortiaID))
		{
			ConsortiaInfo consortiaInfo = new ConsortiaInfo();
			consortiaInfo.BuildDate = DateTime.Now;
			consortiaInfo.Level = 1;
			consortiaInfo.IsExist = true;
			consortiaInfo.ConsortiaName = "";
			consortiaInfo.ConsortiaID = consortiaID;
			_consortia.Add(consortiaID, consortiaInfo);
		}
		return result;
	}

	public static ConsortiaInfo FindConsortiaInfo(int consortiaID)
	{
		if (_consortia.ContainsKey(consortiaID))
		{
			return _consortia[consortiaID];
		}
		return null;
	}

	public static ConsortiaBossConfigInfo FindConsortiaBossConfig(int level)
	{
		if (_consortiaBossConfigInfos.ContainsKey(level))
		{
			return _consortiaBossConfigInfos[level];
		}
		return null;
	}

	public static int FindConsortiaBossMaxLevel(int param1, ConsortiaInfo info)
	{
		int num = 0;
		num = ((param1 != 0) ? param1 : (info.Level + info.SmithLevel + info.ShopLevel + info.StoreLevel + info.SkillLevel));
		int result = 1;
		for (int num2 = _consortiaBossConfigInfos.Count; num2 >= 0; num2--)
		{
			if (num >= _consortiaBossConfigInfos[num2].Level)
			{
				result = num2;
				break;
			}
		}
		return result;
	}

	public static int CanConsortiaFight(int consortiaID1, int consortiaID2)
	{
		if (consortiaID1 == 0 || consortiaID2 == 0 || consortiaID1 == consortiaID2)
		{
			return -1;
		}
		ConsortiaInfo consortiaInfo = FindConsortiaInfo(consortiaID1);
		ConsortiaInfo consortiaInfo2 = FindConsortiaInfo(consortiaID2);
		if (consortiaInfo == null || consortiaInfo2 == null || consortiaInfo.Level < 3 || consortiaInfo2.Level < 3)
		{
			return -1;
		}
		return FindConsortiaAlly(consortiaID1, consortiaID2);
	}

	public static int FindConsortiaAlly(int cosortiaID1, int consortiaID2)
	{
		if (cosortiaID1 == 0 || consortiaID2 == 0 || cosortiaID1 == consortiaID2)
		{
			return -1;
		}
		string key = ((cosortiaID1 >= consortiaID2) ? (consortiaID2 + "&" + cosortiaID1) : (cosortiaID1 + "&" + consortiaID2));
		if (_ally.ContainsKey(key))
		{
			return _ally[key];
		}
		return 0;
	}

	public static int GetOffer(int cosortiaID1, int consortiaID2, eGameType gameType)
	{
		return GetOffer(FindConsortiaAlly(cosortiaID1, consortiaID2), gameType);
	}

	private static int GetOffer(int state, eGameType gameType)
	{
		switch (gameType)
		{
		case eGameType.Free:
			switch (state)
			{
			case 0:
				return 1;
			case 1:
				return 0;
			case 2:
				return 3;
			}
			break;
		case eGameType.Guild:
			switch (state)
			{
			case 0:
				return 5;
			case 1:
				return 0;
			case 2:
				return 10;
			}
			break;
		}
		return 0;
	}

	public static int KillPlayer(GamePlayer win, GamePlayer lose, Dictionary<GamePlayer, Player> players, eRoomType roomType, eGameType gameClass)
	{
		if (roomType != 0)
		{
			return -1;
		}
		int num = FindConsortiaAlly(win.PlayerCharacter.ConsortiaID, lose.PlayerCharacter.ConsortiaID);
		if (num == -1)
		{
			return num;
		}
		int offer = GetOffer(num, gameClass);
		if (lose.PlayerCharacter.Offer < offer)
		{
			offer = lose.PlayerCharacter.Offer;
		}
		if (offer != 0)
		{
			players[win].GainOffer = offer;
			players[lose].GainOffer = -offer;
		}
		return num;
	}

	public static int ConsortiaFight(int consortiaWin, int consortiaLose, Dictionary<int, Player> players, eRoomType roomType, eGameType gameClass, int totalKillHealth, int playercount)
	{
		if (roomType != 0)
		{
			return 0;
		}
		int num = playercount / 2;
		int riches = 0;
		int state = 2;
		int num2 = 1;
		int num3 = 3;
		if (gameClass == eGameType.Guild)
		{
			num3 = 10;
			num2 = (int)RateMgr.GetRate(eRateType.Offer_Rate);
		}
		float rate = RateMgr.GetRate(eRateType.Riches_Rate);
		using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
		{
			if (gameClass == eGameType.Free)
			{
				num = 0;
			}
			else
			{
				consortiaBussiness.ConsortiaFight(consortiaWin, consortiaLose, num, out riches, state, totalKillHealth, rate);
			}
			foreach (KeyValuePair<int, Player> player in players)
			{
				if (player.Value != null)
				{
					if (player.Value.PlayerDetail.PlayerCharacter.ConsortiaID == consortiaWin)
					{
						player.Value.PlayerDetail.AddOffer((num + num3) * num2);
						player.Value.PlayerDetail.PlayerCharacter.RichesRob += riches;
					}
					else if (player.Value.PlayerDetail.PlayerCharacter.ConsortiaID == consortiaLose)
					{
						player.Value.PlayerDetail.AddOffer((int)Math.Round((double)num * 0.5) * num2);
						player.Value.PlayerDetail.RemoveOffer(num3);
					}
				}
			}
		}
		return riches;
	}

	public static void AddRedPackage(ConsortiaRedPackageInfo red)
	{
		lock (_consortiaRedPackage)
		{
			_consortiaRedPackage.Add(red);
		}
		Save();
	}

	public static ConsortiaRedPackageInfo GetRedPackage(int id)
	{
		lock (_consortiaRedPackage)
		{
			foreach (ConsortiaRedPackageInfo item in _consortiaRedPackage)
			{
				if (item.ID == id)
				{
					return item;
				}
			}
		}
		return null;
	}

	public static List<ConsortiaRedPackageInfo> GetRedPackageList(int consortiaId)
	{
		List<ConsortiaRedPackageInfo> list = new List<ConsortiaRedPackageInfo>();
		lock (_consortiaRedPackage)
		{
			foreach (ConsortiaRedPackageInfo item in _consortiaRedPackage)
			{
				if (item.ConsortiaID == consortiaId)
				{
					list.Add(item);
				}
			}
		}
		return list;
	}

	public static bool ReceiveRedPackage(GamePlayer player, int id)
	{
		ConsortiaRedPackageInfo redPackage = GetRedPackage(id);
		if (redPackage != null && redPackage.ConsortiaID == player.PlayerCharacter.ConsortiaID && redPackage.Package > 0)
		{
			Dictionary<int, ConsortiaRedPackageGainInfo> gainRedPackageInfo = GetGainRedPackageInfo(redPackage);
			if (!gainRedPackageInfo.ContainsKey(player.PlayerId))
			{
				ConsortiaRedPackageGainInfo consortiaRedPackageGainInfo = new ConsortiaRedPackageGainInfo();
				consortiaRedPackageGainInfo.UserID = player.PlayerId;
				consortiaRedPackageGainInfo.NickName = player.PlayerCharacter.NickName;
				consortiaRedPackageGainInfo.Money = redPackage.Money;
				consortiaRedPackageGainInfo.TimeGain = DateTime.Now;
				gainRedPackageInfo.Add(consortiaRedPackageGainInfo.UserID, consortiaRedPackageGainInfo);
				redPackage.Package--;
				redPackage.ReceiveData = JsonConvert.SerializeObject(gainRedPackageInfo);
				player.LogAddMoney(AddMoneyType.RedPackage, AddMoneyType.RedPackage_Add, player.PlayerCharacter.ID, consortiaRedPackageGainInfo.Money, player.PlayerCharacter.Money);
				WorldMgr.SendSysRedPackageNotice(player.PlayerCharacter.ConsortiaID, LanguageMgr.GetTranslation("RedPackage.Gain.SysNotice", player.PlayerCharacter.NickName, redPackage.NickName, redPackage.Money));
				player.AddMoney(consortiaRedPackageGainInfo.Money);
				return true;
			}
		}
		return false;
	}

	public static Dictionary<int, ConsortiaRedPackageGainInfo> GetGainRedPackageInfo(ConsortiaRedPackageInfo redInfo)
	{
		Dictionary<int, ConsortiaRedPackageGainInfo> dictionary = JsonConvert.DeserializeObject<Dictionary<int, ConsortiaRedPackageGainInfo>>(redInfo.ReceiveData);
		if (dictionary == null)
		{
			dictionary = new Dictionary<int, ConsortiaRedPackageGainInfo>();
		}
		return dictionary;
	}

	public static bool CheckGainRedPackage(int id, int userId)
	{
		ConsortiaRedPackageInfo redPackage = GetRedPackage(id);
		if (redPackage != null)
		{
			Dictionary<int, ConsortiaRedPackageGainInfo> gainRedPackageInfo = GetGainRedPackageInfo(redPackage);
			if (gainRedPackageInfo.ContainsKey(userId))
			{
				return true;
			}
		}
		return false;
	}
}
