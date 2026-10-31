using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class FightSpiritTemplateMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, List<FightSpiritTemplateInfo>> m_fightSpiritTemplates = new Dictionary<int, List<FightSpiritTemplateInfo>>();

	private static ReaderWriterLock m_clientLocker = new ReaderWriterLock();

	public static bool ReLoad()
	{
		try
		{
			FightSpiritTemplateInfo[] array = LoadFightSpiritTemplateDb();
			Dictionary<int, List<FightSpiritTemplateInfo>> value = LoadFightSpiritTemplates(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_fightSpiritTemplates, value);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad FightSpiritTemplate", exception);
			}
			return false;
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static FightSpiritTemplateInfo[] LoadFightSpiritTemplateDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllFightSpiritTemplate();
	}

	public static Dictionary<int, List<FightSpiritTemplateInfo>> LoadFightSpiritTemplates(FightSpiritTemplateInfo[] fightSpiritTemplates)
	{
		Dictionary<int, List<FightSpiritTemplateInfo>> dictionary = new Dictionary<int, List<FightSpiritTemplateInfo>>();
		foreach (FightSpiritTemplateInfo info in fightSpiritTemplates)
		{
			if (!dictionary.Keys.Contains(info.FightSpiritID))
			{
				IEnumerable<FightSpiritTemplateInfo> source = fightSpiritTemplates.Where((FightSpiritTemplateInfo s) => s.FightSpiritID == info.FightSpiritID);
				dictionary.Add(info.FightSpiritID, source.ToList());
			}
		}
		return dictionary;
	}

	public static List<FightSpiritTemplateInfo> FindFightSpiritTemplates(int id)
	{
		m_clientLocker.AcquireWriterLock(-1);
		try
		{
			if (m_fightSpiritTemplates.ContainsKey(id))
			{
				return m_fightSpiritTemplates[id];
			}
		}
		finally
		{
			m_clientLocker.ReleaseWriterLock();
		}
		return new List<FightSpiritTemplateInfo>();
	}

	public static FightSpiritTemplateInfo FindFightSpiritTemplateInfo(int FigSpiritId, int lv)
	{
		List<FightSpiritTemplateInfo> list = FindFightSpiritTemplates(FigSpiritId);
		foreach (FightSpiritTemplateInfo item in list)
		{
			if (item.Level == lv)
			{
				return item;
			}
		}
		return null;
	}

	public static int GOLDEN_LEVEL(int lv)
	{
		try
		{
			string[] array = GameProperties.FightSpiritLevelAddDamage.Split('|');
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (text.Split(',')[0] == lv.ToString())
				{
					return int.Parse(text.Split(',')[1]);
				}
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("FightSpiritTemplate.GOLDEN_LEVEL: ", exception);
			}
		}
		return 0;
	}

	public static int[] Exps()
	{
		List<FightSpiritTemplateInfo> list = FindFightSpiritTemplates(100001);
		List<int> list2 = new List<int>();
		foreach (FightSpiritTemplateInfo item in list)
		{
			list2.Add(item.Exp);
		}
		return list2.ToArray();
	}

	public static int GetProp(int figSpiritId, int lv, int place, ref int addAtt, ref int rdcDama)
	{
		FightSpiritTemplateInfo fightSpiritTemplateInfo = FindFightSpiritTemplateInfo(figSpiritId, lv);
		if (fightSpiritTemplateInfo == null)
		{
			List<FightSpiritTemplateInfo> list = FindFightSpiritTemplates(figSpiritId);
			if (list.Count <= 0)
			{
				log.ErrorFormat("FigSpiritId: {0} not found! Return 0", figSpiritId);
				return 0;
			}
			fightSpiritTemplateInfo = list[list.Count - 1];
			log.ErrorFormat("FigSpiritId: {0}, level: {1} not found! Return Max level in database is {6}", figSpiritId, lv, fightSpiritTemplateInfo.Level);
		}
		if (figSpiritId == 100001 || figSpiritId == 100003)
		{
			addAtt += GOLDEN_LEVEL(lv);
		}
		else
		{
			rdcDama += GOLDEN_LEVEL(lv);
		}
		return place switch
		{
			2 => fightSpiritTemplateInfo.Attack, 
			5 => fightSpiritTemplateInfo.Agility, 
			11 => fightSpiritTemplateInfo.Defence, 
			3 => fightSpiritTemplateInfo.Lucky, 
			13 => fightSpiritTemplateInfo.Blood, 
			_ => 0, 
		};
	}
}
