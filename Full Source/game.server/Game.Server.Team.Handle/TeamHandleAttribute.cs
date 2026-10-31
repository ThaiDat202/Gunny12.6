using System;

namespace Game.Server.Team.Handle;

internal class TeamHandleAttribute : Attribute
{
	public byte Code { get; private set; }

	public TeamHandleAttribute(byte code)
	{
		Code = code;
	}
}
