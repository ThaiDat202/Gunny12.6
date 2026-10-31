using System;
using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(155, "场景用户离开")]
public class EverydayActivePointHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		byte b = packet.ReadByte();
		int iD = client.Player.PlayerCharacter.ID;
		byte b2 = b;
		byte b3 = b2;
		if (b3 == 8)
		{
			client.Player.Out.SendExpBlessedData(iD);
		}
		else
		{
			ActivityPackageType activityPackageType = (ActivityPackageType)b;
			Console.WriteLine("ActivityPackageType." + activityPackageType);
		}
		return 0;
	}
}
