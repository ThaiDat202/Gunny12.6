using System;
using System.Collections.Generic;
using System.Linq;
using Bussiness;
using Bussiness.Managers;
using EntityDatabase.PlayerModels;
using EntityDatabase.ServerModels;
using Game.Logic;
using Game.Server.CardNewSystem;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils;

public class PlayerEquipInventory : PlayerInventory
{
	private const int END_DRESS_BAG = 380;

	private const int END_EQUIP_BAG = 80;

	private const int START_DRESS_BAG = 81;

	private const int START_EQUIP_BAG = 31;

	public PlayerEquipInventory(GamePlayer player)
		: base(player, saveTodb: true, 1176, 0, 31, autoStack: true)
	{
	}

	public override void LoadFromDatabase()
	{
		BeginChanges();
		List<ItemInfo> list = new List<ItemInfo>();
		try
		{
			base.LoadFromDatabase();
			for (int i = 0; i < 31; i++)
			{
				ItemInfo itemInfo = m_items[i];
				if (itemInfo == null)
				{
					continue;
				}
				if (itemInfo.Template == null)
				{
					TakeOutItem(itemInfo);
					AbstractInventory.log.ErrorFormat("Item not found in DB UserID: {0}, UserName: {1}, ItemID {2}, TemplateID: {3}, item beeb remove!", m_player.PlayerCharacter.ID, m_player.PlayerCharacter.UserName, itemInfo.ItemID, itemInfo.TemplateID);
				}
				else if (itemInfo.IsEquipPet() && itemInfo.Place < 31)
				{
					ItemInfo item = itemInfo.Clone();
					list.Add(item);
					TakeOutItem(itemInfo);
				}
				else if (!CanEquipSlotContains(itemInfo.Place, itemInfo.Template))
				{
					ItemInfo item2 = itemInfo.Clone();
					list.Add(item2);
					TakeOutItem(itemInfo);
				}
				else if (!itemInfo.IsValidItem())
				{
					int num = -1;
					num = (Equip.isAvatar(itemInfo.Template) ? FindFirstEmptySlot(81, 1176) : FindFirstEmptySlot(31, 80));
					if (num >= 0)
					{
						MoveItem(itemInfo.Place, num, itemInfo.Count);
						continue;
					}
					list.Add(itemInfo);
					TakeOutItem(itemInfo);
				}
			}
		}
		finally
		{
			CommitChanges();
			if (list.Count > 0)
			{
				m_player.SendItemsToMail(list, LanguageMgr.GetTranslation("ItemOverdueHandler.Content"), LanguageMgr.GetTranslation("ItemOverdueHandler.Title"), eMailType.ItemOverdue);
				m_player.Out.SendMailResponse(m_player.PlayerCharacter.ID, eMailRespose.Receiver);
			}
		}
	}

	public override bool AddItem(ItemInfo item)
	{
		if (item.Template.BagType != 0)
		{
			return base.AddItem(item);
		}
		if (Equip.isAvatar(item.Template))
		{
			return base.AddItem(item, 81);
		}
		return base.AddItem(item, 31, 80);
	}

	public override bool AddItemTo(ItemInfo item, int place)
	{
		if (Equip.isAvatar(item.Template) && !IsEquipSlot(place))
		{
			place = FindFirstEmptySlot(81);
		}
		return base.AddItemTo(item, place);
	}

	public override bool AddTemplate(ItemInfo cloneItem, int count)
	{
		if (cloneItem.Template.BagType != 0)
		{
			return base.AddTemplate(cloneItem, count);
		}
		if (Equip.isAvatar(cloneItem.Template))
		{
			return base.AddTemplate(cloneItem, count, 81, 1176);
		}
		return base.AddTemplate(cloneItem, count, 31, 80);
	}

	public override bool MoveItem(int fromSlot, int toSlot, int count)
	{
		if (m_items[fromSlot] == null)
		{
			return false;
		}
		if (IsEquipSlot(fromSlot) && !IsEquipSlot(toSlot) && m_items[toSlot] != null && m_items[toSlot].Template.CategoryID != m_items[fromSlot].Template.CategoryID)
		{
			if (!CanEquipSlotContains(fromSlot, m_items[toSlot].Template))
			{
				toSlot = FindFirstEmptySlot(base.BeginSlot);
			}
		}
		else
		{
			if (IsEquipSlot(toSlot))
			{
				if (!CanEquipSlotContains(toSlot, m_items[fromSlot].Template))
				{
					UpdateItem(m_items[fromSlot]);
					return false;
				}
				if (!m_player.CanEquip(m_items[fromSlot].Template) || !m_items[fromSlot].IsValidItem())
				{
					UpdateItem(m_items[fromSlot]);
					return false;
				}
			}
			if (IsEquipSlot(fromSlot) && m_items[toSlot] != null && !CanEquipSlotContains(fromSlot, m_items[toSlot].Template))
			{
				UpdateItem(m_items[toSlot]);
				return false;
			}
		}
		return base.MoveItem(fromSlot, toSlot, count);
	}

	public override void UpdateChangedPlaces()
	{
		int[] array = m_changedPlaces.ToArray();
		bool flag = false;
		int[] array2 = array;
		foreach (int slot in array2)
		{
			if (!IsEquipSlot(slot))
			{
				continue;
			}
			ItemInfo itemAt = GetItemAt(slot);
			if (itemAt != null)
			{
				m_player.OnUsingItem(itemAt.TemplateID, 1);
				itemAt.IsBinds = true;
				if (!itemAt.IsUsed)
				{
					itemAt.IsUsed = true;
					itemAt.BeginDate = DateTime.Now;
				}
			}
			flag = true;
			break;
		}
		base.UpdateChangedPlaces();
		if (flag)
		{
			UpdatePlayerProperties();
		}
	}

