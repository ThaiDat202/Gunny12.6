using System.Collections.Generic;
using System.Linq;
using Bussiness.Managers;
using EntityDatabase.ServerModels;
using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSealSkills.PassiveSkills;

public class PetReduceAllTargetMagicDefence_SealPassive : AbstractPetEffect
{
	private int m_value = 0;

	private bool IsPVP = false;

	private int m_added = 0;

	private bool m_percent = false;

	public PetReduceAllTargetMagicDefence_SealPassive(int skilId, string elementID, bool isPercent)
		: base(ePetEffectType.PetReduceAllTargetMagicDefence_SealPassive, elementID)
	{
		Pet_Special_Skills pet_Special_Skills = PetSpecialSkillMgr.m_petSpecialSkills.Where((Pet_Special_Skills p) => p.SkillID == skilId).FirstOrDefault();
		m_added = pet_Special_Skills.P1;
		IsPVP = pet_Special_Skills.IsPVP;
		m_percent = isPercent;
	}

	public override bool Start(Living living)
	{
		if (!CheckPVP(living, IsPVP))
		{
			return false;
		}
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceAllTargetMagicDefence_SealPassive) is PetReduceAllTargetMagicDefence_SealPassive)
		{
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		living.BeginSelfTurn += player_BeginSelfTurn;
	}

	public override void OnRemoved(Living living)
	{
		living.BeginSelfTurn -= player_BeginSelfTurn;
	}

	private void player_BeginSelfTurn(Living living)
	{
		if (m_value != 0)
		{
			return;
		}
		m_value = m_added;
		List<Player> allEnemyPlayers = living.Game.GetAllEnemyPlayers(living);
		foreach (Player item in allEnemyPlayers)
		{
			item.Game.sendShowPicSkil(item, base.Info, isActive: true);
			if (m_percent)
			{
				item.MagicDefence -= item.MagicDefence * (double)m_value / 100.0;
			}
			else
			{
				item.MagicDefence -= m_value;
			}
		}
	}
}
