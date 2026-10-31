using System;

namespace Game.Server.BombKing;

public class BombKingProcessorAtribute : Attribute
{
	private byte _code;

	private string _descript;

	public byte Code => _code;

	public string Description => _descript;

	public BombKingProcessorAtribute(byte code, string description)
	{
		_code = code;
		_descript = description;
	}
}
