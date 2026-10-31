using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Statics;

namespace Game.Server.Labyrinth.Handle;

[LabyrinthHandle(4)]
public class QuickSweep : ILabyrinthCommandHandler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (!Player.PlayerCharacter.LabyrinthInfo.isSweep)
		{
			Player.SendMessage(LanguageMgr.GetTranslation("Labyrinth.QuickSweep.NeedSweep"));
			return 0;
		}
		int num = Math.Abs(Player.PlayerCharacter.LabyrinthInfo.sweepTotalTimeRemaining / 60);
		int value = GameProperties.WarriorFamRaidPricePerMin * num;
		if (!Player.Extra.UseKingBless(7))
		{
			if (Player.MoneyDirect(MoneyType.Medal, value))
			{
				LabyrinthMgr.ForceComplete(Player);
			}
		}
		else
		{
			LabyrinthMgr.ForceComplete(Player);
		}
		return 0;
	}
}
