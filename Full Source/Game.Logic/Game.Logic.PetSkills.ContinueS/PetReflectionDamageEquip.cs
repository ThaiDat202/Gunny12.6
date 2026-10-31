using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetReflectionDamageEquip : BasePetEffect
{
	private int m_count;

	private int m_value;

	private int m_added;

	public PetReflectionDamageEquip(int count, string elementID)
		: base(ePetEffectType.PetReflectionDamageEquip, elementID)
	{
		m_count = count;
		switch (elementID)
		{
		case "1067":
		case "1228":
			m_value = 30;
			break;
		case "1068":
		case "1229":
			m_value = 50;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReflectionDamageEquip) is PetReflectionDamageEquip petReflectionDamageEquip)
		{
			petReflectionDamageEquip.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeginSelfTurn += player_BeginSelfTurn;
		player.AfterKilledByLiving += player_AfterKilledByLiving;
		IsTrigger = true;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeginSelfTurn -= player_BeginSelfTurn;
		player.AfterKilledByLiving -= player_AfterKilledByLiving;
	}

	private void player_BeginSelfTurn(Living living)
	{
		m_count--;
		IsTrigger = true;
		if (m_count < 0)
		{
			m_added = 0;
			living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: false);
			Stop();
		}
	}

	private void player_AfterKilledByLiving(Living living, Living target, int damageAmount, int criticalAmount)
	{
		if (!IsTrigger)
		{
			return;
		}
		m_added = damageAmount * m_value / 100;
		target.SyncAtTime = true;
		target.AddBlood(-m_added, 1);
		target.SyncAtTime = false;
		if (target.Blood <= 0)
		{
			target.Die();
			if (living != null && living is Player)
			{
				(living as Player).PlayerDetail.OnKillingLiving(living.Game, 2, target.Id, target.IsLiving, m_added);
			}
		}
		IsTrigger = false;
	}
}
