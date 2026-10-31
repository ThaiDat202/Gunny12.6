using System;

namespace Game.Server.HomeFight;

public class HomeFightProcessorAtribute : Attribute
{
	private byte _code;

	private string _descript;

	public byte Code => _code;

	public string Description => _descript;

	public HomeFightProcessorAtribute(byte code, string description)
	{
		_code = code;
		_descript = description;
	}
}
