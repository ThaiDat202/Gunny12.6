using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.GuardCore;

public abstract class AbstractGuardCoreProcessor : IGuardCoreProcessor
{
	public virtual void OnGameData(GamePlayer player, GSPacketIn packet)
	{
	}
}
