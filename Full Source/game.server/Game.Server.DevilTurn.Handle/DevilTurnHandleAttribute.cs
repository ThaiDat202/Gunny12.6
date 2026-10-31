using System;

namespace Game.Server.DevilTurn.Handle;

internal class DevilTurnHandleAttribute : Attribute
{
	public byte Code { get; private set; }

	public DevilTurnHandleAttribute(byte code)
	{
		Code = code;
	}
}
