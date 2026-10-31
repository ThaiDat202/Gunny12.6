using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetShareTakeDamageToTeam : BasePetEffect
{
	private int m_count;

	private int m_probability;

	private int m_skillId;

	public PetShareTakeDamageToTeam(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetShareTakeDamageToTeam, elementID)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_skillId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetShareTakeDamageToTeam) is PetShareTakeDamageToTeam petShareTakeDamageToTeam)
		{
			petShareTakeDamageToTeam.m_probability = ((m_probability > petShareTakeDamageToTeam.m_probability) ? m_probability : petShareTakeDamageToTeam.m_probability);
			return true;
		}
		return base.Start(living);
	}

	private void petUseSkill(Player player)
	{
		if (player.PetEffects.CurrentUseSkill != m_skillId || player.Game.GetAllTeamPlayers(player).Count <= 1)
		{
			return;
		}
		foreach (Player allTeamPlayer in player.Game.GetAllTeamPlayers(player))
		{
			allTeamPlayer.AddPetEffect(new PetShareTakeDamageToTeamEquip(m_count, base.Info.ID.ToString()), 0);
		}
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerBuffSkillPet += petUseSkill;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerBuffSkillPet -= petUseSkill;
	}
}
