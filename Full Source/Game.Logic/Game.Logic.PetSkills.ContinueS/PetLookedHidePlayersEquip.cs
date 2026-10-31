using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetLookedHidePlayersEquip : AbstractPetEffect
{
	private int m_count = 0;

	public PetLookedHidePlayersEquip(int count, string elementID)
		: base(ePetEffectType.PetLookedHidePlayersEquip, elementID)
	{
		m_count = count;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetLookedHidePlayersEquip) is PetLookedHidePlayersEquip petLookedHidePlayersEquip)
		{
			petLookedHidePlayersEquip.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		foreach (Player allEnemyPlayer in living.Game.GetAllEnemyPlayers(living))
		{
			allEnemyPlayer.IsHide = false;
			allEnemyPlayer.Game.SendPlayerPicture(allEnemyPlayer, 20050, state: true);
		}
		living.BeginSelfTurn += player_BeginSelfTurn;
	}

	public override void OnRemoved(Living living)
	{
		foreach (Player allEnemyPlayer in living.Game.GetAllEnemyPlayers(living))
		{
			allEnemyPlayer.IsHide = true;
			allEnemyPlayer.Game.SendPlayerPicture(allEnemyPlayer, 20050, state: false);
		}
		living.BeginSelfTurn -= player_BeginSelfTurn;
	}

	public void player_BeginSelfTurn(Living living)
	{
		m_count--;
		if (m_count < 0)
		{
			living.Game.sendShowPicSkil(living, base.Info, isActive: true);
			Stop();
		}
	}
}
