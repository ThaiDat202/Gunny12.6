using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeTemple;

public class HomeTempleProcessor
{
	private static object _syncStop = new object();

	private IHomeTempleProcessor _processor;

	public HomeTempleProcessor(IHomeTempleProcessor processor)
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
