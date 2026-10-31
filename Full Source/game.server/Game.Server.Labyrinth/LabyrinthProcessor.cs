using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Labyrinth;

public class LabyrinthProcessor
{
	private static object _syncStop = new object();

	private ILabyrinthProcessor _processor;

	public LabyrinthProcessor(ILabyrinthProcessor processor)
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
