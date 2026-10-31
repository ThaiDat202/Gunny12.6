using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class SearchGoodsMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, SearchGoodsPayMoneyInfo> m_searchGoodsPayMoneys;

	private static Dictionary<int, SearchGoodsTempInfo> m_SearchGoodsTemps;

	private static Random random = new Random();

	public static bool ReLoad()
	{
		try
		{
			SearchGoodsPayMoneyInfo[] array = LoadSearchGoodsPayMoneyDb();
			Dictionary<int, SearchGoodsPayMoneyInfo> value = LoadSearchGoodsPayMoneys(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_searchGoodsPayMoneys, value);
			}
			SearchGoodsTempInfo[] array2 = LoadSearchGoodsTempDb();
			Dictionary<int, SearchGoodsTempInfo> value2 = LoadSearchGoodsTemps(array2);
			if (array2.Length != 0)
			{
				Interlocked.Exchange(ref m_SearchGoodsTemps, value2);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad SearchGoods", exception);
			}
			return false;
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static SearchGoodsTempInfo GetSearchGoodsTempInfo(int starId)
	{
		if (m_SearchGoodsTemps.ContainsKey(starId))
		{
			return m_SearchGoodsTemps[starId];
		}
		return null;
	}

	public static int MaxStar()
	{
		return m_SearchGoodsTemps.Count;
	}

	public static SearchGoodsTempInfo[] LoadSearchGoodsTempDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllSearchGoodsTemp();
	}

	public static Dictionary<int, SearchGoodsTempInfo> LoadSearchGoodsTemps(SearchGoodsTempInfo[] SearchGoodsTemp)
	{
		Dictionary<int, SearchGoodsTempInfo> dictionary = new Dictionary<int, SearchGoodsTempInfo>();
		foreach (SearchGoodsTempInfo searchGoodsTempInfo in SearchGoodsTemp)
		{
			if (!dictionary.Keys.Contains(searchGoodsTempInfo.StarID))
			{
				dictionary.Add(searchGoodsTempInfo.StarID, searchGoodsTempInfo);
			}
		}
		return dictionary;
	}

	public static SearchGoodsTempInfo FindSearchGoodsTemp(int ID)
	{
		if (m_SearchGoodsTemps.ContainsKey(ID))
		{
			return m_SearchGoodsTemps[ID];
		}
		return null;
	}

	public static SearchGoodsPayMoneyInfo[] LoadSearchGoodsPayMoneyDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllSearchGoodsPayMoney();
	}

	public static Dictionary<int, SearchGoodsPayMoneyInfo> LoadSearchGoodsPayMoneys(SearchGoodsPayMoneyInfo[] SearchGoodsPayMoney)
	{
		Dictionary<int, SearchGoodsPayMoneyInfo> dictionary = new Dictionary<int, SearchGoodsPayMoneyInfo>();
		foreach (SearchGoodsPayMoneyInfo searchGoodsPayMoneyInfo in SearchGoodsPayMoney)
		{
			if (!dictionary.Keys.Contains(searchGoodsPayMoneyInfo.Number))
			{
				dictionary.Add(searchGoodsPayMoneyInfo.Number, searchGoodsPayMoneyInfo);
			}
		}
		return dictionary;
	}

	public static SearchGoodsPayMoneyInfo FindSearchGoodsPayMoney(int ID)
	{
		if (m_searchGoodsPayMoneys.ContainsKey(ID))
		{
			return m_searchGoodsPayMoneys[ID];
		}
		return null;
	}
}
