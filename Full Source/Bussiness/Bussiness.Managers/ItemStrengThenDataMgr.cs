using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class ItemStrengThenDataMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, List<ItemStrengThenDataInfo>> m_ItemStrengThenDatas;

	private static Random random = new Random();

	public static bool ReLoad()
	{
		try
		{
			ItemStrengThenDataInfo[] array = LoadItemStrengThenDataDb();
			Dictionary<int, List<ItemStrengThenDataInfo>> value = LoadItemStrengThenDatas(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_ItemStrengThenDatas, value);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad ItemStrengThenData", exception);
			}
			return false;
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static ItemStrengThenDataInfo[] LoadItemStrengThenDataDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllItemStrengThenData();
	}

	public static Dictionary<int, List<ItemStrengThenDataInfo>> LoadItemStrengThenDatas(ItemStrengThenDataInfo[] ItemStrengThenData)
	{
		Dictionary<int, List<ItemStrengThenDataInfo>> dictionary = new Dictionary<int, List<ItemStrengThenDataInfo>>();
		foreach (ItemStrengThenDataInfo info in ItemStrengThenData)
		{
			if (!dictionary.Keys.Contains(info.TemplateID))
			{
				IEnumerable<ItemStrengThenDataInfo> source = ItemStrengThenData.Where((ItemStrengThenDataInfo s) => s.TemplateID == info.TemplateID);
				dictionary.Add(info.TemplateID, source.ToList());
			}
		}
		return dictionary;
	}

	public static List<ItemStrengThenDataInfo> FindItemStrengThenData(int DataId)
	{
		if (m_ItemStrengThenDatas.ContainsKey(DataId))
		{
			return m_ItemStrengThenDatas[DataId];
		}
		return null;
	}
}
