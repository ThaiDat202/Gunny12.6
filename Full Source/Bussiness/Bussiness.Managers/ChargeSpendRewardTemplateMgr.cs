using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class ChargeSpendRewardTemplateMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, ChargeSpendRewardTemplateInfo> m_ChargeSpendRewardTemplates;

	private static Random random = new Random();

	public static bool ReLoad()
	{
		try
		{
			ChargeSpendRewardTemplateInfo[] array = LoadChargeSpendRewardTemplateDb();
			Dictionary<int, ChargeSpendRewardTemplateInfo> value = LoadChargeSpendRewardTemplates(array);
			if (array != null)
			{
				Interlocked.Exchange(ref m_ChargeSpendRewardTemplates, value);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad ChargeSpendRewardTemplate", exception);
			}
			return false;
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static ChargeSpendRewardTemplateInfo[] LoadChargeSpendRewardTemplateDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllChargeSpendRewardTemplate();
	}

	public static Dictionary<int, ChargeSpendRewardTemplateInfo> LoadChargeSpendRewardTemplates(ChargeSpendRewardTemplateInfo[] ChargeSpendRewardTemplate)
	{
		Dictionary<int, ChargeSpendRewardTemplateInfo> dictionary = new Dictionary<int, ChargeSpendRewardTemplateInfo>();
		foreach (ChargeSpendRewardTemplateInfo chargeSpendRewardTemplateInfo in ChargeSpendRewardTemplate)
		{
			if (!dictionary.Keys.Contains(chargeSpendRewardTemplateInfo.ID))
			{
				dictionary.Add(chargeSpendRewardTemplateInfo.ID, chargeSpendRewardTemplateInfo);
			}
		}
		return dictionary;
	}

	public static ChargeSpendRewardTemplateInfo FindChargeSpendRewardTemplate(int ID)
	{
		if (m_ChargeSpendRewardTemplates.ContainsKey(ID))
		{
			return m_ChargeSpendRewardTemplates[ID];
		}
		return null;
	}
}
