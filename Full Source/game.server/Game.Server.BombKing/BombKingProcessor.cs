using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.BombKing;

public class BombKingProcessor
{
	private static object _syncStop = new object();

	private IBombKingProcessor _processor;

	public BombKingProcessor(IBombKingProcessor processor)
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
