using System;

namespace Game.Server.MoneyTree.Handle;

internal class MoneyTreeHandleAttbute : Attribute
{
	public byte Code { get; private set; }

	public MoneyTreeHandleAttbute(byte code)
	{
		Code = code;
	}
}
