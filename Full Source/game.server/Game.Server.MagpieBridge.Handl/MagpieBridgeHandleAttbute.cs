using System;

namespace Game.Server.MagpieBridge.Handle;

internal class MagpieBridgeHandleAttbute : Attribute
{
	public byte Code { get; private set; }

	public MagpieBridgeHandleAttbute(byte code)
	{
		Code = code;
	}
}
