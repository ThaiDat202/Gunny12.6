using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.DragonBoat;

public abstract class AbstractDragonBoatProcessor : IDragonBoatProcessor
{
	public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
	{
	}
}
