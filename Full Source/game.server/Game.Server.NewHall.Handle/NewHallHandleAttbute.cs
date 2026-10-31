using System;

namespace Game.Server.NewHall.Handle;

internal class NewHallHandleAttbute : Attribute
{
	public byte Code { get; private set; }

	public NewHallHandleAttbute(byte code)
	{
		Code = code;
	}
}
