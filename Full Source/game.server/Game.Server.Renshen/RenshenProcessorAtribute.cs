using System;

namespace Game.Server.Renshen;

public class RenshenProcessorAtribute : Attribute
{
	private byte _code;

	private string _descript;

	public byte Code => _code;

	public string Description => _descript;

	public RenshenProcessorAtribute(byte code, string description)
	{
		_code = code;
		_descript = description;
	}
}
