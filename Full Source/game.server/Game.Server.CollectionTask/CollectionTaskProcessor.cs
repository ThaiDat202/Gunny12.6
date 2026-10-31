using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.CollectionTask;

public class CollectionTaskProcessor
{
	private static object _syncStop = new object();

	private ICollectionTaskProcessor _processor;

	public CollectionTaskProcessor(ICollectionTaskProcessor processor)
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
