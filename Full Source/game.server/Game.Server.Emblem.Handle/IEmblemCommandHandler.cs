using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Emblem.Handle;

public interface IEmblemCommandHandler
{
	int CommandHandler(GamePlayer Player, GSPacketIn packet);
}
