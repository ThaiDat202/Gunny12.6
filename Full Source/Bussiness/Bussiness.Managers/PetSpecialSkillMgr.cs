using System;
using System.Collections.Generic;
using System.Linq;
using EntityDatabase.ServerModels;

namespace Bussiness.Managers;

public static class PetSpecialSkillMgr
{
	public static List<Pet_Special_Skills> m_petSpecialSkills;

	public static bool Init()
	{
		try
		{
			ServerData serverData = new ServerData();
			m_petSpecialSkills = serverData.Pet_Special_Skills.ToList();
			return true;
		}
		catch
		{
			return false;
		}
	}

	public static int GetSkillLacrar(int KindID)
	{
		Random random = new Random();
		List<Pet_Special_Skills> list = m_petSpecialSkills.Where((Pet_Special_Skills p) => p.KindID == KindID && !p.IsAwaken).ToList();
		return list[random.Next(list.Count)].SkillID;
	}

	public static int GetSkillAwaken(int KindID)
	{
		Random random = new Random();
		List<Pet_Special_Skills> list = m_petSpecialSkills.Where((Pet_Special_Skills p) => p.KindID == KindID && p.IsAwaken).ToList();
		return list[random.Next(list.Count)].SkillID;
	}

	public static List<int> GetPetAwakenSkillByKindID(List<int> listSkillId, int skillId1, int skillId2)
	{
		List<int> list = new List<int>();
		List<string> list2 = new List<string>();
		foreach (int item3 in listSkillId)
		{
			if (item3 == skillId1 || item3 == skillId2)
			{
				list.Add(item3);
			}
		}
		foreach (int item4 in list)
		{
			listSkillId.Remove(item4);
		}
		foreach (Pet_Special_Skills petSpecialSkill in m_petSpecialSkills)
		{
			if (petSpecialSkill.SkillID == skillId1 || petSpecialSkill.SkillID == skillId2)
			{
				string[] array = petSpecialSkill.DeleteSkillIDs.Split(',');
				string[] array2 = array;
				foreach (string item in array2)
				{
					list2.Add(item);
				}
				listSkillId.Add(petSpecialSkill.SkillID);
			}
		}
		foreach (string item5 in list2)
		{
			if (!string.IsNullOrEmpty(item5))
			{
				int item2 = int.Parse(item5);
				listSkillId.Remove(item2);
			}
		}
		listSkillId.Sort();
		return listSkillId;
	}
}
