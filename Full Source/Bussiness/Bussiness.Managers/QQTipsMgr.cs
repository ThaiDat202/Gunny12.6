using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class QQTipsMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, QQtipsMessagesInfo> _qqtips;

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, QQtipsMessagesInfo> dictionary = new Dictionary<int, QQtipsMessagesInfo>();
			if (LoadItem(dictionary))
			{
				_qqtips = dictionary;
				return true;
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad QQTips", exception);
			}
		}
		return false;
	}

	public static bool Init()
	{
		try
		{
			_qqtips = new Dictionary<int, QQtipsMessagesInfo>();
			return LoadItem(_qqtips);
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("Init QQTips", exception);
			}
			return false;
		}
	}

	public static bool LoadItem(Dictionary<int, QQtipsMessagesInfo> infos)
	{
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			QQtipsMessagesInfo[] allQQtipsMessagesLoad = produceBussiness.GetAllQQtipsMessagesLoad();
			QQtipsMessagesInfo[] array = allQQtipsMessagesLoad;
			foreach (QQtipsMessagesInfo qQtipsMessagesInfo in array)
			{
				if (!infos.Keys.Contains(qQtipsMessagesInfo.ID))
				{
					infos.Add(qQtipsMessagesInfo.ID, qQtipsMessagesInfo);
				}
			}
		}
		return true;
	}

	public static QQtipsMessagesInfo GetQQtipsMessages(int zoneID)
	{
		if (_qqtips == null)
		{
			Init();
		}
		List<QQtipsMessagesInfo> list = new List<QQtipsMessagesInfo>();
		foreach (QQtipsMessagesInfo value in _qqtips.Values)
		{
			if (value.ZoneID == zoneID || value.ZoneID == -1)
			{
				list.Add(value);
			}
		}
		if (list.Count > 0)
		{
			int index = ThreadSafeRandom.NextStatic(list.Count);
			return list[index];
		}
		return null;
	}
}
