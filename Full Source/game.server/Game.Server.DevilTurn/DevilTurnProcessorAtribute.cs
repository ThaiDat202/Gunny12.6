using System;

namespace Game.Server.DevilTurn;

public class DevilTurnProcessorAtribute : Attribute
{
	private byte _code;

	private string _descript;

	public byte Code => _code;

	public string Description => _descript;

	public DevilTurnProcessorAtribute(byte code, string description)
	{
		_code = code;
		_descript = description;
	}
}
