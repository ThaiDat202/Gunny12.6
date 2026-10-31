using System;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.ConsortiaTask.Handle;

[ConsortiaTaskHandle(3)]
public class GET_TASKINFO : IConsortiaTaskCommandHandler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		Consortia consortia = ConsortiaTaskMgr.Consortiums.Find((Consortia c) => c.ID == Player.PlayerCharacter.ConsortiaID);
		if (consortia != null && consortia.Task.BeginTime.AddMinutes(consortia.Task.Time) > DateTime.Now)
		{
			ConsortiaTaskMgr.Out.SendTaskInfo(Player, consortia);
		}
		else
		{
			ConsortiaTaskMgr.Out.SendTaskInfo(Player);
		}
		return 0;
	}
}
