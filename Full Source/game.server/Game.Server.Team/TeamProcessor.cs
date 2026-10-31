using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Team;

public class TeamProcessor
{
	private static object _syncStop = new object();

	private ITeamProcessor _processor;

	public TeamProcessor(ITeamProcessor processor)
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
