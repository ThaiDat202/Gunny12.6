using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.GuardCore;

public class GuardCoreProcessor
{
	private static object _syncStop = new object();

	private IGuardCoreProcessor _processor;

	public GuardCoreProcessor(IGuardCoreProcessor processor)
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
