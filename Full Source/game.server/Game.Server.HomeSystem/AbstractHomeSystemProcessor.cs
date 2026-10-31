using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeSystem;

public abstract class AbstractHomeSystemProcessor : IHomeSystemProcessor
{
	public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
	{
	}
}
