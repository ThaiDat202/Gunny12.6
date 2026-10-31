using System;

namespace Game.Server.Academy.Handle;

internal class AcademyHandleAttribute : Attribute
{
	public byte Code { get; private set; }

	public AcademyHandleAttribute(byte code)
	{
		Code = code;
	}
}
