using System;

namespace Game.Server.Labyrinth;

public class LabyrinthProcessorAtribute : Attribute
{
	private byte _code;

	private string _descript;

	public byte Code => _code;

	public string Description => _descript;

	public LabyrinthProcessorAtribute(byte code, string description)
	{
		_code = code;
		_descript = description;
	}
}
