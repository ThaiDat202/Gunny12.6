using System;

namespace Game.Server.Pet;

public class PetProcessorAtribute : Attribute
{
	private byte _code;

	private string _descript;

	public byte Code => _code;

	public string Description => _descript;

	public PetProcessorAtribute(byte code, string description)
	{
		_code = code;
		_descript = description;
	}
}
