using System;

namespace Game.Server.GypsyShop;

public class GypsyShopProcessorAtribute : Attribute
{
	private byte _code;

	private string _descript;

	public byte Code => _code;

	public string Description => _descript;

	public GypsyShopProcessorAtribute(byte code, string description)
	{
		_code = code;
		_descript = description;
	}
}
