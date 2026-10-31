using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.AnotherDimension;

public abstract class AbstractAnotherDimensionProcessor : IAnotherDimensionProcessor
{
	public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
	{
	}
}
