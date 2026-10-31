using System;

namespace Game.Server.MagicHouse;

public class MagicHouseProcessorAtribute : Attribute
{
	private byte _code;

	private string _descript;

	public byte Code => _code;

	public string Description => _descript;

	public MagicHouseProcessorAtribute(byte code, string description)
	{
		_code = code;
		_descript = description;
	}
}
