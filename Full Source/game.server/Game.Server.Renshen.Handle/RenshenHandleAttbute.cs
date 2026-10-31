using System;

namespace Game.Server.Renshen.Handle;

internal class RenshenHandleAttbute : Attribute
{
	public byte Code { get; private set; }

	public RenshenHandleAttbute(byte code)
	{
		Code = code;
	}
}
