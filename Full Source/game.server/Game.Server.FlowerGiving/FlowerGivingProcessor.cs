using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.FlowerGiving;

public class FlowerGivingProcessor
{
	private static object _syncStop = new object();

	private IFlowerGivingProcessor _processor;

	public FlowerGivingProcessor(IFlowerGivingProcessor processor)
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
