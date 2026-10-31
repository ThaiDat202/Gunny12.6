using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetBlockUseTridentEquip : BasePetEffect
{
	private int m_count;

	public PetBlockUseTridentEquip(int count, string elementID)
		: base(ePetEffectType.PetBlockUseTridentEquip, elementID)
	{
		m_count = count;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetBlockUseTridentEquip) is PetBlockUseTridentEquip petBlockUseTridentEquip)
		{
			petBlockUseTridentEquip.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeginSelfTurn += player_BeginSelfTurn;
		player.lockProp(10003);
		player.Prop.Remove(10003);
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeginSelfTurn -= player_BeginSelfTurn;
		player.unlockProp(10003);
	}

	private void player_BeginSelfTurn(Living living)
	{
		m_count--;
		if (base.ElementInfo.Pic != -1)
		{
			living.Game.sendShowPicSkil(living, base.Info, isActive: false);
		}
		if (m_count < 0)
		{
			living.Game.sendShowPicSkil(living, base.Info, isActive: false);
			Stop();
		}
	}
}
