using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.EquipAmulet;

public class EquipAmuletProcessor
{
	private static object _syncStop = new object();

	private IEquipAmuletProcessor _processor;

	public EquipAmuletProcessor(IEquipAmuletProcessor processor)
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
