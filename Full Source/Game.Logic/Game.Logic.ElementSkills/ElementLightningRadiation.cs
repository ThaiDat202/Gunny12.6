using System;
using Game.Logic.Actions;
using Game.Logic.ElementSkills.ContinueSkills;
using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.ElementSkills;

public class ElementLightningRadiation : AbstractPetEffect
{
	private int m_value;

	private int m_rate;

	private int m_skillid;

	public ElementLightningRadiation(int skillID, int value, int rate)
		: base(ePetEffectType.ElementLightningRadiation, "")
	{
		m_value = value;
		m_rate = rate;
		m_skillid = skillID;
	}

	public override void OnAttached(Living living)
	{
		living.AfterKillingLiving += Living_AfterKillingLiving;
	}

	private void Living_AfterKillingLiving(Living living, Living target, int damageAmount, int criticalAmount)
	{
		if (target.EffectBadges || living == target)
		{
			return;
		}
		Random random = new Random();
		if (random.Next(10000) >= m_rate)
		{
			return;
		}
		living.Game.AddAction(new LivingSayAction(living, "Bắn Tia Chớp", 9, 0, 1000));
		foreach (Player item in living.Game.FindAllEnemyByPixel(living, 300))
		{
			item.EffectBadges = true;
			item.AddPetEffect(new ElementLightningRadiationEquip(m_skillid, m_value), 0);
		}
	}

	public override bool Start(Living living)
	{
		return base.Start(living);
	}
}
