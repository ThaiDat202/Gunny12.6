using System;

namespace Game.Server.Elf.Handle;

internal class ElfHandleAttribute : Attribute
{
	public byte Code { get; private set; }

	public ElfHandleAttribute(byte code)
	{
		Code = code;
	}
}
