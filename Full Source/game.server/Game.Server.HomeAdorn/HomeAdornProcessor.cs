using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeAdorn;

public class HomeAdornProcessor
{
	private static object _syncStop = new object();

	private IHomeAdornProcessor _processor;

	public HomeAdornProcessor(IHomeAdornProcessor processor)
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
