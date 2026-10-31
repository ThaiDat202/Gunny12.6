using System;

namespace Game.Server.RingStation;

public class RingStationProcessorAtribute : Attribute
{
	private byte _code;

	private string _descript;

	public byte Code => _code;

	public string Description => _descript;

	public RingStationProcessorAtribute(byte code, string description)
	{
		_code = code;
		_descript = description;
	}
}
