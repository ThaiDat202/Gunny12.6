using System;

namespace Game.Server.Horse.Handle;

internal class HorseHandleAttbute : Attribute
{
	public byte Code { get; private set; }

	public HorseHandleAttbute(byte code)
	{
		Code = code;
	}
}
