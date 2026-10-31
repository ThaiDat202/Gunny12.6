using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetIceImmunityEquip : BasePetEffect
{
	private int m_count;

	public PetIceImmunityEquip(int count, string elementID)
		: base(ePetEffectType.PetIceImmunityEquip, elementID)
	{
		m_count = count;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetIceImmunityEquip) is PetIceImmunityEquip petIceImmunityEquip)
		{
			petIceImmunityEquip.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeginNextTurn += player_BeginNextTurn;
		player.BeginSelfTurn += player_BeginSelfTurn;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeginSelfTurn -= player_BeginSelfTurn;
		player.BeginNextTurn -= player_BeginNextTurn;
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

	private void player_BeginNextTurn(Living living)
	{
		living.EffectList.StopEffect(typeof(IceFronzeEffect));
	}
}
