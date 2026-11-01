using System;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(311, "Vip积分商城")]
internal class BuyVipIntegrationHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int id = packet.ReadInt();
		int num = packet.ReadInt();
		VipStoreList vipStoreList = VipIntegrationMgr.VipStoreLists.Find((VipStoreList p) => p.ID == id);
		if (vipStoreList == null)
		{
			return 0;
		}
		int num2 = 0;
		int num3 = MaxCount(vipStoreList, client.Player.PlayerCharacter.VIPLevel);
		string[] array = client.Player.PlayerCharacter.VipShop.Split(new string[1] { "|" }, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length != 0)
		{
			string[] array2 = array;
			foreach (string text in array2)
			{
				string[] array3 = text.Split(',');
				if (vipStoreList.GoodsID == int.Parse(array3[0]))
				{
					num2 = int.Parse(array3[1]);
				}
			}
		}
		if (num2 >= MaxCount(vipStoreList, client.Player.PlayerCharacter.VIPLevel))
		{
			client.Player.SendMessage(LanguageMgr.GetTranslation("BuyVipIntegrationHandler.MaxCount"));
			return 0;
		}
		int num4 = ((num3 - num2 < num) ? (num3 - num2) : num);
		if (client.Player.RemoveVipIntegral(vipStoreList.Price * num4) > 0)
		{
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(vipStoreList.GoodsID);
			if (itemTemplateInfo != null)
			{
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 102);
				itemInfo.IsBinds = true;
				itemInfo.ValidDate = vipStoreList.ValidDate;
				itemInfo.Count = num4;
				client.Player.SendItemToMail(itemInfo, itemInfo.Template.Name + "x" + num4, "Phần thưởng đổi quà tại đổi điểm VIP.", eMailType.BuyItem);
				client.Player.SendMessage(LanguageMgr.GetTranslation("Mua thành công."));
				client.Player.PlayerCharacter.SaveVipShop(vipStoreList.GoodsID, num4);
			}
		}
		client.Player.SendRefreshInfoVipShop();
		return 0;
	}

	public int MaxCount(VipStoreList info, int vipLevel)
	{
		return vipLevel switch
		{
			1 => info.Vip1Quantity, 
			2 => info.Vip2Quantity, 
			3 => info.Vip3Quantity, 
			4 => info.Vip4Quantity, 
			5 => info.Vip5Quantity, 
			6 => info.Vip6Quantity, 
			7 => info.Vip7Quantity, 
			8 => info.Vip8Quantity, 
			9 => info.Vip9Quantity, 
			10 => info.Vip10Quantity, 
			11 => info.Vip11Quantity, 
			12 => info.Vip12Quantity, 
			13 => info.Vip13Quantity, 
			14 => info.Vip14Quantity, 
			15 => info.Vip15Quantity, 
			_ => 0, 
		};
	}
}
