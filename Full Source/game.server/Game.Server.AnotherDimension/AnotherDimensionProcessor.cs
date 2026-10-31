using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.AnotherDimension;

public class AnotherDimensionProcessor
{
	private static object _syncStop = new object();

	private IAnotherDimensionProcessor _processor;

	public AnotherDimensionProcessor(IAnotherDimensionProcessor processor)
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
