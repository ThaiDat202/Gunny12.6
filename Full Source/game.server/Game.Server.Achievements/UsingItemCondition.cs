using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Achievements;

public class UsingItemCondition : BaseCondition
{
	private int m_templateid;

	public UsingItemCondition(BaseAchievement quest, AchievementConditionInfo info, int templateid, int value)
		: base(quest, info, value)
	{
		m_templateid = templateid;
	}

	public override void AddTrigger(GamePlayer player)
	{
		player.AfterUsingItem += player_AddOffer;
	}

	private void player_AddOffer(int templateid, int count)
	{
		if (m_templateid == templateid)
		{
			base.Value += count;
		}
	}

	public override void RemoveTrigger(GamePlayer player)
	{
		player.AfterUsingItem -= player_AddOffer;
	}

	public override bool IsCompleted(GamePlayer player)
	{
		return base.Value >= m_info.Condiction_Para2;
	}
}
