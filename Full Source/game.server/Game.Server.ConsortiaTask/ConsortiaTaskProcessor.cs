using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.ConsortiaTask;

public class ConsortiaTaskProcessor
{
	private static object _syncStop = new object();

	private IConsortiaTaskProcessor _processor;

	public ConsortiaTaskProcessor(IConsortiaTaskProcessor processor)
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
