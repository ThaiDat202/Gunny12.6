using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetKungFuSpecializationEquip : BasePetEffect
{
	private int m_count = 0;

	public PetKungFuSpecializationEquip(int count, string elementID)
		: base(ePetEffectType.PetKungFuSpecializationEquip, elementID)
	{
		m_count = count;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetKungFuSpecializationEquip) is PetKungFuSpecializationEquip petKungFuSpecializationEquip)
		{
			petKungFuSpecializationEquip.m_count = m_count;
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
