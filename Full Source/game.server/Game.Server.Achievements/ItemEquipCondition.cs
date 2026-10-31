using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Achievements;

public class ItemEquipCondition : BaseCondition
{
	private int m_templateid;

	public ItemEquipCondition(BaseAchievement quest, AchievementConditionInfo info, int value, int templateid)
		: base(quest, info, value)
	{
		m_templateid = templateid;
	}

	public override void AddTrigger(GamePlayer player)
	{
		player.NewGearEvent += player_AddOffer;
	}

	private void player_AddOffer(int templateID)
	{
		if (templateID == m_templateid)
		{
			base.Value++;
		}
	}

	public override void RemoveTrigger(GamePlayer player)
	{
		player.NewGearEvent -= player_AddOffer;
	}

	public override bool IsCompleted(GamePlayer player)
	{
		return base.Value >= m_info.Condiction_Para2;
	}
}