	public void UpdatePlayerProperties()
	{
		if (m_player.PlayerState == ePlayerState.Offline)
		{
			return;
		}
		m_player.BeginChanges();
		try
		{
			string text = "";
			string text2 = "";
			string skin = "";
			int attack = 0;
			int def = 0;
			int agi = 0;
			int luck = 0;
			int hp = m_player.HorseAmuletBag.GetHorseAmuletHp();
			int num = 0;
			int mgDef = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int attack2 = 0;
			int defence = 0;
			int agility = 0;
			int lucky = 0;
			int hp2 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			int mgattack = 0;
			int mgdef = 0;
			int hp3 = 0;
			int num9 = 0;
			int num10 = 0;
			int num11 = 0;
			int num12 = 0;
			int num13 = 0;
			int num14 = 0;
			int num15 = 0;
			int num16 = 0;
			int num17 = 0;
			int num18 = 0;
			int num19 = 0;
			int num20 = 0;
			int attack3 = 0;
			int defence2 = 0;
			int agility2 = 0;
			int lucky2 = 0;
			int hp4 = 0;
			int att = 0;
			int def2 = 0;
			int agi2 = 0;
			int luk = 0;
			int hp5 = 0;
			int num21 = 0;
			int num22 = 0;
			int num23 = 0;
			int num24 = 0;
			int num25 = 0;
			int num26 = 0;
			int num27 = 0;
			int num28 = 0;
			int num29 = 0;
			int num30 = 0;
			int num31 = 0;
			int num32 = 0;
			int num33 = 0;
			int num34 = 0;
			int att2 = 0;
			int def3 = 0;
			int luck2 = 0;
			int blood = 0;
			int mgDef2 = 0;
			int blood2 = 0;
			int mgDef3 = 0;
			int num35 = 0;
			int num36 = 0;
			int num37 = 0;
			int num38 = 0;
			int num39 = 0;
			int num40 = 0;
			int num41 = 0;
			int num42 = 0;
			int num43 = 0;
			int num44 = 0;
			int num45 = 0;
			int num46 = 0;
			int num47 = 0;
			int num48 = 0;
			int num49 = 0;
			int num50 = 0;
			int num51 = 0;
			int num52 = 0;
			int num53 = 0;
			int basedame = 0;
			int basedef = 0;
			int attack4 = 0;
			int def4 = 0;
			int agi3 = 0;
			int luck3 = 0;
			int num54 = 0;
			int num55 = 0;
			int num56 = 0;
			int num57 = 0;
			int num58 = 0;
			int currentVal = 0;
			int currentVal2 = 0;
			int currentVal3 = 0;
			int currentVal4 = 0;
			int currentVal5 = 0;
			int currentVal6 = 0;
			m_player.MarkProperties = m_player.Marks.GetEquipProperties();
			m_player.UpdatePet(m_player.PetBag.GetPetIsEquip());
			List<UsersCardInfo> cards = m_player.CardBag.GetCards(0, 5);
			List<ItemInfo> list = new List<ItemInfo>();
			lock (m_lock)
			{
				text = ((m_items[0] == null) ? "" : (m_items[0].TemplateID + "|" + m_items[0].Pic));
				text2 = ((m_items[0] == null) ? "" : m_items[0].Color);
				skin = ((m_items[5] == null) ? "" : m_items[5].Skin);
				for (int j = 0; j < 31; j++)
				{
					ItemInfo itemInfo = m_items[j];
					if (itemInfo != null && !itemInfo.IsEquipPet())
					{
						attack += itemInfo.Attack;
						def += itemInfo.Defence;
						agi += itemInfo.Agility;
						luck += itemInfo.Luck;
						num2 = ((num2 > itemInfo.StrengthenLevel) ? num2 : itemInfo.StrengthenLevel);
						AddBaseLatentProperty(itemInfo, ref attack, ref def, ref agi, ref luck);
						AddBaseGemstoneProperty(itemInfo, ref attack2, ref defence, ref agility, ref lucky, ref hp2);
						SubActiveConditionInfo subActiveInfo = SubActiveMgr.GetSubActiveInfo(itemInfo);
						if (subActiveInfo != null)
						{
							attack += subActiveInfo.GetValue("1");
							def += subActiveInfo.GetValue("2");
							agi += subActiveInfo.GetValue("3");
							luck += subActiveInfo.GetValue("4");
							hp += subActiveInfo.GetValue("5");
						}
						ItemInfo itemAt = m_player.MagicStoneBag.GetItemAt(j);
						if (itemAt != null)
						{
							num21 += itemAt.AttackCompose;
							num22 += itemAt.DefendCompose;
							num23 += itemAt.AgilityCompose;
							num24 += itemAt.LuckCompose;
							num25 += itemAt.MagicAttack;
							num26 += itemAt.MagicDefence;
						}
						if (itemInfo.IsEnchant())
						{
							MagicItemTemplateInfo magicItemTemplateInfo = MagicItemTemplateMgr.FindMagicItemTemplate(itemInfo.MagicLevel);
							if (magicItemTemplateInfo != null)
							{
								num33 += magicItemTemplateInfo.MagicDefence;
								num34 += magicItemTemplateInfo.MagicAttack;
								if (itemInfo.enchantSpiritLevel > 0)
								{
									TS_UpgradeTemplate upgradeInjetarFusao = ItemMgr.getUpgradeInjetarFusao(itemInfo.enchantSpiritLevel);
									num33 += magicItemTemplateInfo.MagicDefence * upgradeInjetarFusao.Data / 1000;
									num34 += magicItemTemplateInfo.MagicAttack * upgradeInjetarFusao.Data / 1000;
								}
							}
						}
						int num59 = j;
						int num60 = num59;
						if (num60 == 18)
						{
							AmuletItemInfo amuletItemInfo = AmuletItemMgr.FindAmuletItem(itemInfo.StrengthenLevel);
							if (amuletItemInfo != null)
							{
								hp += amuletItemInfo.HP;
							}
						}
						if (Equip.isWeddingRing(itemInfo.Template))
						{
							LoveLevelInfo level = LoveLevelMgr.GetLevel(m_player.PlayerCharacter.RingExp);
							if (level != null)
							{
								if (m_player.LovelSKillID < level.SkillID)
								{
									m_player.LovelSKillID = level.SkillID;
								}
								attack += itemInfo.Template.Attack * level.Attack / 100;
								def += itemInfo.Template.Defence * level.Defence / 100;
								agi += itemInfo.Template.Agility * level.Agility / 100;
								luck += itemInfo.Template.Luck * level.Luck / 100;
							}
						}
						if (Equip.isAvatar(itemInfo.Template))
						{
							list.Add(itemInfo);
						}
						UserEquipGhostInfo ghostEquip = m_player.GetGhostEquip(itemInfo.BagType, itemInfo.Place);
						if (ghostEquip != null)
						{
							SpiritInfo singleSpirit = SpiritInfoMgr.GetSingleSpirit(ghostEquip.BagType, ghostEquip.Place, ghostEquip.Level);
							if (singleSpirit != null)
							{
								attack += itemInfo.Template.Attack * singleSpirit.AttackAdd / 100;
								agi += itemInfo.Template.Agility * singleSpirit.AgilityAdd / 100;
								def += itemInfo.Template.Defence * singleSpirit.DefendAdd / 100;
								luck += itemInfo.Template.Luck * singleSpirit.LuckAdd / 100;
							}
						}
					}
					AddBeadProperty(j, ref attack3, ref defence2, ref agility2, ref lucky2, ref hp4);
				}
				m_player.AvatarBag.SaveDressModel(0, list);
				AddBaseTotemProperty(m_player.PlayerCharacter.totemId, ref attack, ref def, ref agi, ref luck, ref hp);
				foreach (UserRankInfo rank in m_player.Rank.GetRanks())
				{
					num54 += rank.Attack;
					num55 += rank.Defence;
					num56 += rank.Agility;
					num57 += rank.Luck;
				}
				if (m_player.Pet != null)
				{
					num9 += m_player.Pet.TotalAttack;
					num10 += m_player.Pet.TotalDefence;
					num11 += m_player.Pet.TotalAgility;
					num12 += m_player.Pet.TotalLuck;
					num13 += m_player.Pet.TotalBlood;
					PetFightPropertyInfo petFightPropertyInfo = PetMgr.FindFightProperty(m_player.PlayerCharacter.evolutionGrade);
					if (petFightPropertyInfo != null)
					{
						num9 += petFightPropertyInfo.Attack;
						num10 += petFightPropertyInfo.Defence;
						num11 += petFightPropertyInfo.Agility;
						num12 += petFightPropertyInfo.Lucky;
						num13 += petFightPropertyInfo.Blood;
					}
					EatPetsInfo eatPets = m_player.PetBag.EatPets;
					foreach (PetEquipInfo petEquip in m_player.Pet.PetEquips)
					{
						if (!petEquip.IsValidItem() || petEquip.Template == null)
						{
							continue;
						}
						num9 += petEquip.Template.Attack + petEquip.AttackCompose;
						num10 += petEquip.Template.Defence + petEquip.DefendCompose;
						num11 += petEquip.Template.Agility + petEquip.AgilityCompose;
						num12 += petEquip.Template.Luck + petEquip.LuckCompose;
						if (eatPets == null)
						{
							continue;
						}
						if (petEquip.eqType == 0)
						{
							PetMoePropertyInfo petMoePropertyInfo = PetMoePropertyMgr.FindPetMoeProperty(eatPets.weaponLevel);
							if (petMoePropertyInfo != null)
							{
								num9 += petMoePropertyInfo.Attack;
								num12 += petMoePropertyInfo.Lucky;
							}
						}
						else if (petEquip.eqType == 1)
						{
							PetMoePropertyInfo petMoePropertyInfo2 = PetMoePropertyMgr.FindPetMoeProperty(eatPets.hatLevel);
							if (petMoePropertyInfo2 != null)
							{
								num10 += petMoePropertyInfo2.Defence;
							}
						}
						else if (petEquip.eqType == 2)
						{
							PetMoePropertyInfo petMoePropertyInfo3 = PetMoePropertyMgr.FindPetMoeProperty(eatPets.clothesLevel);
							if (petMoePropertyInfo3 != null)
							{
								num11 += petMoePropertyInfo3.Agility;
								num13 += petMoePropertyInfo3.Blood;
							}
						}
					}
				}
				foreach (UsersPetLacrarInfo item in m_player.PetBag.lacresPet)
				{
					if (!item.isSeal)
					{
						continue;
					}
					foreach (StoreFineSealLockInfo item2 in item.lockArr)
					{
						switch (item2.proType)
						{
						case 31:
							num47 += item2.proNum;
							break;
						case 32:
							num48 += item2.proNum;
							break;
						case 33:
							num49 += item2.proNum;
							break;
						case 34:
							num50 += item2.proNum;
							break;
						case 37:
							num51 += item2.proNum;
							break;
						case 39:
							base.Player.PlayerCharacter.CurrentSealSkill = item2.skillId;
							break;
						case 101:
							num52 += item2.proNum;
							break;
						case 102:
							num53 += item2.proNum;
							break;
						}
					}
				}
				if (GameProperties.VERSION < 11000)
				{
					foreach (UsersCardInfo item3 in cards)
					{
						num5 += CardMgr.GetProp(item3, 0);
						num6 += CardMgr.GetProp(item3, 1);
						num7 += CardMgr.GetProp(item3, 2);
						num8 += CardMgr.GetProp(item3, 3);
						if (item3.CardID > 0)
						{
							num5 += item3.Attack;
							num6 += item3.Defence;
							num7 += item3.Agility;
							num8 += item3.Luck;
						}
						if (item3.TemplateID > 0)
						{
							CardTemplateInfo cardTemplateInfo = CardMgr.FindCardTemplate(item3.TemplateID, item3.CardType);
							if (cardTemplateInfo != null)
							{
								num5 += cardTemplateInfo.AddAttack;
								num6 += cardTemplateInfo.AddDefend;
								num7 += cardTemplateInfo.AddAgility;
								num8 += cardTemplateInfo.AddLucky;
							}
						}
					}
					num5 += m_player.CardAchievementInventory.Property[0];
					num6 += m_player.CardAchievementInventory.Property[1];
					hp3 += m_player.CardAchievementInventory.Property[4];
					num8 += m_player.CardAchievementInventory.Property[2];
					mgattack += m_player.CardAchievementInventory.Property[6];
					mgdef += m_player.CardAchievementInventory.Property[7];
				}
				else
				{
					TS_CardMain tS_CardMain = CardNewSystemMgr.CardMains.FirstOrDefault((TS_CardMain p) => p.Level == base.Player.PlayerCharacter.CardInfo.CardLevel);
					if (tS_CardMain != null)
					{
						num5 += tS_CardMain.Attack + m_player.PlayerCharacter.CardInfo.Attack;
						num6 += tS_CardMain.Defence + m_player.PlayerCharacter.CardInfo.Defence;
						num7 += tS_CardMain.Agility + m_player.PlayerCharacter.CardInfo.Agility;
						num8 += tS_CardMain.Lucky + m_player.PlayerCharacter.CardInfo.Lucky;
						CardNewSystemMgr.GetSuitValue(m_player, ref hp3, ref attack4, ref def4, ref agi3, ref luck3, ref mgattack, ref mgdef, ref basedame, ref basedef);
						num5 += attack4;
						num6 += def4;
						num7 += agi3;
						num8 += luck3;
					}
				}
				num14 += ExerciseMgr.GetExercise(m_player.PlayerCharacter.Texp.attTexpExp).ExerciseA;
				num15 += ExerciseMgr.GetExercise(m_player.PlayerCharacter.Texp.defTexpExp).ExerciseD;
				num16 += ExerciseMgr.GetExercise(m_player.PlayerCharacter.Texp.spdTexpExp).ExerciseAG;
				num17 += ExerciseMgr.GetExercise(m_player.PlayerCharacter.Texp.lukTexpExp).ExerciseL;
				num18 += ExerciseMgr.GetExercise(m_player.PlayerCharacter.Texp.hpTexpExp).ExerciseH;
				num19 += ExerciseMgr.GetExercise(m_player.PlayerCharacter.Texp.magicAtkTexpExp).ExerciseMA;
				num20 += ExerciseMgr.GetExercise(m_player.PlayerCharacter.Texp.magicDefTexpExp).ExerciseMD;
				EquipBeadEffect();
				EquipHorseAmuletEffect();
				int[] equipPlace = m_player.EquipPlace;
				for (int k = 0; k < equipPlace.Length; k++)
				{
					text += ",";
					text2 += ",";
					if (m_items[equipPlace[k]] != null)
					{
						text = text + m_items[equipPlace[k]].TemplateID + "|" + m_items[equipPlace[k]].Pic;
						text2 += m_items[equipPlace[k]].Color;
					}
				}
				int num61 = 0;
				int num62 = 0;
				int num63 = 0;
				int num64 = 0;
				int num65 = 0;
				int num66 = 0;
				int num67 = 0;
				int num68 = 0;
				int num69 = 0;
				int num70 = 0;
				int num71 = 0;
				JampsUpgradeItemList[] condictionsByLevel = JampsManualMgr.getCondictionsByLevel(m_player.PlayerCharacter.explorerManualInfo.manualLevel);
				int num72 = 0;
				int num73 = 0;
				int num74 = 0;
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
				Dictionary<int, int> dictionary3 = new Dictionary<int, int>();
				Dictionary<int, int> dictionary4 = new Dictionary<int, int>();
				foreach (JampsManualItemList value in JampsManualMgr._jampsManualItemList.Values)
				{
					if (value.Level <= m_player.PlayerCharacter.explorerManualInfo.manualLevel)
					{
						num71 += value.Boost / 10;
						num66 += value.MagicAttack;
						num67 += value.MagicResistance;
					}
				}
				foreach (PagesInfo i in m_player.PlayerCharacter.explorerManualInfo.activesPage)
				{
					JampsPageItemList pageFromID = JampsManualMgr.getPageFromID(i.pageID);
					if (pageFromID.DebrisCount != m_player.PlayerCharacter.explorerManualInfo.debris.Values.Where((DebrisInfo o) => o.pageID == i.pageID).Count())
					{
						continue;
					}
					num61 += pageFromID.Collect_Attack * (1 + num71);
					num62 += pageFromID.Collect_Defense * (1 + num71);
					num63 += pageFromID.Collect_Agile * (1 + num71);
					num64 += pageFromID.Collect_Lucky * (1 + num71);
					num65 += pageFromID.Collect_Armor * (1 + num71);
					num66 += pageFromID.Collect_MagicAttack * (1 + num71);
					num67 += pageFromID.Collect_MagicResistance * (1 + num71);
					num68 += pageFromID.Collect_HP * (1 + num71);
					num69 += pageFromID.Collect_Stamina * (1 + num71);
					num70 += pageFromID.Collect_Damage * (1 + num71);
					num72++;
					if (dictionary.ContainsKey(pageFromID.ID))
					{
						dictionary[pageFromID.ID]++;
					}
					else
					{
						dictionary.Add(pageFromID.ID, 1);
					}
					if (dictionary2.ContainsKey(pageFromID.ChapterID))
					{
						dictionary2[pageFromID.ChapterID]++;
					}
					else
					{
						dictionary2.Add(pageFromID.ChapterID, 1);
					}
					if (i.activate)
					{
						num73++;
						if (dictionary3.ContainsKey(pageFromID.ID))
						{
							dictionary3[pageFromID.ID]++;
						}
						else
						{
							dictionary3.Add(pageFromID.ID, 1);
						}
						if (dictionary4.ContainsKey(pageFromID.ChapterID))
						{
							dictionary4[pageFromID.ChapterID]++;
						}
						else
						{
							dictionary4.Add(pageFromID.ChapterID, 1);
						}
						num61 += pageFromID.Activate_Attack * (1 + num71);
						num62 += pageFromID.Activate_Defence * (1 + num71);
						num63 += pageFromID.Activate_Agile * (1 + num71);
						num64 += pageFromID.Activate_Lucky * (1 + num71);
						num65 += pageFromID.Activate_Armor * (1 + num71);
						num66 += pageFromID.Activate_MagicAttack * (1 + num71);
						num67 += pageFromID.Activate_MagicResistance * (1 + num71);
						num68 += pageFromID.Activate_HP * (1 + num71);
						num69 += pageFromID.Activate_Stamina * (1 + num71);
						num70 += pageFromID.Activate_Damage * (1 + num71);
					}
				}
				JampsUpgradeItemList[] array = condictionsByLevel;
				foreach (JampsUpgradeItemList jampsUpgradeItemList in array)
				{
					switch (jampsUpgradeItemList.ConditionType)
					{
					case 2:
						num74 += num72;
						break;
					case 3:
						num74 = (dictionary2.ContainsKey(jampsUpgradeItemList.Parameter1) ? (num74 + dictionary2[jampsUpgradeItemList.Parameter1]) : 0);
						break;
					case 4:
						if (dictionary.ContainsKey(jampsUpgradeItemList.Parameter2))
						{
							num74 += dictionary[jampsUpgradeItemList.Parameter2];
						}
						if (dictionary.ContainsKey(jampsUpgradeItemList.Parameter3))
						{
							num74 += dictionary[jampsUpgradeItemList.Parameter3];
						}
						break;
					case 5:
						num74 = (dictionary4.ContainsKey(jampsUpgradeItemList.Parameter1) ? (num74 + dictionary4[jampsUpgradeItemList.Parameter1]) : 0);
						break;
					case 6:
					{
						int num75 = 0;
						if (dictionary3.ContainsKey(jampsUpgradeItemList.Parameter2))
						{
							num75 = dictionary3[jampsUpgradeItemList.Parameter2];
						}
						if (dictionary3.ContainsKey(jampsUpgradeItemList.Parameter1))
						{
							num75 += dictionary3[jampsUpgradeItemList.Parameter1];
						}
						num74 += num75;
						break;
					}
					case 7:
						num74 += dictionary3.Count;
						break;
					}
				}
				m_player.PlayerCharacter.explorerManualInfo.Attack = num61;
				m_player.PlayerCharacter.explorerManualInfo.Agility = num63;
				m_player.PlayerCharacter.explorerManualInfo.Armor = num65;
				m_player.PlayerCharacter.explorerManualInfo.Damage = num70;
				m_player.PlayerCharacter.explorerManualInfo.Defense = num62;
				m_player.PlayerCharacter.explorerManualInfo.Lucky = num64;
				m_player.PlayerCharacter.explorerManualInfo.MagicAttack = num66;
				m_player.PlayerCharacter.explorerManualInfo.MagicResistance = num67;
				m_player.PlayerCharacter.explorerManualInfo.HP = num68;
				m_player.PlayerCharacter.explorerManualInfo.Stamina = num69;
				m_player.PlayerCharacter.explorerManualInfo.conditionCount = num74;
				m_player.PlayerCharacter.AgiAddPlus += num69;
			}
			m_player.AvatarBag.AddPropAvatarColection(ref att, ref def2, ref agi2, ref luk, ref hp5);
			if (m_player.Horse.Info.curUseHorse > 0)
			{
				int curLevel = m_player.Horse.Info.curLevel;
				MountTemplateInfo mountTemplate = MountMgr.GetMountTemplate(curLevel);
				num27 += mountTemplate.MagicAttack;
				num28 += mountTemplate.MagicDefence;
				num29 = mountTemplate.AddBlood;
			}
			MountDrawDataInfo[] getPicCherishs = m_player.Horse.GetPicCherishs;
			MountDrawDataInfo[] array2 = getPicCherishs;
			foreach (MountDrawDataInfo mountDrawDataInfo in array2)
			{
				MountDrawTemplateInfo mountDrawTemplateInfo = MountMgr.FindMountDrawInfo(mountDrawDataInfo.ID);
				if (mountDrawTemplateInfo != null)
				{
					int[] array3 = MountMgr.calculateRate(mountDrawDataInfo);
					num32 += array3[2];
					num27 += array3[3];
					num28 += array3[4];
				}
			}
			SetsBuildTempMgr.GetSetsBuildProp(m_player.PlayerCharacter.fineSuitExp, ref def, ref hp, ref luck, ref agi, ref mgDef);
			if (m_player.Actives.Info != null)
			{
				HomeTempPracticeMgr.BuildProp(m_player.Actives.Info.CurrentLevel, ref att2, ref def3, ref blood, ref luck2, ref mgDef2);
				HomeTempPracticeMgr.BuildProp(m_player.Actives.Info.promoteLevel, ref blood2, ref mgDef3);
			}
			TS_NecklaceCasting tS_NecklaceCasting = NecklaceStrengthMgr.ts_NecklaceCasting.FirstOrDefault((TS_NecklaceCasting p) => p.Level == base.Player.PlayerCharacter.necklaceCastLevel);
			num35 = tS_NecklaceCasting.HP;
			Sys_Users_Elf sys_Users_Elf = m_player.PlayerCharacter.ElfInfo.FirstOrDefault((Sys_Users_Elf e) => e.IsFight);
			if (sys_Users_Elf != null)
			{
				num36 = sys_Users_Elf.HpValue;
			}
			ServerData serverData = new ServerData();
			num58 = serverData.TS_HonorSystem_Template.FirstOrDefault((TS_HonorSystem_Template e) => e.Level == Player.PlayerCharacter.HonorInfo.HonorLevel).Blood;
			attack += attack2 + num5 + num9 + num14 + attack3 + att + num21 + att2 + m_player.PlayerCharacter.explorerManualInfo.Attack + m_player.MarkProperties.Attack + num47 + num41 + num54;
			attack += m_player.KingGrade.PlusProperty(eKingGrade.Attack, attack, ref currentVal);
			def += defence + num6 + num10 + num15 + defence2 + def2 + num22 + def3 + m_player.PlayerCharacter.explorerManualInfo.Defense + m_player.MarkProperties.Defence + num48 + num42 + num55;
			def += m_player.KingGrade.PlusProperty(eKingGrade.Defence, def, ref currentVal2);
			agi += agility + num7 + num11 + num16 + agility2 + agi2 + num23 + num39 + m_player.PlayerCharacter.explorerManualInfo.Agility + m_player.MarkProperties.Agility + num49 + num43 + num56;
			agi += m_player.KingGrade.PlusProperty(eKingGrade.Agility, agi, ref currentVal3);
			luck += lucky + num8 + num12 + num17 + lucky2 + luk + num24 + luck2 + num40 + m_player.PlayerCharacter.explorerManualInfo.Lucky + m_player.MarkProperties.Luck + num50 + num44 + num57;
			luck += m_player.KingGrade.PlusProperty(eKingGrade.Lucky, luck, ref currentVal4);
			hp += hp2 + num13 + num18 + hp4 + hp5 + num29 + num32 + blood + hp3 + m_player.PlayerCharacter.explorerManualInfo.HP + m_player.MarkProperties.Blood + num51 + blood2 + num35 + num36 + num58;
			num += num25 + num27 + num30 + num34 + num19 + mgattack + num37 + m_player.PlayerCharacter.explorerManualInfo.MagicAttack + m_player.MarkProperties.MagicAttack + num52 + num3 + num45;
			num += m_player.KingGrade.PlusProperty(eKingGrade.MagicAttack, num, ref currentVal5);
			mgDef += num26 + num28 + num31 + num33 + num20 + mgDef2 + mgdef + mgDef + num38 + m_player.PlayerCharacter.explorerManualInfo.MagicResistance + m_player.MarkProperties.MagicDefence + num53 + num4 + num46 + mgDef3;
			mgDef += m_player.KingGrade.PlusProperty(eKingGrade.MagicDefence, mgDef, ref currentVal6);
			int enhanceAtt = 0;
			int enhanceDef = 0;
			int critBonus = 0;
			m_player.MagicHouse.UpdateEnhanceProperties(num, mgDef, ref enhanceAtt, ref enhanceDef, ref attack, ref def, ref agi, ref luck, ref critBonus);
			num += enhanceAtt;
			mgDef += enhanceDef;
			int enhanceHp = 0;
			int reduceDamagePow = 0;
			m_player.PetBag.UpdatePetFormProp(hp, ref enhanceHp, ref reduceDamagePow);
			enhanceHp = (int)(((double)(enhanceHp + m_player.LevelPlusBlood + def / 10) + m_player.GetGoldBlood()) * m_player.GetBaseBlood());
			m_player.UpdateBaseProperties(attack, def, agi, luck, enhanceHp, num, mgDef);
			m_player.UpdateWeapon(m_items[6]);
			m_player.UpdateSecondWeapon(m_items[15]);
			m_player.UpdateReduceDame(m_items[17]);
			m_player.PlayerCharacter.badgePvpid = m_items[17]?.TemplateID ?? 0;
			m_player.UpdateHealstone(m_items[18]);
			m_player.UpdateAmulet(m_items[18]);
			m_player.UpdateWeapon3D(m_items[20]);
			m_player.UpdateStyle(text, text2, skin);
			m_player.UpdateBatleConfig("MagicHouse", critBonus);
			m_player.UpdateBatleConfig("PetFormReduceDamage", reduceDamagePow);
			m_player.PlayerProp.CreateProp(isSelf: true, "Texp", num14, num15, num16, num17, num18);
			m_player.PlayerProp.CreateProp(isSelf: true, "Card", num5, num6, num7, num8, hp3);
			m_player.PlayerProp.CreateProp(isSelf: true, "Pet", num9, num10, num11, num12, num13);
			m_player.PlayerProp.CreateProp(isSelf: true, "Gem", attack2, defence, agility, lucky, hp2);
			m_player.PlayerProp.CreateProp(isSelf: true, "Bead", attack3, defence2, agility2, lucky2, hp4);
			m_player.PlayerProp.CreateProp(isSelf: true, "Avatar", att, def2, agi2, luk, hp5);
			m_player.PlayerProp.CreateProp(isSelf: true, "MagicStone", num21, num22, num23, num24, 0);
			m_player.PlayerProp.CreateProp(isSelf: true, "Horse", 0, 0, 0, 0, num29);
			m_player.PlayerProp.CreateProp(isSelf: true, "HorsePicCherish", 0, 0, 0, 0, num32);
			m_player.PlayerProp.CreateProp(isSelf: true, "Temple", att2, def3, 0, luck2, blood + blood2);
			m_player.PlayerProp.CreateProp(isSelf: true, "Mark", m_player.MarkProperties.Attack, m_player.MarkProperties.Defence, m_player.MarkProperties.Agility, m_player.MarkProperties.Luck, m_player.MarkProperties.Blood);
			m_player.PlayerProp.CreateProp(isSelf: true, "SeloPet", num47, num48, num49, num50, num51);
			m_player.PlayerProp.CreateProp(isSelf: true, "titleAdd", num54, num55, num56, num57, 0);
			m_player.PlayerProp.CreateProp(isSelf: true, "honor", 0, 0, 0, 0, num58);
			m_player.PlayerProp.CreateProp(isSelf: true, "marKing", currentVal, currentVal2, currentVal3, currentVal4, 0);
			m_player.PlayerProp.UpadateBaseProp(isSelf: true, "MagicAttack", "MagicStone", num25);
			m_player.PlayerProp.UpadateBaseProp(isSelf: true, "MagicDefence", "MagicStone", num26);
			m_player.PlayerProp.UpadateBaseProp(isSelf: true, "MagicAttack", "Horse", num27);
			m_player.PlayerProp.UpadateBaseProp(isSelf: true, "MagicDefence", "Horse", num28);
			m_player.PlayerProp.UpadateBaseProp(isSelf: true, "MagicAttack", "HorsePicCherish", num30);
			m_player.PlayerProp.UpadateBaseProp(isSelf: true, "MagicDefence", "HorsePicCherish", num31);
			m_player.PlayerProp.UpadateBaseProp(isSelf: true, "MagicAttack", "Enchant", num34);
			m_player.PlayerProp.UpadateBaseProp(isSelf: true, "MagicDefence", "Enchant", num33);
			m_player.PlayerProp.UpadateBaseProp(isSelf: true, "MagicAttack", "Texp", num19);
			m_player.PlayerProp.UpadateBaseProp(isSelf: true, "MagicDefence", "Texp", num20);
			m_player.PlayerProp.UpadateBaseProp(isSelf: true, "MagicDefence", "Temple", mgDef2 + mgDef3);
			m_player.PlayerProp.UpadateBaseProp(isSelf: true, "MagicAttack", "Card", mgattack);
			m_player.PlayerProp.UpadateBaseProp(isSelf: true, "MagicDefence", "Card", mgdef);
			m_player.PlayerProp.UpadateBaseProp(isSelf: true, "MagicAttack", "Mark", m_player.MarkProperties.MagicAttack);
			m_player.PlayerProp.UpadateBaseProp(isSelf: true, "MagicDefence", "Mark", m_player.MarkProperties.MagicDefence);
			m_player.PlayerProp.UpadateBaseProp(isSelf: true, "MagicAttack", "SeloPet", num52);
			m_player.PlayerProp.UpadateBaseProp(isSelf: true, "MagicDefence", "SeloPet", num53);
			m_player.PlayerProp.UpadateBaseProp(isSelf: true, "MagicAttack", "magicHouse", enhanceAtt);
			m_player.PlayerProp.UpadateBaseProp(isSelf: true, "MagicDefence", "magicHouse", enhanceDef);
			m_player.PlayerProp.UpadateBaseProp(isSelf: true, "MagicAttack", "marKing", currentVal5);
			m_player.PlayerProp.UpadateBaseProp(isSelf: true, "MagicDefence", "marKing", currentVal6);
			m_player.UpdateFightPower();
			m_player.ApertureEquip(num2);
			m_player.PlayerProp.ViewCurrent();
			m_player.GetHalloweenCardCount();
			GetUserNimbus();
		}
		finally
		{
			m_player.CommitChanges();
		}
	}

