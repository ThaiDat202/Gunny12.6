using System;
using Bussiness;
using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(370, "添加好友")]
public class DDTKingGradeHandler : IPacketHandler
{
	public static Random random = new Random();

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		byte b = packet.ReadByte();
		byte b2 = 0;
		switch (b)
		{
		case 1:
		{
			GSPacketIn gSPacketIn = new GSPacketIn(370, client.Player.PlayerCharacter.ID);
			gSPacketIn.WriteByte(1);
			b2 = packet.ReadByte();
			if (client.Player.KingGrade.GradeUp(b2 - 1))
			{
				gSPacketIn.WriteBoolean(val: true);
			}
			else
			{
				gSPacketIn.WriteBoolean(val: false);
			}
			client.Player.SendTCP(gSPacketIn);
			break;
		}
		case 2:
		{
			bool flag = packet.ReadBoolean();
			b2 = packet.ReadByte();
			int value = (flag ? GameProperties.MaxLevelAllResetCost : GameProperties.MaxLevelResetCost);
			if (client.Player.MoneyDirect(value))
			{
				client.Player.KingGrade.Reset(flag, b2 - 1);
			}
			break;
		}
		case 3:
			client.Player.KingGrade.UpdateKingInfo();
			break;
		}
		return 0;
	}
}
