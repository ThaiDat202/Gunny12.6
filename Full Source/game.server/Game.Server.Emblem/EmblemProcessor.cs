using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Emblem;

public class EmblemProcessor
{
	private static object _syncStop = new object();

	private IEmblemProcessor _processor;

	public EmblemProcessor(IEmblemProcessor processor)
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
