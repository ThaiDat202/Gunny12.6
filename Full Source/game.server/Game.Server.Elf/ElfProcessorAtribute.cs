using System;

namespace Game.Server.Elf;

public class ElfProcessorAtribute : Attribute
{
	private byte _code;

	private string _descript;

	public byte Code => _code;

	public string Description => _descript;

	public ElfProcessorAtribute(byte code, string description)
	{
		_code = code;
		_descript = description;
	}
}
