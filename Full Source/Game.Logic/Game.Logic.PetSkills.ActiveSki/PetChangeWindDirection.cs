using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetChangeWindDirection : BasePetEffect
{
	private int m_skillId;

	public PetChangeWindDirection(int skillId, string elementID)
		: base(ePetEffectType.PetChangeWindDirection, elementID)
	{
		m_skillId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetChangeWindDirection) is PetChangeWindDirection)
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
			player.PetEffectTrigger = true;
			player.Game.sendShowPicSkil(player, base.Info, isActive: true);
			player.Game.UpdateWind(player.Game.Wind * -1f, sendToClient: true);
		}
	}
}
