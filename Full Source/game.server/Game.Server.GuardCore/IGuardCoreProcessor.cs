using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.GuardCore;

public interface IGuardCoreProcessor
{
	void OnGameData(GamePlayer player, GSPacketIn packet);
}
