using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class BattleBonusMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, BattleBonusInfo> m_battleBonuss = new Dictionary<int, BattleBonusInfo>();

	private static ThreadSafeRandom random = new ThreadSafeRandom();

	public static bool Init()
	{
		return ReLoad();
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, BattleBonusInfo> dictionary = LoadFromDatabase();
			if (dictionary.Values.Count > 0)
			{
				Interlocked.Exchange(ref m_battleBonuss, dictionary);
				return true;
			}
		}
		catch (Exception exception)
		{
			log.Error("BattleBonusMgr init error:", exception);
		}
		return false;
	}

	private static Dictionary<int, BattleBonusInfo> LoadFromDatabase()
	{
		Dictionary<int, BattleBonusInfo> dictionary = new Dictionary<int, BattleBonusInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			BattleBonusInfo[] allBattleBonus = produceBussiness.GetAllBattleBonus();
			BattleBonusInfo[] array = allBattleBonus;
			foreach (BattleBonusInfo battleBonusInfo in array)
			{
				if (!dictionary.ContainsKey(battleBonusInfo.ID))
				{
					dictionary.Add(battleBonusInfo.ID, battleBonusInfo);
				}
			}
		}
		return dictionary;
	}

	public static List<BattleBonusInfo> GetAllBattleBonus()
	{
		if (m_battleBonuss.Count == 0)
		{
			Init();
		}
		return m_battleBonuss.Values.ToList();
	}

	public static BattleBonusInfo FindBattleBonus(int id)
	{
		if (m_battleBonuss.Count == 0)
		{
			Init();
		}
		if (m_battleBonuss.ContainsKey(id))
		{
			return m_battleBonuss[id];
		}
		return null;
	}

	public static BattleBonusInfo FindBattleBonusByType(int id)
	{
		foreach (BattleBonusInfo value in m_battleBonuss.Values)
		{
			if (value.Type == id)
			{
				return value;
			}
		}
		return null;
	}

	public static List<BattleBonusInfo> GetExitBatleBonus()
	{
		List<BattleBonusInfo> list = new List<BattleBonusInfo>();
		foreach (BattleBonusInfo value in m_battleBonuss.Values)
		{
			if (value.IsExist)
			{
				list.Add(value);
			}
		}
		return list;
	}
}
