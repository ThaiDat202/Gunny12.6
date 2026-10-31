using System;

namespace Game.Server.AnotherDimension;

public class AnotherDimensionProcessorAtribute : Attribute
{
	private byte _code;

	private string _descript;

	public byte Code => _code;

	public string Description => _descript;

	public AnotherDimensionProcessorAtribute(byte code, string description)
	{
		_code = code;
		_descript = description;
	}
}
