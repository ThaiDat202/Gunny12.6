using System;

namespace Game.Server.AvatarCollection;

public class AvatarCollectionProcessorAtribute : Attribute
{
	private byte _code;

	private string _descript;

	public byte Code => _code;

	public string Description => _descript;

	public AvatarCollectionProcessorAtribute(byte code, string description)
	{
		_code = code;
		_descript = description;
	}
}
