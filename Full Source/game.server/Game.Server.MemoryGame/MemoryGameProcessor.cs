using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.MemoryGame;

public class MemoryGameProcessor
{
	private static object _syncStop = new object();

	private IMemoryGameProcessor _processor;

	public MemoryGameProcessor(IMemoryGameProcessor processor)
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
