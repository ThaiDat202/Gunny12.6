using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Achievements;

public class GoldCollectionCondition : BaseCondition
{
	public GoldCollectionCondition(BaseAchievement quest, AchievementConditionInfo info, int value)
		: base(quest, info, value)
	{
	}

	public override void AddTrigger(GamePlayer player)
	{
		player.GoldCollect += player_GoldCollect;
	}

	private void player_GoldCollect(int value)
	{
		base.Value += value;
	}

	public override void RemoveTrigger(GamePlayer player)
	{
		player.GoldCollect -= player_GoldCollect;
	}

	public override bool IsCompleted(GamePlayer player)
	{
		return base.Value >= m_info.Condiction_Para2;
	}
}
