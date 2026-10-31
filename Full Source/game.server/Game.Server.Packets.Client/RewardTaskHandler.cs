using System;
using Game.Base.Packets;
using Game.Server.Packets.Package;

namespace Game.Server.Packets.Client;

[PacketHandler(629, "场景用户离开")]
public class RewardTaskHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		RewardTaskPackageType rewardTaskPackageType = (RewardTaskPackageType)packet.ReadByte();
		Console.WriteLine("rewardtask: " + rewardTaskPackageType);
		RewardTaskPackageType rewardTaskPackageType2 = rewardTaskPackageType;
		RewardTaskPackageType rewardTaskPackageType3 = rewardTaskPackageType2;
		if (rewardTaskPackageType3 != RewardTaskPackageType.UEST_OFFER_INFO)
		{
		}
		GSPacketIn gSPacketIn = new GSPacketIn(629);
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteInt(900);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteInt(2);
		gSPacketIn.WriteInt(3);
		gSPacketIn.WriteInt(1);
		client.Out.SendTCP(gSPacketIn);
		return 1;
	}
}
