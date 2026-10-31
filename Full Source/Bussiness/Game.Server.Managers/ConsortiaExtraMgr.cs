using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Bussiness;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Managers;

public class ConsortiaExtraMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, ConsortiaLevelInfo> _consortiaLevel = new Dictionary<int, ConsortiaLevelInfo>();

	private static Dictionary<int, ConsortiaBufferTempInfo> _consortiaBuffTemp = new Dictionary<int, ConsortiaBufferTempInfo>();

	private static Dictionary<int, ConsortiaBadgeConfigInfo> m_consortiaBadgeConfigs = new Dictionary<int, ConsortiaBadgeConfigInfo>();

	private static ReaderWriterLock m_clientLocker = new ReaderWriterLock();

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, ConsortiaLevelInfo> consortiaLevel = new Dictionary<int, ConsortiaLevelInfo>();
			Dictionary<int, ConsortiaBufferTempInfo> consortiaBuffTemp = new Dictionary<int, ConsortiaBufferTempInfo>();
			Dictionary<int, ConsortiaBadgeConfigInfo> dictionary = LoadFromDatabase();
			if (Load(consortiaLevel, consortiaBuffTemp))
			{
				try
				{
					_consortiaLevel = consortiaLevel;
					_consortiaBuffTemp = consortiaBuffTemp;
					if (dictionary.Values.Count > 0)
					{
						Interlocked.Exchange(ref m_consortiaBadgeConfigs, dictionary);
					}
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
				log.Error("ConsortiaLevelMgr", exception);
			}
		}
		return false;
	}

	public static bool Init()
	{
		try
		{
			return ReLoad();
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ConsortiaLevelMgr", exception);
			}
			return false;
		}
	}

	private static Dictionary<int, ConsortiaBadgeConfigInfo> LoadFromDatabase()
	{
		Dictionary<int, ConsortiaBadgeConfigInfo> dictionary = new Dictionary<int, ConsortiaBadgeConfigInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			ConsortiaBadgeConfigInfo[] allConsortiaBadgeConfig = produceBussiness.GetAllConsortiaBadgeConfig();
			ConsortiaBadgeConfigInfo[] array = allConsortiaBadgeConfig;
			foreach (ConsortiaBadgeConfigInfo consortiaBadgeConfigInfo in array)
			{
				if (!dictionary.ContainsKey(consortiaBadgeConfigInfo.BadgeID))
				{
					dictionary.Add(consortiaBadgeConfigInfo.BadgeID, consortiaBadgeConfigInfo);
				}
			}
		}
		return dictionary;
	}

	public static ConsortiaBadgeConfigInfo FindConsortiaBadgeConfig(int id)
	{
		if (m_consortiaBadgeConfigs.ContainsKey(id))
		{
			return m_consortiaBadgeConfigs[id];
		}
		return null;
	}

	private static bool Load(Dictionary<int, ConsortiaLevelInfo> consortiaLevel, Dictionary<int, ConsortiaBufferTempInfo> consortiaBuffTemp)
	{
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			ConsortiaLevelInfo[] allConsortiaLevel = produceBussiness.GetAllConsortiaLevel();
			ConsortiaLevelInfo[] array = allConsortiaLevel;
			foreach (ConsortiaLevelInfo consortiaLevelInfo in array)
			{
				if (!consortiaLevel.ContainsKey(consortiaLevelInfo.Level))
				{
					consortiaLevel.Add(consortiaLevelInfo.Level, consortiaLevelInfo);
				}
			}
			ConsortiaBufferTempInfo[] allConsortiaBuffTemp = produceBussiness.GetAllConsortiaBuffTemp();
			ConsortiaBufferTempInfo[] array2 = allConsortiaBuffTemp;
			foreach (ConsortiaBufferTempInfo consortiaBufferTempInfo in array2)
			{
				if (!consortiaBuffTemp.ContainsKey(consortiaBufferTempInfo.id))
				{
					consortiaBuffTemp.Add(consortiaBufferTempInfo.id, consortiaBufferTempInfo);
				}
			}
		}
		return true;
	}

	public static ConsortiaLevelInfo FindConsortiaLevelInfo(int level)
	{
		try
		{
			if (_consortiaLevel.ContainsKey(level))
			{
				return _consortiaLevel[level];
			}
		}
		catch
		{
		}
		return null;
	}

	public static ConsortiaBufferTempInfo FindConsortiaBuffInfo(int id)
	{
		m_clientLocker.AcquireWriterLock(-1);
		try
		{
			if (_consortiaBuffTemp.ContainsKey(id))
			{
				return _consortiaBuffTemp[id];
			}
		}
		finally
		{
			m_clientLocker.ReleaseWriterLock();
		}
		return null;
	}

	public static List<ConsortiaBufferTempInfo> GetAllConsortiaBuff()
	{
		List<ConsortiaBufferTempInfo> list = new List<ConsortiaBufferTempInfo>();
		try
		{
			foreach (ConsortiaBufferTempInfo value in _consortiaBuffTemp.Values)
			{
				list.Add(value);
			}
		}
		catch
		{
		}
		return list;
	}
}
