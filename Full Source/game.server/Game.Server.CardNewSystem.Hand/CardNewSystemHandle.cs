using System;

namespace Game.Server.CardNewSystem.Handle;

internal class CardNewSystemHandle : Attribute
{
	public byte Code { get; private set; }

	public CardNewSystemHandle(byte code)
	{
		Code = code;
	}
}
