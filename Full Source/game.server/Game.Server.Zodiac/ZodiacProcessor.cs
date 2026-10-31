using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Zodiac;

public class ZodiacProcessor
{
	private static object _syncStop = new object();

	private IZodiacProcessor _processor;

	public ZodiacProcessor(IZodiacProcessor processor)
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
