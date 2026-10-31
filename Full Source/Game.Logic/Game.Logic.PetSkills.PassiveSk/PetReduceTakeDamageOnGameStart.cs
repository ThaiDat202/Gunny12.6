using Game.Logic.PetSkills.ContinueSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.PassiveSkills;

public class PetReduceTakeDamageOnGameStarted_Passive : BasePetEffect
{
	private int m_count = 0;

	private int m_coldDown = 0;

	public PetReduceTakeDamageOnGameStarted_Passive(int count, string elementID)
		: base(ePetEffectType.PetReduceTakeDamageOnGameStarted_Passive, elementID)
	{
		m_count = count;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceTakeDamageOnGameStarted_Passive) is PetReduceTakeDamageOnGameStarted_Passive)
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
		if (m_coldDown >= 8)
		{
			m_coldDown = 0;
			living.Game.sendShowPicSkil(living, base.Info, isActive: true);
			new PetReduceTakeDamageEquip(m_count, base.Info.ID.ToString()).Start(living);
		}
	}

	private void player_GameStarted(Living living)
	{
		living.Game.sendShowPicSkil(living, base.Info, isActive: true);
		new PetReduceTakeDamageEquip(m_count, base.Info.ID.ToString()).Start(living);
	}
}
