using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.MemoryGame;

public abstract class AbstractMemoryGameProcessor : IMemoryGameProcessor
{
	public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
	{
	}
}
