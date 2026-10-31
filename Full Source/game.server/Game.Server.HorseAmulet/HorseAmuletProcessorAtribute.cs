using System;

namespace Game.Server.HorseAmulet;

public class HorseAmuletProcessorAtribute : Attribute
{
	private byte _code;

	private string _descript;

	public byte Code => _code;

	public string Description => _descript;

	public HorseAmuletProcessorAtribute(byte code, string description)
	{
		_code = code;
		_descript = description;
	}
}
