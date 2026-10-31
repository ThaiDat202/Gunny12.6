using System;

namespace Game.Server.MemoryGame.Handle;

internal class MemoryGameHandleAttbute : Attribute
{
	public byte Code { get; private set; }

	public MemoryGameHandleAttbute(byte code)
	{
		Code = code;
	}
}
