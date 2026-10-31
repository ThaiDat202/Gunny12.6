using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Renshen.Handle;

[RenshenHandleAttbute(9)]
public class DataInterActive : IRenshenCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		GSPacketIn gSPacketIn = new GSPacketIn(298);
		gSPacketIn.WriteByte(9);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteInt(1);
		Player.SendTCP(gSPacketIn);
		return false;
	}
}
