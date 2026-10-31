using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.MagicHouse;

public class MagicHouseProcessor
{
	private static object _syncStop = new object();

	private IMagicHouseProcessor _processor;

	public MagicHouseProcessor(IMagicHouseProcessor processor)
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
