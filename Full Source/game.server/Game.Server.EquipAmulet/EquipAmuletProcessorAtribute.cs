using System;

namespace Game.Server.EquipAmulet;

public class EquipAmuletProcessorAtribute : Attribute
{
	private byte _code;

	private string _descript;

	public byte Code => _code;

	public string Description => _descript;

	public EquipAmuletProcessorAtribute(byte code, string description)
	{
		_code = code;
		_descript = description;
	}
}
