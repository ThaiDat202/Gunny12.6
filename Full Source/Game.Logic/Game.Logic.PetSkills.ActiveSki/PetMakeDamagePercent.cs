using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetMakeDamagePercent : BasePetEffect
{
	private int m_probability = 0;

	private int m_currentId;

	private int m_value = 0;

	public PetMakeDamagePercent(int probability, int skillId, string elementID)
		: base(ePetEffectType.PetMakeDamagePercent, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
		switch (elementID)
		{
		case "1328":
			m_value = 100;
			break;
		case "1329":
			m_value = 130;
			break;
		case "1330":
			m_value = 155;
			break;
		case "1133":
		case "1584":
		case "1645":
		case "2573":
		case "2752":
		case "3170":
		case "3553":
		case "3571":
		case "3132":
			m_value = 120;
			break;
		case "1039":
		case "1418":
		case "1548":
		case "1585":
		case "1646":
		case "2574":
		case "2646":
		case "2753":
		case "2958":
		case "3172":
		case "3212":
		case "3554":
		case "3572":
		case "3133":
			m_value = 150;
			break;
		case "1134":
		case "1419":
		case "1549":
		case "1586":
		case "1647":
		case "2575":
		case "2649":
		case "2754":
		case "2959":
		case "3173":
		case "3214":
		case "3555":
		case "3573":
		case "3134":
			m_value = 180;
			break;
		case "1420":
		case "1550":
		case "2651":
		case "2960":
		case "3215":
			m_value = 220;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetMakeDamagePercent) is PetMakeDamagePercent petMakeDamagePercent)
		{
			petMakeDamagePercent.m_probability = ((m_probability > petMakeDamagePercent.m_probability) ? m_probability : petMakeDamagePercent.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerBuffSkillPet += player_PlayerBuffSkillPet;
		player.AfterPlayerShooted += player_AfterPlayerShooted;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerBuffSkillPet -= player_PlayerBuffSkillPet;
		player.AfterPlayerShooted -= player_AfterPlayerShooted;
	}

	private void player_PlayerBuffSkillPet(Player player)
	{
		if (player.PetEffects.CurrentUseSkill == m_currentId)
		{
			player.CurrentDamagePlus += m_value / 100;
		}
	}

	private void player_AfterPlayerShooted(Player player)
	{
		if (player.PetEffects.CurrentUseSkill == m_currentId)
		{
			player.CurrentDamagePlus -= m_value / 100;
		}
	}
}
