using System;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.MoneyTree.Handle;

[MoneyTreeHandleAttbute(4)]
public class Pick : IMoneyTreeCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		MoneyTreeInfo moneyTreeInfo = Player.Home.GetMoneyTreeInfo(num);
		if (moneyTreeInfo == null)
		{
			Player.SendMessage(LanguageMgr.GetTranslation("SpeedUp.GetFail"));
		}
		else
		{
			string[] array = GameProperties.WealthTreeConfigList.Split('|');
			string[] array2 = array[num].Split(',');
			int num2 = int.Parse(array2[1]);
			if (moneyTreeInfo.CanDrop())
			{
				Player.SendMessage(LanguageMgr.GetTranslation("Pick.TimeFail"));
			}
			else
			{
				ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(moneyTreeInfo.ItemID);
				if (itemTemplateInfo == null)
				{
					Player.SendMessage(LanguageMgr.GetTranslation("SpeedUp.GetFail"));
				}
				else
				{
					int num3 = int.Parse(array2[2]);
					moneyTreeInfo.PickDaily++;
					if (num == 2)
					{
						GSPacketIn gSPacketIn = new GSPacketIn(296);
						gSPacketIn.WriteByte(2);
						Player.SendTCP(gSPacketIn);
						Player.Home.Data.NumRedPkgRemain++;
					}
					else
					{
						ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 105);
						itemInfo.IsBinds = true;
						Player.AddTemplate(itemInfo);
						Player.SendMessage(LanguageMgr.GetTranslation("Pick.Success", itemTemplateInfo.Name));
					}
					moneyTreeInfo.ItemID = int.Parse(array2[0]);
					moneyTreeInfo.timeGrown = DateTime.Now.AddMinutes(num2);
					moneyTreeInfo.priceSpeedUp = num3 + num3 * moneyTreeInfo.PickDaily;
					Player.Home.SendMoneyTreeInfo();
				}
			}
		}
		return false;
	}
}
