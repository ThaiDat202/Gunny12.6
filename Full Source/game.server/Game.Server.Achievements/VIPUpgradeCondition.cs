using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Achievements;

public class VIPUpgradeCondition : BaseCondition
{
	public VIPUpgradeCondition(BaseAchievement quest, AchievementConditionInfo info, int value)
		: base(quest, info, value)
	{
	}

	public override void AddTrigger(GamePlayer player)
	{
		player.VIPUpgrade += player_VIPUpgrade;
	}

	private void player_VIPUpgrade(int lv, int exp)
	{
		if (base.Value < lv)
		{
			base.Value = lv;
		}
	}

	public override void RemoveTrigger(GamePlayer player)
	{
		player.VIPUpgrade -= player_VIPUpgrade;
	}

	public override bool IsCompleted(GamePlayer player)
	{
		return base.Value >= m_info.Condiction_Para2;
	}
}
