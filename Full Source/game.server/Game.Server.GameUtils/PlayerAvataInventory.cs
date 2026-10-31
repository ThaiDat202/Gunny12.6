using System;
using System.Collections.Generic;
using System.Linq;
using Bussiness;
using Bussiness.Managers;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils;

public class PlayerAvataInventory
{
	protected object m_lock = new object();

	protected GamePlayer m_player;

	private Dictionary<int, List<ItemInfo>> m_dressModelArr;

	private int m_currentModelIndex;

	private Dictionary<int, UserAvatarColectionInfo> m_avatarColect;

	public GamePlayer Player => m_player;

	public Dictionary<int, List<ItemInfo>> DressModelArr => m_dressModelArr;

	public int CurrentModelIndex
	{
		get
		{
			return m_currentModelIndex;
		}
		set
		{
			if (value > 5)
			{
				m_currentModelIndex = 5;
			}
			else
			{
				m_currentModelIndex = value;
			}
		}
	}

	public Dictionary<int, UserAvatarColectionInfo> AvatarColect => m_avatarColect;

	public PlayerAvataInventory(GamePlayer player)
	{
		m_player = player;
		m_dressModelArr = new Dictionary<int, List<ItemInfo>>
		{
			{
				0,
				new List<ItemInfo>()
			},
			{
				1,
				new List<ItemInfo>()
			},
			{
				2,
				new List<ItemInfo>()
			},
			{
				3,
				new List<ItemInfo>()
			},
			{
				4,
				new List<ItemInfo>()
			},
			{
				5,
				new List<ItemInfo>()
			}
		};
		m_avatarColect = new Dictionary<int, UserAvatarColectionInfo>();
	}

	public virtual void LoadFromDatabase()
	{
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		UserAvatarColectionInfo[] singleUserAvatarColectionInfo = playerBussiness.GetSingleUserAvatarColectionInfo(Player.PlayerCharacter.ID);
		if (singleUserAvatarColectionInfo.Length != 0)
		{
			UserAvatarColectionInfo[] array = singleUserAvatarColectionInfo;
			foreach (UserAvatarColectionInfo userAvatarColectionInfo in array)
			{
				if (AvatarColectionMgr.FindClothPropertyTemplate(userAvatarColectionInfo.dataId) == null)
				{
					continue;
				}
				string[] currentGroup = userAvatarColectionInfo.CurrentGroup;
				int num = 0;
				while (true)
				{
					if (num < currentGroup.Length)
					{
						string value = currentGroup[num];
						if (AvatarColectionMgr.FindClothGroupTemplateInfo(userAvatarColectionInfo.dataId, Convert.ToInt32(value), userAvatarColectionInfo.Sex) == null)
						{
							break;
						}
						num++;
						continue;
					}
					AddAvatarColectionInfo(userAvatarColectionInfo.dataId, userAvatarColectionInfo.Sex, userAvatarColectionInfo);
					break;
				}
			}
		}
		CheckNewAvatarColection();
		CheckInvalidStyle();
	}

	public virtual void SaveToDatabase()
	{
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		lock (m_lock)
		{
			foreach (UserAvatarColectionInfo value in m_avatarColect.Values)
			{
				if (value != null && value.IsDirty)
				{
					if (value.ID > 0)
					{
						playerBussiness.UpdateUserAvatarColectionInfo(value);
					}
					else
					{
						playerBussiness.AddUserAvatarColectionInfo(value);
					}
				}
			}
		}
	}

	public void UpdateInfo()
	{
		UpdateCurrentDressModels();
		AvatarColectionInfoChange();
	}

	public int CountInGroup(int groupId)
	{
		return AvatarColectionMgr.FindClothGroupTemplateInfo(groupId).Count;
	}

	public void CheckNewAvatarColection()
	{
		List<ClothPropertyTemplateInfo> clothPropertyTemplate = AvatarColectionMgr.GetClothPropertyTemplate();
		foreach (ClothPropertyTemplateInfo item in clothPropertyTemplate)
		{
			if (!AvatarColect.ContainsKey(item.ID))
			{
				UserAvatarColectionInfo info = new UserAvatarColectionInfo
				{
					UserID = m_player.PlayerCharacter.ID,
					endTime = DateTime.Now.AddDays(-1.0),
					dataId = item.ID,
					Sex = item.Sex,
					ActiveCount = CountInGroup(item.ID),
					ActiveDress = ""
				};
				AddAvatarColectionInfo(item.ID, item.Sex, info);
			}
		}
	}

