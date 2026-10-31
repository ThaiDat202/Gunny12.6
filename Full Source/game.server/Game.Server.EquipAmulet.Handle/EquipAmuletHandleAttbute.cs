using System;

namespace Game.Server.EquipAmulet.Handle;

internal class EquipAmuletHandleAttbute : Attribute
{
	public byte Code { get; private set; }

	public EquipAmuletHandleAttbute(byte code)
	{
		Code = code;
	}
}
