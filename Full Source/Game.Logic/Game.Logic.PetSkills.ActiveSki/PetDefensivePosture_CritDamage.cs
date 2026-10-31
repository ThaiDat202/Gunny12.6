using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetDefensivePosture_CritDamage : BasePetEffect
{
	private int m_count;

	private int m_probability;

	private int m_currentId;

	public PetDefensivePosture_CritDamage(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetDefensivePosture_CritDamage, elementID)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetDefensivePosture_CritDamage) is PetDefensivePosture_CritDamage petDefensivePosture_CritDamage)
		{
			petDefensivePosture_CritDamage.m_probability = ((m_probability > petDefensivePosture_CritDamage.m_probability) ? m_probability : petDefensivePosture_CritDamage.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerSkip += player_PlayerSkip;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerSkip -= player_PlayerSkip;
	}

	private void player_PlayerSkip(Player player)
	{
		player.Game.sendShowPicSkil(player, base.ElementInfo, isActive: false);
		player.Game.sendShowPicSkil(player, base.ElementInfo, isActive: true);
		new PetDefensivePosture_CritDamageEquip(m_count, base.Info.ID.ToString()).Start(player);
	}
}
