using System;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.HomeSystem.Handle;

[HomeSystemHandleAttbute(19)]
public class FishEnd : IHomeSystemCommandHadler
{
	private Random rand = new Random();

	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.Home.IsFishing)
		{
			Player.SendMessage(LanguageMgr.GetTranslation("FishEnd.FishTip"));
		}
		else
		{
			int num = Player.Home.FishList.Count - 1;
			if (num < 0)
			{
				Player.SendMessage(LanguageMgr.GetTranslation("FishEnd.LoadDbFail"));
			}
			else
			{
				HomeFishInfo homeFishInfo = HomeFishMgr.FindHomeFish(1);
				if (rand.Next(100) < 45)
				{
					homeFishInfo = Player.Home.FishList[rand.Next(num)];
				}
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
					gSPacketIn.WriteByte(19);
					gSPacketIn.WriteInt(homeFishInfo.ID);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(Player.Home.Data.fishScore);
					gSPacketIn.WriteInt(Player.Home.Data.dayScore);
					gSPacketIn.WriteInt(Player.Home.Data.maxScore);
					Player.SendTCP(gSPacketIn);
					Player.Home.IsFishing = true;
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
