using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class MagicFusionDataMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, MagicFusionDataInfo> m_MagicFusionDatas = new Dictionary<int, MagicFusionDataInfo>();

	private static Random random = new Random();

	public static bool ReLoad()
	{
		try
		{
			MagicFusionDataInfo[] array = LoadMagicFusionDataDb();
			Dictionary<int, MagicFusionDataInfo> value = LoadMagicFusionDatas(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_MagicFusionDatas, value);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad MagicFusionData", exception);
			}
			return false;
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static MagicFusionDataInfo[] LoadMagicFusionDataDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllMagicFusionData();
	}

	public static Dictionary<int, MagicFusionDataInfo> LoadMagicFusionDatas(MagicFusionDataInfo[] MagicFusionData)
	{
		Dictionary<int, MagicFusionDataInfo> dictionary = new Dictionary<int, MagicFusionDataInfo>();
		foreach (MagicFusionDataInfo magicFusionDataInfo in MagicFusionData)
		{
			if (!dictionary.Keys.Contains(magicFusionDataInfo.ID))
			{
				dictionary.Add(magicFusionDataInfo.ID, magicFusionDataInfo);
			}
		}
		return dictionary;
	}

	public static MagicFusionDataInfo FindMagicFusionData(int ID)
	{
		if (m_MagicFusionDatas.ContainsKey(ID))
		{
			return m_MagicFusionDatas[ID];
		}
		return null;
	}

	public static MagicFusionDataInfo GetMagicFusionData(int itemID)
	{
		foreach (MagicFusionDataInfo value in m_MagicFusionDatas.Values)
		{
			if (value.ItemID == itemID)
			{
				return value;
			}
		}
		return null;
	}
}
