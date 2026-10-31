using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class FusionMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, FusionInfo> _fusions;

	private static Random rand;

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, FusionInfo> dictionary = new Dictionary<int, FusionInfo>();
			if (LoadFusion(dictionary))
			{
				try
				{
					_fusions = dictionary;
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
				log.Error("FusionMgr", exception);
			}
		}
		return false;
	}

	public static bool Init()
	{
		try
		{
			_fusions = new Dictionary<int, FusionInfo>();
			rand = new Random();
			return LoadFusion(_fusions);
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("FusionMgr", exception);
			}
			return false;
		}
	}

	private static bool LoadFusion(Dictionary<int, FusionInfo> fusion)
	{
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			FusionInfo[] allFusion = produceBussiness.GetAllFusion();
			FusionInfo[] array = allFusion;
			foreach (FusionInfo fusionInfo in array)
			{
				if (!fusion.ContainsKey(fusionInfo.FusionID))
				{
					fusion.Add(fusionInfo.FusionID, fusionInfo);
				}
			}
		}
		return true;
	}

	public static FusionInfo FindItemFusion(int fusionId)
	{
		if (_fusions.ContainsKey(fusionId))
		{
			return _fusions[fusionId];
		}
		return null;
	}
}
