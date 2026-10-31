using Bussiness;
using Game.Base.Packets;
using Game.Server.ConsortiaTask.Data;
using Game.Server.GameObjects;

namespace Game.Server.ConsortiaTask;

public class ConsortiaTaskPacketsOut
{
	public void SendSumbitTask(GamePlayer player, bool status)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(129);
		gSPacketIn.WriteByte(22);
		gSPacketIn.WriteByte(2);
		gSPacketIn.WriteBoolean(status);
		player.SendTCP(gSPacketIn);
	}

	public void SendTaskInfo(GamePlayer player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(129);
		gSPacketIn.WriteByte(22);
		gSPacketIn.WriteByte(3);
		gSPacketIn.WriteInt(0);
		player.SendTCP(gSPacketIn);
	}

	public void SendTaskConditionCompleted(GamePlayer player, string condition)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(129, 0);
		gSPacketIn.WriteByte(20);
		gSPacketIn.WriteByte(3);
		gSPacketIn.WriteString("公会使命通知");
		gSPacketIn.WriteString(LanguageMgr.GetTranslation("Consortia.TaskCondition.Completed", condition));
		player.SendTCP(gSPacketIn);
	}

	public void SendTaskComplete(GamePlayer player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(129, 0);
		gSPacketIn.WriteByte(20);
		gSPacketIn.WriteByte(3);
		gSPacketIn.WriteString("公会使命通知");
		gSPacketIn.WriteString(LanguageMgr.GetTranslation("Consortia.Task.Complete"));
		player.SendTCP(gSPacketIn);
	}

	public void SendTaskUpdate(GamePlayer player, ConsortiaTaskConditionInfo condition)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(129);
		gSPacketIn.WriteByte(22);
		gSPacketIn.WriteByte(4);
		gSPacketIn.WriteInt(condition.ID);
		gSPacketIn.WriteInt(condition.Value);
		gSPacketIn.WriteInt(condition.Finish);
		player.SendTCP(gSPacketIn);
	}

	public void SendTaskInfo(GamePlayer player, Consortia consortia)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(129);
		gSPacketIn.WriteByte(22);
		gSPacketIn.WriteByte(3);
		gSPacketIn.WriteInt(consortia.Task.Conditions.Count);
		foreach (ConsortiaTaskConditionInfo condition in consortia.Task.Conditions)
		{
			gSPacketIn.WriteInt(condition.ID);
			gSPacketIn.WriteInt(condition.CondictionType);
			gSPacketIn.WriteString(condition.CondictionTitle);
			gSPacketIn.WriteInt(condition.Value);
			gSPacketIn.WriteInt(condition.TargetCount);
			gSPacketIn.WriteInt(condition.Finish);
		}
		gSPacketIn.WriteInt(consortia.Task.Experience);
		gSPacketIn.WriteInt(consortia.Task.Offer);
		gSPacketIn.WriteInt(consortia.Task.Contribution);
		gSPacketIn.WriteInt(consortia.Task.Riches);
		gSPacketIn.WriteInt(consortia.Task.BuffID);
		gSPacketIn.WriteDateTime(consortia.Task.BeginTime);
		gSPacketIn.WriteInt(consortia.Task.Time);
		gSPacketIn.WriteInt(consortia.Task.Level);
		player.SendTCP(gSPacketIn);
	}
}
