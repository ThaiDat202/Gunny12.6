using System.Linq;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using MoreLinq;

namespace Game.Server.Elf.Handle;

[ElfHandle(6)]
public class Fight : IElfCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int elfId = packet.ReadInt();
		bool isFight = packet.ReadBoolean();
		player.PlayerCharacter.ElfInfo.Where((Sys_Users_Elf e) => e.IsFight).ForEach(delegate(Sys_Users_Elf e)
		{
			e.IsFight = false;
			ElfMgr.Out.SendUpdateElfFight(player, e);
		});
		Sys_Users_Elf sys_Users_Elf = player.PlayerCharacter.ElfInfo.FirstOrDefault((Sys_Users_Elf e) => e.ElfID == elfId);
		if (sys_Users_Elf == null)
		{
			player.SendMessage("Elf.Fight.Failed");
			return 0;
		}
		sys_Users_Elf.IsFight = isFight;
		ElfMgr.Out.SendUpdateElfFight(player, sys_Users_Elf);
		player.EquipBag.UpdatePlayerProperties();
		return 0;
	}
}
