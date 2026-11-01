using System.Collections.Generic;
using System.Linq;
using Bussiness;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.Elf.Handle;

[ElfHandle(4)]
public class AddGrowth : IElfCommandHandler
{
	public static int ItemId = 12246;

	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int elfId = packet.ReadInt();
		int num = packet.ReadInt();
		//if (num != 6)
		//{
		//	player.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("Elf.AddGrowth.SomethingGoneWrong"));
		//	return 0;
		//}
		List<int> list = new List<int>();
		for (int i = 0; i < num; i++)
		{
			list.Add(packet.ReadInt());
		}
		Sys_Users_Elf sys_Users_Elf = player.PlayerCharacter.ElfInfo.FirstOrDefault((Sys_Users_Elf e) => e.ElfID == elfId);
		if (sys_Users_Elf == null)
		{
			player.SendMessage("Elf.Fight.NotFound");
			return 0;
		}
		if (!player.RemoveTemplate(ItemId, list.Sum()))
		{
			player.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("Elf.AddGrowth.NoItem"));
			return 0;
		}
		ElfMgr.AddGrowthAttributes(sys_Users_Elf, list, ItemId);
		sys_Users_Elf.Quality = ElfMgr.CalculateQuality(sys_Users_Elf);
		ElfMgr.ResetElfProps(sys_Users_Elf);
		ElfMgr.CalculateGradeProps(sys_Users_Elf, sys_Users_Elf.Grade - 1);
		ElfMgr.Out.SendUpdateProps(player, sys_Users_Elf);
		return 0;
	}
}
