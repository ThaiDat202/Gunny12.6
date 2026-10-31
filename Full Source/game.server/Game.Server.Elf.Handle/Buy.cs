using System;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Statics;
using SqlDataProvider.Data;

namespace Game.Server.Elf.Handle;

[ElfHandle(11)]
public class Buy : IElfCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int num2 = packet.ReadInt();
		bool flag = packet.ReadBoolean();
		MoneyType type = MoneyType.Money;
		int num3 = 0;
		int num4 = -1;
		switch (num)
		{
		case 12241:
			if (num2 == 10)
			{
				num4 = int.Parse(GameProperties.ElfBuyItemMoneys.Split('|')[0].Split(',')[1]);
				type = ((!flag) ? MoneyType.Money : MoneyType.DDTMoney);
				num3 = 10;
			}
			else
			{
				num4 = int.Parse(GameProperties.ElfBuyItemMoneys.Split('|')[0].Split(',')[0]);
				type = ((!flag) ? MoneyType.Money : MoneyType.DDTMoney);
				num3 = 1;
			}
			break;
		case 12242:
			if (num2 == 10)
			{
				num4 = int.Parse(GameProperties.ElfBuyItemMoneys.Split('|')[1].Split(',')[1]);
				type = ((!flag) ? MoneyType.Money : MoneyType.DDTMoney);
				num3 = 50;
			}
			else
			{
				num4 = int.Parse(GameProperties.ElfBuyItemMoneys.Split('|')[1].Split(',')[0]);
				type = ((!flag) ? MoneyType.Money : MoneyType.DDTMoney);
				num3 = 5;
			}
			break;
		}
		if (num4 == -1)
		{
			player.SendMessage("Elf.Buy.Failed");
			return 0;
		}
		if (!player.MoneyDirect(type, num4))
		{
			return 0;
		}
		ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(num);
		if (itemTemplateInfo == null)
		{
			throw new NullReferenceException($"template not found with templateId {num}");
		}
		ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, num2, 105);
		itemInfo.Count = num2;
		itemInfo.IsBinds = true;
		player.Extra.Info.Coin += num3;
		player.AddTemplate(itemInfo);
		player.SendMessage(LanguageMgr.GetTranslation("Elf.Buy.Success", itemTemplateInfo.Name, num2));
		ElfMgr.Out.SendElfCoin(player);
		return 0;
	}
}
