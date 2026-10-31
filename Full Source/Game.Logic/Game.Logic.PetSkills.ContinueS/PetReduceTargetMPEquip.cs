using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetReduceTargetMPEquip : BasePetEffect
{
	private int m_probability = 0;

	public PetReduceTargetMPEquip(int probability, string elementID)
		: base(ePetEffectType.PetReduceTargetMPEquip, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceTargetMPEquip) is PetReduceTargetMPEquip petReduceTargetMPEquip)
		{
			petReduceTargetMPEquip.m_probability = ((m_probability > petReduceTargetMPEquip.m_probability) ? m_probability : petReduceTargetMPEquip.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeginSelfTurn += Player_BeginSelfTurn;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.Game.sendShowPicSkil(player, base.ElementInfo, isActive: false);
		player.BeginSelfTurn -= Player_BeginSelfTurn;
	}

	private void Player_BeginSelfTurn(Living living)
	{
		Stop();
	}
}
