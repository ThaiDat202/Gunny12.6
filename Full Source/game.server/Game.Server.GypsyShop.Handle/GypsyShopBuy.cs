using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.GypsyShop.Handle;

[GypsyShopHandleAttbute(3)]
public class GypsyShopBuy : IGypsyShopCommandHadler
{
	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int iD = packet.ReadInt();
		bool flag = packet.ReadBoolean();
		GypsyItemDataInfo mysteryShopByID = player.Actives.GetMysteryShopByID(iD);
		if (mysteryShopByID != null)
		{
			if (mysteryShopByID.CanBuy == 1)
			{
				bool flag2 = false;
				if (mysteryShopByID.Unit == 1)
				{
					if (flag)
					{
						player.RemoveGiftToken(mysteryShopByID.Price);
					}
					else
					{
						player.RemoveMoney(mysteryShopByID.Price);
					}
					flag2 = true;
				}
				else if (mysteryShopByID.Unit == 2 && player.PlayerCharacter.myHonor >= mysteryShopByID.Price)
				{
					flag2 = true;
				}
				if (flag2)
				{
					ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(mysteryShopByID.InfoID);
					if (itemTemplateInfo != null)
					{
						ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 105);
						itemInfo.IsBinds = true;
						itemInfo.ValidDate = mysteryShopByID.Num;
						player.AddTemplate(itemInfo);
					}
					player.SendMessage(LanguageMgr.GetTranslation("GypsyShopBuy.Success"));
					player.Actives.UpdateMysteryShopByID(iD);
					if (mysteryShopByID.Unit == 2)
					{
						player.RemoveHonor(mysteryShopByID.Price);
					}
				}
				if (mysteryShopByID.Unit == 2 && !flag2)
				{
					player.SendMessage(LanguageMgr.GetTranslation("GypsyShopBuy.OutHornor"));
				}
				GSPacketIn gSPacketIn = new GSPacketIn(278, player.PlayerCharacter.ID);
				gSPacketIn.WriteByte(3);
				gSPacketIn.WriteInt(mysteryShopByID.GypsyID);
				gSPacketIn.WriteBoolean(flag2);
				if (flag2)
				{
					gSPacketIn.WriteInt(mysteryShopByID.CanBuy);
				}
				player.Out.SendTCP(gSPacketIn);
			}
			else
			{
				player.SendMessage(LanguageMgr.GetTranslation("GypsyShopBuy.OutOfDate"));
			}
		}
		else
		{
			player.SendMessage(LanguageMgr.GetTranslation("GypsyShopBuy.Fail"));
		}
		return false;
	}
}
