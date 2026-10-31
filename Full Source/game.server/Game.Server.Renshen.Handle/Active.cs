using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Renshen.Handle;

[RenshenHandleAttbute(2)]
public class Active : IRenshenCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		GSPacketIn gSPacketIn = new GSPacketIn(298);
		gSPacketIn.WriteByte(2);
		gSPacketIn.WriteBoolean(val: true);
		Player.SendTCP(gSPacketIn);
		return false;
	}
}
