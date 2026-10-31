using System;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetReduceDamageOnPlayerAboveOrBellowOfMe_Passive : BasePetEffect
{
	private int m_probability;

	private int maxValue;

	private int damagePerPixel;

	private int pixels;

	private PlayerAboveOrBelow AboveOrBelow;

	public PetReduceDamageOnPlayerAboveOrBellowOfMe_Passive(int probability, string elementID)
		: base(ePetEffectType.PetReduceDamageOnPlayerAboveOrBellowOfMe_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		if (elementID == "1776")
		{
			damagePerPixel = 15;
			pixels = 100;
			AboveOrBelow = PlayerAboveOrBelow.Above;
			maxValue = 60;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceDamageOnPlayerAboveOrBellowOfMe_Passive) is PetReduceDamageOnPlayerAboveOrBellowOfMe_Passive petReduceDamageOnPlayerAboveOrBellowOfMe_Passive)
		{
			petReduceDamageOnPlayerAboveOrBellowOfMe_Passive.m_probability = ((m_probability > petReduceDamageOnPlayerAboveOrBellowOfMe_Passive.m_probability) ? m_probability : petReduceDamageOnPlayerAboveOrBellowOfMe_Passive.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeforeTakeDamage += player_BeforeTakeDamage;
		player.BeginSelfTurn += player_BeginSelfTurn;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeforeTakeDamage -= player_BeforeTakeDamage;
	}

	private void player_BeginSelfTurn(Living living)
	{
		living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: true);
		living.BeginSelfTurn -= player_BeginSelfTurn;
	}

	private void player_BeforeTakeDamage(Living living, Living source, ref int damageamount, ref int criticalamount)
	{
		int num = 0;
		if (AboveOrBelow == PlayerAboveOrBelow.Above)
		{
			if (source.Y < living.Y)
			{
				int num2 = Math.Abs(living.Y - source.Y);
				num = damagePerPixel * num2 / pixels;
			}
		}
		else if (AboveOrBelow == PlayerAboveOrBelow.Below && source.Y > living.Y)
		{
			int num3 = Math.Abs(living.Y - source.Y);
			num = damagePerPixel * num3 / pixels;
		}
		if (num >= maxValue)
		{
			num = maxValue;
		}
		damageamount -= damageamount * num / 100;
		criticalamount -= criticalamount * num / 100;
	}
}
