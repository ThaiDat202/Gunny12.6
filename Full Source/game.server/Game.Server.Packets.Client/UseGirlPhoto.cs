using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(307, "场景用户离开")]
public class UseGirlPhoto : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		bool isShow = packet.ReadBoolean();
		client.Player.PlayerCharacter.IsShow = isShow;
		return 0;
	}
}
