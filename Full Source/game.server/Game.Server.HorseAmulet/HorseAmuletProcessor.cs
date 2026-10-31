using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HorseAmulet;

public class HorseAmuletProcessor
{
	private static object _syncStop = new object();

	private IHorseAmuletProcessor _processor;

	public HorseAmuletProcessor(IHorseAmuletProcessor processor)
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
