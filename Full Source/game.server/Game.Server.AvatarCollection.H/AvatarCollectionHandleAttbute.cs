using System;

namespace Game.Server.AvatarCollection.Handle;

internal class AvatarCollectionHandleAttbute : Attribute
{
	public byte Code { get; private set; }

	public AvatarCollectionHandleAttbute(byte code)
	{
		Code = code;
	}
}
