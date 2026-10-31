using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeBar;

public class HomeBarProcessor
{
	private static object _syncStop = new object();

	private IHomeBarProcessor _processor;

	public HomeBarProcessor(IHomeBarProcessor processor)
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
