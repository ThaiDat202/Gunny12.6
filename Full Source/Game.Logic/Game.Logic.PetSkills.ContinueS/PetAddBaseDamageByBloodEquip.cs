using System;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetAddBaseDamageByBloodEquip : BasePetEffect
{
	private int m_count;

	private double m_value;

	private double m_added;

	private bool m_isHide;

	public PetAddBaseDamageByBloodEquip(int count, string elementID, bool IsHide = false)
		: base(ePetEffectType.PetAddBaseDamageByBloodEquip, elementID)
	{
		m_count = count;
		m_isHide = IsHide;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddBaseDamageByBloodEquip) is PetAddBaseDamageByBloodEquip petAddBaseDamageByBloodEquip)
		{
			petAddBaseDamageByBloodEquip.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		if (m_added == 0.0 && player.BaseDamage == player.OldBaseDamage)
		{
			m_value = ((double)Math.Round((decimal)player.MaxBlood / (decimal)player.Blood, 3) - 1.0) * 100.0;
			if (m_value > 80.0)
			{
				m_value = 80.0;
			}
			m_added = player.BaseDamage * m_value / 100.0;
			player.BaseDamage += m_added * 1.3;
			player.BeginSelfTurn += player_BeginSelfTurn;
		}
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		if (m_added != 0.0 && player.BaseDamage > player.OldBaseDamage)
		{
			player.BaseDamage -= m_added * 1.3;
			m_added = 0.0;
			m_value = 0.0;
		}
		if (m_isHide)
		{
			m_value = ((double)Math.Round((decimal)player.MaxBlood / (decimal)player.Blood, 3) - 1.0) * 100.0;
			if (m_value > 80.0)
			{
				m_value = 80.0;
			}
			m_added = player.BaseDamage * m_value / 100.0;
			player.BaseDamage += m_added * 1.3;
			player.BeginSelfTurn += player_BeginSelfTurn;
		}
		else
		{
			player.BeginSelfTurn -= player_BeginSelfTurn;
		}
	}

	private void player_BeginSelfTurn(Living living)
	{
		m_count--;
		if (m_count < 0)
		{
			Stop();
		}
	}
}
