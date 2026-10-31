using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bussiness;
using Bussiness.Helpers;
using Bussiness.Managers;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils;

public class PlayerMark : PlayerInventory
{
	private new static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected new object m_lock = new object();

	private ThreadSafeRandom m_random;

	public readonly int MAX_COUNT_EQUIP = 6;

	public readonly int TYPE_EQUIP = 1;

	protected new GamePlayer m_player;

	private Dictionary<int, List<MarkChipItemInfo>> m_chipBags;

	private List<MarkChipItemInfo> m_chipUpdateList;

	private bool m_saveToDb;

	public new GamePlayer Player => m_player;

	public Dictionary<int, List<MarkChipItemInfo>> ChipBags
	{
		get
		{
			return m_chipBags;
		}
		set
		{
			m_chipBags = value;
		}
	}

	public PlayerMark(GamePlayer player, bool saveTodb)
		: base(player, saveTodb, 0, 666, 1, autoStack: false)
	{
		m_player = player;
		m_random = new ThreadSafeRandom();
		m_saveToDb = saveTodb;
		m_chipBags = new Dictionary<int, List<MarkChipItemInfo>>();
		m_chipUpdateList = new List<MarkChipItemInfo>();
		CreateChipBags();
	}

	public override void LoadFromDatabase()
	{
		if (!m_saveToDb)
		{
			return;
		}
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		try
		{
			List<MarkChipItemInfo> allUsersMark = playerBussiness.GetAllUsersMark(Player.PlayerCharacter.ID);
			if (allUsersMark == null)
			{
				return;
			}
			lock (m_lock)
			{
				foreach (MarkChipItemInfo item in allUsersMark)
				{
					if (m_chipBags.ContainsKey(item.Type))
					{
						m_chipBags[item.Type].Add(item);
					}
				}
			}
		}
		finally
		{
			SendSyncChips();
		}
	}

	public override void SaveToDatabase()
	{
		if (!m_saveToDb)
		{
			return;
		}
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		lock (m_lock)
		{
			foreach (KeyValuePair<int, List<MarkChipItemInfo>> chipBag in m_chipBags)
			{
				List<MarkChipItemInfo> list = new List<MarkChipItemInfo>();
				foreach (MarkChipItemInfo item in chipBag.Value)
				{
					if (!item.IsExist)
					{
						list.Add(item);
					}
					if (item.IsDirty)
					{
						playerBussiness.AddOrUpdateUserMark(item);
						item.IsDirty = false;
					}
				}
				if (list.Count <= 0)
				{
					continue;
				}
				foreach (MarkChipItemInfo item2 in list)
				{
					m_chipBags[chipBag.Key].Remove(item2);
				}
			}
		}
	}

	public virtual Dictionary<int, List<MarkChipItemInfo>> GetAllChipBags()
	{
		Dictionary<int, List<MarkChipItemInfo>> dictionary = new Dictionary<int, List<MarkChipItemInfo>>();
		lock (m_lock)
		{
			foreach (KeyValuePair<int, List<MarkChipItemInfo>> chipBag in m_chipBags)
			{
				List<MarkChipItemInfo> value = chipBag.Value.Where((MarkChipItemInfo a) => a.IsBind).ToList();
				if (!dictionary.ContainsKey(chipBag.Key))
				{
					dictionary.Add(chipBag.Key, value);
				}
			}
		}
		return dictionary;
	}

	public virtual List<MarkChipItemInfo> GetAllChips(int bagType)
	{
		List<MarkChipItemInfo> result = new List<MarkChipItemInfo>();
		lock (m_lock)
		{
			if (m_chipBags.ContainsKey(bagType))
			{
				result = m_chipBags[bagType].Where((MarkChipItemInfo a) => a.IsBind).ToList();
			}
		}
		return result;
	}

