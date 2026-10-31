using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class LocalItemMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, ItemTemplateInfo> _items;

	private static Dictionary<int, ItemTemplateInfo> _newItems;

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, ItemTemplateInfo> dictionary = new Dictionary<int, ItemTemplateInfo>();
			_newItems = new Dictionary<int, ItemTemplateInfo>();
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
			_items = new Dictionary<int, ItemTemplateInfo>();
			_newItems = new Dictionary<int, ItemTemplateInfo>();
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

	public static bool LoadItem(Dictionary<int, ItemTemplateInfo> infos)
	{
		using (ProduceBussiness produceBussiness = new ProduceBussiness(null))
		{
			ItemTemplateInfo[] allGoods = produceBussiness.GetAllGoods();
			ItemTemplateInfo[] array = allGoods;
			foreach (ItemTemplateInfo itemTemplateInfo in array)
			{
				if (!infos.Keys.Contains(itemTemplateInfo.TemplateID))
				{
					infos.Add(itemTemplateInfo.TemplateID, itemTemplateInfo);
				}
			}
		}
		return true;
	}

	public static bool FindLocalItemTemplate(int templateId)
	{
		if (_items == null)
		{
			return false;
		}
		if (_items.Keys.Contains(templateId))
		{
			return true;
		}
		return false;
	}

	public static void AddNewLocalItemTemplate(ItemTemplateInfo template)
	{
		if (_newItems != null && !_newItems.Keys.Contains(template.TemplateID))
		{
			_newItems.Add(template.TemplateID, template);
		}
	}

	public static void UpdateNewLocalItemToDb(bool isUp)
	{
		if (_items == null || _newItems == null || !isUp)
		{
			return;
		}
		using (ProduceBussiness produceBussiness = new ProduceBussiness(null))
		{
			foreach (ItemTemplateInfo value in _newItems.Values)
			{
				produceBussiness.AddItemTemplate(value);
			}
		}
		_newItems.Clear();
	}
}
