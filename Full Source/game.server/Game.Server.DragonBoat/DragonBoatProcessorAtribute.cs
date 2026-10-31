using System;

namespace Game.Server.DragonBoat;

public class DragonBoatProcessorAtribute : Attribute
{
	private byte _code;

	private string _descript;

	public byte Code => _code;

	public string Description => _descript;

	public DragonBoatProcessorAtribute(byte code, string description)
	{
		_code = code;
		_descript = description;
	}
}
