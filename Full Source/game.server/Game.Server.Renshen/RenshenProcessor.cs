using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Renshen;

public class RenshenProcessor
{
	private static object _syncStop = new object();

	private IRenshenProcessor _processor;

	public RenshenProcessor(IRenshenProcessor processor)
	{
		_processor = processor;
	}

	public void ProcessData(GamePlayer player, GSPacketIn data)
	{
		lock (_syncStop)
		{
			_processor.OnGameData(player, data);
		}
	}
}
