using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetReflectionDamage_Passive : BasePetEffect
{
	private int m_probability = 0;

	private int m_value;

	private int m_added;

	public PetReflectionDamage_Passive(int probability, string elementID)
		: base(ePetEffectType.PetReflectionDamage_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		if (!(elementID == "1459"))
		{
			if (elementID == "1460")
			{
				m_value = 5;
			}
		}
		else
		{
			m_value = 3;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReflectionDamage_Passive) is PetReflectionDamage_Passive petReflectionDamage_Passive)
		{
			petReflectionDamage_Passive.m_probability = ((m_probability > petReflectionDamage_Passive.m_probability) ? m_probability : petReflectionDamage_Passive.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.AfterKilledByLiving += player_AfterKilledByLiving;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.AfterKilledByLiving -= player_AfterKilledByLiving;
	}

	private void player_AfterKilledByLiving(Living living, Living target, int damageAmount, int criticalAmount)
	{
		if (!living.PetEffects.ActiveEffect)
		{
			return;
		}
		m_added = living.MaxBlood * m_value / 100;
		if (m_added <= 0)
		{
			return;
		}
		target.SyncAtTime = true;
		target.AddBlood(-m_added, 1);
		target.SyncAtTime = false;
		target.Game.sendShowPicSkil(living, base.ElementInfo, isActive: true);
		if (target.Blood <= 0)
		{
			target.Die();
			if (living != null && living is Player)
			{
				(living as Player).PlayerDetail.OnKillingLiving(living.Game, 2, living.Id, living.IsLiving, m_added);
			}
		}
		else
		{
			target.AddPetEffect(new PetReflectionDamageEffectEquip(m_probability, base.ElementInfo.ID.ToString()), 0);
		}
		living.PetEffects.ActiveEffect = false;
	}
}
