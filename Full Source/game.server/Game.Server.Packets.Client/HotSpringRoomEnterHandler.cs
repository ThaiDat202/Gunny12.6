using Bussiness;
using Game.Base.Packets;
using Game.Server.HotSpringRooms;

namespace Game.Server.Packets.Client;

[PacketHandler(202, "礼堂数据")]
public class HotSpringRoomEnterHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = 10000;
		int roomId = packet.ReadInt();
		string text = packet.ReadString();
		HotSpringRoom singleRoom = HotSpringRoomsMgr.GetSingleRoom(roomId);
		if (singleRoom == null)
		{
			client.Player.SendMessage(LanguageMgr.GetTranslation("HotSpringEnterConfirmHandler.RoomNotExit"));
		}
		else
		{
			client.Player.X = 300;
			client.Player.Y = 400;
			if (client.Player.PlayerCharacter.Gold < num)
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("HotSpringRoomEnterDataHandler.NotEmoughtGold"));
				return 0;
			}
			if (singleRoom.AddPlayer(client.Player))
			{
				GSPacketIn gSPacketIn = new GSPacketIn(202);
				gSPacketIn.WriteInt(singleRoom.RoomId);
				gSPacketIn.WriteInt(singleRoom.RoomNumber);
				gSPacketIn.WriteString(singleRoom.RoomName);
				gSPacketIn.WriteString(singleRoom.RoomPassword);
				gSPacketIn.WriteInt(singleRoom.EffectiveTime);
				gSPacketIn.WriteInt(singleRoom.CurrCount);
				gSPacketIn.WriteInt(client.Player.PlayerId);
				gSPacketIn.WriteString(client.Player.PlayerCharacter.NickName);
				gSPacketIn.WriteDateTime(singleRoom.StartTime);
				gSPacketIn.WriteString(singleRoom.RoomIntroduction);
				gSPacketIn.WriteInt(singleRoom.RoomType);
				gSPacketIn.WriteInt(singleRoom.MaxCount);
				gSPacketIn.WriteDateTime(client.Player.EnterRoomTime);
				gSPacketIn.WriteInt(client.Player.EffectiveTime);
				if (GameProperties.VERSION >= 8100)
				{
					gSPacketIn.WriteInt(client.Player.Extra.Info.EnergyTotal);
				}
				client.Player.SendTCP(gSPacketIn);
				client.Player.RemoveGold(num);
			}
			else
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("HotSpringEnterConfirmHandler.RoomFull"));
			}
		}
		return 0;
	}
}
