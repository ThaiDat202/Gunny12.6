using System;

namespace Game.Server.HomeFight.Handle;

internal class HomeFightHandleAttbute : Attribute
{
	public byte Code { get; private set; }

	public HomeFightHandleAttbute(byte code)
	{
		Code = code;
	}
}
