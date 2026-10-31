using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(528, "user ac action")]
public class ShopPersonalLimitHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int val = packet.ReadInt();
		GSPacketIn gSPacketIn = new GSPacketIn(528);
		gSPacketIn.WriteInt(val);
		gSPacketIn.WriteInt(ShopMgr.FindShopbyShopID(120).Count);
		foreach (ShopItemInfo item in ShopMgr.FindShopbyShopID(120))
		{
			gSPacketIn.WriteInt(item.ID);
			gSPacketIn.WriteInt(0);
		}
		client.SendTCP(gSPacketIn);
		return 1;
	}
}
