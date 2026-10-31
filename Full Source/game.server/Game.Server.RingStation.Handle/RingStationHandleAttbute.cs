using System;

namespace Game.Server.RingStation.Handle;

internal class RingStationHandleAttbute : Attribute
{
	public byte Code { get; private set; }

	public RingStationHandleAttbute(byte code)
	{
		Code = code;
	}
}
