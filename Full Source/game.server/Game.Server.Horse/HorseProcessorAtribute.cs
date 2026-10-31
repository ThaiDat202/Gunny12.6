using System;

namespace Game.Server.Horse;

public class HorseProcessorAtribute : Attribute
{
	private byte _code;

	private string _descript;

	public byte Code => _code;

	public string Description => _descript;

	public HorseProcessorAtribute(byte code, string description)
	{
		_code = code;
		_descript = description;
	}
}
