using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class GoldEquipMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, GoldEquipTemplateInfo> _items;

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, GoldEquipTemplateInfo> dictionary = new Dictionary<int, GoldEquipTemplateInfo>();
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
			_items = new Dictionary<int, GoldEquipTemplateInfo>();
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

	public static bool LoadItem(Dictionary<int, GoldEquipTemplateInfo> infos)
	{
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			GoldEquipTemplateInfo[] allGoldEquipTemplateLoad = produceBussiness.GetAllGoldEquipTemplateLoad();
			GoldEquipTemplateInfo[] array = allGoldEquipTemplateLoad;
			foreach (GoldEquipTemplateInfo goldEquipTemplateInfo in array)
			{
				if (!infos.Keys.Contains(goldEquipTemplateInfo.OldTemplateId))
				{
					infos.Add(goldEquipTemplateInfo.OldTemplateId, goldEquipTemplateInfo);
				}
			}
		}
		return true;
	}

	public static GoldEquipTemplateInfo FindGoldEquipByTemplate(int templateId)
	{
		if (_items == null)
		{
			Init();
		}
		try
		{
			if (_items.Keys.Contains(templateId))
			{
				return _items[templateId];
			}
		}
		catch
		{
		}
		return null;
	}

	public static GoldEquipTemplateInfo FindGoldEquipNewTemplate(int TemplateId)
	{
		if (_items == null)
		{
			Init();
		}
		try
		{
			foreach (GoldEquipTemplateInfo value in _items.Values)
			{
				if (value.OldTemplateId == TemplateId)
				{
					return value;
				}
			}
		}
		catch
		{
		}
		return null;
	}

	public static GoldEquipTemplateInfo FindGoldEquipOldTemplate(int TemplateId)
	{
		if (_items == null)
		{
			Init();
		}
		try
		{
			foreach (GoldEquipTemplateInfo value in _items.Values)
			{
				string text = value.OldTemplateId.ToString();
				if (value.NewTemplateId == TemplateId && text.Substring(4) != "4")
				{
					return value;
				}
			}
		}
		catch
		{
		}
		return null;
	}
}
