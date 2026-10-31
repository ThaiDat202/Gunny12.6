using System.Collections.Generic;
using System.Linq;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.HomeAdorn.Handle;

[HomeAdornHandleAttbute(1)]
public class BuyAndActive : IHomeAdornCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		SpecialItemBoxInfo specialValue = new SpecialItemBoxInfo();
		List<ItemTemplateInfo> list = new List<ItemTemplateInfo>();
		for (int i = 0; i < num; i++)
		{
			int iD = packet.ReadInt();
			bool flag = packet.ReadBoolean();
			ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(iD);
			if (shopItemInfoById != null && ShopMgr.IsOnShop(shopItemInfoById.ID) && shopItemInfoById.ShopID == 89 && !Player.Home.CheckExitAdorn(shopItemInfoById.TemplateID))
			{
				ShopMgr.SetItemType(shopItemInfoById, 1, ref specialValue);
				ItemTemplateInfo item = ItemMgr.FindItemTemplate(shopItemInfoById.TemplateID);
				list.Add(item);
				Player.DirectRemoveValue(specialValue);
			}
		}
		if (list.Count > 0)
		{
			int num2 = 0;
			int num3 = 0;
			foreach (ItemTemplateInfo item2 in list)
			{
				Player.Home.BuyAndActives(item2);
				if (Player.Home.CategoryYardArr.Contains(item2.Property2))
				{
					num2++;
				}
				if (Player.Home.CategoryHouseArr.Contains(item2.Property2))
				{
					num3++;
				}
			}
			Player.SendMessage(LanguageMgr.GetTranslation("BuyAndActive.Success"));
			GSPacketIn gSPacketIn = new GSPacketIn(292);
			if (num3 > 0)
			{
				gSPacketIn.WriteByte(4);
				gSPacketIn.WriteInt(Player.PlayerId);
				gSPacketIn.WriteInt(Player.Home.Data.houseStage);
				AdornInfo[] houseAdorn = Player.Home.HouseAdorn;
				foreach (AdornInfo adornInfo in houseAdorn)
				{
					gSPacketIn.WriteInt(adornInfo.TemplateId);
				}
				Player.SendTCP(gSPacketIn);
				Player.Home.UpdateAdorn("house");
			}
			if (num2 > 0)
			{
				gSPacketIn = new GSPacketIn(292);
				gSPacketIn.WriteByte(7);
				gSPacketIn.WriteInt(Player.PlayerId);
				gSPacketIn.WriteInt(Player.Home.Data.houseStage);
				AdornInfo[] yardAdorn = Player.Home.YardAdorn;
				foreach (AdornInfo adornInfo2 in yardAdorn)
				{
					gSPacketIn.WriteInt(adornInfo2.TemplateId);
				}
				Player.SendTCP(gSPacketIn);
				Player.Home.UpdateAdorn("yard");
			}
		}
		else
		{
			Player.SendMessage(LanguageMgr.GetTranslation("UserBuyItemHandler.FailByPermission"));
		}
		return true;
	}
}
