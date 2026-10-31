using Bussiness;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Elf.Handle;

[ElfHandle(10)]
public class ActivateElf : IElfCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int bagType = packet.ReadInt();
		int place = packet.ReadInt();
		ItemInfo itemAt = player.GetItemAt((eBageType)bagType, place);
		if (itemAt == null)
		{
			player.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("Elf.ActivateElf.Failed"));
			return 0;
		}
		ItemTemplateInfo template = itemAt.Template;
		if (player.RemoveTemplate(template, 1))
		{
			Sys_Users_Elf sys_Users_Elf = ElfMgr.AddElf(player, template);
			player.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("Elf.ActivateElf.Success"));
		}
		return 0;
	}
}
