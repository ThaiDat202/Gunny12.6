using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetBlackFlameBombEquip : BasePetEffect
{
	private int m_count;

	private int oldPowerRatio;

	public PetBlackFlameBombEquip(int count, string elementID)
		: base(ePetEffectType.PetBlackFlameBombEquip, elementID)
	{
		m_count = count;
		oldPowerRatio = 100;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetBlackFlameBombEquip) is PetBlackFlameBombEquip petBlackFlameBombEquip)
		{
			petBlackFlameBombEquip.m_count = m_count;
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
		if (m_count <= 0)
		{
			((Player)living).IsBlackFlame = false;
			living.CanFrozen = true;
			Stop();
		}
		((Player)living).SetBall(200);
		((Player)living).IsBlackFlame = true;
		living.CanFrozen = false;
	}
}
