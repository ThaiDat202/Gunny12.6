using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetReduceEnergyEquip : AbstractPetEffect
{
	private int m_count;

	private int m_value;

	private bool m_percent;

	public PetReduceEnergyEquip(int count, string elementID)
		: base(ePetEffectType.PetReduceEnergyEquip, elementID)
	{
		m_count = count;
		switch (elementID)
		{
		case "1150":
			m_value = 20;
			m_percent = true;
			break;
		case "1151":
			m_value = 25;
			m_percent = true;
			break;
		case "1152":
			m_value = 30;
			m_percent = true;
			break;
		case "1741":
			m_value = 50;
			m_percent = true;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceEnergyEquip) is PetReduceEnergyEquip petReduceEnergyEquip)
		{
			petReduceEnergyEquip.m_count = m_count;
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
		m_count--;
		if (m_count < 0)
		{
			living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: false);
			Stop();
		}
		else if (living is Player)
		{
			if (m_percent)
			{
				(living as Player).ReduceEnergy((living as Player).Energy * m_value / 100);
			}
			else
			{
				(living as Player).ReduceEnergy(m_value);
			}
		}
	}
}
