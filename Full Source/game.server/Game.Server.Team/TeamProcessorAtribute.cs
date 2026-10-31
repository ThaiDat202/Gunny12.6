using System;

namespace Game.Server.Team;

public class TeamProcessorAtribute : Attribute
{
	private byte _code;

	private string _descript;

	public byte Code => _code;

	public string Description => _descript;

	public TeamProcessorAtribute(byte code, string description)
	{
		_code = code;
		_descript = description;
	}
}
