using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.GodCardRaise;

public class GodCardRaiseProcessor
{
	private static object object_0;

	private IGodCardRaiseProcessor igodCardRaiseProcessor_0;

	static GodCardRaiseProcessor()
	{
		object_0 = new object();
	}

	public GodCardRaiseProcessor(IGodCardRaiseProcessor processor)
	{
		igodCardRaiseProcessor_0 = processor;
	}

	public void ProcessData(GamePlayer player, GSPacketIn data)
	{
		lock (object_0)
		{
			igodCardRaiseProcessor_0.OnGameData(player, data);
		}
	}
}
