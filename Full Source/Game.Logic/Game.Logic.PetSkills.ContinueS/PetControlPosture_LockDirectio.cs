using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetControlPosture_LockDirectionEquip : BasePetEffect
{
	private int m_count;

	public PetControlPosture_LockDirectionEquip(int count, string elementID)
		: base(ePetEffectType.PetControlPosture_LockDirectionEquip, elementID)
	{
		m_count = count;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetControlPosture_LockDirectionEquip) is PetControlPosture_LockDirectionEquip petControlPosture_LockDirectionEquip)
		{
			petControlPosture_LockDirectionEquip.m_count = m_count;
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
		player.Game.sendShowPicSkil(player, base.ElementInfo, isActive: false);
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
}
