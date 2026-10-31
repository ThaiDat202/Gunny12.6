using System;

namespace Game.Server.HomeAdorn;

public class HomeAdornProcessorAtribute : Attribute
{
	private byte _code;

	private string _descript;

	public byte Code => _code;

	public string Description => _descript;

	public HomeAdornProcessorAtribute(byte code, string description)
	{
		_code = code;
		_descript = description;
	}
}
