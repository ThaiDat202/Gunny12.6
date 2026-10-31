using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetAddBaseGuardBeforeMoveEquip : BasePetEffect
{
	private int m_count = 0;

	private int m_probability = 0;

	private int m_added = 0;

	private int m_value = 0;

	public PetAddBaseGuardBeforeMoveEquip(int count, int probability, string elementID)
		: base(ePetEffectType.PetAddBaseGuardBeforeMoveEquip, elementID)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		if (!(elementID == "1236"))
		{
			if (elementID == "1237")
			{
				m_value = 365;
			}
		}
		else
		{
			m_value = 250;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddBaseGuardBeforeMoveEquip) is PetAddBaseGuardBeforeMoveEquip petAddBaseGuardBeforeMoveEquip)
		{
			petAddBaseGuardBeforeMoveEquip.m_probability = ((m_probability > petAddBaseGuardBeforeMoveEquip.m_probability) ? m_probability : petAddBaseGuardBeforeMoveEquip.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeginSelfTurn += player_BeginSelfTurn;
		if (m_added == 0)
		{
			m_added = m_value;
			player.BaseGuard += m_added;
		}
	}

	protected override void OnPausedOnPlayer(Player player)
	{
		player.Game.sendShowPicSkil(player, base.ElementInfo, isActive: false);
		player.BaseGuard -= m_added;
	}

	private void player_BeginSelfTurn(Living living)
	{
		m_count--;
		if (m_count > 0)
		{
			m_added += m_value;
		}
	}
}
