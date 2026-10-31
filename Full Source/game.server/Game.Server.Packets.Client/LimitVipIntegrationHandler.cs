using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(312, "Vip积分商城")]
internal class LimitVipIntegrationHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		client.Player.SendRefreshInfoVipShop();
		return 0;
	}
}
