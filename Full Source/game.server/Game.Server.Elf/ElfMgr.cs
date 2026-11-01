using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bussiness;
using Bussiness.Managers;
using EntityDatabase.PlayerModels;
using EntityDatabase.ServerModels;
using Game.Logic;
using Game.Server.GameObjects;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Elf;

public static class ElfMgr
{
	private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public static readonly ThreadSafeRandom Random = new ThreadSafeRandom();

	public static object Locker = new object();

	public static ElfPacketsOut Out = new ElfPacketsOut();

	private static readonly ServerData ServerData = new ServerData();

	public static List<Elf_Template_Info> Elfs { get; private set; }

	public static List<Elf_SkillBook> ElfSkills { get; private set; }

	public static Dictionary<int, TS_UpgradeTemplate> ElfThreeGrades { get; private set; }

	public static Dictionary<int, TS_UpgradeTemplate> ElfFourGrades { get; private set; }

	public static Dictionary<int, TS_UpgradeTemplate> ElfFiveGrades { get; private set; }

	public static bool Init()
	{
		try
		{
			Elfs = ServerData.Elf_Template_Info.ToList();
			ElfSkills = ServerData.Elf_SkillBook.ToList();
			ElfThreeGrades = ServerData.TS_UpgradeTemplate.Where((TS_UpgradeTemplate e) => e.Types == 106).ToDictionary((TS_UpgradeTemplate e) => e.Grades);
			ElfFourGrades = ServerData.TS_UpgradeTemplate.Where((TS_UpgradeTemplate e) => e.Types == 107).ToDictionary((TS_UpgradeTemplate e) => e.Grades);
			ElfFiveGrades = ServerData.TS_UpgradeTemplate.Where((TS_UpgradeTemplate e) => e.Types == 108).ToDictionary((TS_UpgradeTemplate e) => e.Grades);
			if (Log.IsDebugEnabled)
			{
				Log.DebugFormat("Total Elfs: {0}", Elfs.Count);
			}
			return true;
		}
		catch (Exception arg)
		{
			Log.ErrorFormat("ElfMgr error: {0}", arg);
			return false;
		}
	}

	public static Sys_Users_Elf AddElf(GamePlayer player, ItemTemplateInfo template)
	{
		Elf_Template_Info elf_Template_Info = Elfs.FirstOrDefault((Elf_Template_Info p) => p.TemplateID == template.Property5);
		if (elf_Template_Info == null)
		{
			throw new NullReferenceException($"elf is null with templateId {template.Property5}");
		}
		Dictionary<string, int> dictionary = CreateRandomProps(elf_Template_Info);
		Dictionary<string, int> dictionary2 = CreateRandomGrowthProps(elf_Template_Info);
		int elfID = FindFirstEmptyPlace(player);
		Sys_Users_Elf sys_Users_Elf = new Sys_Users_Elf
		{
			UserID = player.PlayerId,
			ElfID = elfID,
			TempID = template.Property5,
			Exp = 0,
			Grade = 1,
			HpBase = dictionary["HpValue"],
			AttackBase = dictionary["AttackValue"],
			HuoBase = dictionary["HuoValue"],
			ShuiBase = dictionary["ShuiValue"],
			TuBase = dictionary["TuValue"],
			FengBase = dictionary["FengValue"],
            GuangBase = dictionary["GuangValue"],
            AnBase = dictionary["AnValue"],
            HpValue = dictionary["HpValue"],
			AttackValue = dictionary["AttackValue"],
			HuoValue = dictionary["HuoValue"],
			ShuiValue = dictionary["ShuiValue"],
			TuValue = dictionary["TuValue"],
			FengValue = dictionary["FengValue"],
            GuangValue = dictionary["GuangValue"],
            AnValue = dictionary["AnValue"],
            HpGrowth = dictionary2["HpGrowth"],
			AttackGrowth = dictionary2["AttackGrowth"],
			HuoGrowth = dictionary2["HuoGrowth"],
			ShuiGrowth = dictionary2["ShuiGrowth"],
			TuGrowth = dictionary2["TuGrowth"],
			FengGrowth = dictionary2["FengGrowth"],
            GuangGrowth = dictionary["GuangGrowth"],
            AnGrowth = dictionary["AnGrowth"],
            GoodFeling = 0,
			IsFight = false,
			Quality = dictionary2["Quality"],
			SkillList = "",
			DungeonGoodFeling = 0,
			GameGoodFeling = 0,
			CultivateGoodFeling = 0,
			ElfEquipId = 0
		};
		player.PlayerCharacter.ElfInfo.Add(sys_Users_Elf);
		Out.SendUpdateElf(player, sys_Users_Elf);
		return sys_Users_Elf;
	}

