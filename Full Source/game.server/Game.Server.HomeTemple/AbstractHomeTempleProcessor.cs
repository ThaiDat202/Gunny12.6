using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeTemple;

public abstract class AbstractHomeTempleProcessor : IHomeTempleProcessor
{
	public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
	{
	}
}
