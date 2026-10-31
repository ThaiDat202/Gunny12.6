using System;

namespace Game.Server.AnotherDimension.Handle;

internal class AnotherDimensionHandleAttbute : Attribute
{
	public byte Code { get; private set; }

	public AnotherDimensionHandleAttbute(byte code)
	{
		Code = code;
	}
}
