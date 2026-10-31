using System.Linq;
using Bussiness.Managers;
using EntityDatabase.ServerModels;
using Game.Logic.Actions;
using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSealSkills;

public class Blind_Seal : AbstractPetEffect
{
	private bool IsPVP;

	private int m_value = 0;

	private int m_count = 0;

	private bool IsUse = false;

	private int SkillID = 0;

	private string ElementID = "";

	public Blind_Seal(int skilId, string ElementID, bool IsSendPic = true)
		: base(ePetEffectType.Blind_Seal, ElementID)
	{
		Pet_Special_Skills pet_Special_Skills = PetSpecialSkillMgr.m_petSpecialSkills.Where((Pet_Special_Skills p) => p.SkillID == skilId).FirstOrDefault();
		IsPVP = pet_Special_Skills.IsPVP;
		m_value = pet_Special_Skills.P1;
		SkillID = skilId;
		ElementID = ElementID;
	}

	public override bool Start(Living living)
	{
		if (!CheckPVP(living, IsPVP))
		{
			return false;
		}
		if (living.PetEffectList.GetOfType(ePetEffectType.Blind_Seal) is Blind_Seal blind_Seal)
		{
			blind_Seal.m_count = m_count;
			return true;
		}
		return Start(living);
	}

	public override void OnAttached(Living living)
	{
		living.AfterKillingLiving += AddEffect;
	}

	public override void OnRemoved(Living living)
	{
		living.AfterKillingLiving -= AddEffect;
	}

	private void AddEffect(Living living, Living target, int damage, int critical)
	{
		if (!IsUse)
		{
			IsUse = true;
			if (!target.FlagUsePetSkill && rand.Next(100) < m_value)
			{
				living.Game.sendShowPicSkil(living, base.Info, isActive: false);
				living.Game.sendShowPicSkil(living, base.Info, isActive: true);
				living.Game.AddAction(new LivingSayAction(living, "致盲", 9, 0, 1000));
				new Blind_Effect(SkillID, ElementID).Start(target);
				target.FlagUsePetSkill = true;
			}
		}
		else
		{
			IsUse = false;
		}
	}
}
