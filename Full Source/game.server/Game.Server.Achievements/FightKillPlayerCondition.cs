using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Achievements;

public class FightKillPlayerCondition : BaseCondition
{
	public FightKillPlayerCondition(BaseAchievement quest, AchievementConditionInfo info, int value)
		: base(quest, info, value)
	{
	}

	public override void AddTrigger(GamePlayer player)
	{
		player.AfterKillingLiving += player_AddOffer;
	}

	private void player_AddOffer(AbstractGame game, int type, int id, bool isLiving, int damage)
	{
		if (type == 1 && !isLiving)
		{
			base.Value++;
		}
	}

	public override void RemoveTrigger(GamePlayer player)
	{
		player.AfterKillingLiving -= player_AddOffer;
	}

	public override bool IsCompleted(GamePlayer player)
	{
		return base.Value >= m_info.Condiction_Para2;
	}
}
