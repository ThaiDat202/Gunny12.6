using System;

namespace Game.Server.GuardCore;

public class GuardCoreProcessorAtribute : Attribute
{
	private byte _code;

	private string _descript;

	public byte Code => _code;

	public string Description => _descript;

	public GuardCoreProcessorAtribute(byte code, string description)
	{
		_code = code;
		_descript = description;
	}
}
