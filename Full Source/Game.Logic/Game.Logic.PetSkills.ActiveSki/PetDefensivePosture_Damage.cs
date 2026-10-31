using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetDefensivePosture_Damage : BasePetEffect
{
	private int m_count;

	private int m_probability;

	private int m_currentId;

	public PetDefensivePosture_Damage(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetDefensivePosture_Damage, elementID)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetDefensivePosture_Damage) is PetDefensivePosture_Damage petDefensivePosture_Damage)
		{
			petDefensivePosture_Damage.m_probability = ((m_probability > petDefensivePosture_Damage.m_probability) ? m_probability : petDefensivePosture_Damage.m_probability);
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
		new PetDefensivePosture_DamageEquip(m_count, base.Info.ID.ToString()).Start(player);
	}
}
