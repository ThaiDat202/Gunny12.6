using System.Collections.Generic;
using Game.Base.Packets;
using Game.Server.HotSpringRooms;

namespace Game.Server.Packets.Client;

[PacketHandler(187, "礼堂数据")]
public class HotSpringRoomListHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		List<HotSpringRoom> allRooms = HotSpringRoomsMgr.GetAllRooms();
		GSPacketIn gSPacketIn = new GSPacketIn(197);
		gSPacketIn.WriteInt(allRooms.Count);
		foreach (HotSpringRoom item in allRooms)
		{
			gSPacketIn.WriteInt(item.RoomNumber);
			gSPacketIn.WriteInt(item.RoomId);
			gSPacketIn.WriteString(item.RoomName);
			gSPacketIn.WriteString(item.RoomPassword);
			gSPacketIn.WriteInt(item.EffectiveTime);
			gSPacketIn.WriteInt(item.CurrCount);
			gSPacketIn.WriteInt(client.Player.PlayerId);
			gSPacketIn.WriteString(client.Player.PlayerCharacter.NickName);
			gSPacketIn.WriteDateTime(item.StartTime);
			gSPacketIn.WriteString(item.RoomIntroduction);
			gSPacketIn.WriteInt(item.RoomType);
			gSPacketIn.WriteInt(item.MaxCount);
		}
		client.Player.SendTCP(gSPacketIn);
		return 0;
	}
}
