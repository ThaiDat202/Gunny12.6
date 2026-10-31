using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Labyrinth;

public class AbstractLabyrinthProcessor : ILabyrinthProcessor
{
	public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
	{
	}
}
