using System;

namespace Game.Server.HotSpringRooms.Handle;

internal class HotSpringRoomsHandleAttbute : Attribute
{
	public byte Code { get; private set; }

	public HotSpringRoomsHandleAttbute(byte code)
	{
		Code = code;
	}
}
