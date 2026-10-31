using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeAdorn;

public abstract class AbstractHomeAdornProcessor : IHomeAdornProcessor
{
	public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
	{
	}
}
