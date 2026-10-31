using System;

namespace Game.Server.CollectionTask;

public class CollectionTaskProcessorAtribute : Attribute
{
	private byte _code;

	private string _descript;

	public byte Code => _code;

	public string Description => _descript;

	public CollectionTaskProcessorAtribute(byte code, string description)
	{
		_code = code;
		_descript = description;
	}
}