	private static Dictionary<string, int> CreateRandomProps(Elf_Template_Info elfTemplate)
	{
		string[] array = elfTemplate.HPRandoms.Split(',');
		string[] array2 = elfTemplate.AtckRandoms.Split(',');
		string[] array3 = elfTemplate.FireRandoms.Split(',');
		string[] array4 = elfTemplate.WindRandoms.Split(',');
		string[] array5 = elfTemplate.WateRandoms.Split(',');
		string[] array6 = elfTemplate.LandRandoms.Split(',');
		string[] array7 = elfTemplate.LighRandoms.Split(',');
        string[] array8 = elfTemplate.DrakRandoms.Split(',');
        Dictionary<string, int> dictionary = new Dictionary<string, int>();
		dictionary.Add("HpValue", Random.Next(int.Parse(array[0]), int.Parse(array[1])));
		dictionary.Add("AttackValue", Random.Next(int.Parse(array2[0]), int.Parse(array2[1])));
		dictionary.Add("HuoValue", Random.Next(int.Parse(array3[0]), int.Parse(array3[1])));
		dictionary.Add("ShuiValue", Random.Next(int.Parse(array5[0]), int.Parse(array5[1])));
		dictionary.Add("TuValue", Random.Next(int.Parse(array6[0]), int.Parse(array6[1])));
		dictionary.Add("FengValue", Random.Next(int.Parse(array4[0]), int.Parse(array4[1])));
        dictionary.Add("GuangValue", Random.Next(int.Parse(array7[0]), int.Parse(array7[1])));
        dictionary.Add("AnValue", Random.Next(int.Parse(array8[0]), int.Parse(array8[1])));
        return dictionary;
	}

	private static Dictionary<string, int> CreateRandomGrowthProps(Elf_Template_Info elfTemplate)
	{
		string[] array = elfTemplate.Growths.Split(',');
		string[] array2 = elfTemplate.HPGrowths.Split(',');
		string[] array3 = elfTemplate.AtckGrowths.Split(',');
		string[] array4 = elfTemplate.FireGrowths.Split(',');
		string[] array5 = elfTemplate.WindGrowths.Split(',');
		string[] array6 = elfTemplate.WateGrowths.Split(',');
		string[] array7 = elfTemplate.LandGrowths.Split(',');
        string[] array8 = elfTemplate.LighGrowths.Split(',');
        string[] array9 = elfTemplate.DrakGrowths.Split(',');
        Dictionary<string, int> dictionary = new Dictionary<string, int>();
		dictionary.Add("HpGrowth", Random.Next(int.Parse(array2[0]), int.Parse(array2[1])));
		dictionary.Add("AttackGrowth", Random.Next(int.Parse(array3[0]), int.Parse(array3[1])));
		dictionary.Add("HuoGrowth", Random.Next(int.Parse(array4[0]), int.Parse(array4[1])));
		dictionary.Add("ShuiGrowth", Random.Next(int.Parse(array6[0]), int.Parse(array6[1])));
		dictionary.Add("TuGrowth", Random.Next(int.Parse(array7[0]), int.Parse(array7[1])));
		dictionary.Add("FengGrowth", Random.Next(int.Parse(array5[0]), int.Parse(array5[1])));
        dictionary.Add("GuangGrowth", Random.Next(int.Parse(array8[0]), int.Parse(array8[1])));
        dictionary.Add("AnGrowth", Random.Next(int.Parse(array9[0]), int.Parse(array9[1])));
        int num = dictionary["HpGrowth"] + dictionary["AttackGrowth"] + dictionary["HuoGrowth"] + dictionary["ShuiGrowth"] + dictionary["TuGrowth"] + dictionary["FengGrowth"] + dictionary["GuangGrowth"] + dictionary["AnGrowth"];
		int num2 = int.Parse(array2[1]) + int.Parse(array3[1]) + int.Parse(array4[1]) + int.Parse(array5[1]) + int.Parse(array6[1]) + int.Parse(array7[1]) + int.Parse(array8[1]) + int.Parse(array9[1]);
		float num3 = (float)num / (float)num2;
		int value = 0;
		if ((double)num3 < 0.6)
		{
			value = 1;
		}
		if ((double)num3 > 0.6)
		{
			value = 2;
		}
		if ((double)num3 > 0.75)
		{
			value = 3;
		}
		if ((double)num3 > 0.9)
		{
			value = 4;
		}
		dictionary.Add("Quality", value);
		return dictionary;
	}

