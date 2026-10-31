using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Team.Handle;

[TeamHandle(21)]
public class UseRoomBorden : ITeamCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		bool flag = packet.ReadBoolean();
		int itemId = packet.ReadInt();
		ItemInfo itemByItemID = player.RoomBordenBag.GetItemByItemID(itemId);
		if (itemByItemID != null)
		{
			if (flag && itemByItemID.IsValidItem())
			{
				player.Extra.Info.curcentRoomBordenTemplateId = itemByItemID.TemplateID;
				itemByItemID.IsUsed = true;
			}
			else
			{
				player.Extra.Info.curcentRoomBordenTemplateId = 0;
			}
			TeamMgr.Out.SendUseRoomBorden(player, itemByItemID);
			player.RoomBordenBag.UpdateItem(itemByItemID);
			if (player.CurrentRoom != null)
			{
				GSPacketIn pkg = TeamMgr.Out.SendRoomBordenChange(player);
				player.CurrentRoom.SendToAll(pkg, player);
			}
		}
		return 0;
	}
}
