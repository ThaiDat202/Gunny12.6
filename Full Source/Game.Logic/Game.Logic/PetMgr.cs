using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using log4net;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Logic;

public class PetMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<string, PetConfig> m_configs;

	private static Dictionary<int, PetLevel> m_levels;

	private static Dictionary<int, PetSkillElementInfo> m_skillElements;

	private static Dictionary<int, PetSkillInfo> m_skills;

	private static Dictionary<int, List<PetSkillLacre>> m_skills_lacres;

	private static Dictionary<int, PetSkillTemplateInfo> m_skillTemplates;

	private static Dictionary<int, PetTemplateInfo> m_templateIds;

	private static Dictionary<int, PetExpItemPriceInfo> m_expItemPrices;

	private static Dictionary<int, PetFightPropertyInfo> m_fightProperty;

	private static Dictionary<int, PetStarExpInfo> m_starExp;

	private static Dictionary<int, PetFormDataInfo> m_petFormDatas = new Dictionary<int, PetFormDataInfo>();

	private static List<GameNeedPetSkillInfo> m_gameNeedPetSkillInfos = new List<GameNeedPetSkillInfo>();

	private static Random rand;

	private static ReaderWriterLock m_clientLocker = new ReaderWriterLock();

	public static bool Init()
	{
		try
		{
			rand = new Random();
			return ReLoad();
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("PetInfoMgr", exception);
			}
			return false;
		}
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<string, PetConfig> dictionary = new Dictionary<string, PetConfig>();
			Dictionary<int, PetLevel> dictionary2 = new Dictionary<int, PetLevel>();
			Dictionary<int, PetSkillElementInfo> dictionary3 = new Dictionary<int, PetSkillElementInfo>();
			Dictionary<int, PetSkillInfo> dictionary4 = new Dictionary<int, PetSkillInfo>();
			Dictionary<int, PetSkillTemplateInfo> dictionary5 = new Dictionary<int, PetSkillTemplateInfo>();
			Dictionary<int, PetTemplateInfo> dictionary6 = new Dictionary<int, PetTemplateInfo>();
			Dictionary<int, PetTemplateInfo> dictionary7 = new Dictionary<int, PetTemplateInfo>();
			Dictionary<int, PetExpItemPriceInfo> dictionary8 = new Dictionary<int, PetExpItemPriceInfo>();
			Dictionary<int, PetFightPropertyInfo> fightProperty = new Dictionary<int, PetFightPropertyInfo>();
			Dictionary<int, PetStarExpInfo> starExp = new Dictionary<int, PetStarExpInfo>();
			PetFormDataInfo[] array = LoadPetFormDataDb();
			Dictionary<int, PetFormDataInfo> value = LoadPetFormDatas(array);
			if (array.Length != 0)
			{
				Interlocked.Exchange(ref m_petFormDatas, value);
			}
			if (LoadPetFromDb(dictionary, dictionary2, dictionary3, dictionary4, dictionary5, dictionary7, dictionary8, fightProperty, starExp))
			{
				try
				{
					m_configs = dictionary;
					m_levels = dictionary2;
					m_skillElements = dictionary3;
					m_skills = dictionary4;
					m_skillTemplates = dictionary5;
					m_templateIds = dictionary7;
					m_expItemPrices = dictionary8;
					m_fightProperty = fightProperty;
					m_starExp = starExp;
					List<GameNeedPetSkillInfo> list = LoadGameNeedPetSkill();
					if (list.Count > 0)
					{
						Interlocked.Exchange(ref m_gameNeedPetSkillInfos, list);
					}
					return true;
				}
				catch
				{
				}
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("PetMgr", exception);
			}
		}
		return false;
	}

	public static PetStarExpInfo[] GetPetStarExp()
	{
		return m_starExp.Values.ToArray();
	}

	public static PetStarExpInfo FindPetStarExp(int oldID)
	{
		m_clientLocker.AcquireWriterLock(-1);
		try
		{
			if (m_starExp.ContainsKey(oldID))
			{
				return m_starExp[oldID];
			}
		}
		finally
		{
			m_clientLocker.ReleaseWriterLock();
		}
		return null;
	}

	public static GameNeedPetSkillInfo[] GetGameNeedPetSkill()
	{
		return m_gameNeedPetSkillInfos.ToArray();
	}

	public static List<GameNeedPetSkillInfo> LoadGameNeedPetSkill()
	{
		List<GameNeedPetSkillInfo> list = new List<GameNeedPetSkillInfo>();
		List<string> list2 = new List<string>();
		foreach (PetSkillInfo value in m_skills.Values)
		{
			string effectPic = value.EffectPic;
			if (!string.IsNullOrEmpty(effectPic) && !list2.Contains(effectPic))
			{
				GameNeedPetSkillInfo gameNeedPetSkillInfo = new GameNeedPetSkillInfo();
				gameNeedPetSkillInfo.Pic = value.Pic;
				gameNeedPetSkillInfo.EffectPic = value.EffectPic;
				list.Add(gameNeedPetSkillInfo);
				list2.Add(effectPic);
			}
		}
		foreach (PetSkillElementInfo value2 in m_skillElements.Values)
		{
			string effectPic2 = value2.EffectPic;
			if (!string.IsNullOrEmpty(effectPic2) && !list2.Contains(effectPic2))
			{
				GameNeedPetSkillInfo gameNeedPetSkillInfo2 = new GameNeedPetSkillInfo();
				gameNeedPetSkillInfo2.Pic = value2.Pic;
				gameNeedPetSkillInfo2.EffectPic = value2.EffectPic;
				list.Add(gameNeedPetSkillInfo2);
				list2.Add(effectPic2);
			}
		}
		return list;
	}

	public static PetFormDataInfo[] LoadPetFormDataDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllPetFormData();
	}

	public static Dictionary<int, PetFormDataInfo> LoadPetFormDatas(PetFormDataInfo[] PetFormData)
	{
		Dictionary<int, PetFormDataInfo> dictionary = new Dictionary<int, PetFormDataInfo>();
		foreach (PetFormDataInfo petFormDataInfo in PetFormData)
		{
			if (!dictionary.Keys.Contains(petFormDataInfo.TemplateID))
			{
				dictionary.Add(petFormDataInfo.TemplateID, petFormDataInfo);
			}
		}
		return dictionary;
	}

	public static PetFormDataInfo FindPetFormData(int ID)
	{
		m_clientLocker.AcquireWriterLock(-1);
		try
		{
			if (m_petFormDatas.ContainsKey(ID))
			{
				return m_petFormDatas[ID];
			}
		}
		finally
		{
			m_clientLocker.ReleaseWriterLock();
		}
		return null;
	}

	public static PetSkillInfo GetSkillLacrar(int KindID, int slot, bool IsAdvance)
	{
		if (m_skills_lacres.ContainsKey(KindID))
		{
			foreach (PetSkillLacre item in m_skills_lacres[KindID])
			{
				if (item.Slot == slot && item.IsAdvance == IsAdvance && m_skills.ContainsKey(item.SkillID))
				{
					return m_skills[item.SkillID];
				}
			}
		}
		return null;
	}

	private static bool LoadPetFromDb(Dictionary<string, PetConfig> Config, Dictionary<int, PetLevel> Level, Dictionary<int, PetSkillElementInfo> SkillElement, Dictionary<int, PetSkillInfo> Skill, Dictionary<int, PetSkillTemplateInfo> SkillTemplate, Dictionary<int, PetTemplateInfo> TemplateId, Dictionary<int, PetExpItemPriceInfo> PetExpItemPrice, Dictionary<int, PetFightPropertyInfo> FightProperty, Dictionary<int, PetStarExpInfo> StarExp)
	{
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			PetConfig[] allPetConfig = produceBussiness.GetAllPetConfig();
			PetLevel[] allPetLevel = produceBussiness.GetAllPetLevel();
			PetSkillElementInfo[] allPetSkillElementInfo = produceBussiness.GetAllPetSkillElementInfo();
			PetSkillInfo[] allPetSkillInfo = produceBussiness.GetAllPetSkillInfo();
			PetSkillTemplateInfo[] allPetSkillTemplateInfo = produceBussiness.GetAllPetSkillTemplateInfo();
			PetTemplateInfo[] allPetTemplateInfo = produceBussiness.GetAllPetTemplateInfo();
			PetExpItemPriceInfo[] allPetExpItemPrice = produceBussiness.GetAllPetExpItemPrice();
			PetFightPropertyInfo[] allPetFightProperty = produceBussiness.GetAllPetFightProperty();
			PetStarExpInfo[] allPetStarExp = produceBussiness.GetAllPetStarExp();
			m_skills_lacres = produceBussiness.GetAllPetSkillsLacre();
			PetExpItemPriceInfo[] array = allPetExpItemPrice;
			foreach (PetExpItemPriceInfo petExpItemPriceInfo in array)
			{
				if (!PetExpItemPrice.ContainsKey(petExpItemPriceInfo.Count))
				{
					PetExpItemPrice.Add(petExpItemPriceInfo.Count, petExpItemPriceInfo);
				}
			}
			PetConfig[] array2 = allPetConfig;
			foreach (PetConfig petConfig in array2)
			{
				if (!Config.ContainsKey(petConfig.Name))
				{
					Config.Add(petConfig.Name, petConfig);
				}
			}
			PetLevel[] array3 = allPetLevel;
			foreach (PetLevel petLevel in array3)
			{
				if (!Level.ContainsKey(petLevel.Level))
				{
					Level.Add(petLevel.Level, petLevel);
				}
			}
			PetSkillElementInfo[] array4 = allPetSkillElementInfo;
			foreach (PetSkillElementInfo petSkillElementInfo in array4)
			{
				if (!SkillElement.ContainsKey(petSkillElementInfo.ID))
				{
					SkillElement.Add(petSkillElementInfo.ID, petSkillElementInfo);
				}
			}
			int num = 0;
			PetSkillTemplateInfo[] array5 = allPetSkillTemplateInfo;
			foreach (PetSkillTemplateInfo value in array5)
			{
				num++;
				if (!SkillTemplate.ContainsKey(num))
				{
					SkillTemplate.Add(num, value);
				}
			}
			PetTemplateInfo[] array6 = allPetTemplateInfo;
			foreach (PetTemplateInfo petTemplateInfo in array6)
			{
				if (!TemplateId.ContainsKey(petTemplateInfo.TemplateID))
				{
					TemplateId.Add(petTemplateInfo.TemplateID, petTemplateInfo);
				}
			}
			PetSkillInfo[] array7 = allPetSkillInfo;
			foreach (PetSkillInfo petSkillInfo in array7)
			{
				if (!Skill.ContainsKey(petSkillInfo.ID))
				{
					Skill.Add(petSkillInfo.ID, petSkillInfo);
				}
			}
			PetFightPropertyInfo[] array8 = allPetFightProperty;
			foreach (PetFightPropertyInfo petFightPropertyInfo in array8)
			{
				if (!FightProperty.ContainsKey(petFightPropertyInfo.ID))
				{
					FightProperty.Add(petFightPropertyInfo.ID, petFightPropertyInfo);
				}
			}
			PetStarExpInfo[] array9 = allPetStarExp;
			foreach (PetStarExpInfo petStarExpInfo in array9)
			{
				if (!StarExp.ContainsKey(petStarExpInfo.OldID))
				{
					StarExp.Add(petStarExpInfo.OldID, petStarExpInfo);
				}
			}
		}
		return true;
	}

	public static PetFightPropertyInfo FindFightProperty(int level)
	{
		m_clientLocker.AcquireWriterLock(-1);
		try
		{
			if (m_fightProperty.ContainsKey(level))
			{
				return m_fightProperty[level];
			}
		}
		finally
		{
			m_clientLocker.ReleaseWriterLock();
		}
		return null;
	}

	public static int GetEvolutionGP(int level)
	{
		return FindFightProperty(level + 1)?.Exp ?? (-1);
	}

	public static int GetEvolutionMax()
	{
		return m_fightProperty.Count;
	}

	public static int GetEvolutionGrade(int GP)
	{
		int count = m_fightProperty.Count;
		if (GP >= FindFightProperty(count).Exp)
		{
			return count;
		}
		for (int i = 1; i <= count; i++)
		{
			PetFightPropertyInfo petFightPropertyInfo = FindFightProperty(i);
			if (petFightPropertyInfo == null)
			{
				return 1;
			}
			if (GP < petFightPropertyInfo.Exp)
			{
				return (i - 1 >= 0) ? (i - 1) : 0;
			}
		}
		return 0;
	}

	public static PetExpItemPriceInfo FindPetExpItemPrice(int count)
	{
		if (m_expItemPrices == null)
		{
			Init();
		}
		if (m_expItemPrices.ContainsKey(count))
		{
			return m_expItemPrices[count];
		}
		return null;
	}

	public static PetConfig FindConfig(string key)
	{
		if (m_configs == null)
		{
			Init();
		}
		if (m_configs.ContainsKey(key))
		{
			return m_configs[key];
		}
		return null;
	}

	public static PetLevel FindPetLevel(int level)
	{
		if (m_levels == null)
		{
			Init();
		}
		if (m_levels.ContainsKey(level))
		{
			return m_levels[level];
		}
		return null;
	}

	public static PetSkillElementInfo FindPetSkillElement(int SkillID)
	{
		if (m_skillElements == null)
		{
			Init();
		}
		if (m_skillElements.ContainsKey(SkillID))
		{
			return m_skillElements[SkillID];
		}
		return null;
	}

	public static PetSkillInfo FindPetSkill(int SkillID)
	{
		if (m_skills == null)
		{
			Init();
		}
		if (m_skills.ContainsKey(SkillID))
		{
			return m_skills[SkillID];
		}
		return null;
	}

	public static PetSkillInfo[] GetPetSkills()
	{
		List<PetSkillInfo> list = new List<PetSkillInfo>();
		if (m_skills == null)
		{
			Init();
		}
		foreach (PetSkillInfo value in m_skills.Values)
		{
			list.Add(value);
		}
		return list.ToArray();
	}

	public static PetSkillTemplateInfo GetPetSkillTemplate(int ID)
	{
		if (m_skillTemplates == null)
		{
			Init();
		}
		if (m_skillTemplates.ContainsKey(ID))
		{
			return m_skillTemplates[ID];
		}
		return null;
	}

	public static PetTemplateInfo FindPetTemplate(int TemplateID)
	{
		m_clientLocker.AcquireWriterLock(-1);
		try
		{
			if (m_templateIds.ContainsKey(TemplateID))
			{
				return m_templateIds[TemplateID];
			}
		}
		finally
		{
			m_clientLocker.ReleaseWriterLock();
		}
		return null;
	}

	public static PetTemplateInfo FindPetTemplateByKind(int star, int kindId)
	{
		foreach (PetTemplateInfo value in m_templateIds.Values)
		{
			if (value.KindID == kindId && star == value.StarLevel)
			{
				return value;
			}
		}
		return null;
	}

	public static PetSkillTemplateInfo[] GetPetSkillByKindID(int KindID)
	{
		List<PetSkillTemplateInfo> list = new List<PetSkillTemplateInfo>();
		foreach (PetSkillTemplateInfo value in m_skillTemplates.Values)
		{
			if (value.KindID == KindID)
			{
				list.Add(value);
			}
		}
		return list.ToArray();
	}

	public static List<int> GetPetSkillByKindID(int KindID, int lv, int playerLevel)
	{
		List<int> list = new List<int>();
		List<string> list2 = new List<string>();
		int num = ((lv > playerLevel) ? playerLevel : lv);
		PetSkillTemplateInfo[] petSkillByKindID = GetPetSkillByKindID(KindID, num);
		for (int i = 1; i <= num; i++)
		{
			PetSkillTemplateInfo[] array = petSkillByKindID;
			foreach (PetSkillTemplateInfo petSkillTemplateInfo in array)
			{
				if (petSkillTemplateInfo.MinLevel == i)
				{
					string[] array2 = petSkillTemplateInfo.DeleteSkillIDs.Split(',');
					string[] array3 = array2;
					foreach (string item in array3)
					{
						list2.Add(item);
					}
					list.Add(petSkillTemplateInfo.SkillID);
				}
			}
		}
		foreach (string item3 in list2)
		{
			if (!string.IsNullOrEmpty(item3))
			{
				int item2 = int.Parse(item3);
				list.Remove(item2);
			}
		}
		list.Sort();
		return list;
	}

	public static PetSkillTemplateInfo[] GetPetSkillByKindID(int kindID, int level)
	{
		List<PetSkillTemplateInfo> list = new List<PetSkillTemplateInfo>();
		foreach (PetSkillTemplateInfo value in m_skillTemplates.Values)
		{
			if (value.KindID == kindID && value.MinLevel <= level)
			{
				list.Add(value);
			}
		}
		return list.ToArray();
	}

	public static List<UsersPetInfo> CreateAdoptList(int userID, int playerLevel, int vipLv)
	{
		int num = Convert.ToInt32(FindConfig("AdoptCount").Value);
		List<UsersPetInfo> list = new List<UsersPetInfo>();
		List<PetTemplateInfo> info = null;
		int num2 = 0;
		while (num2 < num)
		{
			if (DropInventory.GetPetDrop(613, 1, ref info) && info != null)
			{
				int index = rand.Next(info.Count);
				UsersPetInfo usersPetInfo = CreatePet(info[index], userID, num2, playerLevel, vipLv);
				usersPetInfo.IsExit = true;
				list.Add(usersPetInfo);
				num2++;
			}
		}
		return list;
	}

	public static List<UsersPetInfo> CreateFirstAdoptList(int userID, int playerLevel, int vipLv)
	{
		List<int> list = new List<int> { 100301, 110301, 120301, 130301 };
		List<UsersPetInfo> list2 = new List<UsersPetInfo>();
		for (int i = 0; i < list.Count; i++)
		{
			PetTemplateInfo info = FindPetTemplate(list[i]);
			UsersPetInfo usersPetInfo = CreatePet(info, userID, i, playerLevel, vipLv);
			usersPetInfo.IsExit = true;
			list2.Add(usersPetInfo);
		}
		return list2;
	}

	public static string ActiveEquipSkill(int Level, int vipLevel)
	{
		string[] array = new string[5] { "0,0", "-1,1", "-1,2", "-1,3", "-1,4" };
		if (Level >= 20 && Level < 30)
		{
			array[1] = "0,1";
		}
		if (Level >= 30 && Level < 50)
		{
			array[1] = "0,1";
			array[2] = "0,2";
		}
		if (Level >= 50)
		{
			array[1] = "0,1";
			array[2] = "0,2";
			array[3] = "0,3";
		}
		if (vipLevel >= 1)
		{
			array[4] = "0,4";
		}
		string text = array[0];
		for (int i = 1; i < array.Length; i++)
		{
			text = text + "|" + array[i];
		}
		return text;
	}

	public static int UpdateEvolution(int TemplateID, int lv, ref int evoluTime)
	{
		string text = TemplateID.ToString();
		int num = Convert.ToInt32(FindConfig("EvolutionLevel1").Value);
		int num2 = Convert.ToInt32(FindConfig("EvolutionLevel2").Value);
		evoluTime = 1;
		int num3 = -1;
		if (!(text.Substring(text.Length - 1, 1) == "1"))
		{
			num3 = ((!(text.Substring(text.Length - 1, 1) == "2")) ? TemplateID : (TemplateID + 1));
		}
		else if (lv < num)
		{
			num3 = TemplateID;
		}
		else if (lv < num2)
		{
			num3 = TemplateID + 1;
		}
		else
		{
			num3 = TemplateID + 2;
			evoluTime = 2;
		}
		return FindPetTemplate(num3)?.TemplateID ?? TemplateID;
	}

	public static string UpdateSkillPet(int Level, int TemplateID, int playerLevel)
	{
		string text = string.Empty;
		PetTemplateInfo petTemplateInfo = FindPetTemplate(TemplateID);
		if (petTemplateInfo == null)
		{
			log.Error("Pet not found: " + TemplateID);
			return text;
		}
		List<int> petSkillByKindID = GetPetSkillByKindID(petTemplateInfo.KindID, Level, playerLevel);
		if (petSkillByKindID.Count > 0)
		{
			text = petSkillByKindID[0] + ",0";
			for (int i = 1; i < petSkillByKindID.Count; i++)
			{
				text = text + "|" + petSkillByKindID[i] + "," + i;
			}
		}
		else
		{
			log.Error("Pet skill not found! info.KindID: " + petTemplateInfo.KindID);
		}
		return text;
	}

	public static string UpdateAwakenSkill(string Skill, int TemplateID, int skillId1, int skillId2)
	{
		string text = string.Empty;
		PetTemplateInfo petTemplateInfo = FindPetTemplate(TemplateID);
		if (petTemplateInfo == null)
		{
			log.Error("Pet not found: " + TemplateID);
			return text;
		}
		List<int> list = new List<int>();
		string[] array = Skill.Split('|');
		for (int i = 0; i < array.Length; i++)
		{
			list.Add(int.Parse(array[i].Split(',')[0]));
		}
		if (list.Count > 0)
		{
			List<int> petAwakenSkillByKindID = PetSpecialSkillMgr.GetPetAwakenSkillByKindID(list, skillId1, skillId2);
			if (petAwakenSkillByKindID.Count > 0)
			{
				text = petAwakenSkillByKindID[0] + ",0";
				for (int j = 1; j < petAwakenSkillByKindID.Count; j++)
				{
					text = text + "|" + petAwakenSkillByKindID[j] + "," + j;
				}
			}
		}
		else
		{
			log.Error("Pet skill not found! info.KindID: " + petTemplateInfo.KindID);
		}
		return text;
	}

	public static int GetLevel(int GP, int playerLevel)
	{
		if (GP >= FindPetLevel(playerLevel).GP)
		{
			return playerLevel;
		}
		for (int i = 1; i <= playerLevel; i++)
		{
			if (GP < FindPetLevel(i).GP)
			{
				return (i - 1 == 0) ? 1 : (i - 1);
			}
		}
		return 1;
	}

	public static int GetGP(int level, int playerLevel)
	{
		int num = 72;
		for (int i = 1; i <= num; i++)
		{
			if (level == FindPetLevel(i).Level)
			{
				return FindPetLevel(i).GP;
			}
		}
		return 0;
	}

	public static void GetEvolutionPropArr(UsersPetInfo _petInfo, PetTemplateInfo petTempleteInfo, ref double[] propArr, ref double[] growArr)
	{
		double[] array = new double[5]
		{
			_petInfo.Blood * 100,
			_petInfo.Attack * 100,
			_petInfo.Defence * 100,
			_petInfo.Agility * 100,
			_petInfo.Luck * 100
		};
		double[] array2 = new double[5] { _petInfo.BloodGrow, _petInfo.AttackGrow, _petInfo.DefenceGrow, _petInfo.AgilityGrow, _petInfo.LuckGrow };
		double[] array3 = new double[5] { petTempleteInfo.HighBlood, petTempleteInfo.HighAttack, petTempleteInfo.HighDefence, petTempleteInfo.HighAgility, petTempleteInfo.HighLuck };
		double[] propArr2 = new double[5] { petTempleteInfo.HighBloodGrow, petTempleteInfo.HighAttackGrow, petTempleteInfo.HighDefenceGrow, petTempleteInfo.HighAgilityGrow, petTempleteInfo.HighLuckGrow };
		double[] array4 = array3;
		double[] addedPropArr = GetAddedPropArr(1, propArr2);
		double[] array5 = array3;
		double[] addedPropArr2 = GetAddedPropArr(2, propArr2);
		double[] array6 = array3;
		double[] addedPropArr3 = GetAddedPropArr(3, propArr2);
		if (_petInfo.Level < 30)
		{
			for (int i = 0; i < array4.Length; i++)
			{
				array4[i] += (double)(_petInfo.Level - 1) * addedPropArr[i] - array[i];
				addedPropArr[i] -= array2[i];
				array4[i] = Math.Ceiling(array4[i] / 10.0) / 10.0;
				addedPropArr[i] = Math.Ceiling(addedPropArr[i] / 10.0) / 10.0;
			}
			propArr = array4;
			growArr = addedPropArr;
		}
		else if (_petInfo.Level < 50)
		{
			for (int j = 0; j < array5.Length; j++)
			{
				array5[j] += (double)(_petInfo.Level - 30) * addedPropArr2[j] + 29.0 * addedPropArr[j] - array[j];
				addedPropArr2[j] -= array2[j];
				array5[j] = Math.Ceiling(array5[j] / 10.0) / 10.0;
				addedPropArr2[j] = Math.Ceiling(addedPropArr2[j] / 10.0) / 10.0;
			}
			propArr = array5;
			growArr = addedPropArr2;
		}
		else
		{
			for (int k = 0; k < array6.Length; k++)
			{
				array6[k] += (double)(_petInfo.Level - 50) * addedPropArr3[k] + 20.0 * addedPropArr2[k] + 29.0 * addedPropArr[k] - array[k];
				addedPropArr3[k] -= array2[k];
				array6[k] = Math.Ceiling(array6[k] / 10.0) / 10.0;
				addedPropArr3[k] = Math.Ceiling(addedPropArr3[k] / 10.0) / 10.0;
			}
			propArr = array6;
			growArr = addedPropArr3;
		}
	}

	public static double[] GetAddedPropArr(int grade, double[] propArr)
	{
		double[] array = new double[5]
		{
			propArr[0] * Math.Pow(2.0, grade - 1),
			0.0,
			0.0,
			0.0,
			0.0
		};
		for (int i = 1; i < 5; i++)
		{
			array[i] = propArr[i] * Math.Pow(1.5, grade - 1);
		}
		return array;
	}

	public static UsersPetInfo CreatePet(PetTemplateInfo info, int userID, int place, int playerLevel, int vipLv)
	{
		UsersPetInfo usersPetInfo = new UsersPetInfo();
		Random random = new Random();
		double num = (double)info.StarLevel * 0.1;
		usersPetInfo.BloodGrow = (int)Math.Ceiling((double)(random.Next((int)((double)info.HighBlood / (1.7 - num)), info.HighBlood - (int)((double)info.HighBlood / 17.1)) / 10));
		usersPetInfo.AttackGrow = random.Next((int)((double)info.HighAttack / (1.7 - num)), info.HighAttack - (int)((double)info.HighAttack / 17.1));
		usersPetInfo.DefenceGrow = random.Next((int)((double)info.HighDefence / (1.7 - num)), info.HighDefence - (int)((double)info.HighDefence / 17.1));
		usersPetInfo.AgilityGrow = random.Next((int)((double)info.HighAgility / (1.7 - num)), info.HighAgility - (int)((double)info.HighAgility / 17.1));
		usersPetInfo.LuckGrow = random.Next((int)((double)info.HighLuck / (1.7 - num)), info.HighLuck - (int)((double)info.HighLuck / 17.1));
		usersPetInfo.ID = 0;
		usersPetInfo.Hunger = 10000;
		usersPetInfo.TemplateID = info.TemplateID;
		usersPetInfo.Name = info.Name;
		usersPetInfo.UserID = userID;
		usersPetInfo.Place = place;
		usersPetInfo.Level = 1;
		usersPetInfo.BuildProp();
		usersPetInfo.Skill = UpdateSkillPet(1, info.TemplateID, playerLevel);
		usersPetInfo.SkillEquip = ActiveEquipSkill(1, vipLv);
		usersPetInfo.IsExit = true;
		usersPetInfo.PetEquips = new List<PetEquipInfo>();
		usersPetInfo.BaseProp = JsonConvert.SerializeObject(info);
		usersPetInfo.eQPets = SerializePetEquip(usersPetInfo.PetEquips);
		PlayerBussiness playerBussiness = new PlayerBussiness();
		playerBussiness.AddUserPet(usersPetInfo);
		return usersPetInfo;
	}

	public static string SerializePetEquip(List<PetEquipInfo> eqs)
	{
		return JsonConvert.SerializeObject(eqs);
	}

	public static void purificar(UsersPetInfo petInfo, bool bld, bool att, bool def, bool agi, bool luck)
	{
		PetTemplateInfo petTemplateInfo = FindPetTemplate(petInfo.TemplateID);
		double num = (double)petTemplateInfo.StarLevel * 0.1;
		Random random = new Random();
		if (!bld)
		{
			petInfo.BloodGrow = random.Next(petTemplateInfo.HighBloodGrow * (random.Next(1, random.Next(2, 100)) / 100), petTemplateInfo.HighBloodGrow);
		}
		if (!att)
		{
			petInfo.AttackGrow = random.Next(petTemplateInfo.HighAttackGrow * (random.Next(1, random.Next(2, 100)) / 100), petTemplateInfo.HighAttackGrow);
		}
		if (!def)
		{
			petInfo.DefenceGrow = random.Next(petTemplateInfo.HighDefenceGrow * (random.Next(1, random.Next(2, 100)) / 100), petTemplateInfo.HighDefenceGrow);
		}
		if (!agi)
		{
			petInfo.AgilityGrow = random.Next(petTemplateInfo.HighAgilityGrow * (random.Next(1, random.Next(2, 100)) / 100), petTemplateInfo.HighAgilityGrow);
		}
		if (!luck)
		{
			petInfo.LuckGrow = random.Next(petTemplateInfo.HighLuckGrow * (random.Next(1, random.Next(2, 100)) / 100), petTemplateInfo.HighLuckGrow);
		}
	}

	public static UsersPetInfo CreateNewPet(int vipLv)
	{
		string[] array = FindConfig("NewPet").Value.Split(',');
		int num = rand.Next(array.Length);
		PetTemplateInfo info = FindPetTemplate(Convert.ToInt32(array[num]));
		return CreatePet(info, -1, -1, 60, vipLv);
	}
}
