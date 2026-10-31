using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.ElfSkills.PassiveSkills;

public class ElfAddElementSkillDamage_Passive : AbstractPetEffect
{
	private int m_value = 0;

	private bool IsPVP = false;

	private int m_added = 0;

	private bool m_percent = false;

	private int m_skillId = 0;

	private bool m_isRate = false;

	public ElfAddElementSkillDamage_Passive(string elementID, int skillId, bool isRate)
		: base(ePetEffectType.ElfAddElementSkillDamage_Passive, elementID)
	{
		switch (elementID)
		{
		case "2856":
			m_added = 75;
			break;
		case "2857":
			m_added = 125;
			break;
		case "2858":
			m_added = 175;
			break;
		case "2814":
		case "2844":
			m_added = 200;
			break;
		case "2815":
		case "2829":
		case "2845":
			m_added = 350;
			break;
		case "2816":
		case "2830":
		case "2846":
			m_added = 500;
			break;
		case "2831":
			m_added = 800;
			break;
		case "2859":
			m_added = 4;
			m_percent = true;
			break;
		case "2860":
			m_added = 6;
			m_percent = true;
			break;
		case "2817":
		case "2847":
		case "2861":
			m_added = 8;
			m_percent = true;
			break;
		case "2818":
		case "2848":
			m_added = 12;
			m_percent = true;
			break;
		case "2819":
		case "2832":
		case "1849":
			m_added = 16;
			m_percent = true;
			break;
		case "2833":
			m_added = 20;
			m_percent = true;
			break;
		case "2834":
			m_added = 24;
			m_percent = true;
			break;
		case "2820":
		case "2835":
		case "2850":
			m_added = 2000;
			break;
		case "2821":
		case "2836":
		case "2851":
			m_added = 3500;
			break;
		case "2822":
		case "2837":
		case "2852":
			m_added = 5000;
			break;
		}
		m_skillId = skillId;
		m_isRate = isRate;
	}

	public override bool Start(Living living)
	{
		if (!CheckPVP(living, IsPVP))
		{
			return false;
		}
		if (living.PetEffectList.GetOfType(ePetEffectType.ElfAddElementSkillDamage_Passive) is ElfAddElementSkillDamage_Passive)
		{
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		living.BeginSelfTurn += player_BeginSelfTurn;
	}

	public override void OnRemoved(Living living)
	{
		living.BeginSelfTurn -= player_BeginSelfTurn;
	}

	private void player_BeginSelfTurn(Living living)
	{
		if (m_value != 0)
		{
			return;
		}
		m_value = m_added;
		living.Game.sendShowPicSkil(living, base.Info, isActive: true);
		if (m_skillId == (living as Player).PlayerDetail.PlayerCharacter.EmblemInfoAttackSKill && !m_isRate)
		{
			if (m_percent)
			{
				(living as Player).PlayerDetail.PlayerCharacter.EmblemInfoAttackSKill_Value += (living as Player).PlayerDetail.PlayerCharacter.EmblemInfoAttackSKill_Value * m_value / 100;
			}
			else
			{
				(living as Player).PlayerDetail.PlayerCharacter.EmblemInfoAttackSKill_Value += m_value;
			}
		}
		else if (m_skillId == (living as Player).PlayerDetail.PlayerCharacter.EmblemInfoAttackSKill && m_isRate)
		{
			(living as Player).PlayerDetail.PlayerCharacter.EmblemInfoAttackSKill_Rate += m_value;
		}
	}
}
