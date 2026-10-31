using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetActiveReflectionDamage_Passive : BasePetEffect
{
	private int m_probability = 0;

	public PetActiveReflectionDamage_Passive(int probability, string elementID)
		: base(ePetEffectType.PetActiveReflectionDamage_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetActiveReflectionDamage_Passive) is PetActiveReflectionDamage_Passive petActiveReflectionDamage_Passive)
		{
			petActiveReflectionDamage_Passive.m_probability = ((m_probability > petActiveReflectionDamage_Passive.m_probability) ? m_probability : petActiveReflectionDamage_Passive.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeforeTakeDamage += player_BeforeTakeDamage;
		player.AfterKilledByLiving += player_AfterKilledByLiving;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeforeTakeDamage -= player_BeforeTakeDamage;
		player.AfterKilledByLiving -= player_AfterKilledByLiving;
	}

	private void player_AfterKilledByLiving(Living living, Living target, int damageAmount, int criticalAmount)
	{
		living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: false);
	}

	private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
	{
		if (rand.Next(100) <= 20)
		{
			living.PetEffects.ActiveEffect = true;
			living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: true);
		}
	}
}
