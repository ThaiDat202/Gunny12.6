using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetReduceTakeDamageForTeamWithFire : BasePetEffect
{
	private int m_count;

	private int m_probability;

	private int m_skillId;

	private string m_elementID;

	public PetReduceTakeDamageForTeamWithFire(int count, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetShareTakeDamageToTeam, elementID)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_skillId = skillId;
		switch (m_skillId)
		{
		case 560:
			m_elementID = "1767";
			break;
		case 561:
			m_elementID = "1768";
			break;
		case 562:
			m_elementID = "1769";
			break;
		case 595:
			m_elementID = "1774";
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetShareTakeDamageToTeam) is PetReduceTakeDamageForTeamWithFire petReduceTakeDamageForTeamWithFire)
		{
			petReduceTakeDamageForTeamWithFire.m_probability = ((m_probability > petReduceTakeDamageForTeamWithFire.m_probability) ? m_probability : petReduceTakeDamageForTeamWithFire.m_probability);
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
		if (player.PetEffectList.GetOfType(ePetEffectType.PetShowFireofResentmentEquip) is PetShowFireofResentmentEquip petShowFireofResentmentEquip)
		{
			petShowFireofResentmentEquip.Stop();
		}
		foreach (Player allTeamPlayer in player.Game.GetAllTeamPlayers(player))
		{
			allTeamPlayer.AddPetEffect(new PetReduceTakeDamageEquip(m_count, m_elementID), 0);
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
