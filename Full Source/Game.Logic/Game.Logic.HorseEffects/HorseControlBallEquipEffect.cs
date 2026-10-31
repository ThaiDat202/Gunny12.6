using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;

namespace Game.Logic.HorseEffects;

public class HorseControlBallEquipEffect : BaseHorseEffect
{
	private int m_type = 0;

	private float m_count = 0f;

	private int m_probability = 0;

	private int m_delay = 0;

	private int m_currentId;

	public HorseControlBallEquipEffect(int count, int probability, int type, int skillId, int delay, string elementID)
		: base(eHorseEffectType.HorseControlBallEquipEffect, elementID)
	{
		switch (elementID)
		{
		case "10801":
			m_count = 0.5f;
			break;
		case "10802":
			m_count = 0.55f;
			break;
		case "10803":
			m_count = 0.6f;
			break;
		case "10804":
			m_count = 0.65f;
			break;
		case "10805":
			m_count = 0.7f;
			break;
		}
		m_probability = ((probability == -1) ? 10000 : probability);
		m_type = type;
		m_delay = delay;
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.HorseEffectList.GetOfType(eHorseEffectType.HorseControlBallEquipEffect) is HorseControlBallEquipEffect horseControlBallEquipEffect)
		{
			horseControlBallEquipEffect.m_probability = ((m_probability > horseControlBallEquipEffect.m_probability) ? m_probability : horseControlBallEquipEffect.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerBuffSkillHorse += player_playerBuffSkillHorse;
		player.PlayerShoot += player_Shot;
		player.PlayerCompleteShoot += player_playerCompleteShoot;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerBuffSkillHorse -= player_playerBuffSkillHorse;
		player.PlayerShoot -= player_Shot;
		player.PlayerCompleteShoot -= player_playerCompleteShoot;
	}

	private void player_Shot(Player player)
	{
		if (IsTrigger && player.PetEffects.CurrentHorseSkill == m_currentId)
		{
			player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("FatalEffect.msg"), 9, 0, 1000));
			IsTrigger = false;
		}
	}

	private void player_playerCompleteShoot(Player player)
	{
		player.ControlBall = false;
	}

	private void player_playerBuffSkillHorse(Living living)
	{
		if (rand.Next(10000) < m_probability && living.PetEffects.CurrentHorseSkill == m_currentId)
		{
			IsTrigger = true;
			if (living.IsLiving)
			{
				living.ControlBall = true;
				living.CurrentShootMinus *= m_count;
			}
			else if (living.Game.CurrentLiving != null && living.Game.CurrentLiving is Player && living.Game.CurrentLiving.Team == living.Team)
			{
				living.Game.CurrentLiving.ControlBall = true;
				living.Game.CurrentLiving.CurrentShootMinus *= m_count;
			}
		}
	}
}
