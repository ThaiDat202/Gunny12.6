using System;

namespace Game.Server.Zodiac.Handle;

internal class ZodiacHandleAttbute : Attribute
{
	public byte Code { get; private set; }

	public ZodiacHandleAttbute(byte code)
	{
		Code = code;
	}
}
