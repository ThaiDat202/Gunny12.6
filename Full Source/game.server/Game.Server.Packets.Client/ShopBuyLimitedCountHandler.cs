using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(288, "商品数量限制")]
public class ShopBuyLimitedCountHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(288, client.Player.PlayerId);
		gSPacketIn.WriteInt(0);
		client.Player.SendTCP(gSPacketIn);
		return 0;
	}
}
