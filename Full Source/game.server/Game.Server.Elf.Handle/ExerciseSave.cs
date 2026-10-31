using System.Linq;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Elf.Handle;

[ElfHandle(13)]
public class ExerciseSave : IElfCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int elfId = packet.ReadInt();
		Sys_Users_Elf sys_Users_Elf = player.PlayerCharacter.ElfInfo.FirstOrDefault((Sys_Users_Elf e) => e.ElfID == elfId);
		if (sys_Users_Elf == null)
		{
			player.SendMessage("Elf.ExerciseSave.NotFound");
			return 0;
		}
		ElfMgr.Out.SendExerciseProps(player, sys_Users_Elf);
		return 0;
	}
}
