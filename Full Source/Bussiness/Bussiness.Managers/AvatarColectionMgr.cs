using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class AvatarColectionMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static List<ClothPropertyTemplateInfo> m_clothPropertyTemplateInfo = new List<ClothPropertyTemplateInfo>();

	private static Dictionary<int, List<ClothGroupTemplateInfo>> m_clothGroupTemplateInfo = new Dictionary<int, List<ClothGroupTemplateInfo>>();

	private static Dictionary<int, ClothPropertyTemplateInfo> m_clothPropertyTemplates = new Dictionary<int, ClothPropertyTemplateInfo>();

	public static bool Init()
	{
		return ReLoad();
	}

	public static bool ReLoad()
	{
		try
		{
			List<ClothPropertyTemplateInfo> list = LoadClothPropertyTemplateDb();
			Dictionary<int, List<ClothGroupTemplateInfo>> value = LoadClothGroupTemplateInfoDb(list);
			if (list.Count > 0)
			{
				Interlocked.Exchange(ref m_clothPropertyTemplateInfo, list);
				Interlocked.Exchange(ref m_clothGroupTemplateInfo, value);
			}
			return true;
		}
		catch (Exception exception)
		{
			log.Error("AvatarColectionMgr", exception);
		}
		return false;
	}

	public static List<ClothPropertyTemplateInfo> LoadClothPropertyTemplateDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		ClothPropertyTemplateInfo[] allClothPropertyTemplateInfos = produceBussiness.GetAllClothPropertyTemplateInfos();
		ClothPropertyTemplateInfo[] array = allClothPropertyTemplateInfos;
		foreach (ClothPropertyTemplateInfo clothPropertyTemplateInfo in array)
		{
			if (!m_clothPropertyTemplates.ContainsKey(clothPropertyTemplateInfo.ID))
			{
				m_clothPropertyTemplates.Add(clothPropertyTemplateInfo.ID, clothPropertyTemplateInfo);
			}
		}
		return allClothPropertyTemplateInfos?.ToList();
	}

	public static Dictionary<int, List<ClothGroupTemplateInfo>> LoadClothGroupTemplateInfoDb(List<ClothPropertyTemplateInfo> tempClothPropertyTemplates)
	{
		Dictionary<int, List<ClothGroupTemplateInfo>> dictionary = new Dictionary<int, List<ClothGroupTemplateInfo>>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			ClothGroupTemplateInfo[] allClothGroupTemplateInfos = produceBussiness.GetAllClothGroupTemplateInfos();
			foreach (ClothPropertyTemplateInfo info in tempClothPropertyTemplates)
			{
				IEnumerable<ClothGroupTemplateInfo> source = allClothGroupTemplateInfos.Where((ClothGroupTemplateInfo s) => s.ID == info.ID);
				dictionary.Add(info.ID, source.ToList());
			}
		}
		return dictionary;
	}

	public static List<ClothGroupTemplateInfo> FindClothGroupTemplateInfo(int groupId)
	{
		if (m_clothGroupTemplateInfo.ContainsKey(groupId))
		{
			return m_clothGroupTemplateInfo[groupId];
		}
		return new List<ClothGroupTemplateInfo>();
	}

	public static ClothGroupTemplateInfo FindClothGroupTemplateInfo(int groupId, int itemId, int sex)
	{
		if (m_clothGroupTemplateInfo.ContainsKey(groupId))
		{
			foreach (ClothGroupTemplateInfo item in m_clothGroupTemplateInfo[groupId])
			{
				if ((item.TemplateID == itemId || item.OtherTemplateID.Contains(itemId.ToString())) && item.Sex == sex)
				{
					return item;
				}
			}
		}
		return null;
	}

	public static List<ClothPropertyTemplateInfo> GetClothPropertyTemplate()
	{
		if (m_clothPropertyTemplateInfo != null)
		{
			return m_clothPropertyTemplateInfo;
		}
		return null;
	}

	public static ClothPropertyTemplateInfo FindClothPropertyTemplate(int dataId)
	{
		if (m_clothPropertyTemplates.ContainsKey(dataId))
		{
			return m_clothPropertyTemplates[dataId];
		}
		return null;
	}
}
