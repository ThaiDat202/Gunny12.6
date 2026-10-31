using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.MagpieBridge;

public class MagpieBridgeProcessor
{
	private static object _syncStop = new object();

	private IMagpieBridgeProcessor _processor;

	public MagpieBridgeProcessor(IMagpieBridgeProcessor processor)
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
