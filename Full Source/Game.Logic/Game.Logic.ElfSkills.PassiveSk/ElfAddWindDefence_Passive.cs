using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.ElfSkills.PassiveSkills;

public class ElfAddWindDefence_Passive : AbstractPetEffect
{
	private int m_value = 0;

	private bool IsPVP = false;

	private int m_added = 0;

	private bool m_percent = false;

	public ElfAddWindDefence_Passive(string elementID)
		: base(ePetEffectType.ElfAddWindDefence_Passive, elementID)
	{
		switch (elementID)
		{
		case "2784":
			m_added = 150;
			break;
		case "2785":
			m_added = 300;
			break;
		case "2786":
			m_added = 450;
			break;
		case "2808":
			m_added = 5;
			m_percent = true;
			break;
		case "2809":
			m_added = 8;
			m_percent = true;
			break;
		case "2810":
			m_added = 12;
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
		if (living.PetEffectList.GetOfType(ePetEffectType.ElfAddWindDefence_Passive) is ElfAddWindDefence_Passive)
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
				living.WindDefence += living.WindDefence * (double)m_value / 100.0;
			}
			else
			{
				living.WindDefence += m_value;
			}
		}
	}
}
