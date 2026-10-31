using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Achievements;

public class LevelUpgradeCondition : BaseCondition
{
	public LevelUpgradeCondition(BaseAchievement quest, AchievementConditionInfo info, int value)
		: base(quest, info, value)
	{
	}

	public override void AddTrigger(GamePlayer player)
	{
		player.LevelUp += player_GiftToken;
	}

	private void player_GiftToken(GamePlayer player)
	{
		if (base.Value < player.Level)
		{
			base.Value = player.Level;
		}
	}

	public override void RemoveTrigger(GamePlayer player)
	{
		player.LevelUp -= player_GiftToken;
	}

	public override bool IsCompleted(GamePlayer player)
	{
		return base.Value >= m_info.Condiction_Para2;
	}
}
