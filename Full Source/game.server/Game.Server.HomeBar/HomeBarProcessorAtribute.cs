using System;

namespace Game.Server.HomeBar;

public class HomeBarProcessorAtribute : Attribute
{
	private byte _code;

	private string _descript;

	public byte Code => _code;

	public string Description => _descript;

	public HomeBarProcessorAtribute(byte code, string description)
	{
		_code = code;
		_descript = description;
	}
}
