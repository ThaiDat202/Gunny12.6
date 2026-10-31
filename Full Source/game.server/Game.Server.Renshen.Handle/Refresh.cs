using System;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Renshen.Handle;

[RenshenHandleAttbute(1)]
public class Refresh : IRenshenCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(298);
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteBoolean(val: true);
		gSPacketIn.WriteDateTime(DateTime.Now.AddHours(-7.0));
		gSPacketIn.WriteInt(10);
		gSPacketIn.WriteDateTime(DateTime.Now);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteDateTime(DateTime.Now);
		gSPacketIn.WriteBoolean(val: true);
		Player.SendTCP(gSPacketIn);
		return false;
	}
}
