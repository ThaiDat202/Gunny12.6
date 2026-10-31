using System;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.ConsortiaTask.Handle;

[ConsortiaTaskHandle(1)]
public class RESET_TASK : IConsortiaTaskCommandHandler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int num2 = packet.ReadInt();
		if (num > 0 || num2 > 0)
		{
			Player.SendMessage("锁定功能已禁用,请解锁后再刷新~~");
			return 0;
		}
		ConsortiaTaskMgr.ResetTask(Player);
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
