using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetPhoenixNirvana_Passive : BasePetEffect
{
	private int m_probability = 0;

	private int m_value;

	private int m_count;

	public PetPhoenixNirvana_Passive(int value, int probability, string elementID)
		: base(ePetEffectType.PetPhoenixNirvana_Passive, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_value = value;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetPhoenixNirvana_Passive) is PetPhoenixNirvana_Passive petPhoenixNirvana_Passive)
		{
			petPhoenixNirvana_Passive.m_probability = ((m_probability > petPhoenixNirvana_Passive.m_probability) ? m_probability : petPhoenixNirvana_Passive.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.AfterKilledByLiving += player_AfterKilledByLiving;
		player.BeginAttacking += player_BeginAttacking;
		player.BeginSelfTurn += player_BeginSelfTurn;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.AfterKilledByLiving -= player_AfterKilledByLiving;
		player.BeginAttacking += player_BeginAttacking;
		player.BeginSelfTurn -= player_BeginSelfTurn;
	}

	private void player_AfterKilledByLiving(Living living, Living target, int damageAmount, int criticalAmount)
	{
		if (!IsTrigger && m_count == 0 && living != target && living.Blood <= 0)
		{
			living.Blood = 1;
			living.KeepLife = true;
			living.AddPetEffect(new PetChangeSuitEquip(2, "13884|suits667", base.ElementInfo.ID.ToString()), 0);
			IsTrigger = true;
			m_value = 2;
			m_count = 10;
		}
	}

	private void player_BeginAttacking(Living living)
	{
		if (IsTrigger && living.IsAttacking)
		{
			((Player)living).SkipAttack();
		}
	}

	private void player_BeginSelfTurn(Living living)
	{
		if (IsTrigger)
		{
			m_value--;
			m_count--;
			if (m_value < 0)
			{
				living.SyncAtTime = true;
				living.AddBlood(living.MaxBlood * 50 / 100);
				living.SyncAtTime = false;
				living.KeepLife = false;
				IsTrigger = false;
				m_value = 0;
			}
			if (m_count < 0)
			{
				m_count = 0;
			}
		}
	}
}
