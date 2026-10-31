using System;

namespace Game.Server.Academy;

public class AcademyProcessorAtribute : Attribute
{
	private byte _code;

	private string _descript;

	public byte Code => _code;

	public string Description => _descript;

	public AcademyProcessorAtribute(byte code, string description)
	{
		_code = code;
		_descript = description;
	}
}
