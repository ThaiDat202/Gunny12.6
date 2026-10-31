using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.MagicStone;

public class MagicStoneProcessor
{
	private static object _syncStop = new object();

	private IMagicStoneProcessor _processor;

	public MagicStoneProcessor(IMagicStoneProcessor processor)
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
