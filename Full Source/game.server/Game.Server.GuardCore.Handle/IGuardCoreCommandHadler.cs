using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.GuardCore.Handle;

public interface IGuardCoreCommandHadler
{
	int CommandHandler(GamePlayer Player, GSPacketIn packet);
}
