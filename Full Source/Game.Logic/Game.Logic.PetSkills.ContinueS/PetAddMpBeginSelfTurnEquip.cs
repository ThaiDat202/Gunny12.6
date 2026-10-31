using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetAddMpBeginSelfTurnEquip : BasePetEffect
{
	private int m_count;

	private bool m_percent;

	private int m_value;

	public PetAddMpBeginSelfTurnEquip(int count, string elementID)
		: base(ePetEffectType.PetAddMpBeginSelfTurnEquip, elementID)
	{
		m_count = count;
		if (elementID == "1789")
		{
			m_percent = false;
			m_value = 20;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddMpBeginSelfTurnEquip) is PetAddMpBeginSelfTurnEquip petAddMpBeginSelfTurnEquip)
		{
			petAddMpBeginSelfTurnEquip.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeginSelfTurn += player_BeginSelfTurn;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeginSelfTurn -= player_BeginSelfTurn;
	}

	private void player_BeginSelfTurn(Living living)
	{
		if (base.ElementInfo.Pic != -1)
		{
			living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: false);
		}
		m_count--;
		if (m_count < 0)
		{
			living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: false);
			Stop();
		}
		else if (living is Player)
		{
			if (!m_percent)
			{
				(living as Player).AddPetMP(m_value);
			}
			else
			{
				(living as Player).AddPetMP((living as Player).PetMP / 100 * m_value);
			}
		}
	}
}
