using System;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.ConsortiaTask.Handle;

[ConsortiaTaskHandle(0)]
public class ReleaseTask : IConsortiaTaskCommandHandler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int level = packet.ReadInt();
		ConsortiaTaskMgr.CreateTask(Player, level);
		Consortia consortia = ConsortiaTaskMgr.Consortiums.Find((Consortia c) => c.ID == Player.PlayerCharacter.ConsortiaID);
		if (consortia.Task.BeginTime.AddMinutes(consortia.Task.Time) > DateTime.Now)
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
