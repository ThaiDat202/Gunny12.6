using System;
using System.Collections.Generic;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(110, "场景用户离开")]
public class SeparateActivityHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadByte();
		GSPacketIn gSPacketIn = new GSPacketIn(110, client.Player.PlayerCharacter.ID);
		switch (num)
		{
		case 17:
			gSPacketIn.WriteByte(17);
			gSPacketIn.WriteInt(4);
			client.Player.SendTCP(gSPacketIn);
			break;
		case 2:
		{
			SpecialItemBoxInfo specialValue = new SpecialItemBoxInfo();
			List<ItemInfo> list = new List<ItemInfo>();
			packet.ReadBoolean();
			int num2 = packet.ReadInt();
			for (int i = 0; i < num2; i++)
			{
				List<ItemInfo> list2 = new List<ItemInfo>();
				ItemBoxMgr.CreateItemBox(12241, list2, ref specialValue);
				foreach (ItemInfo item in list2)
				{
					list.Add(item);
				}
			}
			gSPacketIn.WriteByte(2);
			gSPacketIn.WriteBoolean(val: true);
			gSPacketIn.WriteInt(list.Count);
			foreach (ItemInfo item2 in list)
			{
				gSPacketIn.WriteInt(item2.TemplateID);
				gSPacketIn.WriteInt(num2);
				gSPacketIn.WriteString(item2.Template.Name);
				gSPacketIn.WriteInt(item2.Count);
			}
			gSPacketIn.WriteInt(0);
			client.Player.SendTCP(gSPacketIn);
			break;
		}
		default:
		{
			TreasureHuntingType treasureHuntingType = (TreasureHuntingType)num;
			Console.WriteLine("TreasureHuntingType." + treasureHuntingType);
			break;
		}
		}
		return 0;
	}
}
