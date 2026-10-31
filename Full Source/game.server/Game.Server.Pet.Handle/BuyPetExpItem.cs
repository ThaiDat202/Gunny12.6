using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Pet.Handle;

[PetHandleAttbute(19)]
public class BuyPetExpItem : IPetCommandHadler
{
	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		bool flag = packet.ReadBoolean();
		if (player.PlayerCharacter.HasBagPassword && player.PlayerCharacter.IsLocked)
		{
			player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return false;
		}
		UserFarmInfo currentFarm = player.Farm.CurrentFarm;
		int buyExpRemainNum = currentFarm.buyExpRemainNum;
		PetExpItemPriceInfo petExpItemPriceInfo = PetMgr.FindPetExpItemPrice(RealMoney(buyExpRemainNum));
		if (petExpItemPriceInfo == null || buyExpRemainNum < 1)
		{
			return false;
		}
		bool flag2 = false;
		int money = petExpItemPriceInfo.Money;
		if (flag)
		{
			if (player.MoneyDirect(money))
			{
				flag2 = true;
			}
		}
		else if (money <= player.PlayerCharacter.DDTMoney)
		{
			player.RemoveGiftToken(money);
			flag2 = true;
		}
		if (!flag2)
		{
			if (GameProperties.IsDDTMoneyActive)
			{
				player.SendMessage(LanguageMgr.GetTranslation("PetHandler.Msg13"));
			}
			else
			{
				player.SendMessage(LanguageMgr.GetTranslation("PetHandler.Msg14"));
			}
			return false;
		}
		ItemTemplateInfo goods = ItemMgr.FindItemTemplate(334102);
		ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, petExpItemPriceInfo.ItemCount, 102);
		itemInfo.IsBinds = true;
		player.AddTemplate(itemInfo, itemInfo.Template.BagType, petExpItemPriceInfo.ItemCount, eGameView.RouletteTypeGet);
		currentFarm.buyExpRemainNum--;
		GSPacketIn gSPacketIn = new GSPacketIn(68);
		gSPacketIn.WriteByte(19);
		gSPacketIn.WriteInt(currentFarm.buyExpRemainNum);
		player.SendTCP(gSPacketIn);
		player.Farm.UpdateFarm(currentFarm);
		return false;
	}

	private int RealMoney(int timebuy)
	{
		return timebuy switch
		{
			19 => 2, 
			18 => 3, 
			17 => 4, 
			16 => 5, 
			15 => 6, 
			14 => 7, 
			13 => 8, 
			12 => 9, 
			11 => 10, 
			10 => 11, 
			9 => 12, 
			8 => 13, 
			7 => 14, 
			6 => 15, 
			5 => 16, 
			4 => 17, 
			3 => 18, 
			2 => 19, 
			1 => 20, 
			_ => 1, 
		};
	}
}
