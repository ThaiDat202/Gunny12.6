using System;

namespace Game.Server.DragonBoat.Handle;

internal class DragonBoatHandleAttbute : Attribute
{
	public byte Code { get; private set; }

	public DragonBoatHandleAttbute(byte code)
	{
		Code = code;
	}
}
