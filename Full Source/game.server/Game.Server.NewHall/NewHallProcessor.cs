using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.NewHall;

public class NewHallProcessor
{
	private static object _syncStop = new object();

	private INewHallProcessor _processor;

	public NewHallProcessor(INewHallProcessor processor)
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
