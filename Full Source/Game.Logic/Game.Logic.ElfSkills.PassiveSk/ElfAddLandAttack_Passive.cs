using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.ElfSkills.PassiveSkills;

public class ElfAddLandAttack_Passive : AbstractPetEffect
{
	private int m_value = 0;

	private bool IsPVP = false;

	private int m_added = 0;

	private bool m_percent = false;

	public ElfAddLandAttack_Passive(string elementID)
		: base(ePetEffectType.ElfAddLandAttack_Passive, elementID)
	{
		switch (elementID)
		{
		case "2775":
			m_added = 100;
			break;
		case "2776":
			m_added = 200;
			break;
		case "2777":
			m_added = 300;
			break;
		case "2799":
			m_added = 3;
			m_percent = true;
			break;
		case "2800":
			m_added = 5;
			m_percent = true;
			break;
		case "2801":
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
		if (living.PetEffectList.GetOfType(ePetEffectType.ElfAddLandAttack_Passive) is ElfAddLandAttack_Passive)
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
				living.LandAttack += living.LandAttack * (double)m_value / 100.0;
			}
			else
			{
				living.LandAttack += m_value;
			}
		}
	}
}
