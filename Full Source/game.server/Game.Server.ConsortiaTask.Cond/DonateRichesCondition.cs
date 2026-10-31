using Game.Server.ConsortiaTask.Data;
using Game.Server.GameObjects;

namespace Game.Server.ConsortiaTask.Conditions;

public class DonateRichesCondition : Condition
{
	public DonateRichesCondition(GamePlayer player, Consortia consortia)
		: base(player, consortia)
	{
	}

	public override void AddTrigger()
	{
		Player.Riches += Execute;
	}

	private void Execute(int value, int type)
	{
		lock (ConsortiaTaskMgr.m_lock)
		{
			if (type == 0)
			{
				return;
			}
			ConsortiaTaskConditionInfo consortiaTaskConditionInfo = Consortia?.Task.Conditions.Find((ConsortiaTaskConditionInfo c) => c.CondictionType == 5);
			if (consortiaTaskConditionInfo != null && consortiaTaskConditionInfo.TargetCount > consortiaTaskConditionInfo.Value)
			{
				consortiaTaskConditionInfo.Value += value;
				consortiaTaskConditionInfo.Finish += value;
				if (Consortia.RankTable.ContainsKey(Player.PlayerId))
				{
					Consortia.RankTable[Player.PlayerId] += value;
				}
				else
				{
					Consortia.RankTable.Add(Player.PlayerId, value);
				}
				Update(consortiaTaskConditionInfo);
			}
		}
	}

	public override void RemoveTrigger()
	{
		Player.Riches -= Execute;
	}
}