	public Dictionary<string, int> GetProp(int attack, int defence, int agility, int lucky, int hp)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		dictionary.Add("Attack", attack);
		dictionary.Add("Defence", defence);
		dictionary.Add("Agility", agility);
		dictionary.Add("Luck", lucky);
		dictionary.Add("HP", hp);
		return dictionary;
	}

	public void GetUserNimbus()
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < 31; i++)
		{
			ItemInfo itemAt = GetItemAt(i);
			if (itemAt == null)
			{
				continue;
			}
			if (itemAt.StrengthenLevel >= 5 && itemAt.StrengthenLevel <= 8)
			{
				if (itemAt.Template.CategoryID == 1 || itemAt.Template.CategoryID == 5)
				{
					num = ((num <= 1) ? 1 : num);
				}
				if (itemAt.Template.CategoryID == 7 || itemAt.Template.CategoryID == 27)
				{
					num2 = ((num2 <= 1) ? 1 : num2);
				}
			}
			else if (itemAt.StrengthenLevel >= 9 && itemAt.StrengthenLevel <= 11)
			{
				if (itemAt.Template.CategoryID == 1 || itemAt.Template.CategoryID == 5)
				{
					num = ((num > 1) ? num : 2);
				}
				if (itemAt.Template.CategoryID == 7 || itemAt.Template.CategoryID == 27)
				{
					num2 = ((num2 > 1) ? num2 : 2);
				}
			}
			else if (itemAt.StrengthenLevel >= 12 && itemAt.StrengthenLevel <= 14)
			{
				if (itemAt.Template.CategoryID == 1 || itemAt.Template.CategoryID == 5)
				{
					num = ((num > 1) ? num : 3);
				}
				if (itemAt.Template.CategoryID == 7 || itemAt.Template.CategoryID == 27)
				{
					num2 = ((num2 > 1) ? num2 : 3);
				}
			}
			else if (itemAt.StrengthenLevel >= 15)
			{
				if (itemAt.Template.CategoryID == 1 || itemAt.Template.CategoryID == 5)
				{
					num = ((num > 1) ? num : 4);
				}
				if (itemAt.Template.CategoryID == 7 || itemAt.Template.CategoryID == 27)
				{
					num2 = ((num2 > 1) ? num2 : 4);
				}
			}
			if (itemAt.GoldValidDate())
			{
				if (itemAt.Template.CategoryID == 1 || itemAt.Template.CategoryID == 5)
				{
					num = 5;
				}
				if (itemAt.Template.CategoryID == 7 || itemAt.Template.CategoryID == 27)
				{
					num2 = 5;
				}
			}
		}
		List<UserEquipGhostInfo> allEquipGhost = m_player.GetAllEquipGhost();
		foreach (UserEquipGhostInfo item in allEquipGhost)
		{
			if (item.Level < SpiritInfoMgr.MAX_LEVEL)
			{
				continue;
			}
			SpiritInfo singleSpirit = SpiritInfoMgr.GetSingleSpirit(item.BagType, item.Place, item.Level);
			if (singleSpirit != null && GetItemAt(item.Place) != null)
			{
				if (singleSpirit.CategoryId == 1 || singleSpirit.CategoryId == 5)
				{
					num = 6;
				}
				else if (singleSpirit.CategoryId == 7)
				{
					num2 = 6;
				}
			}
		}
		m_player.PlayerCharacter.Nimbus = num * 100 + num2;
		m_player.Out.SendUpdatePublicPlayer(m_player.PlayerCharacter, m_player.BattleData.MatchInfo);
	}

	public void EquipBeadEffect()
	{
		m_player.EquipEffect.Clear();
		int[] array = new int[3] { 1, 2, 3 };
		for (int i = 0; i < array.Length; i++)
		{
			ItemInfo itemAt = m_player.BeadBag.GetItemAt(array[i]);
			if (itemAt != null && ((itemAt.Template.Property1 == 31 && itemAt.Template.Property2 == 1) || (itemAt.Template.Property1 == 31 && itemAt.Template.Property2 == 2)))
			{
				m_player.AddBeadEffect(itemAt);
			}
		}
	}

	public void EquipHorseAmuletEffect()
	{
		m_player.HorseAmuletEquipEffect.Clear();
		for (int i = 0; i < 9; i++)
		{
			ItemInfo itemAt = m_player.HorseAmuletBag.GetItemAt(i);
			if (itemAt != null)
			{
				m_player.AddHorseAmuletEffect(itemAt);
			}
		}
	}

	public void AddBeadProperty(int place, ref int attack, ref int defence, ref int agility, ref int lucky, ref int hp)
	{
		ItemInfo itemAt = m_player.BeadBag.GetItemAt(place);
		if (itemAt != null)
		{
			AddRuneProperty(itemAt, ref attack, ref defence, ref agility, ref lucky, ref hp);
		}
	}

	public void AddRuneProperty(ItemInfo item, ref int attack, ref int defence, ref int agility, ref int lucky, ref int hp)
	{
		RuneTemplateInfo runeTemplateInfo = RuneMgr.FindRuneByTemplateID(item.TemplateID);
		if (runeTemplateInfo == null)
		{
			return;
		}
		string[] array = runeTemplateInfo.Attribute1.Split('|');
		string[] array2 = runeTemplateInfo.Attribute2.Split('|');
		string[] array3 = runeTemplateInfo.Attribute3.Split('|');
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		if (item.Hole1 > runeTemplateInfo.BaseLevel)
		{
			if (array.Length > 1)
			{
				num = 1;
			}
			if (array2.Length > 1)
			{
				num2 = 1;
			}
			if (array3.Length > 1)
			{
				num3 = 1;
			}
		}
		int num4 = Convert.ToInt32(array[num]);
		int num5 = Convert.ToInt32(array2[num2]);
		int num6 = Convert.ToInt32(array2[num3]);
		switch (runeTemplateInfo.Type1)
		{
		case 31:
			attack += num4;
			hp += num5;
			break;
		case 32:
			defence += num4;
			hp += num5;
			break;
		case 33:
			agility += num4;
			hp += num5;
			break;
		case 34:
			lucky += num4;
			hp += num5;
			break;
		case 35:
			if (array3.Length != 0)
			{
				switch (runeTemplateInfo.Type2)
				{
				case 31:
					attack += num5;
					break;
				case 32:
					defence += num5;
					break;
				case 33:
					agility += num5;
					break;
				case 34:
					lucky += num5;
					break;
				}
				hp += num6;
			}
			else
			{
				hp += num5;
			}
			break;
		case 36:
			if (array3.Length != 0)
			{
				switch (runeTemplateInfo.Type2)
				{
				case 31:
					attack += num5;
					break;
				case 32:
					defence += num5;
					break;
				case 33:
					agility += num5;
					break;
				case 34:
					lucky += num5;
					break;
				}
				hp += num6;
			}
			else
			{
				hp += num5;
			}
			break;
		case 37:
			hp += num4;
			break;
		}
	}

	public void AddBaseTotemProperty(int totemId, ref int attack, ref int defence, ref int agility, ref int lucky, ref int hp)
	{
		int totalTotemUpgradePlus = m_player.GetTotalTotemUpgradePlus();
		attack += TotemMgr.GetTotemProp(totemId, "att", totalTotemUpgradePlus);
		defence += TotemMgr.GetTotemProp(totemId, "def", totalTotemUpgradePlus);
		agility += TotemMgr.GetTotemProp(totemId, "agi", totalTotemUpgradePlus);
		lucky += TotemMgr.GetTotemProp(totemId, "luc", totalTotemUpgradePlus);
		hp += TotemMgr.GetTotemProp(totemId, "blo", totalTotemUpgradePlus);
	}

	public void AddBaseLatentProperty(ItemInfo item, ref int attack, ref int defence, ref int agility, ref int lucky)
	{
		if (item != null && !item.IsValidLatentEnergy())
		{
			string[] array = item.latentEnergyCurStr.Split(',');
			attack += Convert.ToInt32(array[0]);
			defence += Convert.ToInt32(array[1]);
			agility += Convert.ToInt32(array[2]);
			lucky += Convert.ToInt32(array[3]);
		}
	}

	public void AddBaseGemstoneProperty(ItemInfo item, ref int attack, ref int defence, ref int agility, ref int lucky, ref int hp)
	{
		List<UserGemStone> gemStone = m_player.GemStone;
		int addAtt = 0;
		int rdcDama = 0;
		foreach (UserGemStone item2 in gemStone)
		{
			try
			{
				string[] array = item2.FigSpiritIdValue.Split('|');
				int figSpiritId = item2.FigSpiritId;
				int place = item.Place;
				for (int i = 0; i < array.Length; i++)
				{
					int lv = Convert.ToInt32(array[i].Split(',')[0]);
					switch (item.Place)
					{
					case 2:
						attack += FightSpiritTemplateMgr.GetProp(figSpiritId, lv, place, ref addAtt, ref rdcDama);
						break;
					case 11:
						defence += FightSpiritTemplateMgr.GetProp(figSpiritId, lv, place, ref addAtt, ref rdcDama);
						break;
					case 5:
						agility += FightSpiritTemplateMgr.GetProp(figSpiritId, lv, place, ref addAtt, ref rdcDama);
						break;
					case 3:
						lucky += FightSpiritTemplateMgr.GetProp(figSpiritId, lv, place, ref addAtt, ref rdcDama);
						break;
					case 13:
						hp += FightSpiritTemplateMgr.GetProp(figSpiritId, lv, place, ref addAtt, ref rdcDama);
						break;
					}
				}
			}
			catch
			{
				AbstractInventory.log.ErrorFormat("Add Base Gemstone UserID: {0}, UserName: {1}, FigSpiritId {2}, FigSpiritIdValue: {3}, have error can not get Property", m_player.PlayerCharacter.ID, m_player.PlayerCharacter.UserName, item2.FigSpiritId, item2.FigSpiritIdValue);
			}
		}
		m_player.PlayerCharacter.GoldenAddAttack = addAtt;
		m_player.PlayerCharacter.GoldenReduceDamage = rdcDama;
	}
}
