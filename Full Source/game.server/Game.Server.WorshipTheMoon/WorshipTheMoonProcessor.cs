using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.WorshipTheMoon;

public class WorshipTheMoonProcessor
{
	private static object _syncStop = new object();

	private IWorshipTheMoonProcessor _processor;

	public WorshipTheMoonProcessor(IWorshipTheMoonProcessor processor)
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
