using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.DevilTurn.Handle;

public interface IDevilTurnCommandHandler
{
	int CommandHandler(GamePlayer Player, GSPacketIn packet);
}
