using Game.Server.ConsortiaTask.Data;
using Game.Server.GameObjects;

namespace Game.Server.ConsortiaTask.Conditions;

public class UseItemCondition : Condition
{
	private readonly int _templateId;

	public UseItemCondition(GamePlayer player, Consortia consortia, int templateId)
		: base(player, consortia)
	{
		_templateId = templateId;
	}

	public override void AddTrigger()
	{
		Player.AfterUsingItem += Execute;
	}

	public override void RemoveTrigger()
	{
		Player.AfterUsingItem -= Execute;
	}

	private void Execute(int templateId, int count)
	{
		lock (ConsortiaTaskMgr.m_lock)
		{
			if (Consortia == null || templateId != _templateId)
			{
				return;
			}
			ConsortiaTaskConditionInfo consortiaTaskConditionInfo = Consortia?.Task.Conditions.Find((ConsortiaTaskConditionInfo c) => c.CondictionType == 3);
			if (consortiaTaskConditionInfo != null && consortiaTaskConditionInfo.TargetCount > consortiaTaskConditionInfo.Value)
			{
				consortiaTaskConditionInfo.Value += count;
				consortiaTaskConditionInfo.Finish += count;
				if (Consortia.RankTable.ContainsKey(Player.PlayerId))
				{
					Consortia.RankTable[Player.PlayerId] += count;
				}
				else
				{
					Consortia.RankTable.Add(Player.PlayerId, count);
				}
				Update(consortiaTaskConditionInfo);
			}
		}
	}
}
