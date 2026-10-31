using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.ConsortiaTask.Handle;

public interface IConsortiaTaskCommandHandler
{
	int CommandHandler(GamePlayer Player, GSPacketIn packet);
}
