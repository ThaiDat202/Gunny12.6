using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(37, "用户与用户之间的聊天")]
public class UserPrivateChatHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(37);
		gSPacketIn.WriteInt(packet.ReadInt());
		gSPacketIn.WriteString(packet.ReadString());
		packet.ReadString();
		gSPacketIn.WriteString(client.Player.PlayerCharacter.NickName);
		gSPacketIn.WriteInt(client.Player.PlayerCharacter.ID);
		gSPacketIn.WriteString(packet.ReadString());
		gSPacketIn.WriteBoolean(packet.ReadBoolean());
		GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
		return 1;
	}
}
