using System;

namespace Game.Server.HomeTemple.Handle;

internal class HomeTempleHandleAttbute : Attribute
{
	public byte Code { get; private set; }

	public HomeTempleHandleAttbute(byte code)
	{
		Code = code;
	}
}
