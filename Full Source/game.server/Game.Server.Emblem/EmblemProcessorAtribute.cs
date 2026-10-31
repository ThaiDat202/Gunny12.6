using System;

namespace Game.Server.Emblem;

public class EmblemProcessorAtribute : Attribute
{
	private byte _code;

	private string _descript;

	public byte Code => _code;

	public string Description => _descript;

	public EmblemProcessorAtribute(byte code, string description)
	{
		_code = code;
		_descript = description;
	}
}
