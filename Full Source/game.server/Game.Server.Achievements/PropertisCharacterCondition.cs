using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Achievements;

public class PropertisCharacterCondition : BaseCondition
{
	private string m_type;

	public PropertisCharacterCondition(BaseAchievement quest, AchievementConditionInfo info, int value, string type)
		: base(quest, info, value)
	{
		m_type = type;
	}

	public override void AddTrigger(GamePlayer player)
	{
		player.PropertisChange += player_GiftToken;
	}

	private void player_GiftToken(PlayerInfo player)
	{
		switch (m_type)
		{
		case "attack":
			base.Value = player.Attack;
			break;
		case "agility":
			base.Value = player.Agility;
			break;
		case "luck":
			base.Value = player.Luck;
			break;
		case "defence":
			base.Value = player.Defence;
			break;
		case "fightpower":
			base.Value = player.FightPower;
			break;
		}
	}

	public override void RemoveTrigger(GamePlayer player)
	{
		player.PropertisChange -= player_GiftToken;
	}

	public override bool IsCompleted(GamePlayer player)
	{
		return base.Value >= m_info.Condiction_Para2;
	}
}
