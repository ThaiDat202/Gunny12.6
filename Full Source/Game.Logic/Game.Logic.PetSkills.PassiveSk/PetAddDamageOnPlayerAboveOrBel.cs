using System;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddDamageOnPlayerAboveOrBellowOfMe_Passive : BasePetEffect
{
	private int m_probability;

	private int maxValue;

	private int damagePerPixel;

	private int pixels;

	private PlayerAboveOrBelow AboveOrBelow;

	public PetAddDamageOnPlayerAboveOrBellowOfMe_Passive(int probability, string elementID)
		: base(ePetEffectType.PetAddDamageOnPlayerAboveOrBellowOfMe_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		switch (elementID)
		{
		case "1723":
			damagePerPixel = 10;
			pixels = 100;
			AboveOrBelow = PlayerAboveOrBelow.Below;
			maxValue = 120;
			break;
		case "1724":
			damagePerPixel = 20;
			pixels = 100;
			AboveOrBelow = PlayerAboveOrBelow.Below;
			maxValue = 120;
			break;
		case "1725":
		case "1775":
			damagePerPixel = 30;
			pixels = 100;
			AboveOrBelow = PlayerAboveOrBelow.Below;
			maxValue = 120;
			break;
		case "1807":
			damagePerPixel = 6;
			pixels = 100;
			AboveOrBelow = PlayerAboveOrBelow.Below;
			maxValue = 60;
			break;
		case "1808":
			damagePerPixel = 8;
			pixels = 100;
			AboveOrBelow = PlayerAboveOrBelow.Below;
			maxValue = 60;
			break;
		case "1809":
			damagePerPixel = 10;
			pixels = 100;
			AboveOrBelow = PlayerAboveOrBelow.Below;
			maxValue = 60;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddDamageOnPlayerAboveOrBellowOfMe_Passive) is PetAddDamageOnPlayerAboveOrBellowOfMe_Passive petAddDamageOnPlayerAboveOrBellowOfMe_Passive)
		{
			petAddDamageOnPlayerAboveOrBellowOfMe_Passive.m_probability = ((m_probability > petAddDamageOnPlayerAboveOrBellowOfMe_Passive.m_probability) ? m_probability : petAddDamageOnPlayerAboveOrBellowOfMe_Passive.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.OnMakeDamageEvent += player_OnMakeDamageEvent;
		player.BeginSelfTurn += player_BeginSelfTurn;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.OnMakeDamageEvent -= player_OnMakeDamageEvent;
	}

	private void player_BeginSelfTurn(Living living)
	{
		living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: true);
		living.BeginSelfTurn -= player_BeginSelfTurn;
	}

	private void player_OnMakeDamageEvent(Living target, Living source, ref int damageamount, ref int criticalamount)
	{
		int num = 0;
		if (AboveOrBelow == PlayerAboveOrBelow.Above)
		{
			if (source.Y > target.Y)
			{
				int num2 = Math.Abs(target.Y - source.Y);
				num = damagePerPixel * num2 / pixels;
			}
		}
		else if (AboveOrBelow == PlayerAboveOrBelow.Below && source.Y < target.Y)
		{
			int num3 = Math.Abs(target.Y - source.Y);
			num = damagePerPixel * num3 / pixels;
		}
		if (num >= maxValue)
		{
			num = maxValue;
		}
		damageamount += damageamount * num / 100;
		criticalamount += criticalamount * num / 100;
	}
}