	public void AddAvatarColectionInfo(int dataId, int sex, UserAvatarColectionInfo info)
	{
		if (AvatarColectionMgr.FindClothPropertyTemplate(dataId) == null)
		{
			Console.WriteLine("ClothPropertyTemplate.dataId: {0}, do not exit!", dataId);
			return;
		}
		lock (m_lock)
		{
			if (!m_avatarColect.ContainsKey(dataId))
			{
				m_avatarColect.Add(dataId, info);
			}
		}
	}

	public void AvatarColectionInfoChange()
	{
		Player.Out.SendAvatarColectionAllInfo(Player.PlayerId, AvatarColect);
	}

	public void AddPropAvatarColection(ref int att, ref int def, ref int agi, ref int luk, ref int hp)
	{
		List<ClothPropertyTemplateInfo> clothPropertyTemplate = AvatarColectionMgr.GetClothPropertyTemplate();
		foreach (ClothPropertyTemplateInfo item in clothPropertyTemplate)
		{
			if (AvatarColect.ContainsKey(item.ID) && AvatarColect[item.ID].isValidate)
			{
				if (AvatarColect[item.ID].FullActive())
				{
					att += item.Attack * 2;
					def += item.Defend * 2;
					agi += item.Agility * 2;
					luk += item.Luck * 2;
					hp += item.Blood * 2;
				}
				else if (AvatarColect[item.ID].HaftActive())
				{
					att += item.Attack;
					def += item.Defend;
					agi += item.Agility;
					luk += item.Luck;
					hp += item.Blood;
				}
			}
		}
	}

	public void AddBasePropAvatarColection(ref double avatarGuard, ref double avatarDame)
	{
		List<ClothPropertyTemplateInfo> clothPropertyTemplate = AvatarColectionMgr.GetClothPropertyTemplate();
		foreach (ClothPropertyTemplateInfo item in clothPropertyTemplate)
		{
			if (AvatarColect.ContainsKey(item.ID) && AvatarColect[item.ID].isValidate)
			{
				if (AvatarColect[item.ID].FullActive())
				{
					avatarGuard += item.Guard * 2;
					avatarDame += item.Damage * 2;
				}
				else if (AvatarColect[item.ID].HaftActive())
				{
					avatarGuard += item.Guard;
					avatarDame += item.Damage;
				}
			}
		}
	}

	public UserAvatarColectionInfo GetAvatar(int dataId)
	{
		if (AvatarColect.ContainsKey(dataId))
		{
			return AvatarColect[dataId];
		}
		return null;
	}

	public bool ActiveAvatarColection(int dataId, List<int> ids, int sex, int type)
	{
		lock (m_lock)
		{
			using List<int>.Enumerator enumerator = ids.GetEnumerator();
			if (enumerator.MoveNext())
			{
				int current = enumerator.Current;
				ItemInfo itemByTemplateID = Player.EquipBag.GetItemByTemplateID(0, current);
				if (itemByTemplateID != null && m_avatarColect.ContainsKey(dataId))
				{
					string activeDress = m_avatarColect[dataId].ActiveDress;
					if (activeDress.IndexOf(current.ToString(), StringComparison.Ordinal) != -1)
					{
						return false;
					}
					if (string.IsNullOrEmpty(activeDress))
					{
						List<ClothGroupTemplateInfo> list = AvatarColectionMgr.FindClothGroupTemplateInfo(dataId);
						if (GameProperties.VERSION.ToString().Contains("404"))
						{
							m_avatarColect[dataId].ActiveDress = FindDressActiveInBag(list);
						}
						if (m_avatarColect[dataId].ActiveCount < list.Count)
						{
							m_avatarColect[dataId].ActiveCount = list.Count;
						}
						m_avatarColect[dataId].ActiveDress = current.ToString();
					}
					else
					{
						m_avatarColect[dataId].ActiveDress = activeDress + "," + current;
					}
					itemByTemplateID.IsBinds = true;
					Player.EquipBag.UpdateItem(itemByTemplateID);
				}
				return true;
			}
		}
		return false;
	}

	public bool ActiveAvatarColection(int dataId, int id)
	{
		lock (m_lock)
		{
			if (m_avatarColect.ContainsKey(dataId))
			{
				string activeDress = m_avatarColect[dataId].ActiveDress;
				if (activeDress.IndexOf(id.ToString(), StringComparison.Ordinal) != -1)
				{
					return false;
				}
				if (string.IsNullOrEmpty(activeDress))
				{
					List<ClothGroupTemplateInfo> list = AvatarColectionMgr.FindClothGroupTemplateInfo(dataId);
					if (GameProperties.VERSION.ToString().Contains("404"))
					{
						m_avatarColect[dataId].ActiveDress = FindDressActiveInBag(list);
					}
					if (m_avatarColect[dataId].ActiveCount < list.Count)
					{
						m_avatarColect[dataId].ActiveCount = list.Count;
					}
					m_avatarColect[dataId].ActiveDress = id.ToString();
				}
				else
				{
					m_avatarColect[dataId].ActiveDress = activeDress + "," + id;
				}
				return true;
			}
		}
		return false;
	}

