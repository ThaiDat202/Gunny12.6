using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.CardNewSystem;

public class CardNewSystemProcessor
{
	private static object _syncStop = new object();

	private ICardNewSystemProcessor _processor;

	public CardNewSystemProcessor(ICardNewSystemProcessor processor)
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
