using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Labyrinth;

public interface ILabyrinthProcessor
{
	void OnGameData(GamePlayer player, GSPacketIn packet);
}
