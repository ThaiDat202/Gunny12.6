using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Labyrinth.Handle;

public interface ILabyrinthCommandHandler
{
	int CommandHandler(GamePlayer Player, GSPacketIn packet);
}
