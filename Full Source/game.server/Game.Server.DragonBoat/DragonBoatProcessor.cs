using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.DragonBoat;

public class DragonBoatProcessor
{
	private static object _syncStop = new object();

	private IDragonBoatProcessor _processor;

	public DragonBoatProcessor(IDragonBoatProcessor processor)
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
