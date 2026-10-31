using System;

namespace Game.Server.BombKing.Handle;

internal class BombKingHandleAttbute : Attribute
{
	public byte Code { get; private set; }

	public BombKingHandleAttbute(byte code)
	{
		Code = code;
	}
}
