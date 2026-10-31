using Game.Logic;
using Game.Server.ConsortiaTask.Data;
using Game.Server.GameObjects;

namespace Game.Server.ConsortiaTask.Conditions;

public class PvPBattleCondition : Condition
{
	public PvPBattleCondition(GamePlayer player, Consortia consortia)
		: base(player, consortia)
	{
	}

	public override void AddTrigger()
	{
		Player.GameOver += Execute;
	}

	private void Execute(AbstractGame game, bool isWin, int gainXp)
	{
		lock (ConsortiaTaskMgr.m_lock)
		{
			if (game.RoomType != 0)
			{
				return;
			}
			ConsortiaTaskConditionInfo consortiaTaskConditionInfo = Consortia?.Task.Conditions.Find((ConsortiaTaskConditionInfo c) => c.CondictionType == 1);
			if (consortiaTaskConditionInfo == null)
			{
				return;
			}
			if (consortiaTaskConditionInfo.MustWin && isWin)
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
			else if (!consortiaTaskConditionInfo.MustWin)
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
		Player.GameOver -= Execute;
	}
}
