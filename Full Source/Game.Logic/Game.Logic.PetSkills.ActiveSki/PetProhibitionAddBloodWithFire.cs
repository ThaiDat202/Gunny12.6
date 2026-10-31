using System;
using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetProhibitionAddBloodWithFire : BasePetEffect
{
	private int m_skillId;

	private int m_count;

	private int distanceRequired = -1;

	public PetProhibitionAddBloodWithFire(int count, int skillId, string elementID)
		: base(ePetEffectType.PetProhibitionAddBloodWithFire, elementID)
	{
		m_skillId = skillId;
		m_count = count;
		switch (elementID)
		{
		case "1717":
		case "1718":
		case "1719":
		case "1773":
		case "1801":
		case "1802":
		case "1803":
			distanceRequired = 300;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetProhibitionAddBloodWithFire) is PetProhibitionAddBloodWithFire)
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

	private void player_PlayerBuffSkillPet(Living living)
	{
		if (!(living.PetEffectList.GetOfType(ePetEffectType.PetShowFireOfHatredEquip) is PetShowFireOfHatredEquip petShowFireOfHatredEquip) || living.PetEffects.CurrentUseSkill != m_skillId)
		{
			return;
		}
		foreach (Player allEnemyPlayer in living.Game.GetAllEnemyPlayers(living))
		{
			if (distanceRequired == -1 || Math.Abs(living.X - allEnemyPlayer.X) <= distanceRequired)
			{
				for (int i = 0; i < m_count; i++)
				{
					allEnemyPlayer.Game.sendShowPicSkil(allEnemyPlayer, base.Info, isActive: true);
				}
				allEnemyPlayer.BeginSelfTurn += player_BeginSelfTurn;
			}
		}
		petShowFireOfHatredEquip.Stop();
	}

	private void player_BeginSelfTurn(Living living)
	{
		m_count--;
		if (m_count < 0)
		{
			living.ForbidAidBlood = false;
			living.NoFly(value: false);
			living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: false);
			living.BeginSelfTurn -= player_BeginSelfTurn;
			Stop();
			return;
		}
		living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: false);
		living.ForbidAidBlood = true;
		if (m_skillId == 569)
		{
			living.NoFly(value: true);
		}
		living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: true);
	}
}
