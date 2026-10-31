using System;

namespace Game.Server.HorseAmulet.Handle;

internal class HorseAmuletHandleAttbute : Attribute
{
	public byte Code { get; private set; }

	public HorseAmuletHandleAttbute(byte code)
	{
		Code = code;
	}
}
