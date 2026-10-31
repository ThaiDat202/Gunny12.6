using System;

namespace Game.Server.HomeSystem.Handle;

internal class HomeSystemHandleAttbute : Attribute
{
	public byte Code { get; private set; }

	public HomeSystemHandleAttbute(byte code)
	{
		Code = code;
	}
}
