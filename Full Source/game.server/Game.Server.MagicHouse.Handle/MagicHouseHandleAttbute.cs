using System;

namespace Game.Server.MagicHouse.Handle;

internal class MagicHouseHandleAttbute : Attribute
{
	public byte Code { get; private set; }

	public MagicHouseHandleAttbute(byte code)
	{
		Code = code;
	}
}
