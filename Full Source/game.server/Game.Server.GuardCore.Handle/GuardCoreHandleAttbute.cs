using System;

namespace Game.Server.GuardCore.Handle;

internal class GuardCoreHandleAttbute : Attribute
{
	public byte Code { get; private set; }

	public GuardCoreHandleAttbute(byte code)
	{
		Code = code;
	}
}
