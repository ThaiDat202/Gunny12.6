using System;

namespace Game.Server.MagicStone;

public class MagicStoneProcessorAtribute : Attribute
{
	private byte _code;

	private string _descript;

	public byte Code => _code;

	public string Description => _descript;

	public MagicStoneProcessorAtribute(byte code, string description)
	{
		_code = code;
		_descript = description;
	}
}
