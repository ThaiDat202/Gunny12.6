using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Academy;

public class AcademyProcessor
{
	private static object _syncStop = new object();

	private IAcademyProcessor _processor;

	public AcademyProcessor(IAcademyProcessor processor)
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
