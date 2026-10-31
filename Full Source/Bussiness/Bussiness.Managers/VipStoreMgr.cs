using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class VipStoreMgr
{
	private static readonly ILog KVWDJEBWIZ;

	private static Dictionary<int, VipStoreInfo> dictionary_0;

	private static ThreadSafeRandom threadSafeRandom_0;

	static VipStoreMgr()
	{
		KVWDJEBWIZ = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		dictionary_0 = new Dictionary<int, VipStoreInfo>();
		threadSafeRandom_0 = new ThreadSafeRandom();
	}

	public static VipStoreInfo GetVipStoreInfo(int id)
	{
		throw new NotImplementedException();
	}

	public static VipStoreInfo FindVipStore(int id)
	{
		if (dictionary_0.Count == 0)
		{
			ReLoad();
		}
		if (!dictionary_0.ContainsKey(id))
		{
			return null;
		}
		return dictionary_0[id];
	}

	public static List<VipStoreInfo> GetAllVipStore()
	{
		if (dictionary_0.Count == 0)
		{
			ReLoad();
		}
		return dictionary_0.Values.ToList();
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, VipStoreInfo> dictionary = smethod_0();
			if (dictionary.Values.Count > 0)
			{
				Interlocked.Exchange(ref dictionary_0, dictionary);
				return true;
			}
		}
		catch (Exception exception)
		{
			KVWDJEBWIZ.Error("VipStoreMgr init error:", exception);
		}
		return false;
	}

	private static Dictionary<int, VipStoreInfo> smethod_0()
	{
		Dictionary<int, VipStoreInfo> dictionary = new Dictionary<int, VipStoreInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			VipStoreInfo[] allVipStore = produceBussiness.GetAllVipStore();
			foreach (VipStoreInfo vipStoreInfo in allVipStore)
			{
				if (!dictionary.ContainsKey(vipStoreInfo.ID))
				{
					dictionary.Add(vipStoreInfo.ID, vipStoreInfo);
				}
			}
		}
		return dictionary;
	}
}
