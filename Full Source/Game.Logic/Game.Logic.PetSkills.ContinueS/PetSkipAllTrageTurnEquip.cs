using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetSkipAllTrageTurnEquip : BasePetEffect
{
	private int m_count = 0;

	public PetSkipAllTrageTurnEquip(int count, string elementID)
		: base(ePetEffectType.PetSkipAllTrageTurnEquip, elementID)
	{
		m_count = count;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetSkipAllTrageTurnEquip) is PetSkipAllTrageTurnEquip petSkipAllTrageTurnEquip)
		{
			petSkipAllTrageTurnEquip.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeginAttacking += player_BeginAttacking;
		player.BeginSelfTurn += player_BeginSelfTurn;
		player.Game.sendShowPicSkil(player, base.ElementInfo, isActive: true);
		player.ShowImprisonment(state: true);
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeginAttacking -= player_BeginAttacking;
		player.BeginSelfTurn -= player_BeginSelfTurn;
		player.Game.sendShowPicSkil(player, base.ElementInfo, isActive: false);
		player.ShowImprisonment(state: false);
	}

	public void player_BeginSelfTurn(Living living)
	{
		if (m_count < 0)
		{
			Stop();
		}
	}

	private void player_BeginAttacking(Living living)
	{
		if (living.IsAttacking)
		{
			if (m_count > 0)
			{
				((Player)living).SkipAttack();
			}
			m_count--;
		}
	}
}
