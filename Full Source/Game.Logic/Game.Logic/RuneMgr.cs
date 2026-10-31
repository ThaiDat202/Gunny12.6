using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bussiness;
using log4net;
using SqlDataProvider.Data;

namespace Game.Logic;

public class RuneMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, RuneTemplateInfo> _items;

	private static ReaderWriterLock m_clientLocker = new ReaderWriterLock();

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, RuneTemplateInfo> dictionary = new Dictionary<int, RuneTemplateInfo>();
			if (LoadItem(dictionary))
			{
				try
				{
					_items = dictionary;
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
				log.Error("ReLoad", exception);
			}
		}
		return false;
	}

	public static bool Init()
	{
		try
		{
			_items = new Dictionary<int, RuneTemplateInfo>();
			return LoadItem(_items);
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("Init", exception);
			}
			return false;
		}
	}

	public static bool LoadItem(Dictionary<int, RuneTemplateInfo> infos)
	{
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			RuneTemplateInfo[] allRuneTemplate = produceBussiness.GetAllRuneTemplate();
			RuneTemplateInfo[] array = allRuneTemplate;
			foreach (RuneTemplateInfo runeTemplateInfo in array)
			{
				if (!infos.Keys.Contains(runeTemplateInfo.TemplateID))
				{
					infos.Add(runeTemplateInfo.TemplateID, runeTemplateInfo);
				}
			}
		}
		return true;
	}

	public static RuneTemplateInfo FindRuneByTemplateID(int templateID)
	{
		m_clientLocker.AcquireWriterLock(-1);
		try
		{
			if (!_items.Keys.Contains(templateID))
			{
				return null;
			}
		}
		finally
		{
			m_clientLocker.ReleaseWriterLock();
		}
		return _items[templateID];
	}

	public static RuneTemplateInfo FindRuneByTemplateID(int templateID, int lv)
	{
		List<RuneTemplateInfo> listRuneByTemplate = GetListRuneByTemplate(templateID);
		foreach (RuneTemplateInfo item in listRuneByTemplate)
		{
			if (item.BaseLevel >= lv)
			{
				return item;
			}
		}
		return null;
	}

	public static List<RuneTemplateInfo> GetListRuneByTemplate(int templateID)
	{
		List<RuneTemplateInfo> list = new List<RuneTemplateInfo>();
		int num = templateID;
		foreach (RuneTemplateInfo value in _items.Values)
		{
			if (value.TemplateID == num)
			{
				list.Add(value);
				num = value.NextTemplateID;
			}
		}
		return list;
	}

	public static int FindRuneExp(int lv)
	{
		if (lv < 0)
		{
			return 1;
		}
		return GameProperties.RuneExp()[lv];
	}

	public static int GetRuneLevel(int GP)
	{
		List<int> list = GameProperties.RuneExp();
		if (GP >= list[MaxLv() - 1])
		{
			return MaxLv();
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (GP < list[i])
			{
				return i;
			}
		}
		return 1;
	}

	public static int MaxLv()
	{
		return GameProperties.RuneExp().Count;
	}

	public static int MaxExp()
	{
		List<int> list = GameProperties.RuneExp();
		int index = ((MaxLv() - 1 >= 0) ? (MaxLv() - 1) : 0);
		return list[index];
	}
}
