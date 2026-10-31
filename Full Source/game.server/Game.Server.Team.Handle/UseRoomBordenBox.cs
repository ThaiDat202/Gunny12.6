using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Team.Handle;

[TeamHandle(23)]
public class UseRoomBordenBox : ITeamCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int slot = packet.ReadInt();
		if (player.RoomBordenBag.GetEmptyCount() == 0)
		{
			player.SendMessage(LanguageMgr.GetTranslation("TeamHandler.UseRoomBordenBox.FullBag"));
			return 1;
		}
		ItemInfo itemAt = player.PropBag.GetItemAt(slot);
		if (itemAt != null && itemAt.Template.CategoryID == 43)
		{
			player.PropBag.RemoveCountFromStack(itemAt, 1);
			SpecialItemBoxInfo specialValue = new SpecialItemBoxInfo();
			List<ItemInfo> list = new List<ItemInfo>();
			ItemBoxMgr.CreateItemBox(itemAt.TemplateID, list, ref specialValue);
			foreach (ItemInfo item in list)
			{
				int validDate = item.ValidDate;
				ItemInfo itemByTemplateID = player.RoomBordenBag.GetItemByTemplateID(item.TemplateID);
				if (itemByTemplateID != null && itemByTemplateID.IsValidItem())
				{
					validDate = (itemByTemplateID.ValidDate = ((itemByTemplateID.ValidDate != 0 && validDate != 0) ? (itemByTemplateID.ValidDate + validDate) : 0));
					player.RoomBordenBag.UpdateItem(itemByTemplateID);
				}
				else
				{
					player.AddTemplate(item);
				}
				player.SendMessage(LanguageMgr.GetTranslation("TeamHandler.UseRoomBordenBox.Success", item.Template.Name));
			}
		}
		return 0;
	}
}
