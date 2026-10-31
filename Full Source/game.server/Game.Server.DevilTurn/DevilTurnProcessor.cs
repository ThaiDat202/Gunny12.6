using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.DevilTurn;

public class DevilTurnProcessor
{
	private static object _syncStop = new object();

	private IDevilTurnProcessor _processor;

	public DevilTurnProcessor(IDevilTurnProcessor processor)
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
