using System;
using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetStealthPosture_HidePassive : BasePetEffect
{
	private int m_probability = 0;

	private int m_value;

	private double m_value1;

	private double m_added;

	public PetStealthPosture_HidePassive(int probability, string elementID)
		: base(ePetEffectType.PetStealthPosture_HidePassive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		switch (elementID)
		{
		case "1558":
		case "1614":
			m_value = 40;
			break;
		case "1594":
		case "1615":
			m_value = 30;
			break;
		case "1559":
			m_value = 40;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetStealthPosture_HidePassive) is PetStealthPosture_HidePassive petStealthPosture_HidePassive)
		{
			petStealthPosture_HidePassive.m_probability = ((m_probability > petStealthPosture_HidePassive.m_probability) ? m_probability : petStealthPosture_HidePassive.m_probability);
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
		if ((double)living.Blood / (double)living.MaxBlood * 100.0 >= (double)m_value)
		{
			living.IsHide = false;
			living.Game.sendShowPicSkil(living, base.Info, isActive: false);
			living.BaseDamage = living.OldBaseDamage;
			return;
		}
		living.IsHide = true;
		living.Game.sendShowPicSkil(living, base.Info, isActive: true);
		m_value1 = ((double)Math.Round((decimal)living.MaxBlood / (decimal)living.Blood, 3) - 1.0) * 100.0;
		if (m_value1 > 80.0)
		{
			m_value1 = 80.0;
		}
		m_added = living.BaseDamage * m_value1 / 100.0;
		living.BaseDamage += m_added * 1.3;
		new PetStealthPostureEquip(1, base.ElementInfo.ID.ToString()).Start(living);
	}
}
