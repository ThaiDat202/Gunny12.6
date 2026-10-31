using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.MoneyTree;

public class MoneyTreeProcessor
{
	private static object _syncStop = new object();

	private IMoneyTreeProcessor _processor;

	public MoneyTreeProcessor(IMoneyTreeProcessor processor)
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
