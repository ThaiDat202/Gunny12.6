using System;

namespace Game.Server.Consortia.Handle;

internal class ConsortiaHandleAttbute : Attribute
{
	public byte Code { get; private set; }

	public ConsortiaHandleAttbute(byte code)
	{
		Code = code;
	}
}
