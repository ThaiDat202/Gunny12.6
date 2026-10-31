using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Elf;

public class ElfProcessor
{
	private static object _syncStop = new object();

	private IElfProcessor _processor;

	public ElfProcessor(IElfProcessor processor)
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
