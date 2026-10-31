using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(201, "礼堂数据")]
public class HotSpringRoomEnterViewHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		if (client.Player.CurrentHotSpringRoom != null)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(198);
			PlayerInfo playerCharacter = client.Player.PlayerCharacter;
			gSPacketIn.WriteInt(playerCharacter.ID);
			gSPacketIn.WriteInt(playerCharacter.Grade);
			gSPacketIn.WriteInt(playerCharacter.Hide);
			gSPacketIn.WriteInt(playerCharacter.Repute);
			gSPacketIn.WriteString(playerCharacter.NickName);
			gSPacketIn.WriteByte(playerCharacter.typeVIP);
			gSPacketIn.WriteInt(playerCharacter.VIPLevel);
			gSPacketIn.WriteBoolean(playerCharacter.Sex);
			gSPacketIn.WriteString(playerCharacter.Style);
			gSPacketIn.WriteString(playerCharacter.Colors);
			gSPacketIn.WriteString(playerCharacter.Skin);
			gSPacketIn.WriteInt(client.Player.X);
			gSPacketIn.WriteInt(client.Player.Y);
			gSPacketIn.WriteInt(playerCharacter.FightPower);
			gSPacketIn.WriteInt(playerCharacter.Win);
			gSPacketIn.WriteInt(playerCharacter.Total);
			gSPacketIn.WriteInt(client.Player.Direction);
			gSPacketIn.WriteBoolean(val: false);
			gSPacketIn.WriteInt(playerCharacter.eggNum);
			gSPacketIn.WriteInt(playerCharacter.flowerNum);
			gSPacketIn.WriteBoolean(playerCharacter.isClicked);
			client.Player.CurrentHotSpringRoom.SendToALL(gSPacketIn);
			client.Player.CurrentHotSpringRoom.UpdateAllPlayersInCurrRoom(client.Player);
		}
		return 0;
	}
}
