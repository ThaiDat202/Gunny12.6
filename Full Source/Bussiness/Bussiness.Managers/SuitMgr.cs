using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class SuitMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, SuitTemplateInfo> m_SuitTemplates;

	private static Dictionary<int, List<SuitPartEquipInfo>> m_SuitPartEquips;

	private static Random random = new Random();

	public static bool ReLoad()
	{
		try
		{
			SuitTemplateInfo[] array = LoadSuitTemplateDb();
			Dictionary<int, SuitTemplateInfo> value = LoadSuitTemplates(array);
			Dictionary<int, List<SuitPartEquipInfo>> value2 = LoadSuitPartEquips(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_SuitTemplates, value);
				Interlocked.Exchange(ref m_SuitPartEquips, value2);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad SuitTemplate", exception);
			}
			return false;
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static Dictionary<int, List<SuitPartEquipInfo>> LoadSuitPartEquips(SuitTemplateInfo[] SuitPartEquip)
	{
		Dictionary<int, List<SuitPartEquipInfo>> dictionary = new Dictionary<int, List<SuitPartEquipInfo>>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			SuitPartEquipInfo[] allSuitPartEquip = produceBussiness.GetAllSuitPartEquip();
			foreach (SuitTemplateInfo info in SuitPartEquip)
			{
				if (!dictionary.Keys.Contains(info.SuitId))
				{
					IEnumerable<SuitPartEquipInfo> source = allSuitPartEquip.Where((SuitPartEquipInfo s) => s.ID == info.SuitId);
					dictionary.Add(info.SuitId, source.ToList());
				}
			}
		}
		return dictionary;
	}

	public static List<SuitPartEquipInfo> FindSuitPartEquip(int Id)
	{
		if (m_SuitPartEquips.ContainsKey(Id))
		{
			return m_SuitPartEquips[Id];
		}
		return null;
	}

	public static SuitTemplateInfo[] LoadSuitTemplateDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllSuitTemplate();
	}

	public static Dictionary<int, SuitTemplateInfo> LoadSuitTemplates(SuitTemplateInfo[] SuitTemplate)
	{
		Dictionary<int, SuitTemplateInfo> dictionary = new Dictionary<int, SuitTemplateInfo>();
		foreach (SuitTemplateInfo suitTemplateInfo in SuitTemplate)
		{
			if (!dictionary.Keys.Contains(suitTemplateInfo.SuitId))
			{
				dictionary.Add(suitTemplateInfo.SuitId, suitTemplateInfo);
			}
		}
		return dictionary;
	}

	public static SuitTemplateInfo FindSuitTemplate(int ID)
	{
		if (m_SuitTemplates.ContainsKey(ID))
		{
			return m_SuitTemplates[ID];
		}
		return null;
	}
}
