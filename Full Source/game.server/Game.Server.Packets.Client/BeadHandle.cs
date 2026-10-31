using System;
using System.Collections.Generic;
using System.Linq;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameUtils;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(121, "宝珠系统")]
public class BeadHandle : IPacketHandler
{
	public static Random random = new Random();

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		byte b = packet.ReadByte();
		PlayerBeadInventory beadBag = client.Player.BeadBag;
		int num = -1;
		string text = "";
		if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return 0;
		}
		switch (b)
		{
		case 7:
		{
			int id = packet.ReadInt();
			int slot = packet.ReadInt();
			int slot2 = packet.ReadInt();
			RuneAdvanceTemplateInfo runeAdvanceTemplateInfo = RuneAdvanceTemplateMgr.FindRuneAdvanceTemplate(id);
			if (runeAdvanceTemplateInfo == null)
			{
				beadBag.UpdateChangedPlaces();
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("BeadHandle.RuneAdvanceTemplate.notFound"));
				return 0;
			}
			ItemInfo itemAt4 = beadBag.GetItemAt(slot);
			ItemInfo itemAt5 = beadBag.GetItemAt(slot2);
			if (itemAt4 == null || itemAt5 == null)
			{
				beadBag.UpdateChangedPlaces();
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("BeadHandle.RuneAdvanceTemplate.notFound2"));
				return 0;
			}
			string[] source = runeAdvanceTemplateInfo.MainMaterials.Split('|');
			string[] source2 = runeAdvanceTemplateInfo.AuxiliaryMaterials.Split('|');
			if (!source.Contains(itemAt4.TemplateID.ToString()) || !source2.Contains(itemAt5.TemplateID.ToString()))
			{
				beadBag.UpdateChangedPlaces();
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("BeadHandle.RuneAdvanceTemplate.notFound3"));
				return 0;
			}
			ItemTemplateInfo itemTemplateInfo2 = ItemMgr.FindItemTemplate(runeAdvanceTemplateInfo.AdvancedTempId);
			if (itemTemplateInfo2 == null)
			{
				beadBag.UpdateChangedPlaces();
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("BeadHandle.RuneAdvanceTemplate.notFound4"));
				return 0;
			}
			ItemInfo itemInfo4 = ItemInfo.CreateFromTemplate(itemTemplateInfo2, 1, 105);
			itemInfo4.IsBinds = true;
			beadBag.AddItem(itemInfo4);
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("BeadHandle.RuneAdvanceTemplate.Success"));
			beadBag.RemoveItem(itemAt4);
			beadBag.RemoveItem(itemAt5);
			beadBag.UpdateChangedPlaces();
			break;
		}
		case 4:
		{
			num = packet.ReadInt();
			ItemInfo itemAt = beadBag.GetItemAt(num);
			if (itemAt == null)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("BeadHandle.Msg1"));
				return 0;
			}
			if (itemAt.IsUsed)
			{
				itemAt.IsUsed = false;
			}
			else
			{
				itemAt.IsUsed = true;
			}
			beadBag.UpdateItem(itemAt);
			beadBag.UpdateChangedPlaces();
			break;
		}
		case 1:
		{
			int num10 = packet.ReadInt();
			int num11 = packet.ReadInt();
			int needLv = 10;
			if (DateTime.Compare(client.Player.LastDrillUpTime.AddMilliseconds(200.0), DateTime.Now) > 0)
			{
				client.Player.LastDrillUpTime = DateTime.Now;
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("BeadHandle.Translate1"));
				break;
			}
			beadBag.BeginChanges();
			client.Player.LastDrillUpTime = DateTime.Now;
			if (num11 == -1 || num11 > beadBag.BeginSlot)
			{
				num11 = beadBag.FindFirstEmptySlot();
			}
			if (num11 <= 12 && num11 >= 4 && !beadBag.canEquip(num11, client.Player.PlayerCharacter.Grade, ref needLv))
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("BeadHandle.Msg2", needLv));
				break;
			}
			ItemInfo itemAt2 = beadBag.GetItemAt(num10);
			ItemInfo itemAt3 = beadBag.GetItemAt(num11);
			if (itemAt2 == null)
			{
				beadBag.UpdateChangedPlaces();
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("BeadHandle.Translate2"));
				break;
			}
			RuneTemplateInfo runeTemplateInfo3 = RuneMgr.FindRuneByTemplateID(itemAt2.TemplateID);
			if (num11 < 13)
			{
				if (runeTemplateInfo3.IsAttack() && num11 != 1)
				{
					client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("BeadHandle.Translate3"));
					num11 = beadBag.FindFirstEmptySlot();
				}
				if (runeTemplateInfo3.IsDefend() && (num11 == 1 || num11 > 3))
				{
					client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("BeadHandle.Translate3"));
					num11 = beadBag.FindFirstEmptySlot();
				}
				if (runeTemplateInfo3.IsProp() && num11 <= 3)
				{
					client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("BeadHandle.Translate3"));
					num11 = beadBag.FindFirstEmptySlot();
				}
			}
			if (num10 > 18 || num10 < 13)
			{
				if (num11 <= 18 && num11 >= 13)
				{
					int drillLevel = beadBag.GetDrillLevel(num11);
					if (!beadBag.JudgeLevel(itemAt2.Hole1, drillLevel) || (itemAt3 != null && !beadBag.JudgeLevel(itemAt3.Hole1, drillLevel)))
					{
						beadBag.UpdateChangedPlaces();
						client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("BeadHandle.Translate4"));
						break;
					}
				}
				else if (num11 > 30 && itemAt3 != null)
				{
					RuneTemplateInfo runeTemplateInfo4 = RuneMgr.FindRuneByTemplateID(itemAt3.TemplateID);
					if (num10 == 1 && !runeTemplateInfo4.IsAttack())
					{
						beadBag.UpdateChangedPlaces();
						client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("BeadHandle.Translate3"));
						break;
					}
					if ((num10 == 2 || num10 == 3) && !runeTemplateInfo4.IsDefend())
					{
						beadBag.UpdateChangedPlaces();
						client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("BeadHandle.Translate3"));
						break;
					}
					if (num10 > 3 && num10 < 30 && !runeTemplateInfo4.IsProp())
					{
						beadBag.UpdateChangedPlaces();
						client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("BeadHandle.Translate3"));
						break;
					}
				}
			}
			else
			{
				int drillLevel2 = beadBag.GetDrillLevel(num10);
				if (!beadBag.JudgeLevel(itemAt2.Hole1, drillLevel2) || (itemAt3 != null && !beadBag.JudgeLevel(itemAt3.Hole1, drillLevel2)))
				{
					beadBag.CommitChanges();
					beadBag.UpdateChangedPlaces();
					client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("BeadHandle.Translate4"));
					break;
				}
			}
			if (!beadBag.MoveItem(num10, num11, 1))
			{
				beadBag.UpdateChangedPlaces();
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("BeadHandle.Translate5"));
			}
			else
			{
				beadBag.UpdateChangedPlaces();
				client.Player.EquipBag.UpdatePlayerProperties();
			}
			break;
		}
		case 5:
		{
			int num14 = packet.ReadInt();
			int templateId = packet.ReadInt();
			int num15 = packet.ReadInt();
			if (DateTime.Compare(client.Player.LastDrillUpTime.AddMilliseconds(200.0), DateTime.Now) > 0)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("BeadHandle.Msg6"));
				return 0;
			}
			bool flag3 = false;
			PlayerInventory inventory = client.Player.GetInventory(eBageType.PropBag);
			ItemInfo itemByTemplateID = inventory.GetItemByTemplateID(0, templateId);
			UserDrillInfo userDrillInfo = beadBag.UserDrills[num14];
			if (itemByTemplateID == null || userDrillInfo == null || !itemByTemplateID.isDrill(userDrillInfo.HoleLv))
			{
				client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("BeadHandle.Msg7"));
				return 0;
			}
			int itemCount = inventory.GetItemCount(templateId);
			if (itemCount <= 0 || num15 <= 0)
			{
				client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("BeadHandle.Msg8"));
			}
			else
			{
				if (itemCount < num15)
				{
					num15 = itemCount;
				}
				int num16 = 0;
				int k;
				for (k = 0; k < num15; k++)
				{
					int num17 = random.Next(2, 6);
					userDrillInfo.HoleExp += num17;
					num16 += num17;
					switch (userDrillInfo.HoleLv)
					{
					case 0:
						if (userDrillInfo.HoleExp >= GameProperties.HoleLevelUpExp(0))
						{
							userDrillInfo.HoleLv++;
							userDrillInfo.HoleExp = 0;
							flag3 = true;
						}
						break;
					case 1:
						if (userDrillInfo.HoleExp >= GameProperties.HoleLevelUpExp(1))
						{
							userDrillInfo.HoleLv++;
							userDrillInfo.HoleExp = 0;
							flag3 = true;
						}
						break;
					case 2:
						if (userDrillInfo.HoleExp >= GameProperties.HoleLevelUpExp(2))
						{
							userDrillInfo.HoleLv++;
							userDrillInfo.HoleExp = 0;
							flag3 = true;
						}
						break;
					case 3:
						if (userDrillInfo.HoleExp >= GameProperties.HoleLevelUpExp(3))
						{
							userDrillInfo.HoleLv++;
							userDrillInfo.HoleExp = 0;
							flag3 = true;
						}
						break;
					case 4:
						if (userDrillInfo.HoleExp >= GameProperties.HoleLevelUpExp(4))
						{
							userDrillInfo.HoleLv++;
							userDrillInfo.HoleExp = 0;
							flag3 = true;
						}
						break;
					case 5:
						if (userDrillInfo.HoleExp >= GameProperties.HoleLevelUpExp(5))
						{
							userDrillInfo.HoleLv++;
							userDrillInfo.HoleExp = 0;
							flag3 = true;
						}
						break;
					}
					if (flag3)
					{
						break;
					}
				}
				text = LanguageMgr.GetTranslation("OpenHoleHandler.GetExp", num16);
				beadBag.UpdateDrill(num14, userDrillInfo);
				if (k < num15)
				{
					num15 = k;
				}
			}
			if (text != "")
			{
				client.Out.SendMessage(eMessageType.Normal, text);
			}
			beadBag.SendPlayerDrill();
			inventory.RemoveTemplate(templateId, num15);
			client.Player.LastDrillUpTime = DateTime.Now;
			if (flag3)
			{
				using PlayerBussiness playerBussiness = new PlayerBussiness();
				foreach (UserDrillInfo value in beadBag.UserDrills.Values)
				{
					playerBussiness.UpdateUserDrillInfo(value);
				}
			}
			beadBag.UpdateChangedPlaces();
			break;
		}
		case 3:
		{
			int num12 = packet.ReadInt();
			bool flag = packet.ReadBoolean();
			bool flag2 = false;
			string[] array = GameProperties.OpenRunePackageMoney.Split('|');
			string[] array2 = GameProperties.RunePackageID.Split('|');
			string[] array3 = GameProperties.OpenRunePackageRange.Split('|');
			if (DateTime.Compare(client.Player.LastOpenPack.AddMilliseconds(500.0), DateTime.Now) > 0)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("BeadHandle.Msg6"));
				beadBag.UpdateChangedPlaces();
				return 0;
			}
			if (beadBag.FindFirstEmptySlot() == -1)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("BeadHandle.Msg9"));
				beadBag.UpdateChangedPlaces();
				return 0;
			}
			if (num12 < 0 || num12 > array.Length)
			{
				num12 = array.Length - 1;
			}
			if (client.Player.Extra.UseKingBless(3))
			{
				flag2 = true;
			}
			else if (client.Player.MoneyDirect(int.Parse(array[num12])))
			{
				flag2 = true;
			}
			if (flag2)
			{
				List<ItemInfo> list2 = new List<ItemInfo>();
				int dateId = int.Parse(array2[0]);
				string[] array4 = array3[2].Split(',');
				switch (num12)
				{
				case 1:
					array4 = array3[1].Split(',');
					dateId = int.Parse(array2[1]);
					break;
				case 2:
					array4 = array3[0].Split(',');
					dateId = int.Parse(array2[2]);
					break;
				case 3:
					dateId = int.Parse(array2[3]);
					break;
				}
				SpecialItemBoxInfo specialValue = new SpecialItemBoxInfo();
				ItemBoxMgr.CreateItemBox(dateId, list2, ref specialValue);
				if (list2.Count < 1)
				{
					client.Player.SendMessage(LanguageMgr.GetTranslation("BeadHandle.Msg10"));
					return 0;
				}
				ItemInfo itemInfo3 = list2[0];
				itemInfo3.Count = 1;
				beadBag.AddItem(itemInfo3);
				RuneTemplateInfo runeTemplateInfo5 = RuneMgr.FindRuneByTemplateID(itemInfo3.TemplateID);
				if (runeTemplateInfo5 != null)
				{
					client.Player.SendMessage(LanguageMgr.GetTranslation("BeadHandle.Msg11", runeTemplateInfo5.Name, runeTemplateInfo5.BaseLevel));
					if (runeTemplateInfo5.BaseLevel > 13)
					{
						client.Player.SendItemNotice(itemInfo3, 4, "bead");
					}
				}
				int num13 = random.Next(int.Parse(array4[1]));
				if (num13 == 3 || num13 > 4)
				{
					num13 = 0;
				}
				client.Out.SendRuneOpenPackage(client.Player, num13);
			}
			beadBag.UpdateChangedPlaces();
			client.Player.LastOpenPack = DateTime.Now;
			break;
		}
		case 2:
		{
			ItemInfo itemInfo = beadBag.GetItemAt(31);
			if (itemInfo == null)
			{
				return 0;
			}
			List<int> list = new List<int>();
			int num2 = packet.ReadInt();
			int hole = itemInfo.Hole1;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			for (int i = 0; i < num2; i++)
			{
				int num7 = packet.ReadInt();
				ItemInfo itemAt = beadBag.GetItemAt(num7);
				if (itemAt == null || itemAt.IsUsed)
				{
					continue;
				}
				list.Add(num7);
				num4 = itemInfo.Hole2;
				num3 = itemInfo.Hole1;
				num6 = (itemInfo.Hole2 = itemAt.Hole2 + num4);
				itemInfo.Hole1 = RuneMgr.GetRuneLevel(num6);
				itemInfo.IsBinds = true;
				ItemTemplateInfo itemTemplateInfo = null;
				num5 = itemInfo.TemplateID;
				for (int j = num3; j <= itemInfo.Hole1; j++)
				{
					RuneTemplateInfo runeTemplateInfo = RuneMgr.FindRuneByTemplateID(num5);
					if (runeTemplateInfo != null)
					{
						int num9 = runeTemplateInfo.BaseLevel + 1;
						if (itemInfo.Hole1 > num9)
						{
							itemTemplateInfo = ItemMgr.FindItemTemplate(runeTemplateInfo.NextTemplateID);
						}
						num5 = runeTemplateInfo.NextTemplateID;
					}
				}
				if (itemTemplateInfo != null)
				{
					ItemInfo itemInfo2 = ItemInfo.CloneFromTemplate(itemTemplateInfo, itemInfo);
					beadBag.RemoveItemAt(31);
					beadBag.AddItemTo(itemInfo2, 31);
					if (num6 > RuneMgr.MaxExp())
					{
						itemInfo2.Hole2 = RuneMgr.MaxExp();
					}
					else
					{
						itemInfo2.Hole2 = num6;
					}
					itemInfo = itemInfo2;
				}
				beadBag.UpdateItem(itemInfo);
			}
			beadBag.RemoveAllItem(list);
			beadBag.UpdateChangedPlaces();
			beadBag.SaveToDatabase();
			if (hole < itemInfo.Hole1 && itemInfo.Hole1 >= 17)
			{
				RuneTemplateInfo runeTemplateInfo2 = RuneMgr.FindRuneByTemplateID(itemInfo.TemplateID);
				string translation = LanguageMgr.GetTranslation("BeadHandler.Notice", client.Player.PlayerCharacter.NickName, runeTemplateInfo2.Name, itemInfo.Hole1, client.Player.ZoneName);
				GSPacketIn gSPacketIn = new GSPacketIn(10);
				gSPacketIn.WriteInt(2);
				gSPacketIn.WriteString(translation);
				WorldMgr.SendToAll(gSPacketIn);
				GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
			}
			break;
		}
		default:
			Console.WriteLine("BeadHandle: {0}", b);
			break;
		}
		return 0;
	}
}
