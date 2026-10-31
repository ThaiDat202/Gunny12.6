using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetSuckBloodEquip : BasePetEffect
{
	private int m_count;

	public PetSuckBloodEquip(int count, string elementID)
		: base(ePetEffectType.PetSuckBloodEquip, elementID)
	{
		m_count = count;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetSuckBloodEquip) is PetSuckBloodEquip petSuckBloodEquip)
		{
			petSuckBloodEquip.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.TakePlayerDamage += player_TakePlayerDamage;
		player.BeginSelfTurn += player_BeginSelfTurn;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.TakePlayerDamage -= player_TakePlayerDamage;
		player.BeginSelfTurn -= player_BeginSelfTurn;
	}

	private void player_BeginSelfTurn(Living living)
	{
		m_count--;
		if (m_count < 0)
		{
			living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: false);
			Stop();
		}
	}

	private void player_TakePlayerDamage(Living living, Living target, ref int damageAmount, ref int criticalAmount)
	{
		int value = damageAmount * 30 / 100;
		living.SyncAtTime = true;
		living.AddBlood(value);
		living.SyncAtTime = false;
	}
}
