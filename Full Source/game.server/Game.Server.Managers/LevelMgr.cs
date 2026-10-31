using System;
using System.Collections.Generic;
using System.Reflection;
using Bussiness;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Managers;

public class LevelMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, LevelInfo> _levels;

	private static Random rand;

	public static int MaxLevel => _levels.Count;

	public static bool Init()
	{
		try
		{
			_levels = new Dictionary<int, LevelInfo>();
			rand = new Random();
			return LoadLevel(_levels);
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("LevelMgr", exception);
			}
			return false;
		}
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, LevelInfo> dictionary = new Dictionary<int, LevelInfo>();
			if (LoadLevel(dictionary))
			{
				try
				{
					_levels = dictionary;
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
				log.Error("LevelMgr", exception);
			}
		}
		return false;
	}

	private static bool LoadLevel(Dictionary<int, LevelInfo> Level)
	{
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			LevelInfo[] allLevel = produceBussiness.GetAllLevel();
			LevelInfo[] array = allLevel;
			foreach (LevelInfo levelInfo in array)
			{
				if (!Level.ContainsKey(levelInfo.Grade))
				{
					Level.Add(levelInfo.Grade, levelInfo);
				}
			}
		}
		return true;
	}

	public static LevelInfo FindLevel(int Grade)
	{
		if (_levels.ContainsKey(Grade))
		{
			return _levels[Grade];
		}
		return null;
	}

	public static int LevelPlusBlood(int Grade)
	{
		if (_levels.ContainsKey(Grade))
		{
			return _levels[Grade].Blood;
		}
		log.ErrorFormat("Level {0} not found!", Grade);
		return 0;
	}

	public static int GetLevel(long GP)
	{
		if (GP >= FindLevel(MaxLevel).GP)
		{
			return MaxLevel;
		}
		for (int i = 1; i <= MaxLevel; i++)
		{
			if (GP < FindLevel(i).GP)
			{
				return (i - 1 == 0) ? 1 : (i - 1);
			}
		}
		return 1;
	}

	public static int GetGP(int level)
	{
		if (MaxLevel > level && level > 0)
		{
			return FindLevel(level - 1).GP;
		}
		return 0;
	}

	public static int ReduceGP(int level, int totalGP)
	{
		if (MaxLevel > level && level > 0)
		{
			totalGP -= FindLevel(level - 1).GP;
			if (totalGP < level * 12)
			{
				return (totalGP >= 0) ? totalGP : 0;
			}
			return level * 12;
		}
		return 0;
	}

	public static int IncreaseGP(int level, int totalGP)
	{
		if (MaxLevel > level && level > 0)
		{
			return level * 12;
		}
		return 0;
	}
}
