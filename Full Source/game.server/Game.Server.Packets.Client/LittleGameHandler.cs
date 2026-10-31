using System;
using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(166, "场景用户离开")]
public class LittleGameHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		byte b = packet.ReadByte();
		int iD = client.Player.PlayerCharacter.ID;
		GSPacketIn gSPacketIn = new GSPacketIn(166, iD);
		switch (b)
		{
		case 2:
			gSPacketIn.WriteByte(2);
			gSPacketIn.WriteInt(1);
			gSPacketIn.WriteInt(1);
			gSPacketIn.WriteString("bogu1,bogu2,bogu3,bogu4,bogu5,bogu6,bogu7,bogu8");
			gSPacketIn.WriteString("2001");
			client.Player.SendTCP(gSPacketIn);
			break;
		case 33:
			gSPacketIn.WriteByte(33);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(1);
			gSPacketIn.WriteInt(1);
			gSPacketIn.WriteInt(2039);
			gSPacketIn.WriteInt(123);
			client.Player.SendTCP(gSPacketIn);
			break;
		case 3:
		{
			int num = 1;
			gSPacketIn.WriteByte(3);
			gSPacketIn.WriteInt(num);
			for (int i = 0; i < num; i++)
			{
				gSPacketIn.WriteInt(i + 1);
				gSPacketIn.WriteInt(339);
				gSPacketIn.WriteInt(1197);
				if (i == 0)
				{
					gSPacketIn.WriteInt(1);
					gSPacketIn.WriteInt(client.Player.PlayerCharacter.ID);
					gSPacketIn.WriteInt(client.Player.PlayerCharacter.Grade);
					gSPacketIn.WriteInt(client.Player.PlayerCharacter.Repute);
					gSPacketIn.WriteString(client.Player.PlayerCharacter.NickName);
					gSPacketIn.WriteByte(client.Player.PlayerCharacter.typeVIP);
					gSPacketIn.WriteInt(client.Player.PlayerCharacter.VIPLevel);
					gSPacketIn.WriteBoolean(client.Player.PlayerCharacter.Sex);
					gSPacketIn.WriteString(client.Player.PlayerCharacter.Style);
					gSPacketIn.WriteString(client.Player.PlayerCharacter.Colors);
					gSPacketIn.WriteString(client.Player.PlayerCharacter.Skin);
					gSPacketIn.WriteInt(client.Player.PlayerCharacter.Hide);
					gSPacketIn.WriteInt(client.Player.PlayerCharacter.FightPower);
					gSPacketIn.WriteInt(client.Player.PlayerCharacter.Win);
					gSPacketIn.WriteInt(client.Player.PlayerCharacter.Total);
				}
				else
				{
					gSPacketIn.WriteInt(2);
					gSPacketIn.WriteString("Cờ hó");
					gSPacketIn.WriteString("bogu7");
				}
				gSPacketIn.WriteBoolean(val: false);
				int num2 = 1;
				int j = 0;
				gSPacketIn.WriteInt(num2);
				for (; j < num2; j++)
				{
					gSPacketIn.WriteString("livingInhale");
					gSPacketIn.WriteInt(j + 1);
					gSPacketIn.WriteString("stand");
					gSPacketIn.WriteString("1");
					gSPacketIn.WriteInt(1);
					gSPacketIn.WriteInt(2039);
					gSPacketIn.WriteInt(123);
				}
			}
			client.Player.SendTCP(gSPacketIn);
			break;
		}
		default:
			Console.WriteLine("PackageOut_cmd: {0}, PackageInt_cmd: {1}", (LittleGamePackageOut)b, (LittleGamePackageIn)b);
			break;
		}
		return 0;
	}
}
