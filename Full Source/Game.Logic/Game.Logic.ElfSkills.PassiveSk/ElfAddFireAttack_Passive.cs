using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.ElfSkills.PassiveSkills;

public class ElfAddFireAttack_Passive : AbstractPetEffect
{
	private int m_value = 0;

	private bool IsPVP = false;

	private int m_added = 0;

	private bool m_percent = false;

	public ElfAddFireAttack_Passive(string elementID)
		: base(ePetEffectType.ElfAddFireAttack_Passive, elementID)
	{
		switch (elementID)
		{
		case "2766":
			m_added = 100;
			break;
		case "2767":
			m_added = 200;
			break;
		case "2768":
			m_added = 300;
			break;
		case "2790":
			m_added = 3;
			m_percent = true;
			break;
		case "2791":
			m_added = 5;
			m_percent = true;
			break;
		case "2792":
			m_added = 8;
			m_percent = true;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (!CheckPVP(living, IsPVP))
		{
			return false;
		}
		if (living.PetEffectList.GetOfType(ePetEffectType.ElfAddFireAttack_Passive) is ElfAddFireAttack_Passive)
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
		if (m_value == 0)
		{
			m_value = m_added;
			living.Game.sendShowPicSkil(living, base.Info, isActive: true);
			if (m_percent)
			{
				living.FireAttack += living.FireAttack * (double)m_value / 100.0;
			}
			else
			{
				living.FireAttack += m_value;
			}
		}
	}
}
