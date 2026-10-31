using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetAddBaseDamageOnPlayerBuffSkill_Passive : BasePetEffect
{
	private int m_added;

	public PetAddBaseDamageOnPlayerBuffSkill_Passive(string elementID)
		: base(ePetEffectType.PetAddBaseDamageOnPlayerBuffSkill_Passive, elementID)
	{
		if (!(elementID == "3218"))
		{
			if (elementID == "3229")
			{
				m_added = 20;
			}
		}
		else
		{
			m_added = 10;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddBaseDamageOnPlayerBuffSkill_Passive) is PetAddBaseDamageOnPlayerBuffSkill_Passive)
		{
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerBuffSkill += player_PlayerBuffSkill;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerBuffSkill -= player_PlayerBuffSkill;
		player.BaseDamage = player.OldBaseDamage;
		player.Game.sendShowPicSkil(player, base.ElementInfo, isActive: false);
	}

	private void player_PlayerBuffSkill(Living living)
	{
		living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: false);
		living.BaseDamage += living.BaseDamage * (double)m_added / 100.0;
		living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: true);
	}
}
