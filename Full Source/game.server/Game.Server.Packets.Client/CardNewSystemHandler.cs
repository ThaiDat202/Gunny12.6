using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(648, "新版卡牌系统")]
public class CardNewSystemHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		client.Player.CardNewSystemHandler.ProcessData(client.Player, packet);
		return 1;
	}
}
