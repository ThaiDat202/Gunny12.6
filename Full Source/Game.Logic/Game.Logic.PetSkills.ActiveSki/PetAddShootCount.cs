using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetAddShootCount : BasePetEffect
{
	private int m_skillId;

	public PetAddShootCount(int skillId, string elementID)
		: base(ePetEffectType.PetAddShootCount, elementID)
	{
		m_skillId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddShootCount) is PetAddShootCount)
		{
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerBuffSkillPet += player_PlayerBuffSkillPet;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerBuffSkillPet -= player_PlayerBuffSkillPet;
	}

	private void player_PlayerBuffSkillPet(Player player)
	{
		if (player.PetEffects.CurrentUseSkill == m_skillId)
		{
			player.Game.sendShowPicSkil(player, base.Info, isActive: true);
			player.PlayerShoot -= player_PlayerShoot;
			player.PlayerShoot += player_PlayerShoot;
		}
	}

	private void player_PlayerShoot(Player player)
	{
		player.PlayerShoot -= player_PlayerShoot;
		player.ShootCount++;
	}
}
