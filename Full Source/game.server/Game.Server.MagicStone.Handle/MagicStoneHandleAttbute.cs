using System;

namespace Game.Server.MagicStone.Handle;

internal class MagicStoneHandleAttbute : Attribute
{
	public byte Code { get; private set; }

	public MagicStoneHandleAttbute(byte code)
	{
		Code = code;
	}
}
