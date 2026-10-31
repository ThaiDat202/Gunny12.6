using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.ElfSkills.PassiveSkills;

public class ElfAddLandDefence_Passive : AbstractPetEffect
{
	private int m_value = 0;

	private bool IsPVP = false;

	private int m_added = 0;

	private bool m_percent = false;

	public ElfAddLandDefence_Passive(string elementID)
		: base(ePetEffectType.ElfAddLandDefence_Passive, elementID)
	{
		switch (elementID)
		{
		case "2787":
			m_added = 150;
			break;
		case "2788":
			m_added = 300;
			break;
		case "2789":
			m_added = 450;
			break;
		case "2811":
			m_added = 5;
			m_percent = true;
			break;
		case "2812":
			m_added = 8;
			m_percent = true;
			break;
		case "2813":
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
		if (living.PetEffectList.GetOfType(ePetEffectType.ElfAddLandDefence_Passive) is ElfAddLandDefence_Passive)
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
				living.LandDefence += living.LandDefence * (double)m_value / 100.0;
			}
			else
			{
				living.LandDefence += m_value;
			}
		}
	}
}
