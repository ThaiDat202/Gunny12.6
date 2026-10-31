using System;

namespace Game.Server.NewHall;

public class NewHallProcessorAtribute : Attribute
{
	private byte _code;

	private string _descript;

	public byte Code => _code;

	public string Description => _descript;

	public NewHallProcessorAtribute(byte code, string description)
	{
		_code = code;
		_descript = description;
	}
}
