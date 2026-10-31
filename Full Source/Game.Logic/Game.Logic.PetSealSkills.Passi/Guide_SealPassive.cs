using System.Linq;
using Bussiness.Managers;
using EntityDatabase.ServerModels;
using Game.Logic.Actions;
using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSealSkills.PassiveSkills;

public class Guide_SealPassive : AbstractPetEffect
{
	private int m_random = 0;

	private bool IsPVP = false;

	public Guide_SealPassive(int skilId, string elementID)
		: base(ePetEffectType.Guide_SealPassive, elementID)
	{
		Pet_Special_Skills pet_Special_Skills = PetSpecialSkillMgr.m_petSpecialSkills.Where((Pet_Special_Skills p) => p.SkillID == skilId).FirstOrDefault();
		m_random = pet_Special_Skills.P1;
		IsPVP = pet_Special_Skills.IsPVP;
	}

	public override bool Start(Living living)
	{
		if (!CheckPVP(living, IsPVP))
		{
			return false;
		}
		if (living.PetEffectList.GetOfType(ePetEffectType.Guide_SealPassive) is Guide_SealPassive)
		{
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		((Player)living).PlayerShoot += player_PlayerShoot;
	}

	public override void OnRemoved(Living living)
	{
		((Player)living).PlayerShoot -= player_PlayerShoot;
	}

	private void player_PlayerShoot(Living living)
	{
		if (rand.Next(0, 100) <= m_random)
		{
			Player player = living as Player;
			player.ShootMovieDelay = 50;
			IsTrigger = true;
			if (player.CurrentBall.ID != 3)
			{
				player.ControlBall = true;
			}
			player.Game.AddAction(new LivingSayAction(player, "引导", 9, 0, 1000));
		}
	}
}
