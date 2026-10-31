using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class MagicStoneMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static MagicStoneInfo[] m_magicStoneList;

	private static Dictionary<int, List<MagicStoneInfo>> m_magicStones;

	private static ThreadSafeRandom random = new ThreadSafeRandom();

	private static ReaderWriterLock m_lock;

	public static bool ReLoad()
	{
		try
		{
			m_lock = new ReaderWriterLock();
			MagicStoneInfo[] array = LoadMagicStoneDb();
			Dictionary<int, List<MagicStoneInfo>> value = LoadMagicStones(array);
			if (array != null)
			{
				Interlocked.Exchange(ref m_magicStoneList, array);
				Interlocked.Exchange(ref m_magicStones, value);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad", exception);
			}
			return false;
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static MagicStoneInfo[] LoadMagicStoneDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllMagicStoneTemplate();
	}

	public static Dictionary<int, List<MagicStoneInfo>> LoadMagicStones(MagicStoneInfo[] MagicStones)
	{
		Dictionary<int, List<MagicStoneInfo>> dictionary = new Dictionary<int, List<MagicStoneInfo>>();
		foreach (MagicStoneInfo info in MagicStones)
		{
			if (!dictionary.Keys.Contains(info.TemplateID))
			{
				IEnumerable<MagicStoneInfo> source = MagicStones.Where((MagicStoneInfo s) => s.TemplateID == info.TemplateID);
				dictionary.Add(info.TemplateID, source.ToList());
			}
		}
		return dictionary;
	}

	public static List<MagicStoneInfo> FindMagicStone(int templateId)
	{
		if (m_magicStones == null)
		{
			Init();
		}
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (m_magicStones.ContainsKey(templateId))
			{
				return m_magicStones[templateId];
			}
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return null;
	}

	public static MagicStoneInfo FindMagicStoneByLevel(int templateId, int level)
	{
		if (m_magicStones == null)
		{
			Init();
		}
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (m_magicStones.ContainsKey(templateId))
			{
				List<MagicStoneInfo> list = m_magicStones[templateId];
				foreach (MagicStoneInfo item in list)
				{
					if (item.Level == level)
					{
						return item;
					}
				}
			}
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return null;
	}

	public static MagicStoneInfo GetMagicStoneTemplateWithExp(int templateid, int exp)
	{
		MagicStoneInfo result;
		lock (m_lock)
		{
			List<MagicStoneInfo> list = FindMagicStone(templateid);
			if (list == null || list.Count == 0)
			{
				return null;
			}
			int num = 0;
			foreach (MagicStoneInfo item in list.OrderByDescending((MagicStoneInfo pair) => pair.Exp))
			{
				if (num == 0)
				{
					num = item.Exp;
				}
				if (item.Exp <= exp || exp > num)
				{
					result = item;
					return result;
				}
			}
			result = null;
		}
		return result;
	}

	public static void SetupMagicStoneWithLevel(ItemInfo item)
	{
		lock (m_lock)
		{
			if (item == null || item.Template == null || !item.IsMagicStone())
			{
				return;
			}
			MagicStoneInfo magicStoneInfo;
			if (item.StrengthenLevel == 0)
			{
				item.StrengthenLevel = 1;
				magicStoneInfo = FindMagicStoneByLevel(item.TemplateID, item.StrengthenLevel);
			}
			else
			{
				magicStoneInfo = GetMagicStoneTemplateWithExp(item.TemplateID, item.StrengthenExp);
			}
			if (magicStoneInfo == null)
			{
				return;
			}
			item.StrengthenLevel = magicStoneInfo.Level;
			if (item.StrengthenExp <= 0)
			{
				item.StrengthenExp = magicStoneInfo.Exp;
			}
			List<int> list = new List<int>();
			list.Add(1);
			list.Add(2);
			list.Add(3);
			list.Add(4);
			list.Add(5);
			list.Add(6);
			switch (item.Template.Property3)
			{
			case 1:
				item.AttackCompose = magicStoneInfo.Attack;
				item.DefendCompose = magicStoneInfo.Defence;
				item.LuckCompose = magicStoneInfo.Luck;
				item.AgilityCompose = magicStoneInfo.Agility;
				item.MagicAttack = magicStoneInfo.MagicAttack;
				item.MagicDefence = magicStoneInfo.MagicDefence;
				break;
			case 2:
				if (item.AttackCompose == 0 && item.DefendCompose == 0 && item.LuckCompose == 0 && item.AgilityCompose == 0 && item.MagicAttack == 0 && item.MagicDefence == 0)
				{
					List<int> list4 = new List<int> { 1, 2, 3, 4, 5, 6 };
					for (int k = 0; k < 2; k++)
					{
						int index3 = random.Next(0, list4.Count - 1);
						ChangeWithPropID(ref item, list4[index3], magicStoneInfo);
						list4.RemoveAt(index3);
					}
				}
				else
				{
					ChangePropDefault(ref item, magicStoneInfo);
				}
				break;
			case 3:
				if (item.AttackCompose == 0 && item.DefendCompose == 0 && item.LuckCompose == 0 && item.AgilityCompose == 0 && item.MagicAttack == 0 && item.MagicDefence == 0)
				{
					List<int> list3 = new List<int> { 1, 2, 3, 4, 5, 6 };
					for (int j = 0; j < 3; j++)
					{
						int index2 = random.Next(0, list3.Count - 1);
						ChangeWithPropID(ref item, list3[index2], magicStoneInfo);
						list3.RemoveAt(index2);
					}
				}
				else
				{
					ChangePropDefault(ref item, magicStoneInfo);
				}
				break;
			case 4:
				if (item.AttackCompose == 0 && item.DefendCompose == 0 && item.LuckCompose == 0 && item.AgilityCompose == 0 && item.MagicAttack == 0 && item.MagicDefence == 0)
				{
					List<int> list2 = new List<int> { 1, 2, 3, 4, 5, 6 };
					for (int i = 0; i < 4; i++)
					{
						int index = random.Next(0, list2.Count - 1);
						ChangeWithPropID(ref item, list2[index], magicStoneInfo);
						list2.RemoveAt(index);
					}
				}
				else
				{
					ChangePropDefault(ref item, magicStoneInfo);
				}
				break;
			}
		}
	}

	public static ItemInfo GetMagicStoneInfo(ItemTemplateInfo info, int level)
	{
		int[] array = new int[6];
		MagicStoneInfo magicStoneInfo = FindMagicStoneByLevel(info.TemplateID, level);
		if (magicStoneInfo == null)
		{
			return null;
		}
		ItemInfo itemInfo = ItemInfo.CreateFromTemplate(info, 1, 101);
		PropMgInfo[] array2 = CreateProp(magicStoneInfo);
		random.Shuffer(array2);
		int num = ((array2.Length > info.Property3) ? info.Property3 : array2.Length);
		for (int i = 0; i < num; i++)
		{
			switch (array2[i].key)
			{
			case "att":
				itemInfo.AttackCompose = array2[i].value;
				break;
			case "agi":
				itemInfo.AgilityCompose = array2[i].value;
				break;
			case "def":
				itemInfo.DefendCompose = array2[i].value;
				break;
			case "luc":
				itemInfo.LuckCompose = array2[i].value;
				break;
			case "mgatt":
				itemInfo.MagicAttack = array2[i].value;
				break;
			case "mgdef":
				itemInfo.MagicDefence = array2[i].value;
				break;
			}
		}
		itemInfo.StrengthenLevel = level;
		itemInfo.StrengthenExp = magicStoneInfo.Exp;
		return itemInfo;
	}

	private static void ChangePropDefault(ref ItemInfo item, MagicStoneInfo magicTemp)
	{
		item.AttackCompose = ((item.AttackCompose > 0) ? magicTemp.Attack : item.AttackCompose);
		item.DefendCompose = ((item.DefendCompose > 0) ? magicTemp.Defence : item.DefendCompose);
		item.LuckCompose = ((item.LuckCompose > 0) ? magicTemp.Luck : item.LuckCompose);
		item.AgilityCompose = ((item.AgilityCompose > 0) ? magicTemp.Agility : item.AgilityCompose);
		item.MagicAttack = ((item.MagicAttack > 0) ? magicTemp.MagicAttack : item.MagicAttack);
		item.MagicDefence = ((item.MagicDefence > 0) ? magicTemp.MagicDefence : item.MagicDefence);
	}

	private static void ChangeWithPropID(ref ItemInfo item, int idchoise, MagicStoneInfo magicTemp)
	{
		switch (idchoise)
		{
		case 1:
			item.AttackCompose = magicTemp.Attack;
			break;
		case 2:
			item.DefendCompose = magicTemp.Defence;
			break;
		case 3:
			item.LuckCompose = magicTemp.Luck;
			break;
		case 4:
			item.AgilityCompose = magicTemp.Agility;
			break;
		case 5:
			item.MagicAttack = magicTemp.MagicAttack;
			break;
		case 6:
			item.MagicDefence = magicTemp.MagicDefence;
			break;
		}
	}

	public static PropMgInfo[] CreateProp(MagicStoneInfo info)
	{
		List<PropMgInfo> list = new List<PropMgInfo>();
		if (info.Attack > 0)
		{
			PropMgInfo item = new PropMgInfo
			{
				key = "att",
				value = info.Attack
			};
			list.Add(item);
		}
		if (info.Defence > 0)
		{
			PropMgInfo item2 = new PropMgInfo
			{
				key = "def",
				value = info.Defence
			};
			list.Add(item2);
		}
		if (info.Agility > 0)
		{
			PropMgInfo item3 = new PropMgInfo
			{
				key = "agi",
				value = info.Agility
			};
			list.Add(item3);
		}
		if (info.Luck > 0)
		{
			PropMgInfo item4 = new PropMgInfo
			{
				key = "luc",
				value = info.Luck
			};
			list.Add(item4);
		}
		if (info.MagicAttack > 0)
		{
			PropMgInfo item5 = new PropMgInfo
			{
				key = "mgatt",
				value = info.MagicAttack
			};
			list.Add(item5);
		}
		if (info.MagicDefence > 0)
		{
			PropMgInfo item6 = new PropMgInfo
			{
				key = "mgdef",
				value = info.MagicDefence
			};
			list.Add(item6);
		}
		return list.ToArray();
	}
}
