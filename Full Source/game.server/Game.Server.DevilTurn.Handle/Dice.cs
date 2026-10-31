using System;
using System.Linq;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.DevilTurn.Handle;

[DevilTurnHandle(16)]
public class Dice : IDevilTurnCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int boxId = packet.ReadInt();
		int value = new Random().Next(2, 12);
		Sys_Users_ListBox box = player.PlayerCharacter.ListBoxs.Where((Sys_Users_ListBox p) => p.BoxId == boxId).FirstOrDefault();
		DevilTurnMgr.CheckDiceResult(player, box, value);
		return 0;
	}
}
