using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Team.Handle;

[TeamHandle(22)]
public class SaleRoomBorden : ITeamCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int itemId = packet.ReadInt();
		packet.ReadInt();
		ItemInfo itemByItemID = player.RoomBordenBag.GetItemByItemID(itemId);
		if (itemByItemID != null)
		{
			player.AddGold(itemByItemID.Template.ReclaimValue);
			player.RoomBordenBag.RemoveItem(itemByItemID);
		}
		return 0;
	}
}
