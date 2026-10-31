using System.Collections.Generic;
using System.Linq;
using SqlDataProvider.Data;

namespace Bussiness.Managers;

public class MarkSkillMgr
{
	protected static object m_lock = new object();

	private static List<MarkChipItemInfo> GetAllChipEquip(int place, int UserId)
	{
		lock (m_lock)
		{
			PlayerBussiness playerBussiness = new PlayerBussiness();
			List<MarkChipItemInfo> list = new List<MarkChipItemInfo>();
			List<MarkChipItemInfo> allUsersMark = playerBussiness.GetAllUsersMark(UserId);
			foreach (MarkChipItemInfo item in allUsersMark)
			{
				if (item.Type == 1)
				{
					list.Add(item);
				}
			}
			return list.Where((MarkChipItemInfo a) => a.IsExist && a.SlotType == place).ToList();
		}
	}

	public static List<int> GetMarkSkill(int UserId)
	{
		List<int> list = new List<int>();
		foreach (EngraveSetElementInfo item in EngraveMgr.GetAllEngraveSetElement())
		{
			for (int i = 1; i <= 6; i++)
			{
				int num = 0;
				foreach (MarkChipItemInfo item2 in GetAllChipEquip(i, UserId))
				{
					EngraveDebrisInfo singleEngraveDebris = EngraveMgr.GetSingleEngraveDebris(item2.TemplateID);
					if (singleEngraveDebris.SetID == item.SetId)
					{
						num++;
						if (num >= item.Demand && item.SkillId != 0)
						{
							list.Add(item.SkillId);
							break;
						}
					}
				}
			}
		}
		return list;
	}
}
