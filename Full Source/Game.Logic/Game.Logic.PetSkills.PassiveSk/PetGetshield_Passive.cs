using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetGetshield_Passive : BasePetEffect
{
	private int m_count = 0;

	private int m_added = 0;

	private int m_value = 0;

	private int m_coldDown = 0;

	private int m_cd = 0;

	public PetGetshield_Passive(int count, string elementID)
		: base(ePetEffectType.PetGetshield_Passive, elementID)
	{
		m_count = count;
		if (!(elementID == "3219"))
		{
			if (elementID == "3230")
			{
				m_added = 50;
				m_cd = 8;
			}
		}
		else
		{
			m_added = 30;
			m_cd = 12;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetGetshield_Passive) is PetGetshield_Passive)
		{
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.GameStarted += player_GameStarted;
		player.BeginSelfTurn += player_BeginSelfTurn;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.GameStarted -= player_GameStarted;
		player.BeginSelfTurn -= player_BeginSelfTurn;
	}

	private void player_BeginSelfTurn(Living living)
	{
		m_coldDown++;
		if (m_coldDown >= m_cd)
		{
			m_coldDown = 0;
			living.Game.sendShowPicSkil(living, base.Info, isActive: true);
			new PetGetShieldEquip(m_count, m_value, base.Info.ID.ToString()).Start(living);
		}
	}

	private void player_GameStarted(Living living)
	{
		if (m_value == 0)
		{
			m_value = living.MaxBlood * m_added / 100;
			living.Game.sendShowPicSkil(living, base.Info, isActive: true);
			new PetGetShieldEquip(m_count, m_value, base.Info.ID.ToString()).Start(living);
		}
	}
}
