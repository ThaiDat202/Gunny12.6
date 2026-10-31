using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bussiness;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Managers;

public class ActiveSystermAwardMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, List<TreasurePuzzleRewardInfo>> m_TreasurePuzzleRewards = new Dictionary<int, List<TreasurePuzzleRewardInfo>>();

	private static Dictionary<int, List<BoguAdventureRewardInfo>> m_BoguAdventureRewards = new Dictionary<int, List<BoguAdventureRewardInfo>>();

	private static Random random = new Random();

	public static bool ReLoad()
	{
		try
		{
			TreasurePuzzleRewardInfo[] array = LoadTreasurePuzzleRewardDb();
			Dictionary<int, List<TreasurePuzzleRewardInfo>> value = LoadTreasurePuzzleRewards(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_TreasurePuzzleRewards, value);
			}
			BoguAdventureRewardInfo[] array2 = LoadBoguAdventureRewardDb();
			Dictionary<int, List<BoguAdventureRewardInfo>> value2 = LoadBoguAdventureRewards(array2);
			if (array2.Length != 0)
			{
				Interlocked.Exchange(ref m_BoguAdventureRewards, value2);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad ActiveSystermAwardMgr", exception);
			}
			return false;
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static BoguAdventureRewardInfo[] LoadBoguAdventureRewardDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllBoguAdventureReward();
	}

	public static Dictionary<int, List<BoguAdventureRewardInfo>> LoadBoguAdventureRewards(BoguAdventureRewardInfo[] BoguAdventureReward)
	{
		Dictionary<int, List<BoguAdventureRewardInfo>> dictionary = new Dictionary<int, List<BoguAdventureRewardInfo>>();
		foreach (BoguAdventureRewardInfo info in BoguAdventureReward)
		{
			if (!dictionary.Keys.Contains(info.AwardID))
			{
				IEnumerable<BoguAdventureRewardInfo> source = BoguAdventureReward.Where((BoguAdventureRewardInfo s) => s.AwardID == info.AwardID);
				dictionary.Add(info.AwardID, source.ToList());
			}
		}
		return dictionary;
	}

	public static List<BoguAdventureRewardInfo> FindBoguAdventureReward(int AwardID)
	{
		if (m_BoguAdventureRewards.ContainsKey(AwardID))
		{
			return m_BoguAdventureRewards[AwardID];
		}
		return null;
	}

	public static TreasurePuzzleRewardInfo[] LoadTreasurePuzzleRewardDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllTreasurePuzzleReward();
	}

	public static Dictionary<int, List<TreasurePuzzleRewardInfo>> LoadTreasurePuzzleRewards(TreasurePuzzleRewardInfo[] TreasurePuzzleReward)
	{
		Dictionary<int, List<TreasurePuzzleRewardInfo>> dictionary = new Dictionary<int, List<TreasurePuzzleRewardInfo>>();
		foreach (TreasurePuzzleRewardInfo info in TreasurePuzzleReward)
		{
			if (!dictionary.Keys.Contains(info.PuzzleID))
			{
				IEnumerable<TreasurePuzzleRewardInfo> source = TreasurePuzzleReward.Where((TreasurePuzzleRewardInfo s) => s.PuzzleID == info.PuzzleID);
				dictionary.Add(info.PuzzleID, source.ToList());
			}
		}
		return dictionary;
	}

	public static List<TreasurePuzzleRewardInfo> FindTreasurePuzzleReward(int PuzzleID)
	{
		if (m_TreasurePuzzleRewards.ContainsKey(PuzzleID))
		{
			return m_TreasurePuzzleRewards[PuzzleID];
		}
		return null;
	}
}
