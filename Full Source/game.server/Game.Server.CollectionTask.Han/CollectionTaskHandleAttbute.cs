using System;

namespace Game.Server.CollectionTask.Handle;

internal class CollectionTaskHandleAttbute : Attribute
{
	public byte Code { get; private set; }

	public CollectionTaskHandleAttbute(byte code)
	{
		Code = code;
	}
}
