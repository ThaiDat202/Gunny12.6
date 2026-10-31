using System;

namespace Game.Server.ConsortiaTask.Handle;

internal class ConsortiaTaskHandleAttribute : Attribute
{
	public byte Code { get; private set; }

	public ConsortiaTaskHandleAttribute(byte code)
	{
		Code = code;
	}
}
