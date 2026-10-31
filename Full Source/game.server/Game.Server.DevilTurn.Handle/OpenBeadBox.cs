using System;
using System.Linq;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.DevilTurn.Handle;

[DevilTurnHandle(13)]
public class OpenBeadBox : IDevilTurnCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		Random random = new Random();
		int boxID = packet.ReadInt();
		Sys_Users_ListBox sys_Users_ListBox = player.PlayerCharacter.ListBoxs.Where((Sys_Users_ListBox p) => p.BoxId == boxID).FirstOrDefault();
		int minValue = 0;
		int maxValue = 0;
		switch (sys_Users_ListBox.BoxId)
		{
		case 12711:
			minValue = 3;
			maxValue = 5;
			break;
		case 12712:
			minValue = 5;
			maxValue = 7;
			break;
		case 12713:
			minValue = 7;
			maxValue = 9;
			break;
		}
		if (sys_Users_ListBox.OpenDate < DateTime.Now)
		{
			sys_Users_ListBox.OpenDate = DateTime.Now.AddHours(4.0);
			sys_Users_ListBox.state = 1;
			sys_Users_ListBox.boxIndex = 1;
			sys_Users_ListBox.subtractMin = 1;
			sys_Users_ListBox.needmoney = DevilTurnMgr.GetNeedMoneyToDice(boxID, 1);
			sys_Users_ListBox.subtractMax = random.Next(minValue, maxValue);
			sys_Users_ListBox.min = random.Next(1, 8);
			while (sys_Users_ListBox.max < sys_Users_ListBox.min + 1 || sys_Users_ListBox.max > sys_Users_ListBox.min + 3)
			{
				sys_Users_ListBox.max = random.Next(3, 12);
			}
		}
		DevilTurnMgr.Out.SendUpdateDiceBoxView(player, sys_Users_ListBox);
		DevilTurnMgr.Out.SendUpdateInfo(player);
		return 0;
	}
}
