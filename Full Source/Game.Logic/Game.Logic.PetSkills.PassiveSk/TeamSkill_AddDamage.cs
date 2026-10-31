using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class TeamSkill_AddDamage : BasePetEffect
{
	private int m_value = 0;

	private int m_added = 0;

	public TeamSkill_AddDamage(string elementID)
		: base(ePetEffectType.TeamSkill_AddDamage, elementID)
	{
		switch (elementID)
		{
		case "2028":
			m_added = 5;
			break;
		case "2029":
			m_added = 8;
			break;
		case "2030":
			m_added = 10;
			break;
		case "2031":
			m_added = 12;
			break;
		case "2032":
			m_added = 14;
			break;
		case "2033":
			m_added = 16;
			break;
		case "2034":
			m_added = 18;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.AddAgility_SealPassive) is TeamSkill_AddDamage)
		{
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeginSelfTurn += player_BeginSelfTurn;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeginSelfTurn -= player_BeginSelfTurn;
	}

	private void player_BeginSelfTurn(Living living)
	{
		if (m_value == 0 && living.Game is PVPGame)
		{
			m_value = m_added;
			living.Game.sendShowPicSkil(living, base.Info, isActive: true);
			living.BaseDamage += living.BaseDamage * (double)m_value / 100.0;
		}
	}
}
