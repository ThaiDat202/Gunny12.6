using System;

namespace Game.Server.FlowerGiving.Handle;

internal class FlowerGivingHandleAttbute : Attribute
{
	public byte Code { get; private set; }

	public FlowerGivingHandleAttbute(byte code)
	{
		Code = code;
	}
}
