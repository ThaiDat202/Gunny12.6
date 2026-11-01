using System;
using Game.Logic.Actions;
using Game.Logic.ElementSkills.ContinueSkills;
using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.ElementSkills;

public class ElementFlamingGrilling : AbstractPetEffect
{
	private int m_value;

	private int m_rate;

	private int m_skillid;

	public ElementFlamingGrilling(int skillID, int value, int rate)
		: base(ePetEffectType.ElementFlamingGrilling, "")
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
		if (!target.EffectBadges && living != target)
		{
			Random random = new Random();
			if (random.Next(10000) < m_rate)
			{
				target.EffectBadges = true;
				living.Game.AddAction(new LivingSayAction(living, "Lửa Thiêu Đốt", 9, 0, 1000));
				double num = living.FireAttack * 1.6;
				target.AddPetEffect(new ElementFlamingGrillingEquip(m_skillid, (int)num, living), 0);
			}
		}
	}

	public override bool Start(Living living)
	{
		return base.Start(living);
	}
}
