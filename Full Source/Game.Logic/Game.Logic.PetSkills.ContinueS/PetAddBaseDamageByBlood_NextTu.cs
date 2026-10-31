using System;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetAddBaseDamageByBlood_NextTurn : BasePetEffect
{
	private int m_count;

	private double m_value;

	private double m_added;

	public PetAddBaseDamageByBlood_NextTurn(int count, string elementID)
		: base(ePetEffectType.PetAddBaseDamageByBloodEquip, elementID)
	{
		m_count = count;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddBaseDamageByBloodEquip) is PetAddBaseDamageByBlood_NextTurn petAddBaseDamageByBlood_NextTurn)
		{
			petAddBaseDamageByBlood_NextTurn.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeginNextTurn += player_BeginNextTurn;
		player.BeginSelfTurn += player_BeginSelfTurn;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		if (m_added != 0.0)
		{
			player.BaseDamage -= m_added;
			m_added = 0.0;
			m_value = 0.0;
		}
		player.BeginNextTurn -= player_BeginNextTurn;
		player.BeginSelfTurn -= player_BeginSelfTurn;
	}

	private void player_BeginSelfTurn(Living living)
	{
		m_count--;
		if (m_count < 0)
		{
			Stop();
		}
	}

	private void player_BeginNextTurn(Living living)
	{
		if (m_added == 0.0)
		{
			m_value = ((double)Math.Round((decimal)living.MaxBlood / (decimal)living.Blood, 3) - 1.0) * 100.0;
			if (m_value > 80.0)
			{
				m_value = 80.0;
			}
			m_added = living.BaseDamage * m_value / 100.0;
			living.BaseDamage += m_added * 1.3;
		}
	}
}
