using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Consortia;

public class ConsortiaProcessor
{
	private static object _syncStop = new object();

	private IConsortiaProcessor _processor;

	public ConsortiaProcessor(IConsortiaProcessor processor)
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
