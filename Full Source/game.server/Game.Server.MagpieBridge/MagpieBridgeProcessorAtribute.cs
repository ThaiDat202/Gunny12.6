using System;

namespace Game.Server.MagpieBridge;

public class MagpieBridgeProcessorAtribute : Attribute
{
	private byte _code;

	private string _descript;

	public byte Code => _code;

	public string Description => _descript;

	public MagpieBridgeProcessorAtribute(byte code, string description)
	{
		_code = code;
		_descript = description;
	}
}
