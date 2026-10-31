using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class ChargeActiveTemplateMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, ChargeActiveTemplateInfo> m_ChargeActiveTemplates;

	private static Random random = new Random();

	public static bool ReLoad()
	{
		try
		{
			ChargeActiveTemplateInfo[] array = LoadChargeActiveTemplateDb();
			Dictionary<int, ChargeActiveTemplateInfo> value = LoadChargeActiveTemplates(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_ChargeActiveTemplates, value);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad ChargeActiveTemplate", exception);
			}
			return false;
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static ChargeActiveTemplateInfo[] LoadChargeActiveTemplateDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllChargeActiveTemplate();
	}

	public static Dictionary<int, ChargeActiveTemplateInfo> LoadChargeActiveTemplates(ChargeActiveTemplateInfo[] ChargeActiveTemplate)
	{
		Dictionary<int, ChargeActiveTemplateInfo> dictionary = new Dictionary<int, ChargeActiveTemplateInfo>();
		foreach (ChargeActiveTemplateInfo chargeActiveTemplateInfo in ChargeActiveTemplate)
		{
			if (!dictionary.Keys.Contains(chargeActiveTemplateInfo.ID))
			{
				dictionary.Add(chargeActiveTemplateInfo.ID, chargeActiveTemplateInfo);
			}
		}
		return dictionary;
	}

	public static ChargeActiveTemplateInfo FindChargeActiveTemplate(int ID)
	{
		if (m_ChargeActiveTemplates.ContainsKey(ID))
		{
			return m_ChargeActiveTemplates[ID];
		}
		return null;
	}
}
