using System;
using System.Linq;
using Bussiness;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.DevilTurn.Handle;

[DevilTurnHandle(28)]
public class BoxAbandon : IDevilTurnCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int boxID = packet.ReadInt();
		Sys_Users_ListBox sys_Users_ListBox = player.PlayerCharacter.ListBoxs.Where((Sys_Users_ListBox p) => p.BoxId == boxID).FirstOrDefault();
		sys_Users_ListBox.OpenDate = DateTime.Now.AddHours(-3.0);
		DevilTurnMgr.Out.SendUpdateBoxList(player);
		player.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("DevilTurnHandle.BoxAbandon.Success"));
		return 0;
	}
}
