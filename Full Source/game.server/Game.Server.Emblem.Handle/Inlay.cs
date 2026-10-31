using System.Linq;
using Bussiness;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Emblem.Handle;

[EmblemHandle(6)]
public class Inlay : IEmblemCommandHandler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int id = packet.ReadInt();
		int place = packet.ReadInt();
		int index = packet.ReadInt();
		bool flag = packet.ReadBoolean();
		Sys_Users_Emblem sys_Users_Emblem = Player.EmblemInfos.FirstOrDefault((Sys_Users_Emblem p) => p.ID == id);
		if (sys_Users_Emblem == null)
		{
			Player.SendMessage(LanguageMgr.GetTranslation("Emblem.Inlay.EmblemNotFound"));
			return 0;
		}
		if (flag)
		{
			if (Player.EmblemInfos.Any((Sys_Users_Emblem e) => e.equipIndex == index && e.equipPos == place))
			{
				Player.SendMessage(LanguageMgr.GetTranslation("Emblem.Inlay.EmblemIsExists"));
				return 0;
			}
			sys_Users_Emblem.equipIndex = index;
			sys_Users_Emblem.equipPos = place;
		}
		else
		{
			sys_Users_Emblem.equipIndex = 0;
			sys_Users_Emblem.equipPos = 0;
		}
		EmblemMgr.Out.SendUpdateEmblem(Player, sys_Users_Emblem);
		Player.EquipBag.UpdatePlayerProperties();
		Player.SendMessage(LanguageMgr.GetTranslation("Emblem.Inlay.Success"));
		EmblemMgr.SendProInfoHandler(Player);
		return 0;
	}
}
