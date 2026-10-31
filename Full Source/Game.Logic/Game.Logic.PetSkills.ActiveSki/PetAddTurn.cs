using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetAddTurn : BasePetEffect
{
	private int m_probability = 0;

	private int m_currentId;

	private int m_value;

	private int m_count;

	private int m_removed;

	public PetAddTurn(int value, int probability, int skillId, string elementID)
		: base(ePetEffectType.PetAddTurn, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
		m_value = value;
		switch (m_currentId)
		{
		case 449:
			m_count = 10;
			m_removed = 70;
			break;
		case 470:
			m_count = 15;
			m_removed = 40;
			break;
		case 499:
			m_count = 10;
			m_removed = 80;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddTurn) is PetAddTurn petAddTurn)
		{
			petAddTurn.m_probability = ((m_probability > petAddTurn.m_probability) ? m_probability : petAddTurn.m_probability);
			return true;
		}
		return base.Start(living);
	}

	private void player_BeginNextTurn(Living living)
	{
		if (IsTrigger)
		{
			(living as Player).Delay += (living as Player).DefaultDelay;
			(living as Player).Delay += (living as Player).Delay * m_count / 100;
			((Player)living).IsAddTurn = true;
			((Player)living).Energy = m_value;
			IsTrigger = false;
		}
	}

	public void player_PlayerShoot(Player player)
	{
		if (player.PetEffects.CurrentUseSkill == m_currentId)
		{
			player.AttackGemLimit = 2;
			IsTrigger = true;
			player.EffectTrigger = true;
			player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("PetAttackEffect.Success"));
			player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("AddTurnEquipEffect.msg"), 9, 0, 1000));
			player.Delay -= player.Delay * m_removed / 100;
		}
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerShoot += player_PlayerShoot;
		player.BeginNextTurn += player_BeginNextTurn;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerShoot -= player_PlayerShoot;
		player.BeginNextTurn -= player_BeginNextTurn;
	}
}
