using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Labyrinth.Handle;

[LabyrinthHandle(5)]
public class StopSweep : ILabyrinthCommandHandler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		Player.PlayerCharacter.LabyrinthInfo.isPlayGame = false;
		if (Player.PlayerCharacter.LabyrinthInfo.sweepTotalTimeRemaining > 0)
		{
			Player.PlayerCharacter.LabyrinthInfo.isSweep = false;
		}
		LabyrinthMgr.StopSweep(Player);
		return 0;
	}
}
