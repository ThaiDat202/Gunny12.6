using System;

namespace Game.Server.HomeBar.Handle;

internal class HomeBarHandleAttbute : Attribute
{
	public byte Code { get; private set; }

	public HomeBarHandleAttbute(byte code)
	{
		Code = code;
	}
}
