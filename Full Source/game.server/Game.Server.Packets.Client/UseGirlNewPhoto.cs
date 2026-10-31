using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(339, "场景用户离开")]
public class UseGirlNewPhoto : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		bool flag = packet.ReadBoolean();
		client.Player.PlayerCharacter.IsShow = false;
		client.Player.PlayerCharacter.ImagePath = "";
		GSPacketIn gSPacketIn = new GSPacketIn(339);
		gSPacketIn.WriteBoolean(val: true);
		client.Out.SendTCP(gSPacketIn);
		return 0;
	}
}
