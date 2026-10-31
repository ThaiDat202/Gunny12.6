using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Game.Logic;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils;

public abstract class PetAbstractInventory
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected object m_lock = new object();

	private int m_capalility;

	private int m_aCapalility;

	private int m_beginSlot;

	protected UsersPetInfo[] m_pets;

	protected UsersPetInfo[] m_adoptPets;

	protected ItemInfo[] m_adoptItems;

	public List<UsersPetLacrarInfo> lacresPet;

	protected List<int> m_changedPlaces = new List<int>();

	private int m_changeCount;

	public int BeginSlot => m_beginSlot;

	public int Capalility
	{
		get
		{
			return m_capalility;
		}
		set
		{
			m_capalility = ((value >= 0) ? ((value > m_pets.Length) ? m_pets.Length : value) : 0);
		}
	}

	public int ACapalility
	{
		get
		{
			return m_aCapalility;
		}
		set
		{
			m_aCapalility = ((value >= 0) ? ((value > m_adoptPets.Length) ? m_adoptPets.Length : value) : 0);
		}
	}

	public PetAbstractInventory(int capability, int aCapability, int beginSlot)
	{
		m_capalility = capability;
		m_aCapalility = aCapability;
		m_beginSlot = beginSlot;
		m_pets = new UsersPetInfo[capability];
		m_adoptPets = new UsersPetInfo[aCapability];
		m_adoptItems = new ItemInfo[aCapability];
		lacresPet = new List<UsersPetLacrarInfo>();
	}

	public virtual UsersPetInfo GetPetIsEquip()
	{
		lock (m_lock)
		{
			for (int i = 0; i < m_capalility; i++)
			{
				if (m_pets[i] != null && m_pets[i].IsEquip)
				{
					return m_pets[i];
				}
			}
		}
		return null;
	}

	public virtual bool AddAdoptPetTo(UsersPetInfo pet, int place)
	{
		if (pet == null || place >= m_aCapalility || place < 0)
		{
			return false;
		}
		lock (m_lock)
		{
			if (m_adoptPets[place] != null)
			{
				place = -1;
			}
			else
			{
				m_adoptPets[place] = pet;
				pet.Place = place;
			}
		}
		return place != -1;
	}

	public virtual bool AddPetTo(UsersPetInfo pet, int place)
	{
		if (pet == null || place >= m_capalility || place < 0)
		{
			return false;
		}
		lock (m_lock)
		{
			if (m_pets[place] == null)
			{
				m_pets[place] = pet;
				pet.Place = place;
			}
			else
			{
				place = -1;
			}
		}
		if (place != -1)
		{
			OnPlaceChanged(place);
		}
		return place != -1;
	}

	public virtual bool RemovePet(UsersPetInfo pet)
	{
		if (pet == null)
		{
			return false;
		}
		int num = -1;
		lock (m_lock)
		{
			for (int i = 0; i < m_capalility; i++)
			{
				if (m_pets[i] == pet)
				{
					num = i;
					m_pets[i] = null;
					break;
				}
			}
		}
		if (num != -1)
		{
			RemoveAt(ref m_pets, num);
			pet.Place = -1;
		}
		return num != -1;
	}

	public void RemoveAt(ref UsersPetInfo[] arr, int index)
	{
		for (int i = index; i < arr.Length - 1; i++)
		{
			arr[i] = arr[i + 1];
			OnPlaceChanged(i);
			if (arr[i] != null)
			{
				arr[i].Place = i;
			}
		}
	}

	public virtual bool RemoveAdoptPet(UsersPetInfo pet)
	{
		if (pet == null)
		{
			return false;
		}
		int num = -1;
		lock (m_lock)
		{
			for (int i = 0; i < m_aCapalility; i++)
			{
				if (m_adoptPets[i] == pet)
				{
					num = i;
					m_adoptPets[i] = null;
					break;
				}
			}
		}
		return num != -1;
	}

	public virtual UsersPetInfo GetPetAt(int slot)
	{
		if (slot < 0 || slot >= m_capalility)
		{
			return null;
		}
		lock (m_lock)
		{
			return m_pets[slot];
		}
	}

	public virtual UsersPetInfo GetPetById(int petId)
	{
		lock (m_lock)
		{
			UsersPetInfo[] pets = m_pets;
			foreach (UsersPetInfo usersPetInfo in pets)
			{
				if (usersPetInfo != null && usersPetInfo.ID == petId)
				{
					return usersPetInfo;
				}
			}
		}
		return null;
	}

	public int FindFirstEmptySlot()
	{
		return FindFirstEmptySlot(m_beginSlot);
	}

	public int FindFirstEmptySlot(int minSlot)
	{
		if (minSlot >= m_capalility)
		{
			return -1;
		}
		lock (m_lock)
		{
			for (int i = minSlot; i < m_capalility; i++)
			{
				if (m_pets[i] == null)
				{
					return i;
				}
			}
			return -1;
		}
	}

	public int FindLastEmptySlot()
	{
		lock (m_lock)
		{
			for (int num = m_capalility - 1; num >= 0; num--)
			{
				if (m_pets[num] == null)
				{
					return num;
				}
			}
			return -1;
		}
	}

	public virtual void Clear()
	{
		lock (m_lock)
		{
			for (int i = 0; i < m_capalility; i++)
			{
				m_pets[i] = null;
			}
		}
	}

	public virtual UsersPetInfo GetPetByTemplateID(int minSlot, int templateId)
	{
		lock (m_lock)
		{
			for (int i = minSlot; i < m_capalility; i++)
			{
				if (m_pets[i] != null && m_pets[i].TemplateID == templateId)
				{
					return m_pets[i];
				}
			}
			return null;
		}
	}

	public virtual UsersPetInfo[] GetPets()
	{
		List<UsersPetInfo> list = new List<UsersPetInfo>();
		for (int i = 0; i < m_capalility; i++)
		{
			if (m_pets[i] != null)
			{
				list.Add(m_pets[i]);
			}
		}
		return list.ToArray();
	}

	public int GetEmptyCount()
	{
		return GetEmptyCount(m_beginSlot);
	}

	public virtual int GetEmptyCount(int minSlot)
	{
		if (minSlot < 0 || minSlot > m_capalility - 1)
		{
			return 0;
		}
		int num = 0;
		lock (m_lock)
		{
			for (int i = minSlot; i < m_capalility; i++)
			{
				if (m_pets[i] == null)
				{
					num++;
				}
			}
		}
		return num;
	}

	public virtual UsersPetInfo GetAdoptPetAt(int slot)
	{
		if (slot < 0 || slot >= m_aCapalility)
		{
			return null;
		}
		return m_adoptPets[slot];
	}

	public virtual UsersPetInfo[] GetAdoptPet(int vipLv)
	{
		List<UsersPetInfo> list = new List<UsersPetInfo>();
		for (int i = 0; i < m_aCapalility; i++)
		{
			if (m_adoptPets[i] != null && m_adoptPets[i].IsExit)
			{
				list.Add(m_adoptPets[i]);
			}
		}
		list.Add(PetMgr.CreateNewPet(vipLv));
		return list.ToArray();
	}

	public void OnPlaceChanged(int place)
	{
		if (!m_changedPlaces.Contains(place))
		{
			m_changedPlaces.Add(place);
		}
		if (m_changeCount <= 0 && m_changedPlaces.Count > 0)
		{
			UpdateChangedPlaces();
		}
	}

	public void BeginChanges()
	{
		Interlocked.Increment(ref m_changeCount);
	}

	public void CommitChanges()
	{
		int num = Interlocked.Decrement(ref m_changeCount);
		if (num < 0)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("Inventory changes counter is bellow zero (forgot to use BeginChanges?)!\n\n" + Environment.StackTrace);
			}
			Thread.VolatileWrite(ref m_changeCount, 0);
		}
		if (num <= 0 && m_changedPlaces.Count > 0)
		{
			UpdateChangedPlaces();
		}
	}

	public virtual void UpdateChangedPlaces()
	{
		m_changedPlaces.Clear();
	}

	public virtual bool RenamePet(int place, string name)
	{
		lock (m_lock)
		{
			if (m_pets[place] != null)
			{
				m_pets[place].Name = name;
			}
		}
		OnPlaceChanged(place);
		return true;
	}

	public virtual bool EquipSkillPet(int place, int killId, int skillPlace, ref string msg)
	{
		string skill = killId + "," + skillPlace;
		lock (m_lock)
		{
			UsersPetInfo usersPetInfo = m_pets[place];
			if (usersPetInfo == null)
			{
				return false;
			}
			string[] skillEquip = usersPetInfo.GetSkillEquip();
			if (killId == 0)
			{
				m_pets[place].SkillEquip = SetSkillEquip(usersPetInfo.SkillEquip, skillEquip, skillPlace, skill);
				OnPlaceChanged(place);
				return true;
			}
			string[] array = skillEquip;
			foreach (string text in array)
			{
				if (!(text.Split(',')[0] == "-1") && text.Split(',')[0] == killId.ToString())
				{
					msg = "PetHandler.Msg18";
					return false;
				}
			}
			m_pets[place].SkillEquip = SetSkillEquip(usersPetInfo.SkillEquip, skillEquip, skillPlace, skill);
			OnPlaceChanged(place);
			return true;
		}
	}

	public string SetSkillEquip(string skillEquip, string[] list, int place, string skill)
	{
		string text = "";
		try
		{
			list[place] = skill;
			text = list[0];
			for (int i = 1; i < list.Length; i++)
			{
				text = text + "|" + list[i];
			}
		}
		catch (Exception ex)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("SetSkillEquip: " + ex);
			}
			return skillEquip;
		}
		return text;
	}

	public virtual bool UpdatePet(UsersPetInfo pet, int place)
	{
		if (pet == null)
		{
			return false;
		}
		int num = -1;
		lock (m_lock)
		{
			for (int i = 0; i < m_pets.Length; i++)
			{
				if (m_pets[i] != null)
				{
					num = m_pets[i].Place;
					if (num == place)
					{
						m_pets[i] = pet;
					}
					OnPlaceChanged(num);
				}
			}
		}
		return num > -1;
	}

	public virtual void UpdatePetFiveKillSlot(int vipLv)
	{
		lock (m_lock)
		{
			for (int i = 0; i < m_pets.Length; i++)
			{
				if (m_pets[i] != null)
				{
					m_pets[i].SkillEquip = PetMgr.ActiveEquipSkill(m_pets[i].Level, vipLv);
				}
			}
		}
	}

	public virtual bool EquipPet(int place, bool isEquip)
	{
		int num = -1;
		lock (m_lock)
		{
			for (int i = 0; i < m_pets.Length; i++)
			{
				if (m_pets[i] == null)
				{
					continue;
				}
				num = m_pets[i].Place;
				if (num == place)
				{
					if (m_pets[i].Hunger == 0)
					{
						return false;
					}
					m_pets[i].IsEquip = isEquip;
				}
				else
				{
					m_pets[i].IsEquip = false;
				}
				OnPlaceChanged(num);
			}
		}
		return num > -1;
	}

	public double GetRandomDouble(Random random, double min, double max)
	{
		return min + random.NextDouble() * (max - min);
	}

	public virtual void UpdatePet(UsersPetInfo pet)
	{
		if (pet.Place <= Capalility && pet.Place >= 0)
		{
			lock (m_lock)
			{
				m_pets[pet.Place] = pet;
			}
			OnPlaceChanged(pet.Place);
		}
	}

	public virtual bool UpdateEvolutionPet(UsersPetInfo pet, int level, int maxLevel, int vipLv)
	{
		int evoluTime = 1;
		int num = PetMgr.UpdateEvolution(pet.TemplateID, level, ref evoluTime);
		if (num > pet.TemplateID)
		{
			pet.TemplateID = num;
			PetTemplateInfo petTemplateInfo = PetMgr.FindPetTemplate(num);
			if (petTemplateInfo != null)
			{
				Random random = new Random();
				for (int i = 0; i < evoluTime; i++)
				{
					double[] propArr = null;
					double[] growArr = null;
					PetMgr.GetEvolutionPropArr(pet, petTemplateInfo, ref propArr, ref growArr);
					if (propArr != null && growArr != null)
					{
						double num2 = (double)petTemplateInfo.RareLevel * 0.1;
						if (num2 < growArr[0])
						{
							pet.BloodGrow += (int)(GetRandomDouble(random, num2, growArr[0]) * 10.0);
						}
						if (num2 < growArr[1])
						{
							pet.AttackGrow += (int)(GetRandomDouble(random, num2, growArr[1]) * 10.0);
						}
						if (num2 < growArr[2])
						{
							pet.DefenceGrow += (int)(GetRandomDouble(random, num2, growArr[2]) * 10.0);
						}
						if (num2 < growArr[3])
						{
							pet.AgilityGrow += (int)(GetRandomDouble(random, num2, growArr[3]) * 10.0);
						}
						if (num2 < growArr[4])
						{
							pet.LuckGrow += (int)(GetRandomDouble(random, num2, growArr[4]) * 10.0);
						}
					}
				}
			}
		}
		string skill = pet.Skill;
		string text = PetMgr.UpdateSkillPet(level, pet.TemplateID, maxLevel);
		pet.Skill = ((text == "") ? skill : text);
		pet.SkillEquip = PetMgr.ActiveEquipSkill(level, vipLv);
		pet.BuildProp();
		return true;
	}
}
