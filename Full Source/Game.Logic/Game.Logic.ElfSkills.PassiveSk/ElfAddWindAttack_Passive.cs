using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.ElfSkills.PassiveSkills;

public class ElfAddWindAttack_Passive : AbstractPetEffect
{
	private int m_value = 0;

	private bool IsPVP = false;

	private int m_added = 0;

	private bool m_percent = false;

	public ElfAddWindAttack_Passive(string elementID)
		: base(ePetEffectType.ElfAddWindAttack_Passive, elementID)
	{
		switch (elementID)
		{
		case "2772":
			m_added = 100;
			break;
		case "2773":
			m_added = 200;
			break;
		case "2774":
			m_added = 300;
			break;
		case "2796":
			m_added = 3;
			m_percent = true;
			break;
		case "2797":
			m_added = 5;
			m_percent = true;
			break;
		case "2798":
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
		if (living.PetEffectList.GetOfType(ePetEffectType.ElfAddWindAttack_Passive) is ElfAddWindAttack_Passive)
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
				living.WindAttack += living.WindAttack * (double)m_value / 100.0;
			}
			else
			{
				living.WindAttack += m_value;
			}
		}
	}
}
