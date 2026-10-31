using System;
using System.Collections.Generic;
using Game.Base.Packets;
using Game.Server.Managers;

namespace Game.Server.Packets.Client;

[PacketHandler(321, "客户端日记")]
public class GetPlayerSpecialPropertyHandler : IPacketHandler
{
	//__onGetPlayerRspecial packet: 321 PVE
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		GSPacketIn gSPacketIn = new GSPacketIn(321);
		Dictionary<int, int> dungeonCount = client.Player.Extra.GetDungeonCount();
		gSPacketIn.WriteInt(num);
		switch (num)
		{
			case 1:
				gSPacketIn.WriteInt(client.Player.PlayerCharacter.ticketNum);
				gSPacketIn.WriteInt((!BeautyVoteMgr.CanJoin(client.Player.PlayerId)) ? 1 : 0);
				break;
			case 2:
				gSPacketIn.WriteInt(3);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(client.Player.Extra.Info.coupleNum);
				gSPacketIn.WriteInt(1);
				gSPacketIn.WriteInt(client.Player.Extra.Info.dungeonNum);
				gSPacketIn.WriteInt(2);
				gSPacketIn.WriteInt(client.Player.Extra.Info.propsNum);
				break;

			case 3:
				gSPacketIn.WriteInt(dungeonCount.Count);
				foreach (KeyValuePair<int, int> item in dungeonCount)
				{
					gSPacketIn.WriteInt(item.Key);
					gSPacketIn.WriteInt(item.Value);
				}
				break;
			case 6:
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
			case 12:
			case 13:
			case 40:
				gSPacketIn.WriteInt(dungeonCount.Count);
				foreach (KeyValuePair<int, int> item6 in dungeonCount)
				{
					gSPacketIn.WriteInt(item6.Key);
					gSPacketIn.WriteInt(item6.Value);
				}
				break;
			default:
				Console.WriteLine("GETPLAYERSPECIAL: {0}", num);
				break;
		}
		client.Out.SendTCP(gSPacketIn);
		return 0;
	}
}
