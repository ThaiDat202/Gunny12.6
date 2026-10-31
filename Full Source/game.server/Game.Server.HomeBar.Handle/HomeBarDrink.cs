using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.HomeBar.Handle;

[HomeBarHandleAttbute(2)]
public class HomeBarDrink : IHomeBarCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		UserHomeInfo data = Player.Home.Data;
		if (data != null)
		{
			if (data.drinkCount > 0)
			{
				if (Player.Home.isDrink())
				{
					Player.SendMessage(LanguageMgr.GetTranslation("HomeBarDrink.CDTime"));
				}
				else
				{
					data.drinkCount--;
					data.cdEndTime = DateTime.Now.AddMinutes(30.0);
					Player.AddHomeBarEnergy(GameProperties.HomeBarDrinkPower);
				}
			}
			else
			{
				Player.SendMessage(LanguageMgr.GetTranslation("HomeBarDrink.NotEnoughtDrink"));
			}
		}
		else
		{
			Player.SendMessage(LanguageMgr.GetTranslation("HomeBarInfo.LoadDataFail"));
		}
		return true;
	}
}
