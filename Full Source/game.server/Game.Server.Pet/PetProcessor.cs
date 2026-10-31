using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Pet;

public class PetProcessor
{
	private static object _syncStop = new object();

	private IPetProcessor _processor;

	public PetProcessor(IPetProcessor processor)
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
