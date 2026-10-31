using Game.Base.Packets;
using Game.Logic;

namespace Game.Server.Packets.Client;

[PacketHandler(91, "游戏数据")]
public class GameDataHandler : IPacketHandler
{
	private static readonly string BASE64_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		if (client.Player.CurrentRoom != null)
		{
			if (UseBackupID(client.Player.CurrentRoom.RoomType))
			{
				packet.Parameter1 = client.Player.TempGameId;
			}
			else
			{
				packet.Parameter1 = client.Player.GamePlayerId;
			}
			if (client.Player.CurrentRoom.Game == null && client.Player.LastTakeCardItem != null)
			{
				GSPacketIn gSPacketIn = packet.Clone();
				gSPacketIn.ClearOffset();
				if (gSPacketIn.ReadByte() == 98)
				{
					int num = gSPacketIn.ReadByte();
					if (num > 9)
					{
						num = 8;
					}
					GSPacketIn gSPacketIn2 = new GSPacketIn(91, gSPacketIn.Parameter1);
					gSPacketIn2.Parameter1 = gSPacketIn.Parameter1;
					gSPacketIn2.WriteByte(98);
					gSPacketIn2.WriteBoolean(val: true);
					gSPacketIn2.WriteByte((byte)num);
					gSPacketIn2.WriteInt(client.Player.LastTakeCardItem.TemplateID);
					gSPacketIn2.WriteInt(client.Player.LastTakeCardItem.Count);
					gSPacketIn2.WriteBoolean(val: false);
					client.Player.SendTCP(gSPacketIn2);
				}
				return 0;
			}
			client.Player.CurrentRoom.ProcessData(packet);
		}
		return 0;
	}

	private bool UseBackupID(eRoomType roomType)
	{
		switch (roomType)
		{
		case eRoomType.Match:
		case eRoomType.FightGround:
		case eRoomType.SingleBattle:
		case eRoomType.PolarRegion:
		case eRoomType.SurvivalMode:
			return true;
		default:
			return false;
		}
	}

	public string EncodeBuffer(byte[] buffer)
	{
		string text = "";
		byte[] array = new byte[4];
		for (int i = 0; i < buffer.Length; i += 4)
		{
			if (buffer[i] <= 0)
			{
				continue;
			}
			byte[] array2 = new byte[3];
			int num = 0;
			for (num = 0; num < buffer.Length; num++)
			{
				if (num < 3)
				{
					if (num + i > buffer.Length)
					{
						break;
					}
					array2[num] = buffer[num + i];
				}
			}
			array[0] = (byte)((array2[0] & 0xFC) >> 2);
			array[1] = (byte)(((array2[0] & 3) << 4) | (array2[1] >> 4));
			array[2] = (byte)(((array2[1] & 0xF) << 2) | (array2[2] >> 6));
			array[3] = (byte)(array2[2] & 0x3Fu);
			for (int j = array2.Length; j < 3; j++)
			{
				array[j + 1] = 64;
			}
			for (int k = 0; k < array.Length; k++)
			{
				text += BASE64_CHARS.Substring(array[k], 1);
			}
		}
		return text + "=";
	}
}
