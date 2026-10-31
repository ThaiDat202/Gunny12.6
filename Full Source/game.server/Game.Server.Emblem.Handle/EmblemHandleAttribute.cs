using System;

namespace Game.Server.Emblem.Handle;

internal class EmblemHandleAttribute : Attribute
{
	public byte Code { get; private set; }

	public EmblemHandleAttribute(byte code)
	{
		Code = code;
	}
}
