using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetShowEffectEquip : BasePetEffect
{
	private int m_count;

	public PetShowEffectEquip(int count, string elementID)
		: base(ePetEffectType.PetShowEffectEquip, elementID)
	{
		m_count = count;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetShowEffectEquip) is PetShowEffectEquip petShowEffectEquip)
		{
			petShowEffectEquip.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeginSelfTurn += player_BeginSelfTurn;
		player.Game.sendShowPicSkil(player, base.ElementInfo, isActive: true);
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
			living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: false);
		}
	}
}
