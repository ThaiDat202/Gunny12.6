using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeFight;

public class HomeFightProcessor
{
	private static object _syncStop = new object();

	private IHomeFightProcessor _processor;

	public HomeFightProcessor(IHomeFightProcessor processor)
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
