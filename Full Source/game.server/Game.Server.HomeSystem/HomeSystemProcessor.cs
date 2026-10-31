using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeSystem;

public class HomeSystemProcessor
{
	private static object _syncStop = new object();

	private IHomeSystemProcessor _processor;

	public HomeSystemProcessor(IHomeSystemProcessor processor)
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
