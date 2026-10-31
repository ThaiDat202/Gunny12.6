using System;
using System.Linq;
using Bussiness;
using Bussiness.Managers;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.DevilTurn.Handle;

[DevilTurnHandle(19)]
public class GetBox : IDevilTurnCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int boxID = packet.ReadInt();
		Sys_Users_ListBox sys_Users_ListBox = player.PlayerCharacter.ListBoxs.Where((Sys_Users_ListBox p) => p.BoxId == boxID).FirstOrDefault();
		ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(GameProperties.DevilTreasureTemplateID), sys_Users_ListBox.subtractMax, 101);
		itemInfo.IsBinds = true;
		player.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("DevilTurnHandle.GetBox.Success"));
		player.SendItemToMail(itemInfo, LanguageMgr.GetTranslation("DevilTurnHandle.GetBox.Content"), LanguageMgr.GetTranslation("DevilTurnHandle.GetBox.Title"), eMailType.Active);
		sys_Users_ListBox.OpenDate = DateTime.Now.AddHours(-3.0);
		DevilTurnMgr.Out.SendUpdateBoxList(player);
		return 0;
	}
}
