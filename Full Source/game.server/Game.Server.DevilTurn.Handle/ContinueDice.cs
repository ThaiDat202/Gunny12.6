using System;
using System.Linq;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.DevilTurn.Handle;

[DevilTurnHandle(17)]
public class ContinueDice : IDevilTurnCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int boxId = packet.ReadInt();
		packet.ReadBoolean();
		int value = new Random().Next(2, 12);
		Sys_Users_ListBox sys_Users_ListBox = player.PlayerCharacter.ListBoxs.Where((Sys_Users_ListBox p) => p.BoxId == boxId).FirstOrDefault();
		player.RemoveMoney(sys_Users_ListBox.needmoney);
		DevilTurnMgr.CheckDiceResult(player, sys_Users_ListBox, value);
		return 0;
	}
}
