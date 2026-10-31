using System;

namespace Game.Server.CardNewSystem;

public class CardNewSystemProcessorAtribute : Attribute
{
	private byte _code;

	private string _descript;

	public byte Code => _code;

	public string Description => _descript;

	public CardNewSystemProcessorAtribute(byte code, string description)
	{
		_code = code;
		_descript = description;
	}
}
