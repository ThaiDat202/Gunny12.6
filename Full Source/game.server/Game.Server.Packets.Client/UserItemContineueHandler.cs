using System;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(62, "续费功能")]
public class UserItemContineueHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return 0;
		}
		int num = packet.ReadInt();
		string translateId = "UserItemContineueHandler.Success";
		for (int i = 0; i < num; i++)
		{
			eBageType eBageType = (eBageType)packet.ReadByte();
			int num2 = packet.ReadInt();
			int iD = packet.ReadInt();
			byte b = packet.ReadByte();
			bool flag = packet.ReadBoolean();
			packet.ReadInt();
			ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(iD);
			ItemInfo itemAt = client.Player.GetItemAt(eBageType, num2);
			if (eBageType == eBageType.EquipBag && itemAt != null && shopItemInfoById != null && shopItemInfoById.TemplateID == itemAt.TemplateID && itemAt.ValidDate != 0)
			{
				int gold = 0;
				int money = 0;
				int offer = 0;
				int gifttoken = 0;
				int medal = 0;
				int damageScore = 0;
				int petScore = 0;
				int iTemplateID = 0;
				int iCount = 0;
				int hardCurrency = 0;
				int LeagueMoney = 0;
				int scoreCatchInsect = 0;
				int validDate = itemAt.ValidDate;
				int fishScore = 0;
				int useableScore = 0;
				DateTime beginDate = itemAt.BeginDate;
				int count = itemAt.Count;
				if (itemAt.IsValidItem())
				{
					translateId = "UserItemContineueHandler.Translate1";
					client.Out.SendMessage(eMessageType.ALERT, translateId);
					return 0;
				}
				if (!ShopMgr.SetItemType(shopItemInfoById, b, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref useableScore, ref scoreCatchInsect, ref fishScore))
				{
					client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserItemContineueHandler.Translate5"));
					return 0;
				}
				if (gold <= client.Player.PlayerCharacter.Gold && money <= client.Player.PlayerCharacter.Money && offer <= client.Player.PlayerCharacter.Offer && gifttoken <= client.Player.PlayerCharacter.DDTMoney)
				{
					if (!itemAt.IsValidItem())
					{
						if (1 == b)
						{
							itemAt.ValidDate = shopItemInfoById.AUnit;
						}
						if (2 == b)
						{
							itemAt.ValidDate = shopItemInfoById.BUnit;
						}
						if (3 == b)
						{
							itemAt.ValidDate = shopItemInfoById.CUnit;
						}
						itemAt.BeginDate = DateTime.Now;
						itemAt.IsUsed = true;
						client.Player.RemoveMoney(money);
						client.Player.RemoveGold(gold);
						client.Player.RemoveOffer(offer);
						client.Player.RemoveGiftToken(gifttoken);
					}
					else
					{
						translateId = "UserItemContineueHandler.Translate6";
					}
				}
				else
				{
					itemAt.ValidDate = validDate;
					itemAt.Count = count;
					translateId = "UserItemContineueHandler.NoMoney";
				}
				if (flag)
				{
					int num3 = client.Player.EquipBag.FindItemEpuipSlot(itemAt.Template);
					if (client.Player.GetItemAt(eBageType, num3) == null && num2 > client.Player.EquipBag.BeginSlot)
					{
						client.Player.EquipBag.MoveItem(num2, num3, 1);
					}
				}
				else
				{
					client.Player.EquipBag.UpdateItem(itemAt);
				}
			}
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(translateId));
		}
		return 0;
	}
}
