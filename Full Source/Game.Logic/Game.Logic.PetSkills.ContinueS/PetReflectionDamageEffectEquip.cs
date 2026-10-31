using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetReflectionDamageEffectEquip : BasePetEffect
{
	private int m_count;

	public PetReflectionDamageEffectEquip(int count, string elementID)
		: base(ePetEffectType.PetReflectionDamageEffectEquip, elementID)
	{
		m_count = count;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReflectionDamageEffectEquip) is PetReflectionDamageEffectEquip petReflectionDamageEffectEquip)
		{
			petReflectionDamageEffectEquip.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeginSelfTurn += player_BeginSelfTurn;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.Game.sendShowPicSkil(player, base.ElementInfo, isActive: false);
		player.BeginSelfTurn -= player_BeginSelfTurn;
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