	public virtual MarkChipItemInfo GetChip(int bagType, int ItemID)
	{
		List<MarkChipItemInfo> allChips = GetAllChips(bagType);
		return allChips.SingleOrDefault((MarkChipItemInfo a) => a.ItemID == ItemID);
	}

	public virtual MarkChipItemInfo GetChip(int ItemID)
	{
		lock (m_lock)
		{
			foreach (List<MarkChipItemInfo> value in m_chipBags.Values)
			{
				MarkChipItemInfo markChipItemInfo = value.SingleOrDefault((MarkChipItemInfo a) => a.IsBind && a.ItemID == ItemID);
				if (markChipItemInfo != null)
				{
					return markChipItemInfo;
				}
			}
		}
		return null;
	}

	public virtual List<MarkChipItemInfo> GetAllChipEquip(int place)
	{
		lock (m_lock)
		{
			return m_chipBags[TYPE_EQUIP].Where((MarkChipItemInfo a) => a.IsExist && a.SlotType == place).ToList();
		}
	}

	public virtual MarkChipItemInfo GetChipEquip(int postion)
	{
		lock (m_lock)
		{
			return m_chipBags[TYPE_EQUIP].SingleOrDefault((MarkChipItemInfo a) => a.IsExist && a.Place == postion);
		}
	}

	public virtual int GetPostionChipCanEquip(MarkChipItemInfo item, int slot)
	{
		int result = -1;
		EngraveDebrisInfo singleEngraveDebris = EngraveMgr.GetSingleEngraveDebris(item.TemplateID);
		if (singleEngraveDebris != null)
		{
			int num = item.MinPosition(slot) + (singleEngraveDebris.Place - 1);
			if (GetChipEquip(num) == null)
			{
				result = num;
			}
		}
		return result;
	}

