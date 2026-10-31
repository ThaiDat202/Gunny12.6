using System;

namespace Game.Server.WorshipTheMoon.Handle;

internal class WorshipTheMoonHandleAttbute : Attribute
{
	public byte Code { get; private set; }

	public WorshipTheMoonHandleAttbute(byte code)
	{
		Code = code;
	}
}
