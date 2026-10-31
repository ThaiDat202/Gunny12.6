using System;

namespace Game.Server.MemoryGame;

public class MemoryGameProcessorAtribute : Attribute
{
	private byte _code;

	private string _descript;

	public byte Code => _code;

	public string Description => _descript;

	public MemoryGameProcessorAtribute(byte code, string description)
	{
		_code = code;
		_descript = description;
	}
}
