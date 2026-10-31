using System;
using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(403, "二级密码")]
public class BaglockedHandle : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadByte();
		GSPacketIn gSPacketIn = new GSPacketIn(403, client.Player.PlayerCharacter.ID);
		int num2 = num;
		int num3 = num2;
		if (num3 == 5)
		{
			gSPacketIn.WriteByte(5);
			gSPacketIn.WriteBoolean(val: true);
			client.Out.SendTCP(gSPacketIn);
		}
		else
		{
			BaglockedPackageType baglockedPackageType = (BaglockedPackageType)num;
			Console.WriteLine("BaglockedPackageType." + baglockedPackageType);
		}
		return 0;
	}
}
