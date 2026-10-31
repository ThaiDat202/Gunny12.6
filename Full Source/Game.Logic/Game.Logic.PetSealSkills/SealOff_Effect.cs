using System.Linq;
using Bussiness.Managers;
using EntityDatabase.ServerModels;
using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSealSkills;

public class SealOff_Effect : AbstractPetEffect
{
	private bool IsPVP;

	private int m_count = 0;

	public SealOff_Effect(int skilId, string ElementID, bool IsSendPic = true)
		: base(ePetEffectType.SealOff_Effect, ElementID)
	{
		Pet_Special_Skills pet_Special_Skills = PetSpecialSkillMgr.m_petSpecialSkills.Where((Pet_Special_Skills p) => p.SkillID == skilId).FirstOrDefault();
		IsPVP = pet_Special_Skills.IsPVP;
		m_count = pet_Special_Skills.P2;
	}

	public override bool Start(Living living)
	{
		if (!CheckPVP(living, IsPVP))
		{
			return false;
		}
		if (living.PetEffectList.GetOfType(ePetEffectType.SealOff_Effect) is SealOff_Effect sealOff_Effect)
		{
			sealOff_Effect.m_count = m_count;
			return true;
		}
		return Start(living);
	}

	public override void OnAttached(Living living)
	{
		living.BeginSelfTurn += AddEffect;
	}

	public override void OnRemoved(Living living)
	{
		living.BeginSelfTurn -= AddEffect;
	}

	private void AddEffect(Living living)
	{
		Player player = living as Player;
		if (m_count > 0)
		{
			m_count--;
			living.Game.sendShowPicSkil(living, base.Info, isActive: true);
			player.Game.SkillLock(player, isLock: true);
		}
		else
		{
			player.Game.SkillLock(player, isLock: false);
			player.FlagUsePetSkill = false;
			living.Game.sendShowPicSkil(living, base.Info, isActive: false);
			Stop();
		}
	}
}
