using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class TotemHonorMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, TotemHonorTemplateInfo> _totemHonorTemplate;

	private static Random rand;

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, TotemHonorTemplateInfo> totemHonorTemplate = new Dictionary<int, TotemHonorTemplateInfo>();
			if (Load(totemHonorTemplate))
			{
				try
				{
					_totemHonorTemplate = totemHonorTemplate;
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
				log.Error("TotemHonorMgr", exception);
			}
		}
		return false;
	}

	public static bool Init()
	{
		try
		{
			_totemHonorTemplate = new Dictionary<int, TotemHonorTemplateInfo>();
			rand = new Random();
			return Load(_totemHonorTemplate);
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("TotemHonorMgr", exception);
			}
			return false;
		}
	}

	private static bool Load(Dictionary<int, TotemHonorTemplateInfo> TotemHonorTemplate)
	{
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			TotemHonorTemplateInfo[] allTotemHonorTemplate = produceBussiness.GetAllTotemHonorTemplate();
			TotemHonorTemplateInfo[] array = allTotemHonorTemplate;
			foreach (TotemHonorTemplateInfo totemHonorTemplateInfo in array)
			{
				if (!TotemHonorTemplate.ContainsKey(totemHonorTemplateInfo.ID))
				{
					TotemHonorTemplate.Add(totemHonorTemplateInfo.ID, totemHonorTemplateInfo);
				}
			}
		}
		return true;
	}

	public static TotemHonorTemplateInfo FindTotemHonorTemplateInfo(int ID)
	{
		if (_totemHonorTemplate.ContainsKey(ID))
		{
			return _totemHonorTemplate[ID];
		}
		return null;
	}
}
