using System;
using System.Collections.Generic;
using System.Linq;
using Bussiness;
using EntityDatabase.PlayerModels;
using EntityDatabase.ServerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.DevilTurn.Handle;

[DevilTurnHandle(12)]
public class BeadConversion : IDevilTurnCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int boxId = packet.ReadInt();
		DevilTreasSarahToBoxList devilTreasSarahToBoxList = DevilTurnMgr.ListDevilTreasSarahToBoxList.Where((DevilTreasSarahToBoxList p) => p.ID == boxId).FirstOrDefault();
		List<string> list = devilTreasSarahToBoxList.Exchange.Split(',').ToList();
		if (player.PlayerCharacter.DevilTurn.beadCount1 < Convert.ToInt32(list[0]) || player.PlayerCharacter.DevilTurn.beadCount2 < Convert.ToInt32(list[1]) || player.PlayerCharacter.DevilTurn.beadCount3 < Convert.ToInt32(list[2]) || player.PlayerCharacter.DevilTurn.beadCount4 < Convert.ToInt32(list[3]) || player.PlayerCharacter.DevilTurn.beadCount5 < Convert.ToInt32(list[4]))
		{
			player.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("DevilTurnHandle.BeadConversion.Fail.Count"));
			return 0;
		}
		player.PlayerCharacter.DevilTurn.beadCount1 -= Convert.ToInt32(list[0]);
		player.PlayerCharacter.DevilTurn.beadCount2 -= Convert.ToInt32(list[1]);
		player.PlayerCharacter.DevilTurn.beadCount3 -= Convert.ToInt32(list[2]);
		player.PlayerCharacter.DevilTurn.beadCount4 -= Convert.ToInt32(list[3]);
		player.PlayerCharacter.DevilTurn.beadCount5 -= Convert.ToInt32(list[4]);
		AddBox(player, boxId);
		player.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("DevilTurnHandle.BeadConversion.Success"));
		return 0;
	}

	private void AddBox(GamePlayer player, int BoxId)
	{
		player.PlayerCharacter.ListBoxs.Where((Sys_Users_ListBox p) => p.BoxId == BoxId).FirstOrDefault().Count++;
		DevilTurnMgr.Out.SendUpdateInfo(player);
		DevilTurnMgr.Out.SendUpdateBoxList(player);
	}
}
