using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.HomeSystem.Handle;

[HomeSystemHandleAttbute(21)]
public class FishBuy : IHomeSystemCommandHadler
{
	private Random rand = new Random();

	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.MoneyDirect(100))
		{
			int num = Player.Home.FishList.Count - 1;
			if (num < 0)
			{
				Player.SendMessage(LanguageMgr.GetTranslation("FishEnd.LoadDbFail"));
			}
			else
			{
				HomeFishInfo homeFishInfo = Player.Home.FishList[rand.Next(num)];
				if (homeFishInfo == null)
				{
					Player.SendMessage(LanguageMgr.GetTranslation("FishEnd.LoadDbFail"));
				}
				else
				{
					Player.Home.Data.fishScore += homeFishInfo.Score;
					Player.Home.Data.dayScore += homeFishInfo.Score;
					if (Player.Home.Data.dayScore > Player.Home.Data.maxScore && Player.Home.Data.maxScore < 2147483597)
					{
						Player.Home.Data.maxScore += homeFishInfo.Score;
					}
					GSPacketIn gSPacketIn = new GSPacketIn(285);
					gSPacketIn.WriteByte(21);
					gSPacketIn.WriteInt(homeFishInfo.ID);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(Player.Home.Data.fishScore);
					gSPacketIn.WriteInt(Player.Home.Data.dayScore);
					gSPacketIn.WriteInt(Player.Home.Data.maxScore);
					Player.SendTCP(gSPacketIn);
					if (homeFishInfo.Score >= 40)
					{
						Player.Home.SendBroadcast(homeFishInfo.Name);
					}
				}
			}
		}
		return false;
	}
}