	public static int CalculateQuality(Sys_Users_Elf elf)
	{
		Elf_Template_Info elf_Template_Info = Elfs.FirstOrDefault((Elf_Template_Info e) => e.TemplateID == elf.TempID);
		if (elf_Template_Info == null)
		{
			throw new NullReferenceException($"elfTemplate is null with templateId = {elf.TempID}");
		}
		string[] array = elf_Template_Info.HPGrowths.Split(',');
		string[] array2 = elf_Template_Info.AtckGrowths.Split(',');
		string[] array3 = elf_Template_Info.FireGrowths.Split(',');
		string[] array4 = elf_Template_Info.WindGrowths.Split(',');
		string[] array5 = elf_Template_Info.WateGrowths.Split(',');
		string[] array6 = elf_Template_Info.LandGrowths.Split(',');
        string[] array7 = elf_Template_Info.LighGrowths.Split(',');
        string[] array8 = elf_Template_Info.DrakGrowths.Split(',');
        int num = int.Parse(array[1]) + int.Parse(array2[1]) + int.Parse(array3[1]) + int.Parse(array4[1]) + int.Parse(array5[1]) + int.Parse(array6[1]) + int.Parse(array7[1]) + int.Parse(array8[1]);
		int num2 = elf.HpGrowth + elf.AttackGrowth + elf.FengGrowth + elf.HuoGrowth + elf.ShuiGrowth + elf.TuGrowth + elf.GuangGrowth + elf.AnGrowth;
		float num3 = (float)num2 / (float)num;
		int result = 0;
		if ((double)num3 < 0.6)
		{
			result = 1;
		}
		if ((double)num3 > 0.6)
		{
			result = 2;
		}
		if ((double)num3 > 0.75)
		{
			result = 3;
		}
		if ((double)num3 > 0.9)
		{
			result = 4;
		}
		return result;
	}

	public static void AddGrowthAttributes(Sys_Users_Elf elf, List<int> props, int itemId)
	{
		ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(itemId);
		if (itemTemplateInfo == null)
		{
			throw new NullReferenceException($"item is null with templateId = {itemId}");
		}
		Elf_Template_Info elf_Template_Info = Elfs.FirstOrDefault((Elf_Template_Info e) => e.TemplateID == elf.TempID);
		if (elf_Template_Info == null)
		{
			throw new NullReferenceException($"elfTemplate is null with templateId = {elf.TempID}");
		}
		int num = int.Parse(elf_Template_Info.HPGrowths.Split(',')[1]);
		int num2 = int.Parse(elf_Template_Info.AtckGrowths.Split(',')[1]);
		int num3 = int.Parse(elf_Template_Info.FireGrowths.Split(',')[1]);
		int num4 = int.Parse(elf_Template_Info.WindGrowths.Split(',')[1]);
		int num5 = int.Parse(elf_Template_Info.WateGrowths.Split(',')[1]);
		int num6 = int.Parse(elf_Template_Info.LandGrowths.Split(',')[1]);
		int num7 = CalculateElfGrowth(props[0], itemTemplateInfo.Property2, elf.HpGrowth, num);
		int num8 = CalculateElfGrowth(props[1], itemTemplateInfo.Property2, elf.AttackGrowth, num2);
		int num9 = CalculateElfGrowth(props[2], itemTemplateInfo.Property2, elf.HuoGrowth, num3);
		int num10 = CalculateElfGrowth(props[3], itemTemplateInfo.Property2, elf.ShuiGrowth, num5);
		int num11 = CalculateElfGrowth(props[4], itemTemplateInfo.Property2, elf.FengGrowth, num4);
		int num12 = CalculateElfGrowth(props[5], itemTemplateInfo.Property2, elf.TuGrowth, num6);
		//sáng tối
        int num13 = int.Parse(elf_Template_Info.LighGrowths.Split(',')[1]);
        int num14 = int.Parse(elf_Template_Info.DrakGrowths.Split(',')[1]);
        int num15 = CalculateElfGrowth(props[6], itemTemplateInfo.Property2, elf.GuangGrowth, num13);
        int num16 = CalculateElfGrowth(props[7], itemTemplateInfo.Property2, elf.AnGrowth, num14);
        if (num7 + elf.HpGrowth > num)
		{
			elf.HpGrowth = num;
		}
		else
		{
			elf.HpGrowth += num7;
		}
		if (num8 + elf.AttackGrowth > num2)
		{
			elf.AttackGrowth = num2;
		}
		else
		{
			elf.AttackGrowth += num8;
		}
		if (num9 + elf.HuoGrowth > num3)
		{
			elf.HuoGrowth = num3;
		}
		else
		{
			elf.HuoGrowth += num9;
		}
		if (num10 + elf.ShuiGrowth > num4)
		{
			elf.ShuiGrowth = num4;
		}
		else
		{
			elf.ShuiGrowth += num10;
		}
		if (num12 + elf.TuGrowth > num5)
		{
			elf.TuGrowth = num5;
		}
		else
		{
			elf.TuGrowth += num12;
		}
		if (num11 + elf.FengGrowth > num6)
		{
			elf.FengGrowth = num6;
		}
		else
		{
			elf.FengGrowth += num11;
		}
        //sáng tối
        if (num15 + elf.GuangGrowth > num13)
        {
            elf.GuangGrowth = num13;
        }
        else
        {
            elf.GuangGrowth += num15;
        }
        if (num16 + elf.AnGrowth > num14)
        {
            elf.AnGrowth = num14;
        }
        else
        {
            elf.AnGrowth += num16;
        }
    }

