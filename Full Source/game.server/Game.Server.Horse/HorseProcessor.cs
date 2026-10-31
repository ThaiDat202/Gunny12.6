using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Horse;

public class HorseProcessor
{
	private static object _syncStop = new object();

	private IHorseProcessor _processor;

	public HorseProcessor(IHorseProcessor processor)
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
