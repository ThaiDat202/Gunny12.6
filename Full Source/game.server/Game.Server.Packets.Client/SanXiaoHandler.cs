using System;
using Bussiness.Protocol;
using Game.Base.Packets;
using Game.Server.MemoryGame;

namespace Game.Server.Packets.Client;

[PacketHandler(329, "大喇叭")]
public class SanXiaoHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = packet.Clone();
		gSPacketIn.ClearOffset();
		byte b = gSPacketIn.ReadByte();
		if (Enum.IsDefined(typeof(SanXiaoPackageType), (int)b))
		{
			if (client.Player.SanXiao == null)
			{
				return 0;
			}
			client.Player.SanXiao.ProcessData(client.Player, packet);
			return 1;
		}
		if (Enum.IsDefined(typeof(GodCardRaisePackageType), (int)b))
		{
			if (client.Player.GodCardRaise == null)
			{
				return 0;
			}
			client.Player.GodCardRaise.ProcessData(client.Player, packet);
			return 1;
		}
		if (Enum.IsDefined(typeof(MemoryGamePackageType), (int)b))
		{
			if (client.Player.MemoryGame == null)
			{
				return 0;
			}
			client.Player.MemoryGame.ProcessData(client.Player, packet);
			return 1;
		}
		if (Enum.IsDefined(typeof(QiYuanPackageType), (int)b))
		{
			if (client.Player.DDTQiYuan == null)
			{
				return 0;
			}
			client.Player.DDTQiYuan.ProcessData(client.Player, packet);
			return 1;
		}
		Console.WriteLine("SanXiaoHandler Do not support handler {0}", b);
		return 0;
	}
}
