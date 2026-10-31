using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeBar;

public abstract class AbstractHomeBarProcessor : IHomeBarProcessor
{
	public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
	{
	}
}
