using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Academy.Handle;

public interface IAcademyCommandHandler
{
	int CommandHandler(GamePlayer Player, GSPacketIn packet);
}
