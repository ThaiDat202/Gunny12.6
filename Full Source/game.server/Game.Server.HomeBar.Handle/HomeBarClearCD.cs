using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.HomeBar.Handle;

[HomeBarHandleAttbute(5)]
public class HomeBarClearCD : IHomeBarCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		bool flag = packet.ReadBoolean();
		UserHomeInfo data = Player.Home.Data;
		if (data != null)
		{
			if (Player.MoneyDirect(GameProperties.HomeBarCleanCDPrice))
			{
				data.cdEndTime = DateTime.Now.AddMinutes(-30.0);
				Player.Home.SendHomeBarInfo(0);
			}
		}
		else
		{
			Player.SendMessage(LanguageMgr.GetTranslation("HomeBarInfo.LoadDataFail"));
		}
		return true;
	}
}
