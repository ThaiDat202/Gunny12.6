using System;

namespace Game.Server.FlowerGiving;

public class FlowerGivingProcessorAtribute : Attribute
{
	private byte _code;

	private string _descript;

	public byte Code => _code;

	public string Description => _descript;

	public FlowerGivingProcessorAtribute(byte code, string description)
	{
		_code = code;
		_descript = description;
	}
}
