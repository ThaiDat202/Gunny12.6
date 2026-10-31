using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(69, "用户列表")]
public class SceneUsersListHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(69, client.Player.PlayerCharacter.ID);
		byte b = packet.ReadByte();
		byte b2 = packet.ReadByte();
		GamePlayer[] allPlayersNoRoom = WorldMgr.GetAllPlayersNoRoom();
		int num = allPlayersNoRoom.Length;
		byte b3 = ((num > b2) ? b2 : ((byte)num));
		gSPacketIn.WriteByte(b3);
		for (int i = b * b2; i < b * b2 + b3; i++)
		{
			PlayerInfo playerCharacter = allPlayersNoRoom[i % num].PlayerCharacter;
			gSPacketIn.WriteInt(playerCharacter.ID);
			gSPacketIn.WriteString(playerCharacter.NickName);
			gSPacketIn.WriteByte(playerCharacter.typeVIP);
			gSPacketIn.WriteInt(playerCharacter.VIPLevel);
			gSPacketIn.WriteBoolean(playerCharacter.Sex);
			gSPacketIn.WriteInt(playerCharacter.Grade);
			gSPacketIn.WriteInt(playerCharacter.ConsortiaID);
			gSPacketIn.WriteString(playerCharacter.ConsortiaName);
			gSPacketIn.WriteInt(playerCharacter.Offer);
			gSPacketIn.WriteInt(playerCharacter.Win);
			gSPacketIn.WriteInt(playerCharacter.Total);
			gSPacketIn.WriteInt(playerCharacter.Escape);
			gSPacketIn.WriteInt(playerCharacter.Repute);
			gSPacketIn.WriteInt(playerCharacter.FightPower);
			gSPacketIn.WriteBoolean(playerCharacter.IsOldPlayer);
			gSPacketIn.WriteBoolean(playerCharacter.isAttest);
		}
		client.Out.SendTCP(gSPacketIn);
		return 0;
	}
}
