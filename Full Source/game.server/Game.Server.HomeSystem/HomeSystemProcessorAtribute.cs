using System;

namespace Game.Server.HomeSystem;

public class HomeSystemProcessorAtribute : Attribute
{
	private byte _code;

	private string _descript;

	public byte Code => _code;

	public string Description => _descript;

	public HomeSystemProcessorAtribute(byte code, string description)
	{
		_code = code;
		_descript = description;
	}
}