	private static int CalculateElfGrowth(int count, int property, int growthValue, int maxGrowthValue)
	{
		if (count == 0)
		{
			return 0;
		}
		count--;
		int num = (int)((double)property * (Math.Floor((float)maxGrowthValue / (float)growthValue * 100f) / 100.0));
		if (count <= 0)
		{
			return num;
		}
		return CalculateElfGrowth(count, property, num + growthValue, maxGrowthValue) + num;
	}

	public static void CalculateGradeProps(Sys_Users_Elf elf, int count = 1)
	{
		if (count > 0)
		{
			do
			{
				count--;
				elf.HpValue += elf.HpGrowth / 10;
				elf.AttackValue += elf.AttackGrowth / 10;
				elf.HuoValue += elf.HuoGrowth / 10;
				elf.ShuiValue += elf.ShuiGrowth / 10;
				elf.FengValue += elf.FengGrowth / 10;
				elf.TuValue += elf.TuGrowth / 10;
                elf.GuangValue += elf.GuangGrowth / 10;
                elf.AnValue += elf.AnGrowth / 10;
            }
			while (count > 0);
		}
	}

	public static void ResetElfProps(Sys_Users_Elf elf)
	{
		elf.HpValue = elf.HpBase;
		elf.AttackValue = elf.AttackBase;
		elf.HuoValue = elf.HuoBase;
		elf.ShuiValue = elf.ShuiBase;
		elf.FengValue = elf.FengBase;
		elf.TuValue = elf.TuBase;
        elf.GuangValue = elf.GuangBase;
        elf.AnValue = elf.AnBase;
    }

	public static Dictionary<int, TS_UpgradeTemplate> UpgradeList(Sys_Users_Elf elf)
	{
		Elf_Template_Info elf_Template_Info = Elfs.FirstOrDefault((Elf_Template_Info e) => e.TemplateID == elf.TempID);
		if (elf_Template_Info == null)
		{
			throw new NullReferenceException($"elfTemplate is null with templateId = {elf.TempID}");
		}
		return elf_Template_Info.StarLevel switch
		{
			3 => ElfThreeGrades, 
			4 => ElfFourGrades, 
			5 => ElfFiveGrades, 
			_ => null, 
		};
	}

	private static int FindFirstEmptyPlace(IGamePlayer player)
	{
		int i;
		for (i = 0; i < 100; i++)
		{
			if (player.PlayerCharacter.ElfInfo.FirstOrDefault((Sys_Users_Elf e) => e.ElfID == i) == null)
			{
				return i;
			}
		}
		return -1;
	}

	public static bool AddSkillBook(GamePlayer player, ItemInfo item)
	{
		Sys_Users_SkillBooks sys_Users_SkillBooks = new Sys_Users_SkillBooks
		{
			UserID = player.PlayerId,
			TemplateID = item.TemplateID,
			Count = item.Count,
			IsBind = item.IsBinds
		};
		player.PlayerCharacter.SkillBooks.Add(sys_Users_SkillBooks);
		Out.SendSkillBooks(player, sys_Users_SkillBooks);
		return true;
	}

	public static bool StackSkillBook(GamePlayer player, ItemInfo item)
	{
		Sys_Users_SkillBooks sys_Users_SkillBooks = player.PlayerCharacter.SkillBooks.FirstOrDefault((Sys_Users_SkillBooks b) => b.TemplateID == item.TemplateID);
		if (sys_Users_SkillBooks == null)
		{
			return false;
		}
		sys_Users_SkillBooks.Count += item.Count;
		Out.SendSkillBooks(player, sys_Users_SkillBooks);
		return true;
	}
}
