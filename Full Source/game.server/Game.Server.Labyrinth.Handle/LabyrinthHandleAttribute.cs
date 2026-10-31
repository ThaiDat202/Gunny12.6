using System;

namespace Game.Server.Labyrinth.Handle;

internal class LabyrinthHandleAttribute : Attribute
{
	public byte Code { get; private set; }

	public LabyrinthHandleAttribute(byte code)
	{
		Code = code;
	}
}
