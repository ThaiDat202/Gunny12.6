using System;
using Bussiness;
using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(342, "打开物品")]
public class GetActivePveInfoHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int[] array = new int[7] { 19, 20, 21, 22, 0, 0, 0 };
		if (GameProperties.VERSION >= 8200)
		{
			array[4] = 23;
			array[5] = 24;
		}
		if (GameProperties.VERSION >= 8400)
		{
			array[6] = 29;
		}
		int[] array2 = new int[7] { 15, 16, 17, 18, 0, 0, 0 };
		if (GameProperties.VERSION >= 8200)
		{
			array2[4] = 25;
			array2[5] = 26;
		}
		if (GameProperties.VERSION >= 8400)
		{
			array2[6] = 28;
		}
		GSPacketIn gSPacketIn = new GSPacketIn(342);
		int[] array3 = array;
		foreach (int num in array3)
		{
			gSPacketIn.WriteDateTime(DateTime.Now.AddDays(-1.0));
			gSPacketIn.WriteDateTime(DateTime.Now.AddDays(1.0));
			gSPacketIn.WriteBoolean(val: false);
		}
		int[] array4 = array2;
		foreach (int num2 in array4)
		{
			gSPacketIn.WriteDateTime(DateTime.Now.AddDays(-1.0));
			gSPacketIn.WriteDateTime(DateTime.Now.AddDays(1.0));
			gSPacketIn.WriteBoolean(val: false);
		}
		client.SendTCP(gSPacketIn);
		return 1;
	}
}
