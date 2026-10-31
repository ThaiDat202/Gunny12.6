using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Labyrinth.Handle;

[LabyrinthHandle(2)]
public class RequestUpdate : ILabyrinthCommandHandler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		Player.ResetNewDayLabyrinth();
		Player.CalculateSweepTime();
		Player.Out.SendLabyrinthUpdataInfo(Player);
		return 0;
	}
}
