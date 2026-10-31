using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Elf.Handle;

public interface IElfCommandHandler
{
	int CommandHandler(GamePlayer Player, GSPacketIn packet);
}
