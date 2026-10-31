using System;
using Game.Base;
using Game.Base.Packets;
using Game.Server.Packets.Package;

namespace Game.Server.Packets.Client;

[PacketHandler(411, "user ac action")]
public class StockHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		eStockPackageType eStockPackageType = (eStockPackageType)packet.ReadByte();
		Console.WriteLine(eStockPackageType);
		switch (eStockPackageType)
		{
		case eStockPackageType.UPDATE_USER_CMD:
		{
			byte b = packet.ReadByte();
			Console.WriteLine(b + "---UPDATE_USER_CMD");
			byte b2 = b;
			byte b3 = b2;
			if (b3 == 1)
			{
				GSPacketIn gSPacketIn3 = new GSPacketIn(411);
				gSPacketIn3.WriteByte(1);
				gSPacketIn3.WriteInt(1);
				gSPacketIn3.WriteInt(1001);
				gSPacketIn3.WriteInt(623);
				gSPacketIn3.WriteInt(23);
				gSPacketIn3.WriteInt(10);
				gSPacketIn3.WriteInt(600);
				gSPacketIn3.WriteInt(600);
				gSPacketIn3.WriteInt(100);
				client.SendTCP(gSPacketIn3);
			}
			break;
		}
		case eStockPackageType.STOCK_NEWS:
		{
			GSPacketIn gSPacketIn2 = new GSPacketIn(411);
			gSPacketIn2.WriteByte(4);
			gSPacketIn2.WriteByte(1);
			gSPacketIn2.WriteShort(1001);
			gSPacketIn2.WriteByte(2);
			gSPacketIn2.WriteByte(30);
			gSPacketIn2.WriteLong(Marshal.GetTimeStampMilliseconds(DateTime.Parse("2017-09-13 16:10:00.000")));
			gSPacketIn2.WriteByte(48);
			gSPacketIn2.WriteLong(Marshal.GetTimeStampMilliseconds(DateTime.Parse("2017-09-13 16:20:00.000")));
			client.SendTCP(gSPacketIn2);
			break;
		}
		case eStockPackageType.STOCK_SPECIFIC:
		{
			int val = packet.ReadInt();
			byte val2 = packet.ReadByte();
			GSPacketIn gSPacketIn = new GSPacketIn(411);
			gSPacketIn.WriteByte(3);
			gSPacketIn.Parameter2 = 4;
			gSPacketIn.WriteInt(val);
			gSPacketIn.WriteByte(val2);
			gSPacketIn.WriteLong(Marshal.GetTimeStampMilliseconds(DateTime.Parse("2017-09-03 16:00:00.000")));
			gSPacketIn.WriteInt(70);
			gSPacketIn.WriteInt(610);
			gSPacketIn.WriteInt(580);
			gSPacketIn.WriteInt(520);
			gSPacketIn.WriteInt(660);
			client.SendTCP(gSPacketIn);
			break;
		}
		}
		return 1;
	}
}
