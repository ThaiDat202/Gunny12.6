using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Elf.Handle;

[ElfHandle(16)]
public class HidePet : IElfCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		bool isHideElf = packet.ReadBoolean();
		player.PlayerCharacter.IsHideElf = isHideElf;
		return 0;
	}
}
