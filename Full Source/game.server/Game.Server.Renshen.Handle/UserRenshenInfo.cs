using System;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Renshen.Handle;

[RenshenHandleAttbute(12)]
public class UserRenshenInfo : IRenshenCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int val = packet.ReadInt();
		GSPacketIn gSPacketIn = new GSPacketIn(298);
		gSPacketIn.WriteByte(12);
		gSPacketIn.WriteInt(val);
		gSPacketIn.WriteBoolean(val: false);
		gSPacketIn.WriteDateTime(DateTime.Now.AddHours(-7.0));
		gSPacketIn.WriteDateTime(DateTime.Now);
		gSPacketIn.WriteBoolean(val: false);
		Player.SendTCP(gSPacketIn);
		return false;
	}
}
