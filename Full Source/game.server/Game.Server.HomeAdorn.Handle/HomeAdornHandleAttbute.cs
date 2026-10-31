using System;

namespace Game.Server.HomeAdorn.Handle;

internal class HomeAdornHandleAttbute : Attribute
{
	public byte Code { get; private set; }

	public HomeAdornHandleAttbute(byte code)
	{
		Code = code;
	}
}
