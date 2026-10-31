using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.WorshipTheMoon.Handle;

[WorshipTheMoonHandleAttbute(2)]
public class WorshipTheMoonFreeCounts : IWorshipTheMoonCommandHadler
{
	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		if (player.Actives.Info.lastEnterWorshiped.Date < DateTime.Now.Date)
		{
			player.Actives.Info.updateFreeCounts = int.Parse(GameProperties.WorshipMoonPriceInfo.Split('|')[0]);
			player.Actives.Info.lastEnterWorshiped = DateTime.Now;
		}
		if (player.Actives.Info.updateWorshipedCounts > 1000)
		{
			player.Actives.Info.updateWorshipedCounts = 0;
		}
		player.Actives.SendUpdateFreeCount();
		return false;
	}
}
