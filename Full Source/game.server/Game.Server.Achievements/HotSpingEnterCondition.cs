using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Achievements;

public class HotSpingEnterCondition : BaseCondition
{
	public HotSpingEnterCondition(BaseAchievement quest, AchievementConditionInfo info, int value)
		: base(quest, info, value)
	{
	}

	public override void AddTrigger(GamePlayer player)
	{
		player.HotSpingExpAdd += player_VIPUpgrade;
	}

	private void player_VIPUpgrade(int min, int exp)
	{
		if (min > 0)
		{
			base.Value++;
		}
	}

	public override void RemoveTrigger(GamePlayer player)
	{
		player.HotSpingExpAdd -= player_VIPUpgrade;
	}

	public override bool IsCompleted(GamePlayer player)
	{
		return base.Value >= m_info.Condiction_Para2;
	}
}
