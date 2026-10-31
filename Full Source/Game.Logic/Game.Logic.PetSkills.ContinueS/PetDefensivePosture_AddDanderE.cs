using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetDefensivePosture_AddDanderEquip : BasePetEffect
{
	private int m_count;

	private int m_value;

	public PetDefensivePosture_AddDanderEquip(int count, string elementID)
		: base(ePetEffectType.PetDefensivePosture_AddDanderEquip, elementID)
	{
		m_count = count;
		if (elementID == "1617")
		{
			m_value = 20;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetDefensivePosture_AddDanderEquip) is PetDefensivePosture_AddDanderEquip petDefensivePosture_AddDanderEquip)
		{
			petDefensivePosture_AddDanderEquip.m_count = m_count;
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
		(living as Player).AddDander(m_value);
		m_count--;
		if (m_count < 0)
		{
			living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: false);
			Stop();
		}
	}
}
