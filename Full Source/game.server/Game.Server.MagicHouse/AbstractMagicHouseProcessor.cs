using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.MagicHouse;

public abstract class AbstractMagicHouseProcessor : IMagicHouseProcessor
{
	public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
	{
	}
}
