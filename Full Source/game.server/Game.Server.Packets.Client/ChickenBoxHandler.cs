using System;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(87, "客户端日记")]
public class ChickenBoxHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		GSPacketIn gSPacketIn = new GSPacketIn(87);
		ActiveSystemInfo info = client.Player.Actives.Info;
		switch (num)
		{
		case 13:
		{
			int pos2 = packet.ReadInt();
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
				return 1;
			}
			int num4 = info.canOpenCounts;
			if (num4 > 0)
			{
				NewChickenBoxItemInfo award2 = client.Player.Actives.GetAward(pos2);
				if (award2 != null)
				{
					award2.IsBinds = true;
					award2.IsSelected = true;
					if (num4 > client.Player.Actives.openCardPrice.Length)
					{
						num4 = client.Player.Actives.openCardPrice.Length;
					}
					int value2 = client.Player.Actives.openCardPrice[num4 - 1];
					if (client.Player.MoneyDirect(value2))
					{
						gSPacketIn.WriteInt(4);
						gSPacketIn.WriteInt(award2.TemplateID);
						gSPacketIn.WriteInt(award2.StrengthenLevel);
						gSPacketIn.WriteInt(award2.Count);
						gSPacketIn.WriteInt(award2.ValidDate);
						gSPacketIn.WriteInt(award2.AttackCompose);
						gSPacketIn.WriteInt(award2.DefendCompose);
						gSPacketIn.WriteInt(award2.AgilityCompose);
						gSPacketIn.WriteInt(award2.LuckCompose);
						gSPacketIn.WriteInt(award2.Position);
						gSPacketIn.WriteBoolean(award2.IsSelected);
						gSPacketIn.WriteBoolean(award2.IsSeeded);
						gSPacketIn.WriteBoolean(award2.IsBinds);
						gSPacketIn.WriteInt(client.Player.Actives.freeOpenCardCount);
						client.Out.SendTCP(gSPacketIn);
						client.Player.Actives.UpdateChickenBoxAward(award2);
						ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(award2.TemplateID), 1, 105);
						itemInfo.IsBinds = award2.IsBinds;
						itemInfo.ValidDate = award2.ValidDate;
						client.Player.AddTemplate(itemInfo, LanguageMgr.GetTranslation("ChickenBoxHandler.Msg7"));
						client.Player.SendMessage(LanguageMgr.GetTranslation("ChickenBoxHandler.Msg8", itemInfo.Template.Name, award2.Count));
						info.canOpenCounts--;
						if (info.canOpenCounts == 0)
						{
							GSPacketIn gSPacketIn2 = new GSPacketIn(87);
							gSPacketIn2.WriteInt(6);
							client.Player.SendTCP(gSPacketIn2);
						}
					}
				}
				else
				{
					client.Player.SendMessage(LanguageMgr.GetTranslation("ChickenBoxHandler.Msg1"));
				}
			}
			else
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("ChickenBoxHandler.Msg9"));
			}
			break;
		}
		case 11:
		{
			int pos = packet.ReadInt();
			int num3 = info.canEagleEyeCounts;
			if (num3 > 0)
			{
				NewChickenBoxItemInfo newChickenBoxItemInfo = client.Player.Actives.ViewAward(pos);
				if (newChickenBoxItemInfo != null)
				{
					if (num3 > client.Player.Actives.eagleEyePrice.Length)
					{
						num3 = client.Player.Actives.eagleEyePrice.Length;
					}
					int value = client.Player.Actives.eagleEyePrice[num3 - 1];
					if (client.Player.MoneyDirect(value))
					{
						newChickenBoxItemInfo.IsSeeded = true;
						gSPacketIn.WriteInt(7);
						gSPacketIn.WriteInt(newChickenBoxItemInfo.TemplateID);
						gSPacketIn.WriteInt(newChickenBoxItemInfo.StrengthenLevel);
						gSPacketIn.WriteInt(newChickenBoxItemInfo.Count);
						gSPacketIn.WriteInt(newChickenBoxItemInfo.ValidDate);
						gSPacketIn.WriteInt(newChickenBoxItemInfo.AttackCompose);
						gSPacketIn.WriteInt(newChickenBoxItemInfo.DefendCompose);
						gSPacketIn.WriteInt(newChickenBoxItemInfo.AgilityCompose);
						gSPacketIn.WriteInt(newChickenBoxItemInfo.LuckCompose);
						gSPacketIn.WriteInt(newChickenBoxItemInfo.Position);
						gSPacketIn.WriteBoolean(newChickenBoxItemInfo.IsSelected);
						gSPacketIn.WriteBoolean(newChickenBoxItemInfo.IsSeeded);
						gSPacketIn.WriteBoolean(newChickenBoxItemInfo.IsBinds);
						gSPacketIn.WriteInt(client.Player.Actives.freeEyeCount);
						client.Player.SendTCP(gSPacketIn);
						client.Player.Actives.UpdateChickenBoxAward(newChickenBoxItemInfo);
						info.canEagleEyeCounts--;
					}
				}
				else
				{
					client.Player.SendMessage(LanguageMgr.GetTranslation("ChickenBoxHandler.Msg1"));
				}
			}
			else
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("ChickenBoxHandler.Msg2"));
			}
			break;
		}
		case 14:
		{
			int flushPrice = client.Player.Actives.flushPrice;
			if (client.Player.Actives.IsFreeFlushTime())
			{
				if (client.Player.MoneyDirect(flushPrice))
				{
					client.Player.Actives.PayFlushView();
					client.Player.Actives.SendChickenBoxItemList();
					client.Player.SendMessage(LanguageMgr.GetTranslation("ChickenBoxHandler.Msg3"));
				}
			}
			else
			{
				client.Player.Actives.PayFlushView();
				client.Player.Actives.SendChickenBoxItemList();
				client.Player.SendMessage(LanguageMgr.GetTranslation("ChickenBoxHandler.Msg4"));
			}
			break;
		}
		case 12:
			client.Player.Actives.SendChickenBoxItemList();
			client.Player.Actives.PayFlushView();
			break;
		case 15:
			info.isShowAll = false;
			client.Player.Actives.RandomPosition();
			gSPacketIn.WriteInt(5);
			gSPacketIn.WriteBoolean(val: true);
			client.Player.SendTCP(gSPacketIn);
			break;
		case 10:
			client.Player.Actives.EnterChickenBox();
			client.Player.Actives.SendChickenBoxItemList();
			break;
		case 31:
			client.Player.Actives.CreateLuckyStartAward();
			client.Player.Actives.SendLuckStarAllGoodsInfo();
			client.Player.Actives.SendLuckStarRewardRank();
			client.Player.Actives.SendLuckStarRewardRecord();
			break;
		case 33:
		{
			int num2 = DateTime.Compare(client.Player.Actives.LuckyStartStartTurn.AddSeconds(7.0), DateTime.Now);
			if (num2 <= 0)
			{
				int templateId = 201192;
				ItemTemplateInfo itemTemplateInfo2 = ItemMgr.FindItemTemplate(templateId);
				if (itemTemplateInfo2 != null)
				{
					PlayerInventory inventory = client.Player.GetInventory(itemTemplateInfo2.BagType);
					ItemInfo itemByTemplateID = inventory.GetItemByTemplateID(0, templateId);
					if (itemByTemplateID != null && itemByTemplateID.Count > 0)
					{
						inventory.RemoveTemplate(templateId, 1);
						client.Player.Actives.ChangeLuckyStartAwardPlace();
						client.Player.Actives.SendLuckStarTurnGoodsInfo();
					}
					else
					{
						client.Player.SendMessage(LanguageMgr.GetTranslation("ChickenBoxHandler.Msg5", itemTemplateInfo2.Name));
					}
				}
				client.Player.Actives.LuckyStartStartTurn = DateTime.Now;
			}
			else
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("ChickenBoxHandler.Msg6"));
			}
			break;
		}
		case 34:
		{
			client.Player.Actives.SendUpdateReward();
			NewChickenBoxItemInfo award = client.Player.Actives.Award;
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(award.TemplateID);
			if (itemTemplateInfo != null && itemTemplateInfo.CategoryID != client.Player.Actives.coinTemplateID)
			{
				ItemInfo cloneItem = ItemInfo.CreateFromTemplate(itemTemplateInfo, award.Count, 105);
				client.Player.AddTemplate(cloneItem, LanguageMgr.GetTranslation("ChickenBoxHandler.Msg10"));
			}
			break;
		}
		default:
		{
			NewChickenBoxPackageType newChickenBoxPackageType = (NewChickenBoxPackageType)num;
			Console.WriteLine("NewChickenBoxPackageType." + newChickenBoxPackageType);
			break;
		}
		case 32:
			break;
		}
		return 0;
	}
}
