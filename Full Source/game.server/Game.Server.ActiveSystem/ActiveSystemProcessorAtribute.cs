using System;

namespace Game.Server.ActiveSystem;

public class ActiveSystemProcessorAtribute : Attribute
{
	private byte _code;

	private string _descript;

	public byte Code => _code;

	public string Description => _descript;

	public ActiveSystemProcessorAtribute(byte code, string description)
	{
		_code = code;
		_descript = description;
	}
}
