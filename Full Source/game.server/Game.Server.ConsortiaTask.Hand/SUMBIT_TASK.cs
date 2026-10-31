using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.ConsortiaTask.Handle;

[ConsortiaTaskHandle(2)]
public class SUMBIT_TASK : IConsortiaTaskCommandHandler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		ConsortiaTaskMgr.Out.SendSumbitTask(Player, status: true);
		return 0;
	}
}
