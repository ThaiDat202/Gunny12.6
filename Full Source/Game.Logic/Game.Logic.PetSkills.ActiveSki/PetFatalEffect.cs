using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetFatalEffect : BasePetEffect
{
	private int m_delay;

	private int m_probability;

	private int m_currentId;

	private int m_count;

	public PetFatalEffect(int count, int probability, int skillId, int delay, string elementID)
		: base(ePetEffectType.PetFatalEffect, elementID)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_delay = delay;
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetFatalEffect) is PetFatalEffect petFatalEffect)
		{
			petFatalEffect.m_probability = ((m_probability > petFatalEffect.m_probability) ? m_probability : petFatalEffect.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerShoot += player_PlayerShoot;
		player.BeginSelfTurn += player_BeginSelfTurn;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerShoot -= player_PlayerShoot;
		player.BeginSelfTurn -= player_BeginSelfTurn;
	}

	private void player_BeginSelfTurn(Living living)
	{
		m_count--;
		if (m_count < 0 && IsTrigger)
		{
			IsTrigger = false;
			living.ControlBall = false;
			living.EffectTrigger = false;
			living.Game.sendShowPicSkil(living, base.Info, isActive: false);
			Stop();
		}
	}

	public void player_PlayerShoot(Living living)
	{
		IsTrigger = false;
		if (living.PetEffects.CurrentUseSkill == m_currentId)
		{
			living.Game.sendShowPicSkil(living, base.Info, isActive: true);
			IsTrigger = true;
			living.PetEffectTrigger = true;
			living.PetEffects.PetDelay = m_delay;
			if (living.PetEffects.IsPetUseSkill)
			{
				living.PetEffects.IsPetUseSkill = false;
				(living as Player).ControlBall = true;
			}
		}
	}
}