	public bool DelayAvatarColection(int dataId, int delay, int type)
	{
		if (AvatarColect.ContainsKey(dataId))
		{
			lock (m_lock)
			{
				if (!AvatarColect[dataId].isValidate)
				{
					m_avatarColect[dataId].endTime = DateTime.Now;
				}
				m_avatarColect[dataId].endTime = AvatarColect[dataId].endTime.AddDays(delay);
				if (m_avatarColect[dataId].FullActive() || m_avatarColect[dataId].HaftActive())
				{
					Player.EquipBag.UpdatePlayerProperties();
				}
				return true;
			}
		}
		return false;
	}

	public string FindDressActiveInBag(List<ClothGroupTemplateInfo> groups)
	{
		List<int> list = new List<int>();
		foreach (ClothGroupTemplateInfo group in groups)
		{
			ItemInfo itemByTemplateID = m_player.EquipBag.GetItemByTemplateID(0, group.TemplateID);
			if (itemByTemplateID != null && !list.Contains(itemByTemplateID.TemplateID))
			{
				list.Add(itemByTemplateID.TemplateID);
			}
		}
		if (list.Count == 0)
		{
			return "";
		}
		string text = list.Aggregate("", (string current, int id) => current + "," + id);
		return text.Substring(1);
	}

	public void UpdateCurrentDressModels()
	{
		m_player.Out.SendSetDressModelArr(m_player.PlayerCharacter.ID, DressModelArr);
		m_player.Out.SendCurentDressModel(m_player.PlayerCharacter.ID, CurrentModelIndex);
	}

	public void SaveDressModel(int slot, List<ItemInfo> dresses)
	{
		if (m_dressModelArr.ContainsKey(slot))
		{
			m_dressModelArr[slot] = dresses;
		}
		else if (m_dressModelArr.Count < 6)
		{
			m_dressModelArr.Add(slot, dresses);
		}
	}

	public void SetCurrentModel(int slot)
	{
		if (m_dressModelArr.ContainsKey(slot))
		{
			m_currentModelIndex = slot;
			m_player.Out.SendCurentDressModel(m_player.PlayerCharacter.ID, slot);
		}
		else
		{
			m_currentModelIndex = 0;
			m_player.Out.SendCurentDressModel(m_player.PlayerCharacter.ID, slot);
		}
	}

	public string DressModelArrToString()
	{
		string text = "";
		foreach (int key in m_dressModelArr.Keys)
		{
			if (m_dressModelArr[key].Count > 0)
			{
				string text2 = m_dressModelArr[key].Aggregate("", (string current, ItemInfo info) => current + $"{info.ItemID}|{info.TemplateID};");
				text2 = text2.Substring(0, text2.Length - 1);
				text = text + text2 + ",";
			}
			else
			{
				text += ",";
			}
		}
		return text;
	}

	public void StringToDressModelArr(string modelArr)
	{
		int num = 0;
		string[] array = modelArr.Split(',');
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (!string.IsNullOrWhiteSpace(text))
			{
				string[] source = text.Split(';');
				List<ItemInfo> dresses = (from item in source.Select(FindEquipDressModel)
					where item != null
					select item).ToList();
				SaveDressModel(num, dresses);
			}
			else
			{
				SaveDressModel(num, new List<ItemInfo>());
			}
			num++;
		}
		if (CurrentModelIndex > num)
		{
			CurrentModelIndex = num;
		}
	}

	private void CheckInvalidStyle()
	{
		string[] array = m_player.PlayerCharacter.Style.Split(',');
		string[] array2 = m_player.PlayerCharacter.Style.Split(',');
		for (int i = 0; i < array2.Length; i++)
		{
			string value = array2[i].Split('|')[0];
			if (!string.IsNullOrEmpty(value) && ItemMgr.FindItemTemplate(Convert.ToInt32(value)) == null)
			{
				array[i] = "";
			}
		}
		m_player.PlayerCharacter.Style = string.Join(",", array);
	}

	public ItemInfo FindEquipDressModel(string id)
	{
		int itemId = int.Parse(id.Split('|')[0]);
		return m_player.EquipBag.GetEquipDressModel(itemId);
	}
}
