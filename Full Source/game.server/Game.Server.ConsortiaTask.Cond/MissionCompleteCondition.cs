using Game.Logic;
using Game.Server.ConsortiaTask.Data;
using Game.Server.GameObjects;

namespace Game.Server.ConsortiaTask.Conditions;

public class MissionCompleteCondition : Condition
{
	public MissionCompleteCondition(GamePlayer player, Consortia consortia)
		: base(player, consortia)
	{
	}

	public override void AddTrigger()
	{
		Player.MissionFullOver += Execute;
	}

	private void Execute(AbstractGame game, int missionId, bool isWin, int turnNum)
	{
		lock (ConsortiaTaskMgr.m_lock)
		{
			ConsortiaTaskConditionInfo consortiaTaskConditionInfo = Consortia?.Task.Conditions.Find((ConsortiaTaskConditionInfo c) => c.CondictionType == 4);
			if (consortiaTaskConditionInfo != null && consortiaTaskConditionInfo.Value < consortiaTaskConditionInfo.TargetCount && missionId == consortiaTaskConditionInfo.MissionID && isWin)
			{
				consortiaTaskConditionInfo.Value++;
				consortiaTaskConditionInfo.Finish++;
				if (Consortia.RankTable.ContainsKey(Player.PlayerId))
				{
					Consortia.RankTable[Player.PlayerId]++;
				}
				else
				{
					Consortia.RankTable.Add(Player.PlayerId, 1);
				}
				Update(consortiaTaskConditionInfo);
			}
		}
	}

	public override void RemoveTrigger()
	{
		Player.MissionFullOver -= Execute;
	}
}
