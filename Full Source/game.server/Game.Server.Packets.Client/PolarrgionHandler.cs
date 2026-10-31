using System;
using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(305, "POLARREGION")]
public class PolarrgionHandler : IPacketHandler
{
	public int maxTime
	{
		get
		{
			return 3500;
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(305);
		gSPacketIn.WriteByte(0);
		gSPacketIn.WriteBoolean(val: false);
		client.Out.SendTCP(gSPacketIn);
		return 0;
	}
}
