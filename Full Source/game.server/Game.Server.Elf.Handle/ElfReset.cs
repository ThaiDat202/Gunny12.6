using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Elf.Handle;

[ElfHandle(17)]
public class ElfReset : IElfCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		return 0;
	}
}
