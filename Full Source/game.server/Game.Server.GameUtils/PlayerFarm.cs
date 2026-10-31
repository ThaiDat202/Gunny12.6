using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bussiness;
using Bussiness.Managers;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils;

public class PlayerFarm : PlayerFarmInventory
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected GamePlayer m_player;

	private bool m_saveToDb;

	private bool m_loadFromDb;

	private List<UserFieldInfo> m_removedList = new List<UserFieldInfo>();

	public GamePlayer Player => m_player;

	public PlayerFarm(GamePlayer player, bool saveTodb, int capibility, int beginSlot)
		: base(capibility, beginSlot)
	{
		m_player = player;
		m_saveToDb = saveTodb;
		m_loadFromDb = false;
	}

	public virtual void LoadFromDatabase()
	{
		if (!m_saveToDb)
		{
			return;
		}
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		UserFarmInfo singleFarm = playerBussiness.GetSingleFarm(m_player.PlayerCharacter.ID);
		UserFieldInfo[] singleFields = playerBussiness.GetSingleFields(m_player.PlayerCharacter.ID);
		try
		{
			UpdateFarm(singleFarm);
			if (singleFarm != null)
			{
				UserFieldInfo[] array = singleFields;
				foreach (UserFieldInfo userFieldInfo in array)
				{
					AddFieldTo(userFieldInfo, userFieldInfo.FieldID, singleFarm.FarmID);
				}
			}
		}
		finally
		{
			if (m_farm == null)
			{
				CreateFarm(m_player.PlayerCharacter.ID, m_player.PlayerCharacter.NickName);
			}
		}
	}

	public virtual void SaveToDatabase()
	{
		if (!m_saveToDb)
		{
			return;
		}
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		lock (m_lock)
		{
			if (m_farm != null && m_farm.IsDirty)
			{
				if (m_farm.ID > 0)
				{
					playerBussiness.UpdateFarm(m_farm);
				}
				else
				{
					playerBussiness.AddFarm(m_farm);
				}
			}
		}
		lock (m_lock)
		{
			if (m_farm == null)
			{
				return;
			}
			for (int i = 0; i < m_fields.Length; i++)
			{
				UserFieldInfo userFieldInfo = m_fields[i];
				if (userFieldInfo != null && userFieldInfo.IsDirty)
				{
					if (userFieldInfo.ID > 0)
					{
						playerBussiness.UpdateFields(userFieldInfo);
					}
					else
					{
						playerBussiness.AddFields(userFieldInfo);
					}
				}
			}
		}
	}

	public void SendUpdateFarmPoultry()
	{
		TreeTemplateInfo treeTemplateInfo = TreeTemplateMgr.FindNextTreeTemplate(base.CurrentFarm.TreeLevel + 1);
		Player.Out.SendFarmPoultryLevel(Player.PlayerId, base.CurrentFarm, treeTemplateInfo.Level);
	}

	public void WakeFarmPoultry()
	{
		base.CurrentFarm.PoultryState = 2;
		SendUpdateFarmPoultry();
	}

	public void FarmPoultryFight(ref int bossId)
	{
		base.CurrentFarm.PoultryState = 3;
		TreeTemplateInfo treeTemplateInfo = TreeTemplateMgr.FindNextTreeTemplate(base.CurrentFarm.TreeLevel);
		if (treeTemplateInfo != null)
		{
			bossId = treeTemplateInfo.MonsterID;
		}
	}

	public void AddLoveScore(int value)
	{
		if (m_farm == null)
		{
			EnterFarm(isEnter: false);
		}
		if (value > 0)
		{
			m_farm.LoveScore += value;
		}
	}

	public void RemoveLoveScore(int value)
	{
		if (m_farm == null)
		{
			EnterFarm(isEnter: false);
		}
		if (base.CurrentFarm.LoveScore > 0)
		{
			m_farm.LoveScore -= value;
		}
	}

	public void FeedFarmPoultry()
	{
		TreeTemplateInfo treeTemplateInfo = TreeTemplateMgr.FindNextTreeTemplate(base.CurrentFarm.TreeLevel + 1);
		if (treeTemplateInfo != null && base.CurrentFarm.MonsterExp < treeTemplateInfo.MonsterExp)
		{
			base.CurrentFarm.MonsterExp++;
			base.CurrentFarm.CountDownTime = DateTime.Now.AddMilliseconds(3600000.0);
		}
		SendUpdateFarmPoultry();
	}

	public bool CanFight()
	{
		TreeTemplateInfo treeTemplateInfo = TreeTemplateMgr.FindNextTreeTemplate(base.CurrentFarm.TreeLevel + 1);
		if (treeTemplateInfo != null && base.CurrentFarm.MonsterExp >= treeTemplateInfo.MonsterExp)
		{
			return true;
		}
		return false;
	}

	public void FeedFriendFarmPoultry(int userId)
	{
		GamePlayer playerById = WorldMgr.GetPlayerById(userId);
		TreeTemplateInfo treeTemplateInfo = TreeTemplateMgr.FindNextTreeTemplate(m_otherFarm.TreeLevel + 1);
		m_otherFarm.CountDownTime = DateTime.Now.AddMilliseconds(3600000.0);
		if (playerById == null)
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			if (m_otherFarm.MonsterExp < treeTemplateInfo.MonsterExp)
			{
				m_otherFarm.MonsterExp++;
			}
			playerBussiness.UpdateFarm(base.OtherFarm);
		}
		else
		{
			playerById.Farm.FeedFarmPoultry();
		}
		Player.Out.SendFarmPoultryLevel(Player.PlayerId, m_otherFarm, treeTemplateInfo.Level);
	}

	public bool WakeFarmPoultry(int userId)
	{
		GamePlayer playerById = WorldMgr.GetPlayerById(userId);
		TreeTemplateInfo treeTemplateInfo = TreeTemplateMgr.FindNextTreeTemplate(m_otherFarm.TreeLevel + 1);
		if (playerById == null)
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			m_otherFarm.PoultryState = 2;
			playerBussiness.UpdateFarm(base.OtherFarm);
		}
		else
		{
			playerById.Farm.WakeFarmPoultry();
		}
		Player.Out.SendFarmPoultryLevel(Player.PlayerId, m_otherFarm, treeTemplateInfo.Level);
		return true;
	}

	public bool AddFieldTo(UserFieldInfo item, int place, int farmId)
	{
		if (base.AddFieldTo(item, place))
		{
			item.FarmID = farmId;
			return true;
		}
		return false;
	}

	public bool AddOtherFieldTo(UserFieldInfo item, int place, int farmId)
	{
		if (base.AddOtherFieldTo(item, place))
		{
			item.FarmID = farmId;
			return true;
		}
		return false;
	}

	public override void GropFastforward(bool isAllField, int fieldId)
	{
		base.GropFastforward(isAllField, fieldId);
		m_player.Out.SenddoMature(this);
	}

	public void FarmPoultryFightFail()
	{
		if (base.CurrentFarm.PoultryState == 3)
		{
			base.CurrentFarm.PoultryState = 2;
		}
	}

	public void SendFarmPoultryAward(bool isWin)
	{
		if (!isWin)
		{
			return;
		}
		TreeTemplateInfo treeTemplateInfo = null;
		if (base.CurrentFarm == null)
		{
			EnterFarm(isEnter: false);
		}
		if (base.CurrentFarm != null && base.CurrentFarm.PoultryState == 3)
		{
			base.CurrentFarm.PoultryState = 0;
			base.CurrentFarm.MonsterExp = 0;
			treeTemplateInfo = TreeTemplateMgr.FindTreeTemplate(base.CurrentFarm.TreeLevel);
		}
		else
		{
			int spouseID = Player.PlayerCharacter.SpouseID;
			GamePlayer playerById = WorldMgr.GetPlayerById(spouseID);
			UserFarmInfo userFarmInfo = null;
			if (playerById == null)
			{
				using PlayerBussiness playerBussiness = new PlayerBussiness();
				userFarmInfo = playerBussiness.GetSingleFarm(spouseID);
			}
			else
			{
				userFarmInfo = playerById.Farm.CurrentFarm;
			}
			if (playerById != null)
			{
				treeTemplateInfo = TreeTemplateMgr.FindTreeTemplate(userFarmInfo.TreeLevel);
			}
		}
		if (treeTemplateInfo != null)
		{
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(treeTemplateInfo.AwardID);
			if (itemTemplateInfo != null)
			{
				List<ItemInfo> list = new List<ItemInfo>();
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 102);
				itemInfo.Count = 1;
				itemInfo.ValidDate = 0;
				itemInfo.IsBinds = true;
				list.Add(itemInfo);
				string translation = LanguageMgr.GetTranslation("FarmPoultryAward.Msg1", treeTemplateInfo.MonsterName);
				WorldEventMgr.SendItemsToMails(list, Player.PlayerCharacter.ID, Player.PlayerCharacter.NickName, Player.ZoneId, translation);
			}
		}
	}

	public void EnterFarm(bool isEnter)
	{
		FarmPoultryFightFail();
		CropHelperSwitchField(isStopFarmHelper: false);
		if (isEnter)
		{
			m_player.Out.SendEnterFarm(m_player.PlayerCharacter, base.CurrentFarm, GetFields());
			m_farmstatus = 1;
		}
	}

	public void LoadFarmLand()
	{
		if (!m_loadFromDb)
		{
			LoadFromDatabase();
			m_loadFromDb = true;
			m_player.Out.SendFarmLandInfo(this);
		}
	}

	public void CropHelperSwitchField(bool isStopFarmHelper)
	{
		if (m_farm == null || !m_farm.isFarmHelper)
		{
			return;
		}
		ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(m_farm.isAutoId);
		ItemTemplateInfo goods = ItemMgr.FindItemTemplate(itemTemplateInfo.Property4);
		ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, 1, 102);
		int num = 0;
		int num2 = m_farm.AutoValidDate * 60;
		TimeSpan timeSpan = DateTime.Now - m_farm.AutoPayTime;
		int killCropId = m_farm.KillCropId;
		num = ((!(timeSpan.TotalMilliseconds < 0.0)) ? (num2 - (int)timeSpan.TotalMilliseconds) : num2);
		int num3 = (1 - num / num2) * killCropId / 1000;
		if (num3 > killCropId)
		{
			num3 = killCropId;
			isStopFarmHelper = true;
		}
		if (isStopFarmHelper)
		{
			itemInfo.Count = num3;
			if (num3 > 0)
			{
				string translation = LanguageMgr.GetTranslation("PlayerFarm.Msg1", num3, itemInfo.Template.Name);
				string translation2 = LanguageMgr.GetTranslation("PlayerFarm.Msg2");
				m_player.SendItemToMail(itemInfo, translation, translation2, eMailType.ItemOverdue);
				m_player.Out.SendMailResponse(m_player.PlayerCharacter.ID, eMailRespose.Receiver);
			}
			lock (m_lock)
			{
				m_farm.isFarmHelper = false;
				m_farm.isAutoId = 0;
				m_farm.AutoPayTime = DateTime.Now;
				m_farm.AutoValidDate = 0;
				m_farm.GainFieldId = 0;
				m_farm.KillCropId = 0;
			}
			m_player.Out.SendHelperSwitchField(m_player.PlayerCharacter, m_farm);
		}
	}

	public void ExitFarm()
	{
		m_farmstatus = 0;
	}

	public virtual void HelperSwitchField(bool isHelper, int seedID, int seedTime, int haveCount, int getCount)
	{
		if (base.HelperSwitchFields(isHelper, seedID, seedTime, haveCount, getCount))
		{
			m_player.Out.SendHelperSwitchField(m_player.PlayerCharacter, m_farm);
		}
	}

	public virtual bool GainFriendFields(int userId, int fieldId)
	{
		GamePlayer playerById = WorldMgr.GetPlayerById(userId);
		UserFieldInfo userFieldInfo = m_otherFields[fieldId];
		if (userFieldInfo == null)
		{
			return false;
		}
		ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(userFieldInfo.SeedID);
		ItemTemplateInfo goods = ItemMgr.FindItemTemplate(itemTemplateInfo.Property4);
		ItemInfo item = ItemInfo.CreateFromTemplate(goods, 1, 102);
		List<ItemInfo> list = new List<ItemInfo>();
		if (GetOtherFieldAt(fieldId).isDig())
		{
			return false;
		}
		lock (m_lock)
		{
			if (m_otherFields[fieldId].GainCount <= 9)
			{
				return false;
			}
			m_otherFields[fieldId].GainCount--;
		}
		if (!m_player.PropBag.StackItemToAnother(item) && !m_player.PropBag.AddItem(item))
		{
			list.Add(item);
		}
		if (playerById == null)
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			for (int i = 0; i < m_otherFields.Length; i++)
			{
				UserFieldInfo userFieldInfo2 = m_otherFields[i];
				if (userFieldInfo2 != null)
				{
					playerBussiness.UpdateFields(userFieldInfo2);
				}
			}
		}
		else if (playerById.Farm.Status == 1)
		{
			playerById.Farm.UpdateGainCount(fieldId, m_otherFields[fieldId].GainCount);
			playerById.Out.SendtoGather(playerById.PlayerCharacter, m_otherFields[fieldId]);
		}
		m_player.Out.SendtoGather(m_player.PlayerCharacter, m_otherFields[fieldId]);
		if (list.Count > 0)
		{
			string translation = LanguageMgr.GetTranslation("PlayerFarm.Msg3");
			m_player.SendItemsToMail(list, translation, translation, eMailType.ItemOverdue);
			m_player.Out.SendMailResponse(m_player.PlayerCharacter.ID, eMailRespose.Receiver);
		}
		return true;
	}

	public void EnterFriendFarm(int userId)
	{
		GamePlayer playerById = WorldMgr.GetPlayerById(userId);
		UserFarmInfo userFarmInfo;
		UserFieldInfo[] array;
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			if (playerById == null)
			{
				userFarmInfo = playerBussiness.GetSingleFarm(userId);
				array = playerBussiness.GetSingleFields(userId);
			}
			else
			{
				userFarmInfo = playerById.Farm.CurrentFarm;
				array = playerById.Farm.CurrentFields;
				playerById.ViFarmsAdd(m_player.PlayerCharacter.ID);
			}
			if (userFarmInfo == null)
			{
				if (playerById == null)
				{
					userFarmInfo = CreateFarmForNulll(userId);
					array = CreateFieldsForNull(userId);
				}
				else
				{
					userFarmInfo = playerBussiness.GetSingleFarm(userId);
					array = playerBussiness.GetSingleFields(userId);
				}
			}
		}
		m_farmstatus = m_player.PlayerCharacter.ID;
		if (userFarmInfo == null)
		{
			return;
		}
		UpdateOtherFarm(userFarmInfo);
		ClearOtherFields();
		UserFieldInfo[] array2 = array;
		foreach (UserFieldInfo userFieldInfo in array2)
		{
			if (userFieldInfo != null)
			{
				AddOtherFieldTo(userFieldInfo, userFieldInfo.FieldID, userFarmInfo.FarmID);
			}
		}
		m_midAutumnFlag = false;
		m_player.Out.SendEnterFarm(m_player.PlayerCharacter, base.OtherFarm, GetOtherFields());
	}

	public virtual void PayField(List<int> fieldIds, int payFieldTime)
	{
		if (!base.CreateField(m_player.PlayerCharacter.ID, fieldIds, payFieldTime))
		{
			return;
		}
		foreach (int viFarm in m_player.ViFarms)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(viFarm);
			if (playerById != null && playerById.Farm.Status == viFarm)
			{
				playerById.Out.SendPayFields(m_player, fieldIds);
			}
		}
		m_player.Out.SendPayFields(m_player, fieldIds);
	}

	public override bool GrowField(int fieldId, int templateID)
	{
		if (base.GrowField(fieldId, templateID))
		{
			foreach (int viFarm in m_player.ViFarms)
			{
				GamePlayer playerById = WorldMgr.GetPlayerById(viFarm);
				if (playerById != null && playerById.Farm.Status == viFarm)
				{
					playerById.Out.SendSeeding(m_player.PlayerCharacter, m_fields[fieldId]);
				}
			}
			m_player.Out.SendSeeding(m_player.PlayerCharacter, m_fields[fieldId]);
			return true;
		}
		return false;
	}

	public override bool killCropField(int fieldId)
	{
		if (base.killCropField(fieldId))
		{
			m_player.Out.SendKillCropField(m_player.PlayerCharacter, m_fields[fieldId]);
			return true;
		}
		return false;
	}

	public virtual bool GainField(int fieldId)
	{
		if (fieldId < 0 || fieldId > GetFields().Count())
		{
			return false;
		}
		if (!base.CurrentFields[fieldId].isDig())
		{
			return false;
		}
		ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(base.CurrentFields[fieldId].SeedID);
		if (itemTemplateInfo == null)
		{
			return false;
		}
		List<ItemInfo> list = new List<ItemInfo>();
		ItemTemplateInfo goods = ItemMgr.FindItemTemplate(itemTemplateInfo.Property4);
		ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, base.CurrentFields[fieldId].GainCount, 102);
		if (base.killCropField(fieldId))
		{
			if (!m_player.AddTemplate(itemInfo))
			{
				list.Add(itemInfo);
			}
			m_player.Out.SendtoGather(m_player.PlayerCharacter, base.CurrentFields[fieldId]);
			foreach (int viFarm in m_player.ViFarms)
			{
				GamePlayer playerById = WorldMgr.GetPlayerById(viFarm);
				if (playerById != null && playerById.Farm.Status == viFarm)
				{
					playerById.Out.SendtoGather(m_player.PlayerCharacter, base.CurrentFields[fieldId]);
				}
			}
			m_player.OnCropPrimaryEvent();
			if (list.Count > 0)
			{
				string translation = LanguageMgr.GetTranslation("PlayerFarm.Msg3");
				m_player.SendItemsToMail(list, translation, translation, eMailType.ItemOverdue);
				m_player.Out.SendMailResponse(m_player.PlayerCharacter.ID, eMailRespose.Receiver);
			}
			return true;
		}
		return false;
	}
}