	public virtual Dictionary<int, int> GetSuitData(int equipPlace)
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		List<MarkChipItemInfo> allChipEquip = GetAllChipEquip(equipPlace);
		EngraveDebrisInfo engraveDebrisInfo = null;
		foreach (MarkChipItemInfo item in allChipEquip)
		{
			engraveDebrisInfo = EngraveMgr.GetSingleEngraveDebris(item.TemplateID);
			if (engraveDebrisInfo != null)
			{
				if (dictionary.ContainsKey(engraveDebrisInfo.SetID))
				{
					dictionary[engraveDebrisInfo.SetID]++;
				}
				else
				{
					dictionary.Add(engraveDebrisInfo.SetID, 1);
				}
			}
		}
		return dictionary;
	}

	public virtual Dictionary<int, EngraveSetElementInfo> GetSuitList(int equipPlace)
	{
		Dictionary<int, int> suitData = GetSuitData(equipPlace);
		Dictionary<int, EngraveSetElementInfo> dictionary = new Dictionary<int, EngraveSetElementInfo>();
		foreach (EngraveSetElementInfo item in EngraveMgr.GetAllEngraveSetElement())
		{
			if (!dictionary.ContainsKey(item.Id) && suitData.ContainsKey(item.SetId) && item.Demand <= suitData[item.SetId])
			{
				dictionary.Add(item.Id, item);
			}
		}
		return dictionary;
	}

	public virtual List<EngraveSetElementInfo> GetAllSuitList()
	{
		List<EngraveSetElementInfo> list = new List<EngraveSetElementInfo>();
		for (int i = 1; i <= 6; i++)
		{
			list.AddRange(GetSuitList(i).Values.ToList());
		}
		return list;
	}

	public virtual void CreateChipBags()
	{
		lock (m_lock)
		{
			m_chipBags.Add(1, new List<MarkChipItemInfo>());
			foreach (EngraveSetInfo item in EngraveMgr.GetAllEngraveSet())
			{
				m_chipBags.Add(item.SetId, new List<MarkChipItemInfo>());
			}
		}
	}

	public override bool AddItem(ItemInfo item)
	{
		MarkChipItemInfo markChipItemInfo = CreateMarkItem(item.TemplateID, item.IsBinds, item.Template.Property1);
		if (markChipItemInfo != null)
		{
			for (int i = 0; i < item.Count; i++)
			{
				AddChipToBag(CreateMarkItem(item.TemplateID, item.IsBinds, item.Template.Property1));
			}
			return true;
		}
		return false;
	}

	public override bool AddItem(ItemInfo item, int minSlot)
	{
		return AddItem(item);
	}

	public override bool AddItem(ItemInfo item, int minSlot, int maxSlot)
	{
		return AddItem(item);
	}

	public virtual void AddChipToBag(MarkChipItemInfo item)
	{
		if (item.ItemID == 0)
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			playerBussiness.AddOrUpdateUserMark(item);
		}
		lock (m_lock)
		{
			if (m_chipBags.ContainsKey(item.Type))
			{
				m_chipBags[item.Type].Add(item);
			}
		}
		OnChipUpdate(item, sendToClient: true);
	}

	public virtual bool EquipChip(MarkChipItemInfo chipEquip, int equipType)
	{
		List<MarkChipItemInfo> allChipEquip = GetAllChipEquip(equipType);
		if (allChipEquip.Count < MAX_COUNT_EQUIP && chipEquip != null && chipEquip.Type != TYPE_EQUIP)
		{
			int postionChipCanEquip = GetPostionChipCanEquip(chipEquip, equipType);
			if (postionChipCanEquip >= 0)
			{
				lock (m_lock)
				{
					m_chipBags[chipEquip.Type].Remove(chipEquip);
					chipEquip.Type = TYPE_EQUIP;
					chipEquip.Place = postionChipCanEquip;
					m_chipBags[TYPE_EQUIP].Add(chipEquip);
				}
				OnChipUpdate(chipEquip, sendToClient: true);
				return true;
			}
		}
		return false;
	}

	public virtual bool UnEquipChip(MarkChipItemInfo chipEquip)
	{
		EngraveDebrisInfo singleEngraveDebris = EngraveMgr.GetSingleEngraveDebris(chipEquip.TemplateID);
		if (singleEngraveDebris != null && chipEquip.Type == TYPE_EQUIP)
		{
			lock (m_lock)
			{
				m_chipBags[TYPE_EQUIP].Remove(chipEquip);
				chipEquip.Type = singleEngraveDebris.SetID;
				chipEquip.Place = singleEngraveDebris.SetID;
				m_chipBags[chipEquip.Type].Add(chipEquip);
			}
			OnChipUpdate(chipEquip, sendToClient: true);
			return true;
		}
		return false;
	}

	public virtual void RemoveChipFromBag(MarkChipItemInfo item)
	{
		item.IsExist = false;
		OnChipUpdate(item, sendToClient: true);
	}

	public virtual MarkChipItemInfo CreateMarkItem(int templateId, bool isBind, int bornLv = 1)
	{
		MarkChipItemInfo markChipItemInfo = null;
		EngraveDebrisInfo singleEngraveDebris = EngraveMgr.GetSingleEngraveDebris(templateId);
		if (singleEngraveDebris != null)
		{
			markChipItemInfo = new MarkChipItemInfo();
			markChipItemInfo.UserID = m_player.PlayerId;
			markChipItemInfo.TemplateID = templateId;
			markChipItemInfo.Type = singleEngraveDebris.SetID;
			markChipItemInfo.Place = singleEngraveDebris.SetID;
			markChipItemInfo.bornLv = bornLv;
			markChipItemInfo.hammerLv = 0;
			markChipItemInfo.hLv = 0;
			markChipItemInfo.IsBind = isBind;
			MarkProDataInfo markProDataInfo = null;
			List<MarkProDataInfo> avalibleEngraveDebrisProp = EngraveMgr.GetAvalibleEngraveDebrisProp(singleEngraveDebris);
			if (avalibleEngraveDebrisProp.Count > 0)
			{
				markProDataInfo = avalibleEngraveDebrisProp[m_random.Next(0, avalibleEngraveDebrisProp.Count)];
				markProDataInfo.value = markProDataInfo.value / 6 * 2;
				markChipItemInfo.SetMainProp(markProDataInfo);
			}
			CreateSecondProp(markChipItemInfo);
		}
		return markChipItemInfo;
	}

	public virtual void CreateSecondProp(MarkChipItemInfo chip)
	{
		int num = 0;
		switch (chip.bornLv + chip.hammerLv)
		{
		case 2:
			num = 1;
			break;
		case 3:
			num = 2;
			break;
		case 4:
			num = 3;
			break;
		case 5:
			num = 4;
			break;
		}
		if (num <= 0)
		{
			return;
		}
		List<MarkProDataInfo> secondProps = chip.SecondProps;
		List<DebrisPropertyConfigInfo> debrisProperty = EngraveMgr.GetDebrisProperty(chip.TemplateID, secondProps);
		for (int i = 0; i < num; i++)
		{
			if (secondProps[i].type == 0)
			{
				DebrisPropertyConfigInfo debrisPropertyConfigInfo = debrisProperty[m_random.Next(0, debrisProperty.Count)];
				secondProps[i].type = debrisPropertyConfigInfo.AttributeType;
				secondProps[i].value = m_random.NextLow(debrisPropertyConfigInfo.MinValue, debrisPropertyConfigInfo.MaxValue);
				debrisProperty.Remove(debrisPropertyConfigInfo);
			}
		}
		chip.SetSecondProps(secondProps);
	}

	public virtual UserPropertiesInfo GetEquipProperties()
	{
		UserPropertiesInfo propertiesInfo = new UserPropertiesInfo();
		foreach (MarkChipItemInfo allChip in GetAllChips(TYPE_EQUIP))
		{
			UpdateUserProp(allChip.MainProp, ref propertiesInfo);
		}
		SetSuitProperties(ref propertiesInfo);
		return propertiesInfo;
	}

	private void SetSuitProperties(ref UserPropertiesInfo propertiesInfo)
	{
		foreach (EngraveSetElementInfo item in EngraveMgr.GetAllEngraveSetElement())
		{
			for (int i = 1; i <= 6; i++)
			{
				int num = 0;
				foreach (MarkChipItemInfo item2 in GetAllChipEquip(i))
				{
					EngraveDebrisInfo singleEngraveDebris = EngraveMgr.GetSingleEngraveDebris(item2.TemplateID);
					if (singleEngraveDebris.SetID != item.SetId)
					{
						continue;
					}
					num++;
					if (num < item.Demand)
					{
						continue;
					}
					foreach (MarkProDataInfo attribute in item.AttributeList)
					{
						UpdateUserProp(attribute, ref propertiesInfo);
					}
					break;
				}
			}
		}
	}

	private void SetSuitSecondProperties(ref Sys_Users_SecondPro propertiesInfo)
	{
		foreach (EngraveSetElementInfo item in EngraveMgr.GetAllEngraveSetElement())
		{
			for (int i = 1; i <= 6; i++)
			{
				int num = 0;
				foreach (MarkChipItemInfo item2 in GetAllChipEquip(i))
				{
					EngraveDebrisInfo singleEngraveDebris = EngraveMgr.GetSingleEngraveDebris(item2.TemplateID);
					if (singleEngraveDebris.SetID != item.SetId)
					{
						continue;
					}
					num++;
					if (num < item.Demand)
					{
						continue;
					}
					foreach (MarkProDataInfo attribute in item.AttributeList)
					{
						UpdateUserSecondProp(attribute, ref propertiesInfo);
					}
					break;
				}
			}
		}
	}

	public virtual Sys_Users_SecondPro GetEquipSecondProperties()
	{
		Sys_Users_SecondPro propertiesInfo = new Sys_Users_SecondPro();
		foreach (MarkChipItemInfo allChip in GetAllChips(TYPE_EQUIP))
		{
			foreach (MarkProDataInfo secondProp in allChip.SecondProps)
			{
				UpdateUserSecondProp(secondProp, ref propertiesInfo);
			}
		}
		SetSuitSecondProperties(ref propertiesInfo);
		return propertiesInfo;
	}

	public virtual void SetElementProperties(ref UserPropertiesInfo propertiesInfo)
	{
		foreach (EngraveSetElementInfo allSuit in GetAllSuitList())
		{
			foreach (MarkProDataInfo attribute in allSuit.AttributeList)
			{
				UpdateUserProp(attribute, ref propertiesInfo);
			}
		}
	}

	private void UpdateUserProp(MarkProDataInfo data, ref UserPropertiesInfo propertiesInfo)
	{
		switch (data.type)
		{
		case 31:
			propertiesInfo.Attack += data.value + data.attachValue;
			break;
		case 32:
			propertiesInfo.Defence += data.value + data.attachValue;
			break;
		case 33:
			propertiesInfo.Agility += data.value + data.attachValue;
			break;
		case 34:
			propertiesInfo.Luck += data.value + data.attachValue;
			break;
		case 37:
			propertiesInfo.Blood += data.value + data.attachValue;
			break;
		case 36:
			propertiesInfo.Damage += data.value + data.attachValue;
			break;
		case 35:
			propertiesInfo.Guard += data.value + data.attachValue;
			break;
		case 101:
			propertiesInfo.MagicAttack += data.value + data.attachValue;
			break;
		case 102:
			propertiesInfo.MagicDefence += data.value + data.attachValue;
			break;
		}
	}

	private void UpdateUserSecondProp(MarkProDataInfo data, ref Sys_Users_SecondPro propertiesInfo)
	{
		switch (data.type)
		{
		case 1:
			propertiesInfo.Crit += data.value + data.attachValue;
			break;
		case 3:
			propertiesInfo.SunderArmor += data.value + data.attachValue;
			break;
		case 7:
			propertiesInfo.ViolenceInjury += data.value + data.attachValue;
			break;
		case 9:
			propertiesInfo.Speed += data.value + data.attachValue;
			break;
		case 5:
			propertiesInfo.Kill += data.value + data.attachValue;
			break;
		case 2:
		case 4:
		case 6:
		case 8:
			break;
		}
	}

	public virtual void OnChipUpdate(MarkChipItemInfo chip, bool sendToClient = false)
	{
		lock (m_chipUpdateList)
		{
			if (!m_chipUpdateList.Contains(chip))
			{
				m_chipUpdateList.Add(chip);
			}
		}
		if (sendToClient)
		{
			SendUpdateChip();
		}
	}

	public virtual void SendUpdateChip()
	{
		lock (m_chipUpdateList)
		{
			if (m_chipUpdateList.Count > 20)
			{
				IEnumerable<IEnumerable<MarkChipItemInfo>> enumerable = m_chipUpdateList.ToArray().Split(20);
				foreach (IEnumerable<MarkChipItemInfo> item in enumerable)
				{
					_SendUpdateChip(item.ToList());
				}
			}
			else
			{
				_SendUpdateChip(m_chipUpdateList);
			}
			m_chipUpdateList.Clear();
		}
	}

	private void _SendUpdateChip(List<MarkChipItemInfo> chipItemInfos)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(529);
		gSPacketIn.WriteByte(26);
		gSPacketIn.WriteInt(chipItemInfos.Count);
		foreach (MarkChipItemInfo chipItemInfo in chipItemInfos)
		{
			gSPacketIn.WriteInt(chipItemInfo.ItemID);
			gSPacketIn.WriteInt(chipItemInfo.TemplateID);
			gSPacketIn.WriteInt(chipItemInfo.Place);
			gSPacketIn.WriteBoolean(chipItemInfo.IsExist);
			if (!chipItemInfo.IsExist)
			{
				gSPacketIn.WriteBoolean(val: false);
				continue;
			}
			gSPacketIn.WriteBoolean(val: false);
			gSPacketIn.WriteBoolean(chipItemInfo.IsBind);
			gSPacketIn.WriteInt(chipItemInfo.bornLv);
			gSPacketIn.WriteInt(chipItemInfo.hammerLv);
			gSPacketIn.WriteInt(chipItemInfo.hLv);
			gSPacketIn.WriteInt(chipItemInfo.MainProp.type);
			gSPacketIn.WriteInt(chipItemInfo.MainProp.value);
			gSPacketIn.WriteInt(chipItemInfo.MainProp.attachValue);
			foreach (MarkProDataInfo secondProp in chipItemInfo.SecondProps)
			{
				gSPacketIn.WriteInt(secondProp.type);
				gSPacketIn.WriteInt(secondProp.value);
				gSPacketIn.WriteInt(secondProp.attachValue);
				gSPacketIn.WriteInt(secondProp.hummerCount);
			}
		}
		m_player.SendTCP(gSPacketIn);
	}

	public virtual void SendSyncChips()
	{
		Dictionary<int, List<MarkChipItemInfo>> allChipBags = GetAllChipBags();
		foreach (KeyValuePair<int, List<MarkChipItemInfo>> item in allChipBags)
		{
			IEnumerable<MarkChipItemInfo>[] array = item.Value.ToArray().Split(40).ToArray();
			if (array.Length == 0)
			{
				GSPacketIn gSPacketIn = new GSPacketIn(529);
				gSPacketIn.WriteByte(2);
				gSPacketIn.WriteInt(item.Key);
				gSPacketIn.WriteInt(0);
				m_player.SendTCP(gSPacketIn);
				continue;
			}
			bool flag = array.Length > 1;
			MarkChipItemInfo[] array2 = array[0].ToArray();
			GSPacketIn gSPacketIn2 = new GSPacketIn(529);
			gSPacketIn2.WriteByte(2);
			gSPacketIn2.WriteInt(item.Key);
			gSPacketIn2.WriteInt(array2.Length);
			MarkChipItemInfo[] array3 = array2;
			foreach (MarkChipItemInfo markChipItemInfo in array3)
			{
				gSPacketIn2.WriteInt(markChipItemInfo.ItemID);
				gSPacketIn2.WriteInt(markChipItemInfo.TemplateID);
				gSPacketIn2.WriteInt(markChipItemInfo.Place);
				gSPacketIn2.WriteBoolean(markChipItemInfo.IsExist);
				if (!markChipItemInfo.IsExist)
				{
					gSPacketIn2.WriteBoolean(val: false);
					continue;
				}
				gSPacketIn2.WriteBoolean(val: false);
				gSPacketIn2.WriteBoolean(markChipItemInfo.IsBind);
				gSPacketIn2.WriteInt(markChipItemInfo.bornLv);
				gSPacketIn2.WriteInt(markChipItemInfo.hammerLv);
				gSPacketIn2.WriteInt(markChipItemInfo.hLv);
				gSPacketIn2.WriteInt(markChipItemInfo.MainProp.type);
				gSPacketIn2.WriteInt(markChipItemInfo.MainProp.value);
				gSPacketIn2.WriteInt(markChipItemInfo.MainProp.attachValue);
				foreach (MarkProDataInfo secondProp in markChipItemInfo.SecondProps)
				{
					gSPacketIn2.WriteInt(secondProp.type);
					gSPacketIn2.WriteInt(secondProp.value);
					gSPacketIn2.WriteInt(secondProp.attachValue);
					gSPacketIn2.WriteInt(secondProp.hummerCount);
				}
			}
			m_player.SendTCP(gSPacketIn2);
			if (!flag)
			{
				continue;
			}
			foreach (IEnumerable<MarkChipItemInfo> item2 in array.Skip(1))
			{
				_SendUpdateChip(item2.ToList());
			}
		}
	}
}
