using System;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(61, "物品转移")]
public class ItemTransferHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(61, client.Player.PlayerCharacter.ID);
		StringBuilder stringBuilder = new StringBuilder();
		int num = 40000;
		bool tranHole = packet.ReadBoolean();
		bool tranHoleFivSix = packet.ReadBoolean();
		eBageType bageType = (eBageType)packet.ReadInt();
		int num2 = packet.ReadInt();
		eBageType bageType2 = (eBageType)packet.ReadInt();
		int num3 = packet.ReadInt();
		PlayerInventory inventory = client.Player.GetInventory(bageType);
		PlayerInventory inventory2 = client.Player.GetInventory(bageType2);
		ItemInfo itemZero = inventory.GetItemAt(num2);
		ItemInfo itemOne = inventory2.GetItemAt(num3);
		if (itemZero != null && itemOne != null && itemZero.Template.CategoryID == itemOne.Template.CategoryID && itemOne.Count == 1 && itemZero.Count == 1 && itemZero.IsValidItem() && itemOne.IsValidItem())
		{
			if (client.Player.PlayerCharacter.Gold < num)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("itemtransferhandler.nogold"));
				return 1;
			}
			client.Player.RemoveGold(num);
			StrengthenMgr.InheritTransferProperty(ref itemZero, ref itemOne, tranHole, tranHoleFivSix);
			int num4 = OrginWeaponID(itemZero);
			int num5 = OrginWeaponID(itemOne);
			ItemTemplateInfo itemTemplateInfo = null;
			ItemTemplateInfo itemTemplateInfo2 = null;
			if (num4 > 0)
			{
				itemTemplateInfo = ItemMgr.FindItemTemplate(GainWeaponID(itemZero.StrengthenLevel, num4));
			}
			if (num5 > 0)
			{
				itemTemplateInfo2 = ItemMgr.FindItemTemplate(GainWeaponID(itemOne.StrengthenLevel, num5));
			}
			if (TransferCondition(itemOne, itemZero))
			{
				if (itemTemplateInfo != null)
				{
					ItemInfo itemInfo = ItemInfo.CloneFromTemplate(itemTemplateInfo, itemZero);
					if (itemInfo.GoldValidDate())
					{
						GoldEquipTemplateInfo goldEquipTemplateInfo = GoldEquipMgr.FindGoldEquipByTemplate(itemTemplateInfo.TemplateID);
						if (goldEquipTemplateInfo != null)
						{
							ItemTemplateInfo itemTemplateInfo3 = ItemMgr.FindItemTemplate(goldEquipTemplateInfo.NewTemplateId);
							if (itemTemplateInfo3 != null)
							{
								itemInfo.GoldEquip = itemTemplateInfo3;
							}
						}
					}
					inventory.RemoveItemAt(num2);
					inventory.AddItemTo(itemInfo, 0);
				}
				if (itemTemplateInfo != null && itemTemplateInfo2 != null)
				{
					ItemInfo itemInfo2 = ItemInfo.CloneFromTemplate(itemTemplateInfo2, itemOne);
					if (itemInfo2.GoldValidDate())
					{
						GoldEquipTemplateInfo goldEquipTemplateInfo2 = GoldEquipMgr.FindGoldEquipByTemplate(itemTemplateInfo2.TemplateID);
						if (goldEquipTemplateInfo2 != null)
						{
							ItemTemplateInfo itemTemplateInfo4 = ItemMgr.FindItemTemplate(goldEquipTemplateInfo2.NewTemplateId);
							if (itemTemplateInfo4 != null)
							{
								itemInfo2.GoldEquip = itemTemplateInfo4;
							}
						}
					}
					inventory2.RemoveItemAt(num3);
					inventory2.AddItemTo(itemInfo2, 1);
				}
				Console.WriteLine($"m_temIdOrd: {num4} m_temIdNew: {num5}");
			}
			else
			{
				inventory.UpdateItem(itemZero);
				inventory2.UpdateItem(itemOne);
			}
			gSPacketIn.WriteByte(0);
			client.Out.SendTCP(gSPacketIn);
		}
		else
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("itemtransferhandler.nocondition"));
		}
		return 0;
	}

	public bool TransferCondition(ItemInfo itemAtZero, ItemInfo itemAtOne)
	{
		if (itemAtZero.Template.CategoryID != 7 && itemAtOne.Template.CategoryID != 7)
		{
			return false;
		}
		if (itemAtZero.StrengthenLevel < 10 && itemAtOne.StrengthenLevel < 10)
		{
			return false;
		}
		return true;
	}

	private int OrginWeaponID(ItemInfo item)
	{
		StrengthenGoodsInfo strengthenGoodsInfo = StrengthenMgr.FindTransferInfo(item.TemplateID);
		if (strengthenGoodsInfo == null)
		{
			GoldEquipTemplateInfo goldEquipTemplateInfo = GoldEquipMgr.FindGoldEquipOldTemplate(item.TemplateID);
			if (goldEquipTemplateInfo == null)
			{
				return item.TemplateID;
			}
			strengthenGoodsInfo = StrengthenMgr.FindTransferInfo(goldEquipTemplateInfo.OldTemplateId);
		}
		if (strengthenGoodsInfo == null || strengthenGoodsInfo.Level < item.StrengthenLevel)
		{
			return item.TemplateID;
		}
		return strengthenGoodsInfo.OriginalEquip;
	}

	private int GainWeaponID(int strengthenLevel, int transId)
	{
		if (strengthenLevel >= 10 && strengthenLevel <= 19)
		{
			return StrengthenMgr.FindTransferInfo(strengthenLevel, transId)?.GainEquip ?? (-1);
		}
		StrengthenGoodsInfo strengthenGoodsInfo = StrengthenMgr.FindTransferInfo(transId);
		if (strengthenGoodsInfo == null)
		{
			return 0;
		}
		if (strengthenGoodsInfo.Level < strengthenLevel)
		{
			return transId;
		}
		return strengthenGoodsInfo.OriginalEquip;
	}
}
